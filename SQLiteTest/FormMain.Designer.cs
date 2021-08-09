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
            this.btnViewThread = new System.Windows.Forms.Button();
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
            this.label2 = new System.Windows.Forms.Label();
            this.numThreads = new System.Windows.Forms.NumericUpDown();
            this.lblVersion = new System.Windows.Forms.Label();
            this.tiConnectionQuery = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.tcTabs.SuspendLayout();
            this.tsGeneral.SuspendLayout();
            this.tsApps.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThreads)).BeginInit();
            this.SuspendLayout();
            // 
            // btnShowManual
            // 
            this.btnShowManual.Enabled = false;
            this.btnShowManual.Location = new System.Drawing.Point(17, 378);
            this.btnShowManual.Margin = new System.Windows.Forms.Padding(2);
            this.btnShowManual.Name = "btnShowManual";
            this.btnShowManual.Size = new System.Drawing.Size(151, 43);
            this.btnShowManual.TabIndex = 0;
            this.btnShowManual.Text = "User\'s Manual";
            this.btnShowManual.UseVisualStyleBackColor = true;
            this.btnShowManual.Click += new System.EventHandler(this.btnShowManual_Click);
            // 
            // btnStartThreads
            // 
            this.btnStartThreads.Location = new System.Drawing.Point(17, 174);
            this.btnStartThreads.Margin = new System.Windows.Forms.Padding(2);
            this.btnStartThreads.Name = "btnStartThreads";
            this.btnStartThreads.Size = new System.Drawing.Size(151, 47);
            this.btnStartThreads.TabIndex = 2;
            this.btnStartThreads.Text = "Start Threads";
            this.btnStartThreads.UseVisualStyleBackColor = true;
            this.btnStartThreads.Click += new System.EventHandler(this.buttonStartThreads_Click);
            // 
            // btnStopThreads
            // 
            this.btnStopThreads.Location = new System.Drawing.Point(17, 243);
            this.btnStopThreads.Margin = new System.Windows.Forms.Padding(2);
            this.btnStopThreads.Name = "btnStopThreads";
            this.btnStopThreads.Size = new System.Drawing.Size(151, 49);
            this.btnStopThreads.TabIndex = 3;
            this.btnStopThreads.Text = "Stop Threads";
            this.btnStopThreads.UseVisualStyleBackColor = true;
            this.btnStopThreads.Click += new System.EventHandler(this.btnStopThreads_Click);
            // 
            // btnViewThread
            // 
            this.btnViewThread.Enabled = false;
            this.btnViewThread.Location = new System.Drawing.Point(17, 315);
            this.btnViewThread.Margin = new System.Windows.Forms.Padding(2);
            this.btnViewThread.Name = "btnViewThread";
            this.btnViewThread.Size = new System.Drawing.Size(151, 41);
            this.btnViewThread.TabIndex = 5;
            this.btnViewThread.Text = "View Thread";
            this.btnViewThread.UseVisualStyleBackColor = true;
            this.btnViewThread.Click += new System.EventHandler(this.btnViewThread_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::SQLiteTest.Properties.Resources.Haufe_Transparent;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(941, 31);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(320, 41);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImage = global::SQLiteTest.Properties.Resources.CS;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.Location = new System.Drawing.Point(627, 71);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(86, 77);
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(185, 85);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
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
            this.pictureBox3.Location = new System.Drawing.Point(464, 71);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(148, 77);
            this.pictureBox3.TabIndex = 9;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox4.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox4.BackgroundImage = global::SQLiteTest.Properties.Resources.Haufe_Transparent;
            this.pictureBox4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox4.Location = new System.Drawing.Point(464, 16);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(249, 37);
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
            this.tcTabs.Location = new System.Drawing.Point(190, 152);
            this.tcTabs.Margin = new System.Windows.Forms.Padding(2);
            this.tcTabs.Name = "tcTabs";
            this.tcTabs.SelectedIndex = 0;
            this.tcTabs.Size = new System.Drawing.Size(524, 302);
            this.tcTabs.TabIndex = 11;
            this.tcTabs.SelectedIndexChanged += new System.EventHandler(this.tcTabs_SelectedIndexChanged);
            // 
            // tsGeneral
            // 
            this.tsGeneral.Controls.Add(this.rePrompt);
            this.tsGeneral.Location = new System.Drawing.Point(4, 22);
            this.tsGeneral.Margin = new System.Windows.Forms.Padding(2);
            this.tsGeneral.Name = "tsGeneral";
            this.tsGeneral.Padding = new System.Windows.Forms.Padding(2);
            this.tsGeneral.Size = new System.Drawing.Size(516, 276);
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
            this.rePrompt.Location = new System.Drawing.Point(2, 2);
            this.rePrompt.Name = "rePrompt";
            this.rePrompt.Size = new System.Drawing.Size(512, 272);
            this.rePrompt.TabIndex = 0;
            this.rePrompt.Text = "";
            this.rePrompt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.rePrompt_KeyUp);
            // 
            // tsApps
            // 
            this.tsApps.Controls.Add(this.treeApps);
            this.tsApps.Location = new System.Drawing.Point(4, 22);
            this.tsApps.Margin = new System.Windows.Forms.Padding(2);
            this.tsApps.Name = "tsApps";
            this.tsApps.Padding = new System.Windows.Forms.Padding(2);
            this.tsApps.Size = new System.Drawing.Size(516, 276);
            this.tsApps.TabIndex = 2;
            this.tsApps.Text = "Analyzation Tree";
            this.tsApps.UseVisualStyleBackColor = true;
            // 
            // treeApps
            // 
            this.treeApps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeApps.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeApps.Location = new System.Drawing.Point(2, 2);
            this.treeApps.Margin = new System.Windows.Forms.Padding(2);
            this.treeApps.Name = "treeApps";
            this.treeApps.Size = new System.Drawing.Size(512, 272);
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(22, 157);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Number of Threads:";
            // 
            // numThreads
            // 
            this.numThreads.Location = new System.Drawing.Point(125, 153);
            this.numThreads.Margin = new System.Windows.Forms.Padding(2);
            this.numThreads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numThreads.Name = "numThreads";
            this.numThreads.Size = new System.Drawing.Size(43, 20);
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
            this.lblVersion.Location = new System.Drawing.Point(14, 434);
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
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::SQLiteTest.Properties.Resources.Connections2_Light;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(731, 461);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.numThreads);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tcTabs);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnViewThread);
            this.Controls.Add(this.btnStopThreads);
            this.Controls.Add(this.btnStartThreads);
            this.Controls.Add(this.btnShowManual);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(746, 398);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnShowManual;
        private System.Windows.Forms.Button btnStartThreads;
        private System.Windows.Forms.Button btnStopThreads;
        private System.Windows.Forms.Button btnViewThread;
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numThreads;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Timer tiConnectionQuery;
        private System.Windows.Forms.RichTextBox rePrompt;
    }
}

