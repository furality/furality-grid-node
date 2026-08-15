using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FuralityGridNode
{
    internal static class Program
    {
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        public static extern void TimeBeginPeriod(int t);

        [StructLayout(LayoutKind.Sequential)]
        struct PROCESS_POWER_THROTTLING_STATE
        {
            public uint version;
            public uint controlMask;
            public uint stateMask;
        }
        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        [DllImport("Kernel32.dll")]
        static extern bool SetProcessInformation(IntPtr process, int ProcessInformationClass, ref PROCESS_POWER_THROTTLING_STATE ProcessInformation, int size);

        [DllImport("kernel32.dll")]
        static extern uint GetLastError();

        [DllImport("kernel32.dll")]
        static extern bool SetPriorityClass(IntPtr process, uint PriorityClass);

        [DllImport("kernel32.dll")]
        static extern uint SetThreadExecutionState(uint flags);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // windows be fast please
            uint PROCESS_POWER_THROTTLING_CURRENT_VERSION = 1;
            uint PROCESS_POWER_THROTTLING_EXECUTION_SPEED = 0x1;
            uint PROCESS_POWER_THROTTLING_IGNORE_TIMER_RESOLUTION = 0x4; // windows 11 only??

            var state = new PROCESS_POWER_THROTTLING_STATE();
            state.version = PROCESS_POWER_THROTTLING_CURRENT_VERSION;
            state.controlMask = PROCESS_POWER_THROTTLING_EXECUTION_SPEED; // | PROCESS_POWER_THROTTLING_IGNORE_TIMER_RESOLUTION
            state.stateMask = 0;
            int size = Marshal.SizeOf<PROCESS_POWER_THROTTLING_STATE>();
            bool result = SetProcessInformation(GetCurrentProcess(), 4, ref state, 12);
            uint error = GetLastError();

            // please????
            TimeBeginPeriod(1);

            if(!SetPriorityClass(GetCurrentProcess(), 0x8000))
            {
                Debug.WriteLine("failed to set PriorityClass");
            }

            if (SetThreadExecutionState(0x80000000 | 0x00000001) == 0)
            {
                Debug.WriteLine("failed to set ThreadExecutionState");
            }

            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.AutoFlush = true;
            Trace.Indent();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            Trace.Unindent();
        }
    }
}
