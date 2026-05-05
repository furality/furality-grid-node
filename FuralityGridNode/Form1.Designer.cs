using System.Media;

namespace FuralityGridNode
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.helloToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.gridPreview = new System.Windows.Forms.GroupBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.res1440p = new System.Windows.Forms.CheckBox();
      this.slow = new System.Windows.Forms.Label();
      this.turboExpand = new System.Windows.Forms.CheckBox();
      this.largeCRC = new System.Windows.Forms.CheckBox();
      this.layoutStatus = new System.Windows.Forms.Label();
      this.rigTypeDropdown = new System.Windows.Forms.ComboBox();
      this.UnloadLayout = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.selectRig = new System.Windows.Forms.Button();
      this.LoadLayout = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.statusLabel = new System.Windows.Forms.Label();
      this.ipInput = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.portInput = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.unicast = new System.Windows.Forms.CheckBox();
      this.connect = new System.Windows.Forms.Button();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.testingPlayAnimation = new System.Windows.Forms.Button();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.bigDataCheck = new System.Windows.Forms.CheckBox();
      this.editorCheck = new System.Windows.Forms.CheckBox();
      this.midiConnect = new System.Windows.Forms.Button();
      this.midiDevice = new System.Windows.Forms.ComboBox();
      this.midiStatus = new System.Windows.Forms.Label();
      this.button1 = new System.Windows.Forms.Button();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.label5 = new System.Windows.Forms.Label();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.label4 = new System.Windows.Forms.Label();
      this.expandScreenshotTo16by9 = new System.Windows.Forms.CheckBox();
      this.contextMenuStrip1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.SuspendLayout();
      // 
      // contextMenuStrip1
      // 
      this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
      this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helloToolStripMenuItem,
            this.exitToolStripMenuItem});
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new System.Drawing.Size(111, 48);
      // 
      // helloToolStripMenuItem
      // 
      this.helloToolStripMenuItem.Name = "helloToolStripMenuItem";
      this.helloToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
      this.helloToolStripMenuItem.Text = "Config";
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
      this.exitToolStripMenuItem.Text = "Exit";
      this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
      // 
      // gridPreview
      // 
      this.gridPreview.Location = new System.Drawing.Point(474, 6);
      this.gridPreview.Name = "gridPreview";
      this.gridPreview.Size = new System.Drawing.Size(671, 180);
      this.gridPreview.TabIndex = 17;
      this.gridPreview.TabStop = false;
      this.gridPreview.Text = "Spout Output";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.res1440p);
      this.groupBox2.Controls.Add(this.slow);
      this.groupBox2.Controls.Add(this.turboExpand);
      this.groupBox2.Controls.Add(this.largeCRC);
      this.groupBox2.Controls.Add(this.layoutStatus);
      this.groupBox2.Controls.Add(this.rigTypeDropdown);
      this.groupBox2.Controls.Add(this.UnloadLayout);
      this.groupBox2.Controls.Add(this.label3);
      this.groupBox2.Controls.Add(this.selectRig);
      this.groupBox2.Controls.Add(this.LoadLayout);
      this.groupBox2.Location = new System.Drawing.Point(238, 6);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(230, 209);
      this.groupBox2.TabIndex = 16;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Rig";
      // 
      // res1440p
      // 
      this.res1440p.AutoSize = true;
      this.res1440p.Location = new System.Drawing.Point(131, 42);
      this.res1440p.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
      this.res1440p.Name = "res1440p";
      this.res1440p.Size = new System.Drawing.Size(56, 17);
      this.res1440p.TabIndex = 18;
      this.res1440p.Text = "1440p";
      this.res1440p.UseVisualStyleBackColor = true;
      // 
      // slow
      // 
      this.slow.AutoSize = true;
      this.slow.Location = new System.Drawing.Point(128, 68);
      this.slow.Name = "slow";
      this.slow.Size = new System.Drawing.Size(0, 13);
      this.slow.TabIndex = 17;
      // 
      // turboExpand
      // 
      this.turboExpand.AutoSize = true;
      this.turboExpand.Location = new System.Drawing.Point(131, 21);
      this.turboExpand.Name = "turboExpand";
      this.turboExpand.Size = new System.Drawing.Size(93, 17);
      this.turboExpand.TabIndex = 16;
      this.turboExpand.Text = "Turbo Expand";
      this.turboExpand.UseVisualStyleBackColor = true;
      this.turboExpand.CheckedChanged += new System.EventHandler(this.turboExpand_CheckedChanged);
      // 
      // largeCRC
      // 
      this.largeCRC.AutoSize = true;
      this.largeCRC.Location = new System.Drawing.Point(131, 62);
      this.largeCRC.Name = "largeCRC";
      this.largeCRC.Size = new System.Drawing.Size(71, 17);
      this.largeCRC.TabIndex = 15;
      this.largeCRC.Text = "8-bit CRC";
      this.largeCRC.UseVisualStyleBackColor = true;
      this.largeCRC.Visible = false;
      // 
      // layoutStatus
      // 
      this.layoutStatus.Location = new System.Drawing.Point(7, 130);
      this.layoutStatus.Name = "layoutStatus";
      this.layoutStatus.Size = new System.Drawing.Size(217, 55);
      this.layoutStatus.TabIndex = 13;
      this.layoutStatus.Click += new System.EventHandler(this.layoutStatus_Click);
      // 
      // rigTypeDropdown
      // 
      this.rigTypeDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.rigTypeDropdown.FormattingEnabled = true;
      this.rigTypeDropdown.Items.AddRange(new object[] {
            "VRSL",
            "FRig",
            "Binary"});
      this.rigTypeDropdown.Location = new System.Drawing.Point(10, 19);
      this.rigTypeDropdown.Name = "rigTypeDropdown";
      this.rigTypeDropdown.Size = new System.Drawing.Size(64, 21);
      this.rigTypeDropdown.TabIndex = 7;
      this.rigTypeDropdown.SelectedIndexChanged += new System.EventHandler(this.colorTypeDropdown_SelectedIndexChanged);
      this.rigTypeDropdown.TextChanged += new System.EventHandler(this.inputChanged_TextChanged);
      // 
      // UnloadLayout
      // 
      this.UnloadLayout.Location = new System.Drawing.Point(10, 104);
      this.UnloadLayout.Name = "UnloadLayout";
      this.UnloadLayout.Size = new System.Drawing.Size(104, 23);
      this.UnloadLayout.TabIndex = 13;
      this.UnloadLayout.Text = "Unload Layout";
      this.UnloadLayout.UseVisualStyleBackColor = true;
      this.UnloadLayout.Click += new System.EventHandler(this.UnloadLayout_Click);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(80, 22);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(31, 13);
      this.label3.TabIndex = 14;
      this.label3.Text = "Type";
      // 
      // selectRig
      // 
      this.selectRig.Location = new System.Drawing.Point(10, 46);
      this.selectRig.Name = "selectRig";
      this.selectRig.Size = new System.Drawing.Size(104, 23);
      this.selectRig.TabIndex = 10;
      this.selectRig.Text = "Select FRig";
      this.selectRig.UseVisualStyleBackColor = true;
      this.selectRig.Click += new System.EventHandler(this.selectRig_Click);
      // 
      // LoadLayout
      // 
      this.LoadLayout.Location = new System.Drawing.Point(10, 75);
      this.LoadLayout.Name = "LoadLayout";
      this.LoadLayout.Size = new System.Drawing.Size(104, 23);
      this.LoadLayout.TabIndex = 12;
      this.LoadLayout.Text = "Load Layout";
      this.LoadLayout.UseVisualStyleBackColor = true;
      this.LoadLayout.Click += new System.EventHandler(this.LoadLayout_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.statusLabel);
      this.groupBox1.Controls.Add(this.ipInput);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.portInput);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.unicast);
      this.groupBox1.Controls.Add(this.connect);
      this.groupBox1.Location = new System.Drawing.Point(6, 6);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(226, 127);
      this.groupBox1.TabIndex = 15;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "ArtNet";
      // 
      // statusLabel
      // 
      this.statusLabel.AutoSize = true;
      this.statusLabel.Location = new System.Drawing.Point(87, 96);
      this.statusLabel.Name = "statusLabel";
      this.statusLabel.Size = new System.Drawing.Size(37, 13);
      this.statusLabel.TabIndex = 12;
      this.statusLabel.Text = "Status";
      // 
      // ipInput
      // 
      this.ipInput.Location = new System.Drawing.Point(9, 16);
      this.ipInput.Name = "ipInput";
      this.ipInput.Size = new System.Drawing.Size(89, 20);
      this.ipInput.TabIndex = 0;
      this.ipInput.Text = "127.0.0.1";
      this.ipInput.TextChanged += new System.EventHandler(this.inputChanged_TextChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(104, 19);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(58, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "IP Address";
      // 
      // portInput
      // 
      this.portInput.Location = new System.Drawing.Point(9, 42);
      this.portInput.Name = "portInput";
      this.portInput.Size = new System.Drawing.Size(44, 20);
      this.portInput.TabIndex = 1;
      this.portInput.Text = "6454";
      this.portInput.TextChanged += new System.EventHandler(this.inputChanged_TextChanged);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(58, 45);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(26, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Port";
      // 
      // unicast
      // 
      this.unicast.AutoSize = true;
      this.unicast.Checked = true;
      this.unicast.CheckState = System.Windows.Forms.CheckState.Checked;
      this.unicast.Location = new System.Drawing.Point(9, 68);
      this.unicast.Name = "unicast";
      this.unicast.Size = new System.Drawing.Size(62, 17);
      this.unicast.TabIndex = 11;
      this.unicast.Text = "Unicast";
      this.unicast.UseVisualStyleBackColor = true;
      // 
      // connect
      // 
      this.connect.Location = new System.Drawing.Point(6, 91);
      this.connect.Name = "connect";
      this.connect.Size = new System.Drawing.Size(75, 23);
      this.connect.TabIndex = 9;
      this.connect.Text = "Connect";
      this.connect.UseVisualStyleBackColor = true;
      this.connect.Click += new System.EventHandler(this.button1_Click);
      // 
      // timer1
      // 
      this.timer1.Enabled = true;
      this.timer1.Interval = 1;
      this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
      // 
      // testingPlayAnimation
      // 
      this.testingPlayAnimation.Location = new System.Drawing.Point(593, 192);
      this.testingPlayAnimation.Name = "testingPlayAnimation";
      this.testingPlayAnimation.Size = new System.Drawing.Size(122, 23);
      this.testingPlayAnimation.TabIndex = 15;
      this.testingPlayAnimation.Text = "Play Test Animation";
      this.testingPlayAnimation.UseVisualStyleBackColor = true;
      this.testingPlayAnimation.Click += new System.EventHandler(this.testAnimation_Click);
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.bigDataCheck);
      this.groupBox3.Controls.Add(this.editorCheck);
      this.groupBox3.Controls.Add(this.midiConnect);
      this.groupBox3.Controls.Add(this.midiDevice);
      this.groupBox3.Location = new System.Drawing.Point(6, 139);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(226, 71);
      this.groupBox3.TabIndex = 18;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "MIDIDMX";
      this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
      // 
      // bigDataCheck
      // 
      this.bigDataCheck.AutoSize = true;
      this.bigDataCheck.Location = new System.Drawing.Point(74, 49);
      this.bigDataCheck.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
      this.bigDataCheck.Name = "bigDataCheck";
      this.bigDataCheck.Size = new System.Drawing.Size(141, 17);
      this.bigDataCheck.TabIndex = 13;
      this.bigDataCheck.Text = "Big Data [client crashes]";
      this.bigDataCheck.UseVisualStyleBackColor = true;
      this.bigDataCheck.Visible = false;
      this.bigDataCheck.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged_2);
      // 
      // editorCheck
      // 
      this.editorCheck.AutoSize = true;
      this.editorCheck.Location = new System.Drawing.Point(6, 49);
      this.editorCheck.Name = "editorCheck";
      this.editorCheck.Size = new System.Drawing.Size(65, 17);
      this.editorCheck.TabIndex = 12;
      this.editorCheck.Text = "In Editor";
      this.editorCheck.UseVisualStyleBackColor = true;
      this.editorCheck.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
      // 
      // midiConnect
      // 
      this.midiConnect.Location = new System.Drawing.Point(162, 18);
      this.midiConnect.Name = "midiConnect";
      this.midiConnect.Size = new System.Drawing.Size(59, 23);
      this.midiConnect.TabIndex = 10;
      this.midiConnect.Text = "Refresh";
      this.midiConnect.UseVisualStyleBackColor = true;
      this.midiConnect.Click += new System.EventHandler(this.button1_Click_1);
      // 
      // midiDevice
      // 
      this.midiDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.midiDevice.FormattingEnabled = true;
      this.midiDevice.Location = new System.Drawing.Point(5, 20);
      this.midiDevice.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
      this.midiDevice.Name = "midiDevice";
      this.midiDevice.Size = new System.Drawing.Size(156, 21);
      this.midiDevice.TabIndex = 0;
      this.midiDevice.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
      // 
      // midiStatus
      // 
      this.midiStatus.AutoSize = true;
      this.midiStatus.Location = new System.Drawing.Point(93, 213);
      this.midiStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.midiStatus.Name = "midiStatus";
      this.midiStatus.Size = new System.Drawing.Size(37, 13);
      this.midiStatus.TabIndex = 11;
      this.midiStatus.Text = "Status";
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(474, 192);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(113, 23);
      this.button1.TabIndex = 19;
      this.button1.Text = "Save Screenshot";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click_2);
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tabPage1);
      this.tabControl1.Controls.Add(this.tabPage2);
      this.tabControl1.Location = new System.Drawing.Point(2, 2);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(1155, 255);
      this.tabControl1.TabIndex = 1;
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.expandScreenshotTo16by9);
      this.tabPage1.Controls.Add(this.gridPreview);
      this.tabPage1.Controls.Add(this.midiStatus);
      this.tabPage1.Controls.Add(this.groupBox1);
      this.tabPage1.Controls.Add(this.groupBox2);
      this.tabPage1.Controls.Add(this.button1);
      this.tabPage1.Controls.Add(this.testingPlayAnimation);
      this.tabPage1.Controls.Add(this.groupBox3);
      this.tabPage1.Controls.Add(this.label5);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
      this.tabPage1.Size = new System.Drawing.Size(1147, 229);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Main";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(3, 213);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(50, 13);
      this.label5.TabIndex = 13;
      this.label5.Text = "version 5";
      this.label5.Click += new System.EventHandler(this.label5_Click);
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.label4);
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Size = new System.Drawing.Size(1147, 229);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "creature??";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(431, 106);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(52, 13);
      this.label4.TabIndex = 15;
      this.label4.Text = "creature!!";
      // 
      // expandScreenshotTo16by9
      // 
      this.expandScreenshotTo16by9.AutoSize = true;
      this.expandScreenshotTo16by9.Location = new System.Drawing.Point(720, 196);
      this.expandScreenshotTo16by9.Margin = new System.Windows.Forms.Padding(2);
      this.expandScreenshotTo16by9.Name = "expandScreenshotTo16by9";
      this.expandScreenshotTo16by9.Size = new System.Drawing.Size(160, 17);
      this.expandScreenshotTo16by9.TabIndex = 19;
      this.expandScreenshotTo16by9.Text = "Expand Screenshots to 16:9";
      this.expandScreenshotTo16by9.UseVisualStyleBackColor = true;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1163, 258);
      this.ContextMenuStrip = this.contextMenuStrip1;
      this.Controls.Add(this.tabControl1);
      this.DoubleBuffered = true;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "Form1";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.Text = "Furality Grid Node";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.Load += new System.EventHandler(this.Form1_Load);
      this.contextMenuStrip1.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.tabPage1.PerformLayout();
      this.tabPage2.ResumeLayout(false);
      this.tabPage2.PerformLayout();
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helloToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox portInput;
        private System.Windows.Forms.TextBox ipInput;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ComboBox rigTypeDropdown;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button connect;
        private System.Windows.Forms.Button selectRig;
        private System.Windows.Forms.Button LoadLayout;
        private System.Windows.Forms.Button UnloadLayout;
        private System.Windows.Forms.CheckBox unicast;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label layoutStatus;
        private System.Windows.Forms.GroupBox gridPreview;
        private System.Windows.Forms.Button testingPlayAnimation;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox largeCRC;
        private System.Windows.Forms.CheckBox turboExpand;
        private System.Windows.Forms.Label slow;
        private System.Windows.Forms.ComboBox midiDevice;
        private System.Windows.Forms.Label midiStatus;
        private System.Windows.Forms.Button midiConnect;
        private System.Windows.Forms.CheckBox editorCheck;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox bigDataCheck;
        private System.Windows.Forms.CheckBox res1440p;
    private System.Windows.Forms.CheckBox expandScreenshotTo16by9;
  }
}

