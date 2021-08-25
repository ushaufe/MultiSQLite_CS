namespace SQLiteTest
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnShowManual = new System.Windows.Forms.Button();
            this.btnStartThreads = new System.Windows.Forms.Button();
            this.btnStopThreads = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.tcTabs = new System.Windows.Forms.TabControl();
            this.tsGeneral = new System.Windows.Forms.TabPage();
            this.rePrompt = new System.Windows.Forms.RichTextBox();
            this.tsApps = new System.Windows.Forms.TabPage();
            this.treeApps = new System.Windows.Forms.TreeView();
            this.tiPollApps = new System.Windows.Forms.Timer(this.components);
            this.tiUpdateApps = new System.Windows.Forms.Timer(this.components);
            this.lblNumThreads = new System.Windows.Forms.Label();
            this.numThreads = new System.Windows.Forms.NumericUpDown();
            this.lblVersion = new System.Windows.Forms.Label();
            this.tiConnectionQuery = new System.Windows.Forms.Timer(this.components);
            this.sqLiteCommandBuilder1 = new System.Data.SQLite.SQLiteCommandBuilder();
            this.sqLiteCommand1 = new System.Data.SQLite.SQLiteCommand();
            this.sqLiteCommand2 = new System.Data.SQLite.SQLiteCommand();
            this.tiLiveUpdate = new System.Windows.Forms.Timer(this.components);
            this.btnLiveUpdate = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOpenExternalDB = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuStartThread = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuStopThread = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuLiveUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuShowManual = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tiLiveUpdateFlicker = new System.Windows.Forms.Timer(this.components);
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.tbDBInterval = new System.Windows.Forms.TrackBar();
            this.LBL_DB_PollingInterval = new System.Windows.Forms.Label();
            this.tiScrollCaret = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.tcTabs.SuspendLayout();
            this.tsGeneral.SuspendLayout();
            this.tsApps.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThreads)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDBInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // btnShowManual
            // 
            this.btnShowManual.Image = global::SQLiteTest.Properties.Resources.user_manual_32;
            this.btnShowManual.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnShowManual.Location = new System.Drawing.Point(23, 465);
            this.btnShowManual.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnShowManual.Name = "btnShowManual";
            this.btnShowManual.Size = new System.Drawing.Size(201, 55);
            this.btnShowManual.TabIndex = 0;
            this.btnShowManual.Text = "User\'s Manual";
            this.btnShowManual.UseVisualStyleBackColor = true;
            this.btnShowManual.Click += new System.EventHandler(this.btnShowManual_Click);
            // 
            // btnStartThreads
            // 
            this.btnStartThreads.Image = global::SQLiteTest.Properties.Resources.Start_Process_32;
            this.btnStartThreads.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStartThreads.Location = new System.Drawing.Point(23, 214);
            this.btnStartThreads.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnStartThreads.Name = "btnStartThreads";
            this.btnStartThreads.Size = new System.Drawing.Size(201, 55);
            this.btnStartThreads.TabIndex = 2;
            this.btnStartThreads.Text = "Start Threads";
            this.btnStartThreads.UseVisualStyleBackColor = true;
            this.btnStartThreads.Click += new System.EventHandler(this.buttonStartThreads_Click);
            // 
            // btnStopThreads
            // 
            this.btnStopThreads.Enabled = false;
            this.btnStopThreads.Image = global::SQLiteTest.Properties.Resources.stop_process_2_321;
            this.btnStopThreads.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStopThreads.Location = new System.Drawing.Point(23, 299);
            this.btnStopThreads.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnStopThreads.Name = "btnStopThreads";
            this.btnStopThreads.Size = new System.Drawing.Size(201, 55);
            this.btnStopThreads.TabIndex = 3;
            this.btnStopThreads.Text = "Stop Threads";
            this.btnStopThreads.UseVisualStyleBackColor = true;
            this.btnStopThreads.Click += new System.EventHandler(this.btnStopThreads_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::SQLiteTest.Properties.Resources.Haufe_Transparent;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(1764, 48);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(427, 50);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImage = global::SQLiteTest.Properties.Resources.CS;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.Location = new System.Drawing.Point(1240, 110);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(115, 95);
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(247, 105);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(264, 52);
            this.label1.TabIndex = 8;
            this.label1.Text = "Multithread Connection \r\nTester for SQLite + C#";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox3.BackgroundImage = global::SQLiteTest.Properties.Resources._1920px_SQLite370_svg;
            this.pictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox3.Location = new System.Drawing.Point(969, 110);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(197, 95);
            this.pictureBox3.TabIndex = 9;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox4.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox4.BackgroundImage = global::SQLiteTest.Properties.Resources.Haufe_Transparent;
            this.pictureBox4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox4.Location = new System.Drawing.Point(969, 25);
            this.pictureBox4.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(332, 46);
            this.pictureBox4.TabIndex = 10;
            this.pictureBox4.TabStop = false;
            // 
            // tcTabs
            // 
            this.tcTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcTabs.Controls.Add(this.tsGeneral);
            this.tcTabs.Controls.Add(this.tsApps);
            this.tcTabs.Location = new System.Drawing.Point(253, 187);
            this.tcTabs.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tcTabs.Name = "tcTabs";
            this.tcTabs.SelectedIndex = 0;
            this.tcTabs.Size = new System.Drawing.Size(723, 334);
            this.tcTabs.TabIndex = 11;
            this.tcTabs.SelectedIndexChanged += new System.EventHandler(this.tcTabs_SelectedIndexChanged);
            // 
            // tsGeneral
            // 
            this.tsGeneral.Controls.Add(this.rePrompt);
            this.tsGeneral.Location = new System.Drawing.Point(4, 25);
            this.tsGeneral.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tsGeneral.Name = "tsGeneral";
            this.tsGeneral.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tsGeneral.Size = new System.Drawing.Size(715, 305);
            this.tsGeneral.TabIndex = 0;
            this.tsGeneral.Text = "Prompt";
            this.tsGeneral.UseVisualStyleBackColor = true;
            // 
            // rePrompt
            // 
            this.rePrompt.BackColor = System.Drawing.Color.Black;
            this.rePrompt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rePrompt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rePrompt.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.rePrompt.Location = new System.Drawing.Point(3, 2);
            this.rePrompt.Margin = new System.Windows.Forms.Padding(4);
            this.rePrompt.Name = "rePrompt";
            this.rePrompt.Size = new System.Drawing.Size(709, 301);
            this.rePrompt.TabIndex = 0;
            this.rePrompt.Text = "";
            this.rePrompt.Enter += new System.EventHandler(this.rePrompt_Enter);
            this.rePrompt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rePrompt_KeyDown);
            this.rePrompt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.rePrompt_KeyUp);
            // 
            // tsApps
            // 
            this.tsApps.Controls.Add(this.treeApps);
            this.tsApps.Location = new System.Drawing.Point(4, 25);
            this.tsApps.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tsApps.Name = "tsApps";
            this.tsApps.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tsApps.Size = new System.Drawing.Size(715, 305);
            this.tsApps.TabIndex = 2;
            this.tsApps.Text = "Analyzation Tree";
            this.tsApps.UseVisualStyleBackColor = true;
            // 
            // treeApps
            // 
            this.treeApps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeApps.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeApps.Location = new System.Drawing.Point(3, 2);
            this.treeApps.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.treeApps.Name = "treeApps";
            this.treeApps.Size = new System.Drawing.Size(709, 301);
            this.treeApps.TabIndex = 0;
            this.treeApps.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeApps_BeforeExpand_1);
            // 
            // tiPollApps
            // 
            this.tiPollApps.Interval = 10000;
            this.tiPollApps.Tick += new System.EventHandler(this.tiPollApps_Tick);
            // 
            // tiUpdateApps
            // 
            this.tiUpdateApps.Interval = 10000;
            this.tiUpdateApps.Tick += new System.EventHandler(this.tiUpdateApps_Tick);
            // 
            // lblNumThreads
            // 
            this.lblNumThreads.AutoSize = true;
            this.lblNumThreads.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumThreads.Location = new System.Drawing.Point(29, 193);
            this.lblNumThreads.Name = "lblNumThreads";
            this.lblNumThreads.Size = new System.Drawing.Size(101, 13);
            this.lblNumThreads.TabIndex = 12;
            this.lblNumThreads.Text = "Number of Threads:";
            // 
            // numThreads
            // 
            this.numThreads.Location = new System.Drawing.Point(173, 188);
            this.numThreads.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numThreads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numThreads.Name = "numThreads";
            this.numThreads.Size = new System.Drawing.Size(51, 22);
            this.numThreads.TabIndex = 13;
            this.numThreads.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblVersion.AutoSize = true;
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.Color.Green;
            this.lblVersion.Location = new System.Drawing.Point(20, 542);
            this.lblVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(53, 13);
            this.lblVersion.TabIndex = 14;
            this.lblVersion.Text = "Version:";
            // 
            // tiConnectionQuery
            // 
            this.tiConnectionQuery.Interval = 500;
            this.tiConnectionQuery.Tick += new System.EventHandler(this.tiConnectionQuery_Tick);
            // 
            // sqLiteCommandBuilder1
            // 
            this.sqLiteCommandBuilder1.DataAdapter = null;
            this.sqLiteCommandBuilder1.QuoteSuffix = "]";
            // 
            // sqLiteCommand1
            // 
            this.sqLiteCommand1.CommandText = null;
            // 
            // sqLiteCommand2
            // 
            this.sqLiteCommand2.CommandText = null;
            // 
            // tiLiveUpdate
            // 
            this.tiLiveUpdate.Interval = 5000;
            this.tiLiveUpdate.Tick += new System.EventHandler(this.tiLiveUpdate_Tick);
            // 
            // btnLiveUpdate
            // 
            this.btnLiveUpdate.Image = global::SQLiteTest.Properties.Resources.LiveUpdate_32_blau;
            this.btnLiveUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLiveUpdate.Location = new System.Drawing.Point(23, 384);
            this.btnLiveUpdate.Margin = new System.Windows.Forms.Padding(4);
            this.btnLiveUpdate.Name = "btnLiveUpdate";
            this.btnLiveUpdate.Size = new System.Drawing.Size(201, 55);
            this.btnLiveUpdate.TabIndex = 15;
            this.btnLiveUpdate.Text = "Live Update";
            this.btnLiveUpdate.UseVisualStyleBackColor = true;
            this.btnLiveUpdate.Click += new System.EventHandler(this.btnLiveUpdate_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.actionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(975, 28);
            this.menuStrip1.TabIndex = 16;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOpenExternalDB,
            this.toolStripMenuItem3,
            this.mnuConnect,
            this.mnuDisconnect,
            this.toolStripMenuItem2,
            this.menuExit});
            this.fileToolStripMenuItem.Image = global::SQLiteTest.Properties.Resources.fileopen32_transparent;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
            this.fileToolStripMenuItem.Text = "&File";
            this.fileToolStripMenuItem.DropDownOpening += new System.EventHandler(this.fileToolStripMenuItem_DropDownOpening);
            // 
            // mnuOpenExternalDB
            // 
            this.mnuOpenExternalDB.Image = global::SQLiteTest.Properties.Resources.OpenFile;
            this.mnuOpenExternalDB.Name = "mnuOpenExternalDB";
            this.mnuOpenExternalDB.Size = new System.Drawing.Size(199, 22);
            this.mnuOpenExternalDB.Text = "Open e&xternal Database";
            this.mnuOpenExternalDB.Click += new System.EventHandler(this.mnuOpenExternalDB_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(196, 6);
            // 
            // mnuConnect
            // 
            this.mnuConnect.BackColor = System.Drawing.Color.Transparent;
            this.mnuConnect.Image = global::SQLiteTest.Properties.Resources.Connected1;
            this.mnuConnect.Name = "mnuConnect";
            this.mnuConnect.Size = new System.Drawing.Size(199, 22);
            this.mnuConnect.Text = "&Connect";
            this.mnuConnect.Click += new System.EventHandler(this.mnuConnect_Click);
            // 
            // mnuDisconnect
            // 
            this.mnuDisconnect.Image = global::SQLiteTest.Properties.Resources.Disconnected;
            this.mnuDisconnect.Name = "mnuDisconnect";
            this.mnuDisconnect.Size = new System.Drawing.Size(199, 22);
            this.mnuDisconnect.Text = "&Disconnect";
            this.mnuDisconnect.Click += new System.EventHandler(this.mnuDisconnect_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(196, 6);
            // 
            // menuExit
            // 
            this.menuExit.Image = global::SQLiteTest.Properties.Resources.Close_161;
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(199, 22);
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuStartThread,
            this.mnuStopThread,
            this.toolStripMenuItem1,
            this.mnuLiveUpdate});
            this.actionsToolStripMenuItem.Image = global::SQLiteTest.Properties.Resources.Action_Transparent;
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(79, 24);
            this.actionsToolStripMenuItem.Text = "&Actions";
            // 
            // mnuStartThread
            // 
            this.mnuStartThread.Image = global::SQLiteTest.Properties.Resources.Start_Process_321;
            this.mnuStartThread.Name = "mnuStartThread";
            this.mnuStartThread.Size = new System.Drawing.Size(142, 22);
            this.mnuStartThread.Text = "Start Threads";
            this.mnuStartThread.Click += new System.EventHandler(this.mnuStartThread_Click);
            // 
            // mnuStopThread
            // 
            this.mnuStopThread.Enabled = false;
            this.mnuStopThread.Image = global::SQLiteTest.Properties.Resources.stop_process_2_322;
            this.mnuStopThread.Name = "mnuStopThread";
            this.mnuStopThread.Size = new System.Drawing.Size(142, 22);
            this.mnuStopThread.Text = "Stop Threads";
            this.mnuStopThread.Click += new System.EventHandler(this.mnuStopThread_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(139, 6);
            // 
            // mnuLiveUpdate
            // 
            this.mnuLiveUpdate.Image = global::SQLiteTest.Properties.Resources.LiveUpdate_32_blau1;
            this.mnuLiveUpdate.Name = "mnuLiveUpdate";
            this.mnuLiveUpdate.Size = new System.Drawing.Size(142, 22);
            this.mnuLiveUpdate.Text = "Live Update";
            this.mnuLiveUpdate.Click += new System.EventHandler(this.mnuLiveUpdate_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuUpdate,
            this.toolStripMenuItem4,
            this.mnuShowManual,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Image = global::SQLiteTest.Properties.Resources.Close_162;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(64, 24);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // mnuUpdate
            // 
            this.mnuUpdate.Image = global::SQLiteTest.Properties.Resources.Update;
            this.mnuUpdate.Name = "mnuUpdate";
            this.mnuUpdate.Size = new System.Drawing.Size(247, 26);
            this.mnuUpdate.Text = "&Update to latest version";
            this.mnuUpdate.Click += new System.EventHandler(this.mnuUpdate_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(244, 6);
            // 
            // mnuShowManual
            // 
            this.mnuShowManual.Image = global::SQLiteTest.Properties.Resources.Manual_Mono;
            this.mnuShowManual.Name = "mnuShowManual";
            this.mnuShowManual.Size = new System.Drawing.Size(247, 26);
            this.mnuShowManual.Text = "Show Manual";
            this.mnuShowManual.Click += new System.EventHandler(this.mnuShowManual_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = global::SQLiteTest.Properties.Resources.About_161;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(247, 26);
            this.aboutToolStripMenuItem.Text = "About Haufe MultiSQLite for C#";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tiLiveUpdateFlicker
            // 
            this.tiLiveUpdateFlicker.Interval = 500;
            this.tiLiveUpdateFlicker.Tick += new System.EventHandler(this.tiLiveUpdateFlicker_Tick);
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar1.Location = new System.Drawing.Point(320, 766);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.trackBar1.Maximum = 60;
            this.trackBar1.Minimum = 1;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(1060, 45);
            this.trackBar1.TabIndex = 17;
            this.trackBar1.Value = 1;
            // 
            // tbDBInterval
            // 
            this.tbDBInterval.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbDBInterval.Location = new System.Drawing.Point(260, 522);
            this.tbDBInterval.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbDBInterval.Maximum = 60;
            this.tbDBInterval.Minimum = 1;
            this.tbDBInterval.Name = "tbDBInterval";
            this.tbDBInterval.Size = new System.Drawing.Size(711, 45);
            this.tbDBInterval.TabIndex = 19;
            this.tbDBInterval.Value = 1;
            this.tbDBInterval.Scroll += new System.EventHandler(this.tbDBInterval_Scroll);
            this.tbDBInterval.ValueChanged += new System.EventHandler(this.tbDBInterval_ValueChanged);
            // 
            // LBL_DB_PollingInterval
            // 
            this.LBL_DB_PollingInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LBL_DB_PollingInterval.AutoSize = true;
            this.LBL_DB_PollingInterval.Location = new System.Drawing.Point(260, 545);
            this.LBL_DB_PollingInterval.Name = "LBL_DB_PollingInterval";
            this.LBL_DB_PollingInterval.Size = new System.Drawing.Size(120, 16);
            this.LBL_DB_PollingInterval.TabIndex = 20;
            this.LBL_DB_PollingInterval.Text = "DB Polling Interval:";
            // 
            // tiScrollCaret
            // 
            this.tiScrollCaret.Tick += new System.EventHandler(this.tiScrollCaret_Tick);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::SQLiteTest.Properties.Resources.Connections2_Light;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(975, 567);
            this.Controls.Add(this.LBL_DB_PollingInterval);
            this.Controls.Add(this.tbDBInterval);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.btnLiveUpdate);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.numThreads);
            this.Controls.Add(this.lblNumThreads);
            this.Controls.Add(this.tcTabs);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnStopThreads);
            this.Controls.Add(this.btnStartThreads);
            this.Controls.Add(this.btnShowManual);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(989, 605);
            this.Name = "frmMain";
            this.Text = "Haufe Multi-SQLite for C#";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.tcTabs.ResumeLayout(false);
            this.tsGeneral.ResumeLayout(false);
            this.tsApps.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numThreads)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDBInterval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnShowManual;
        private System.Windows.Forms.Button btnStartThreads;
        private System.Windows.Forms.Button btnStopThreads;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.TabControl tcTabs;
        private System.Windows.Forms.TabPage tsGeneral;
        private System.Windows.Forms.Timer tiPollApps;
        private System.Windows.Forms.Timer tiUpdateApps;
        private System.Windows.Forms.TabPage tsApps;
        private System.Windows.Forms.TreeView treeApps;
        private System.Windows.Forms.Label lblNumThreads;
        private System.Windows.Forms.NumericUpDown numThreads;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Timer tiConnectionQuery;
        private System.Windows.Forms.RichTextBox rePrompt;
        private System.Data.SQLite.SQLiteCommandBuilder sqLiteCommandBuilder1;
        private System.Data.SQLite.SQLiteCommand sqLiteCommand1;
        private System.Data.SQLite.SQLiteCommand sqLiteCommand2;
        private System.Windows.Forms.Timer tiLiveUpdate;
        private System.Windows.Forms.Button btnLiveUpdate;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuShowManual;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuStartThread;
        private System.Windows.Forms.ToolStripMenuItem mnuStopThread;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuLiveUpdate;
        private System.Windows.Forms.Timer tiLiveUpdateFlicker;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.TrackBar tbDBInterval;
        private System.Windows.Forms.Label LBL_DB_PollingInterval;
        private System.Windows.Forms.Timer tiScrollCaret;
        private System.Windows.Forms.ToolStripMenuItem mnuConnect;
        private System.Windows.Forms.ToolStripMenuItem mnuDisconnect;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem mnuOpenExternalDB;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem mnuUpdate;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
    }
}

