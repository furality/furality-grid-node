using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace FuralityGridNode
{
    public partial class Form1 : Form
    {
        static int bladeSizeX = 120;
        static int bladeSizeY = 13;

        static bool customLayout;
        static int customLayoutBladeSizeX = 120;
        static int customLayoutBladeSizeY = 13;

        static int outputScale = 16;
        static int previewScale = 4;

        private ArtNet artnetClient;

        private byte[] midiData = new byte[512 * ArtNet.maxUniverses];
        List<byte[]> overlayData = new List<byte[]>();
        private int midiScanPosition = 0;
        private int midiCatchup = 0;
        private long midiUpdate = 0;
        private string midiSavedDevice = "";
        private int maxMidiChannels = 4096;
        private int bankStatus = 0;

        private FileStream logStream;

        private OutputDevice midiOutput;

        private Bitmap outputImage;

        [Serializable]
        class FuralityGridNodeSettings
        {
            public string ArtNetAddress;
            public string ArtNetPort;
            public bool Unicast;
            public string RigType;
            public string MidiDevice;
            public bool Is1440pModeOn;
            public bool ExpandScreenshotsTo16By9;
        }

        public void SaveSettings()
        {
            FuralityGridNodeSettings settings = new FuralityGridNodeSettings();
            settings.ExpandScreenshotsTo16By9 = expandScreenshotTo16by9.Checked;
            settings.ArtNetAddress = ipInput.Text;
            settings.ArtNetPort = portInput.Text;
            settings.Unicast = unicast.Checked;
            settings.RigType = rigTypeDropdown.SelectedItem != null ? rigTypeDropdown.SelectedItem.ToString() : "VRSL";
            settings.MidiDevice = midiDevice.SelectedItem != null ? midiDevice.SelectedItem.ToString() : "(none)";
            settings.Is1440pModeOn = res1440p.Checked;
            string json = JsonSerializer.Serialize<FuralityGridNodeSettings>(settings, new JsonSerializerOptions { PropertyNameCaseInsensitive = false, IncludeFields = true, WriteIndented = true });
            File.WriteAllText("FuralityGridNodeSettings.json", json);
        }
        public void LoadSettings()
        {
            string json = File.ReadAllText("FuralityGridNodeSettings.json");
            FuralityGridNodeSettings settings = JsonSerializer.Deserialize<FuralityGridNodeSettings>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = false, IncludeFields = true, WriteIndented = true });
            ipInput.Text = settings.ArtNetAddress;
            portInput.Text = settings.ArtNetPort;
            unicast.Checked = settings.Unicast;
            rigTypeDropdown.SelectedItem = settings.RigType == null ? "VRSL" : settings.RigType;
            midiSavedDevice = settings.MidiDevice;
            res1440p.Checked = settings.Is1440pModeOn;
            expandScreenshotTo16by9.Checked = settings.ExpandScreenshotsTo16By9;
        }

        public Form1()
        {
            InitializeComponent();
            g = this.CreateGraphics();
            form = this;
            this.MouseDown += new MouseEventHandler(MainForm_MouseDown);
            g.Clear(Color.Black);

            rigTypeDropdown.SelectedIndex = 0;

            if (!File.Exists("FuralityGridNodeSettings.json"))
            {
                SaveSettings();
            }
            else
            {
                LoadSettings();
            }

            populateMidi();
            connectMidi();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            public RGBQUAD bmiColors;
        }

        [DllImport("gdi32.dll")]
        public static extern int StretchDIBits(
            IntPtr hdc,
            int xDest,
            int yDest,
            int DestWidth,
            int DestHeight,
            int xSrc,
            int ySrc,
            int SrcWidth,
            int SrcHeight,
            IntPtr lpBits,
            ref BITMAPINFO lpBitsInfo,
            uint iUsage,
            uint dwRop);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        // Define the necessary constants
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }
        public static Graphics g;
        public static Form1 form;
        static byte[] combinedData = new byte[512 * 8];

        static bool update = false;
        static string statusText = "";
        static string statusTextLast = "";

        public static bool IsValidIndex<T>(IList<T> source, int index)
        {
            return source != null && index >= 0 && index < source.Count;
        }

        // CRC-8 (x⁸ + x² + x + 1)
        public static byte Crc8For6(
            byte b0, byte b1, byte b2,
            byte b3, byte b4, byte b5)
        {
            uint crc = 0;
            uint poly = 0x07;

            uint[] data = { b0, b1, b2, b3, b4, b5 };
            foreach (uint v in data)
            {
                crc ^= v;
                for (int i = 0; i < 8; ++i)
                    crc = (crc & 0x80) != 0
                          ? ((crc << 1) ^ poly) & 0xFF
                          : (crc << 1) & 0xFF;
            }
            return (byte)crc;
        }

        // CRC-4 (x⁴ + x + 1)
        public static byte Crc4For6(
            byte b0, byte b1, byte b2,
            byte b3, byte b4, byte b5)
        {
            uint crc = 0;
            uint poly = 0x03;

            uint[] data = { b0, b1, b2, b3, b4, b5 };
            foreach (uint v in data)
            {
                for (int bit = 7; bit >= 0; --bit)
                {
                    uint inBit = (v >> bit) & 1;
                    uint top = (crc >> 3) & 1;
                    crc = ((crc << 1) | inBit) & 0xF;
                    if (top == 1) crc ^= poly;
                }
            }

            crc = (crc << 4);

            return (byte)crc; // put crc on the left and pad 0s
        }

        void SetPixel(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, 
            byte R, byte G, byte B, byte A, 
            bool setR = true, bool setG = true, bool setB = true, bool setA = true)
        {
            if (X >= dataSizeX || Y >= dataSizeY)
                return;

            if (X + 1 <= 0 || Y + 1 <= 0)
                return;

            int rowIndex = (Y * dataSizeX) * 4;
            int idx = rowIndex + X * 4;
            if(setR) data[idx + 0] = B;  // B
            if(setG) data[idx + 1] = G;  // G
            if(setB) data[idx + 2] = R;  // R
            if(setA) data[idx + 3] = A;  // A
        }
        void SetPixel(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, bool value)
        {
            SetPixel(data, dataSizeX, dataSizeY, X, Y,
                value ? (byte)255 : (byte)0,
                value ? (byte)255 : (byte)0,
                value ? (byte)255 : (byte)0, 255);
        }

        void DrawSquare(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, int sizeX, int sizeY, byte R, byte G, byte B, byte A)
        {
            if (sizeX <= 0 || sizeY <= 0)
                return;
            if (X >= dataSizeX || Y >= dataSizeY)
                return;
            if (X + sizeX <= 0 || Y + sizeY <= 0)
                return;

            int startX = Math.Max(X, 0);
            int startY = Math.Max(Y, 0);
            int endX = Math.Min(X + sizeX, dataSizeX);
            int endY = Math.Min(Y + sizeY, dataSizeY);

            for (int y = startY; y < endY; y++)
            {
                int rowIndex = (y * dataSizeX + startX) * 4;

                for (int x = startX; x < endX; x++)
                {
                    int idx = rowIndex + (x - startX) * 4;
                    data[idx + 0] = B;  // B
                    data[idx + 1] = G;  // G
                    data[idx + 2] = R;  // R
                    data[idx + 3] = A;  // A
                }
            }
        }
        void DrawSquare(byte[] data, int dataSizeX, int dataSizeY, int X, int Y, int sizeX, int sizeY, bool value)
        {
            DrawSquare(data, dataSizeX, dataSizeY, X, Y, sizeX, sizeY,
                value ? (byte)255 : (byte)0,
                value ? (byte)255 : (byte)0,
                value ? (byte)255 : (byte)0, 255);
        }
        bool GetBit(byte currentByte, int index)
        {
            return (currentByte & (1 << index)) != 0;
        }

        static void ScaleImage(byte[] src, int srcW, int srcH, int scale, byte[] dst)
        {
            if (dst.Length != srcW * scale * srcH * scale * 4)
                return;

            int dstW = srcW * scale;
            int srcStrideBytes = srcW * 4;
            int dstStrideBytes = dstW * 4;

            for (int y = 0; y < srcH; y++)
            {
                int srcRowOfs = y * srcStrideBytes;
                int dstRowOfs = (y * scale) * dstStrideBytes;

                int dstWriteOfs = dstRowOfs;
                for (int x = 0; x < srcW; x++)
                {
                    Buffer.BlockCopy(src, srcRowOfs + x * 4, dst, dstWriteOfs, 4);

                    for (int s = 1; s < scale; s++)
                    {
                        Buffer.BlockCopy(dst, dstWriteOfs,
                                         dst, dstWriteOfs + s * 4,
                                         4);
                    }

                    dstWriteOfs += scale * 4;
                }
                for (int s = 1; s < scale; s++)
                {
                    Buffer.BlockCopy(dst, dstRowOfs,
                                     dst, dstRowOfs + s * dstStrideBytes,
                                     dstStrideBytes);
                }
            }
        }

        byte[] output = new byte[0];
        byte[] rawData = new byte[0];
        byte[] preview = new byte[0];
        public void DrawData(byte[] combinedData)
        {
            long checkTimestamp;
            checkTimestamp = Stopwatch.GetTimestamp();
            long startTimestamp = checkTimestamp;

            string debug = "";
            outputScale = 16;
            previewScale = 4;
            bladeSizeX = 120;
            bladeSizeY = 13;

            string selectedItem = rigTypeDropdown.SelectedItem as string;

            //largeCRC.Enabled = (selectedItem == "Binary");
            selectRig.Enabled = (selectedItem == "FRig");
            LoadLayout.Enabled = !(selectedItem == "Binary");
            UnloadLayout.Enabled = !(selectedItem == "Binary");
            if (framerate)
                debug += "FRAMERATE\n";
            else
                debug += "\n";
            debug += "setup: " + (Stopwatch.GetTimestamp() - checkTimestamp) * 1000 * 1000 / Stopwatch.Frequency + "\n";
            checkTimestamp = Stopwatch.GetTimestamp();

            if (selectedItem == "VRSL")
            {
                if (customLayout)
                {
                    bladeSizeX = customLayoutBladeSizeX;
                    bladeSizeY = customLayoutBladeSizeY;
                }

                rawData = new byte[bladeSizeX * bladeSizeY * 4];
                int countX = bladeSizeX;
                int countY = bladeSizeY;
                int maxUniverse = 3;
                if (customLayout)
                    maxUniverse = 8;

                for (int universe = 0; universe < maxUniverse; universe++)
                {
                    for (int i = 0; i < 512; i++)
                    {
                        int channel = i + universe * 512;
                        if (channel >= combinedData.Length)
                            break;
                        byte data = combinedData[channel];

                        channel = i + universe * 520;
                        int x = channel / countY;
                        int y = channel % countY;

                        if (customLayout)
                        {
                            if (channel >= layoutMapping.Count)
                                continue;
                            x = layoutMapping[channel].x;
                            y = layoutMapping[channel].y;
                        }

                        SetPixel(rawData, bladeSizeX, bladeSizeY, x, y, data, data, data, 255);
                    }
                }
            }
            else if (selectedItem == "FRig")
            {
                if (customLayout)
                {
                    bladeSizeX = customLayoutBladeSizeX;
                    bladeSizeY = customLayoutBladeSizeY;
                }

                rawData = new byte[bladeSizeX * bladeSizeY * 4];
                int countX = bladeSizeX;
                int countY = bladeSizeY;
                if (currentRig != null && currentRig.Fixtures != null)
                {
                    int index = 0;
                    foreach (Fixture fixture in currentRig.Fixtures)
                    {
                        int gridX = fixture.GridChannel / countY;
                        int gridY = fixture.GridChannel % countY;
                        if (customLayout)
                        {
                            if (fixture.GridChannel >= layoutMapping.Count)
                                continue;
                            gridX = layoutMapping[fixture.GridChannel].x;
                            gridY = layoutMapping[fixture.GridChannel].y;
                        }
                        int pixelX = gridX;
                        int pixelY = gridY;
                        byte data = 0;

                        if (IsValidIndex(combinedData, fixture.UnityChannel - 1))
                            data = combinedData[fixture.UnityChannel - 1];
                
                        int color = fixture.GridColor;

                        if (fixture.GridColor == 0)
                        {
                            SetPixel(rawData, bladeSizeX, bladeSizeY, pixelX, pixelY, data, data, data, 255, true, false, false, true);
                        }
                        if (fixture.GridColor == 1)
                        {
                            SetPixel(rawData, bladeSizeX, bladeSizeY, pixelX, pixelY, data, data, data, 255, false, true, false, true);
                        }
                        if (fixture.GridColor == 2)
                        {
                            SetPixel(rawData, bladeSizeX, bladeSizeY, pixelX, pixelY, data, data, data, 255, false, false, true, true);
                        }
                        if (fixture.GridColor == 3)
                        {
                            SetPixel(rawData, bladeSizeX, bladeSizeY, pixelX, pixelY, data, data, data, 255);
                        }

                        index++;
                    }
                }
                else
                {
                    var frigFile = new FRigFile();
                    if (FuralityGridNode.Properties.Resources.Blockout != null)
                    {
                        var frigBase = Encoding.UTF8.GetString(FuralityGridNode.Properties.Resources.Blockout);
                        currentRig = frigFile.LoadFromJsonString(frigBase).ConvertToFRig();
                    }
                }
            }
            else if (selectedItem == "Binary")
            {
                outputScale = 4;
                previewScale = 1;
                int turboExpandSize = 1;
                if (turboExpand.Checked)
                    turboExpandSize = 5;

                bladeSizeX = res1440p.Checked ? 640 : 480;
                bladeSizeY = 52 * turboExpandSize;

                rawData = new byte[bladeSizeX * bladeSizeY * 4];

                for (int i = 0; i < combinedData.Length; i++)
                {
                    int turboExpandOffset = i / 6 / bladeSizeX;
                    int x = (i / 6) % bladeSizeX;
                    int y = i % 6;
                    byte currentByte = combinedData[i];
                    for (int j = 0; j < 8; j++)
                    {
                        int y2 = y * 8 + j;
                        bool value = GetBit(currentByte, 7 - j);
                        SetPixel(rawData, bladeSizeX, bladeSizeY, x, y2 + turboExpandOffset * (bladeSizeY / turboExpandSize), value);
                    }
                    // at the end of each row, calculate crc
                    if (y == 5) 
                    {
                        byte mask = Crc4For6(combinedData[i - 5], combinedData[i - 4], combinedData[i - 3], combinedData[i - 2], combinedData[i - 1], combinedData[i - 0]);
                        for (int j = 0; j < 4; j++)
                        {
                            bool value = GetBit(mask, 7 - j);
                            SetPixel(rawData, bladeSizeX, bladeSizeY, x, (6 * 8 + j) + turboExpandOffset * (bladeSizeY / turboExpandSize), value);
                        }
                    }
                }
            }
            else
            {
                return;
            }

            if (midiOutput != null)
            {
                if (isMidiReady())
                {
                    //Midi updates
                    int midiUpdates = 0;
                    int midiCap = bigDataCheck.Checked ? 3200 : 100;
                    int scanCap = bigDataCheck.Checked ? 100 : 10;
                    int maxMidiChannels = bigDataCheck.Checked ? 16384 : 4096;
                    for (int i = midiCatchup; i < combinedData.Length; i++)
                    {
                        if ((combinedData[i] != midiData[i] || (i >= midiScanPosition && i < midiScanPosition + scanCap)) && i < maxMidiChannels)
                        {
                            midiUpdates++;
                            if (midiUpdates >= midiCap)
                            {
                                midiCatchup = i;
                                break;
                            }
                            midiData[i] = combinedData[i];

                            int bank = i / 2048;

                            if (bank != bankStatus)
                            {
                                ChangeBanks(bank);
                                midiUpdates++;
                            }

                            int t = i - (bank * 2048);

                            if (t < 1024)
                            {
                                NoteOnEvent noteOn = new NoteOnEvent();
                                noteOn.Channel = (FourBitNumber)((t >> 6) & 0xF);
                                noteOn.NoteNumber = (SevenBitNumber)(((t << 1) & 0x7F) + ((combinedData[i] >> 7) & 0x1));
                                noteOn.Velocity = (SevenBitNumber)(combinedData[i] & 0x7F);
                                midiOutput.SendEvent(noteOn);
                            }
                            else {
                                t = t - 1024;
                                NoteOffEvent noteOff = new NoteOffEvent();
                                noteOff.Channel = (FourBitNumber)((t >> 6) & 0xF);
                                noteOff.NoteNumber = (SevenBitNumber)(((t << 1) & 0x7F) + ((combinedData[i] >> 7) & 0x1));
                                noteOff.Velocity = (SevenBitNumber)(combinedData[i] & 0x7F);
                                midiOutput.SendEvent(noteOff);
                            }
                        }
                    }

                    if (midiUpdates < midiCap)
                    {
                        midiCatchup = 0;
                    }

                    midiStatus.Text = "Connected - Sending Data";
                    midiStatus.ForeColor = Color.Black;

                    midiScanPosition += scanCap;
                    if (midiScanPosition > maxMidiChannels)
                    {
                        midiScanPosition = 0;
                    }

                    midiWatchdog();
                    midiUpdate = Stopwatch.GetTimestamp();
                } else
                {
                    float midiTimeout = (float) (Stopwatch.GetTimestamp() - midiUpdate) / (float) Stopwatch.Frequency;
                    if (midiTimeout > 1)
                    {
                        midiCatchup = 0;
                        midiStatus.Text = "Connected - Waiting";
                        midiStatus.ForeColor = Color.Black;

                        midiReset();

                        midiUpdate = Stopwatch.GetTimestamp();
                    }
                }
            }
            
            debug += "render: " + (Stopwatch.GetTimestamp() - checkTimestamp) * 1000 * 1000 / Stopwatch.Frequency + "\n";
            checkTimestamp = Stopwatch.GetTimestamp();
            // scale up
            //if (output.Length != bladeSizeX * bladeSizeY * 4 * outputScale * outputScale)
                output = new byte[bladeSizeX * bladeSizeY * 4 * outputScale * outputScale];

            string spoutStatus = $"Spout Output - {bladeSizeX * outputScale}x{bladeSizeY * outputScale}";
            if (spoutStatus != gridPreview.Text)
            {
                gridPreview.Text = spoutStatus;
            }

            ScaleImage(rawData, bladeSizeX, bladeSizeY, outputScale, output);

            debug += "scale output: " + (Stopwatch.GetTimestamp() - checkTimestamp) * 1000 * 1000 / Stopwatch.Frequency + "\n";
            checkTimestamp = Stopwatch.GetTimestamp();

            // send to spout
            SpoutWrapper.SendImage(output, bladeSizeX * outputScale, bladeSizeY * outputScale);

            debug += "spout send: " + (Stopwatch.GetTimestamp() - checkTimestamp) * 1000 * 1000 / Stopwatch.Frequency + "\n";
            checkTimestamp = Stopwatch.GetTimestamp();

            // draw preview
            int previewSizeX = (bladeSizeX * previewScale);
            int previewSizeY = (bladeSizeY * previewScale);
            //if (preview.Length != previewSizeX * previewSizeY * 4)
               preview = new byte[previewSizeX * previewSizeY * 4];
            for (int i = 0; i < (previewSizeX * previewSizeY); i++)
            {
                int x = (i % previewSizeX);
                int y = (i / previewSizeX);

                int i2 = x + y * previewSizeX;

                int j = ((x / previewScale) + (y / previewScale) * (previewSizeX / previewScale));
                preview[i*4+0] = rawData[j*4+0]; // R
                preview[i*4+1] = rawData[j*4+1]; // G
                preview[i*4+2] = rawData[j*4+2]; // B
                preview[i*4+3] = rawData[j*4+3]; // A
            }

            debug += "scale preview: " + (Stopwatch.GetTimestamp() - checkTimestamp) * 1000 * 1000 / Stopwatch.Frequency + "\n";
            checkTimestamp = Stopwatch.GetTimestamp();
            // win32 fast draw byte[] to a control. Idk why the default C# functions are so disgustingly slow.
            GCHandle pinnedArray = GCHandle.Alloc(preview, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();
            Graphics gr = gridPreview.CreateGraphics();
            IntPtr hdc = gr.GetHdc();
            BITMAPINFO bmi = new BITMAPINFO();
            bmi.bmiHeader = new BITMAPINFOHEADER
            {
                biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER)),
                biWidth = previewSizeX,
                biHeight = -previewSizeY, // Negative height to indicate a top-down DIB
                biPlanes = 1,
                biBitCount = 32,
                biCompression = 0, // BI_RGB
                biSizeImage = (uint)(previewSizeX * previewSizeY) // pixel count
            };
            StretchDIBits(hdc, 8, 16, previewSizeX, previewSizeY, 0, 0, previewSizeX, previewSizeY, pointer, ref bmi, 0, 0x00CC0020);
            gr.ReleaseHdc(hdc);
            pinnedArray.Free();

            debug += "draw: " + (Stopwatch.GetTimestamp() - checkTimestamp) * 1000 * 1000 / Stopwatch.Frequency + "\n";
            long totalTime = (Stopwatch.GetTimestamp() - startTimestamp) * 1000 * 1000 / Stopwatch.Frequency;
            framerate = totalTime > 33000;
            if(framerate)
                slow.ForeColor = Color.Red;
            else
                slow.ForeColor = Color.Black;
        }

        bool framerate = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            StartArtNetClient();
            SpoutWrapper.CreateSender("Furality Grid Node");
            //layoutStatus.Text = $"VRSL\nsize: 1920x208\nchannels: 1560";
        }

        void StartArtNetClient()
        {
            artnetClient = new ArtNet(ipInput.Text, portInput.Text);
            artnetClient.StartClient();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (testAnimationTime > 0)
            {
                float sin2 = (float)Math.Sin(testAnimationTime * 8.0f) * 0.5f + 0.5f;
                byte[] data;
                if (customLayout)
                    data = new byte[layoutMapping.Count*4];
                else
                    data = new byte[(int)(512 * 16 * sin2)];

                for (int i = 0; i < data.Length; i++)
                {
                    float sin = (float)Math.Sin(i / 80.0f + testAnimationTime * 4.0f);
                    if (sin < 0)
                        sin += 1;
                    float t = (float)Math.Min(Math.Max(sin, 0), 1);
                    float fade = Math.Min(Math.Max(8 - Math.Abs(testAnimationTime * 16 - 8), 0), 1);
                    data[i] = (byte)(t*fade*255);
                }
                DrawData(data);
                testAnimationTime -= 0.0025f;
                return;
            }
#if DEBUG
           //Trace.WriteLine($"ArtNet Status: {artnetClient.status}");
#endif
            if (artnetClient.status == ArtNet.ArtNetClientStatus.Waiting)
                update = true;
            if (artnetClient.status == ArtNet.ArtNetClientStatus.ReceivingData)
                update = true;
            if (artnetClient.status != ArtNet.ArtNetClientStatus.Disconnected)
                update = true;
            if (artnetClient.status != ArtNet.ArtNetClientStatus.Error)
                update = true;

            if (update)
            {
                update = false;
                DrawData(artnetClient.combinedData);
            }

            if (statusTextLast != statusText)
            {
                statusLabel.Text = statusText;
                statusTextLast = statusText;
            }
            statusLabel.Text = artnetClient.status.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }

        private void colorTypeDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (artnetClient != null)
                DrawData(artnetClient.combinedData);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }

        private void inputChanged_TextChanged(object sender, EventArgs e)
        {
            if(artnetClient != null)
                DrawData(artnetClient.combinedData);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Trace.WriteLine("Button1 Clicked");
            if (artnetClient != null)
            {
                artnetClient.Unicast = unicast.Checked;
                artnetClient.RestartClient(ipInput.Text, portInput.Text);
            }
        }

        private void selectRig_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "FRig Files (*.frig)|*.frig|All Files (*.*)|*.*";
                openFileDialog.Title = "Select FRig File";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
#if DEBUG
                    Trace.WriteLine($"Yay it found the file, path: {filePath}");
#endif
                    LoadFRigFile(filePath);
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
            Application.Exit();
            Environment.Exit(0);
        }

        private FRig currentRig;
        private void LoadFRigFile(string filePath)
        {
            try
            {
                // Use the static FromJson method here too for consistency and error handling
                FRigFile frigFile = new FRigFile();
                frigFile = frigFile.LoadFromFile(filePath);
                currentRig = frigFile.ConvertToFRig();
                if (currentRig != null)
                {
                    Trace.WriteLine($"FRig file loaded");
                    if (currentRig.Fixtures != null)
                        Trace.WriteLine($"FRig fixtures loaded successfully. Version: Micca Broke it, Fixtures: {currentRig.Fixtures.Length}");
                } else {
                     Trace.WriteLine($"Failed to load FRig file: {filePath}");
                     // Optionally show a MessageBox error here as well
                }
            }
            catch (Exception ex)
            {
                // Catch potential File IO errors or other unexpected exceptions
                MessageBox.Show($"Error loading FRig file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                currentRig = null; // Ensure rig is null on error
            }
        }


        [Serializable]
        struct DMXCoord
        {
            public float uvX;
            public float uvY;
            public int dmxX;
            public int dmxY;
            public int channel;
        }

        [Serializable]
        class DMXLayout
        {
            public int resolutionX;
            public int resolutionY;
            public int dmxSizeX;
            public int dmxSizeY;
            public int channelCount;
            public List<DMXCoord> coords = new List<DMXCoord>();
        }
        Dictionary<int, (int x, int y)> layoutMapping;

        private void LoadLayout_Click(object sender, EventArgs e)
        {
            layoutStatus.Text = "Loading...";

            OpenFileDialog f = new OpenFileDialog();
            f.Title = "Select Layout File.";
            f.Filter = "Json files (*.json) | *.json";

            if (f.ShowDialog() != DialogResult.OK)
                return;

            if (!File.Exists(f.FileName))
                return;

            string json = File.ReadAllText(f.FileName);
            DMXLayout layout = JsonSerializer.Deserialize<DMXLayout>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive=false, IncludeFields = true, WriteIndented = true });

            //this.Size = new Size(layout.resolutionX, layout.resolutionY);

            customLayoutBladeSizeX = layout.dmxSizeX;
            customLayoutBladeSizeY = layout.dmxSizeY;

            layoutMapping = new Dictionary<int, (int x, int y)>();
            for (int i = 0; i < layout.coords.Count; i++)
            {
                if (layout.coords[i].uvX < 0 || layout.coords[i].uvX > 1 || layout.coords[i].uvY < 0 || layout.coords[i].uvY > 1)
                {
                    MessageBox.Show("Layout file had data outside of the screen and will not load.", "Layout file error.");
                    //layoutStatus.Text = $"VRSL\nsize: 1920x208\nchannels: 1560";
                    return;
                }
                layoutMapping.Add(i, ((layout.coords[i].dmxX), (layout.coords[i].dmxY)));
            }

            //layoutStatus.Text = $"{Path.GetFileNameWithoutExtension(f.FileName)}\nsize: {layout.resolutionX}x{layout.resolutionY}\nchannels: {layout.channelCount}";
            customLayout = true;
            layoutStatus.Text = "";
        }

        private void UnloadLayout_Click(object sender, EventArgs e)
        {
            customLayout = false;
            //bladeSizeX = 120;
            //bladeSizeY = 13;
            //layoutStatus.Text = $"VRSL\nsize: 1920x208\nchannels: 1560";
        }


        private void groupBox3_Enter(object sender, EventArgs e)
        {
            
        }

        private void populateMidi()
        {
            string selected = midiSavedDevice;
            if (midiDevice.SelectedItem != null)
            {
                selected = midiDevice.SelectedItem.ToString();
            }

            midiDevice.Items.Clear();
            ICollection<OutputDevice> devices = OutputDevice.GetAll();

            midiDevice.Items.Add("(none)");
            if (selected == null)
            {
                midiDevice.SelectedIndex = 0;
            }

            foreach (OutputDevice device in devices)
            {
                int t = midiDevice.Items.Add(device.Name);
                if (device.Name == selected)
                {
                    midiDevice.SelectedIndex = t;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (midiDevice.SelectedIndex == 0)
            {
                midiStatus.Text = "Disconnected";
                if (midiOutput != null)
                    midiOutput.Dispose();
                midiOutput = null;
            }
            else
            {
                connectMidi();
            }

            SaveSettings();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            populateMidi();
            connectMidi();
            midiReset();
        }

        private void connectMidi()
        {
            if (midiOutput != null)
            {
                midiOutput.Dispose();
                midiOutput = null;
            }

            try
            {
                if (midiDevice.SelectedItem != null)
                {
                    midiOutput = OutputDevice.GetByName(midiDevice.SelectedItem.ToString());
                    midiStatus.Text = "Connected";
                }
                else
                {
                    midiStatus.Text = "Failed to connect";
                }
            }
            catch
            {
                midiStatus.Text = "Failed to connect";
            }
        }

        private void findVRCLog()
        {
            if (logStream != null)
            {
                logStream.Close();
                logStream = null;
            }

            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string log = "";
            if (editorCheck.Checked)
            {
                log = path + "\\..\\Local\\Unity\\Editor\\Editor.log";
            }
            else
            {
                string[] logs = Directory.GetFiles(path + "\\..\\LocalLow\\VRChat\\VRChat", "output_log_*.txt", SearchOption.TopDirectoryOnly);
                if (logs.Length == 0) return;

                Array.Sort(logs);
                log = logs[logs.Length - 1];
            }

            logStream = new FileStream(log, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            //forward to the end to wait on it
            if (logStream.Length != 0)
            {
                logStream.Position = logStream.Length - 1;
            }

            //logStream.ReadTimeout = 10;
        }

        private bool isMidiReady()
        {
            if (logStream == null)
            {
                findVRCLog();
                return false;
            }

            int length = (int) (logStream.Length - logStream.Position);

            byte[] searchWord = { (byte) 'M', (byte)'I', (byte)'D', (byte)'I', (byte)'R', (byte)'E', (byte)'A', (byte)'D', (byte)'Y', };

            if (length > 1)
            {
                int c;
                int i = 0;
                while ((c = logStream.ReadByte()) != -1) { 
                    if (c == searchWord[i])
                    {
                        i++;

                        if (i >= searchWord.Length)
                        {
                            logStream.Position = logStream.Length - 1;
                            return true;
                        }
                    }
                }
            } else
            {
                return false;
            }

            return false;
        }

        //Todo: Wait for callback to ensure the bank swap was triggered
        private void ChangeBanks(int bank)
        {
            bankStatus = bank;

            SendMidiControl(15, 127, bank);
        }

        private void midiWatchdog()
        {
            if (midiOutput != null)
            {
                SendMidiControl(15, 127, 127);
            }
        }

        private void SendMidiControl(int channel, int control, int value)
        {
            if (midiOutput != null)
            {
                ControlChangeEvent midiWD = new ControlChangeEvent();
                midiWD.Channel = (FourBitNumber)channel;
                midiWD.ControlNumber = (SevenBitNumber)control;
                midiWD.ControlValue = (SevenBitNumber)value;

                midiOutput.SendEvent(midiWD);
            }
        }

        //Unlocks world receiver
        private void midiKnock()
        {
            SendMidiControl(15, 127, 101);
            SendMidiControl(15, 127, 120);
            SendMidiControl(15, 127, 107);
        }

        private void midiReset()
        {
            midiData = new byte[16384];
            midiScanPosition = 0;
            midiCatchup = 0;

            findVRCLog();
            ChangeBanks(0);
            midiKnock();
            midiWatchdog();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            midiReset();
        }

        private void layoutStatus_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            int previewSizeX = (bladeSizeX * previewScale);
            int previewSizeY = (bladeSizeY * previewScale);

            int outputSizeX = bladeSizeX * outputScale;
            int outputSizeY = bladeSizeY * outputScale;

            int fileSizeY = outputSizeY;
            if(expandScreenshotTo16by9.Checked) {
              fileSizeY = (outputSizeX / 16) * 9;
            }

            Bitmap image = new Bitmap(outputSizeX, fileSizeY, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GCHandle pinnedArray = GCHandle.Alloc(preview, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();
            Graphics gr = Graphics.FromImage(image);
            IntPtr hdc = gr.GetHdc();
            BITMAPINFO bmi = new BITMAPINFO();
            bmi.bmiHeader = new BITMAPINFOHEADER
            {
                biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER)),
                biWidth = previewSizeX,
                biHeight = -previewSizeY, // Negative height to indicate a top-down DIB
                biPlanes = 1,
                biBitCount = 32,
                biCompression = 0, // BI_RGB
                biSizeImage = (uint)(previewSizeX * previewSizeY) // pixel count
            };
            StretchDIBits(hdc, 0, 0, outputSizeX, outputSizeY, 0, 0, previewSizeX, previewSizeY, pointer, ref bmi, 0, 0x00CC0020);
            gr.ReleaseHdc(hdc);
            pinnedArray.Free();

            string filename = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss fff");

            image.Save($"{filename}.png", ImageFormat.Png);
        }


        float testAnimationTime = 0;
        private void testAnimation_Click(object sender, EventArgs e)
        {
            // clear all overwrites when test animation plays
            testAnimationTime = 1.0f;
        }

        // for now we assume the users doesn't put very many overwrites.
        //private void testingSet_Click(object sender, EventArgs e)
        //{
        //    int universe;
        //    int channel;
        //    int value;
        //    if (!int.TryParse(testingUniverse.Text, out universe))
        //        return;
        //    if (!int.TryParse(testingChannel.Text, out channel))
        //        return;
        //    if (!int.TryParse(testingValue.Text, out value))
        //        return;
        //
        //    value = Math.Min(Math.Max(value, 0), 255);
        //    channel = Math.Min(Math.Max(channel, 0), 511);
        //    universe = Math.Min(Math.Max(universe, 1), 100);
        //
        //    artnetClient.combinedData[(universe-1) * 512 + channel] = (byte)value;
        //}

        private void testingClearAll_Click(object sender, EventArgs e)
        {
            Array.Clear(artnetClient.combinedData, 0, artnetClient.combinedData.Length);
        }

        private void checkBox1_CheckedChanged_2(object sender, EventArgs e)
        {

        }

        private void turboExpand_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
