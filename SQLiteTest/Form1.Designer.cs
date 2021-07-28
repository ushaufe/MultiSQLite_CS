namespace SQLiteTest
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
            this.btnShowContent = new System.Windows.Forms.Button();
            this.lb = new System.Windows.Forms.ListBox();
            this.btnStartThreads = new System.Windows.Forms.Button();
            this.btnStopThreads = new System.Windows.Forms.Button();
            this.btnShowStatus = new System.Windows.Forms.Button();
            this.btnViewThread = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnShowContent
            // 
            this.btnShowContent.Location = new System.Drawing.Point(23, 12);
            this.btnShowContent.Name = "btnShowContent";
            this.btnShowContent.Size = new System.Drawing.Size(201, 53);
            this.btnShowContent.TabIndex = 0;
            this.btnShowContent.Text = "Show Content";
            this.btnShowContent.UseVisualStyleBackColor = true;
            this.btnShowContent.Click += new System.EventHandler(this.btnShowContent_Click);
            // 
            // lb
            // 
            this.lb.FormattingEnabled = true;
            this.lb.ItemHeight = 16;
            this.lb.Location = new System.Drawing.Point(307, 26);
            this.lb.Name = "lb";
            this.lb.Size = new System.Drawing.Size(449, 404);
            this.lb.TabIndex = 1;
            // 
            // btnStartThreads
            // 
            this.btnStartThreads.Location = new System.Drawing.Point(23, 108);
            this.btnStartThreads.Name = "btnStartThreads";
            this.btnStartThreads.Size = new System.Drawing.Size(201, 58);
            this.btnStartThreads.TabIndex = 2;
            this.btnStartThreads.Text = "Start Threads";
            this.btnStartThreads.UseVisualStyleBackColor = true;
            this.btnStartThreads.Click += new System.EventHandler(this.buttonStartThreads_Click);
            // 
            // btnStopThreads
            // 
            this.btnStopThreads.Location = new System.Drawing.Point(23, 205);
            this.btnStopThreads.Name = "btnStopThreads";
            this.btnStopThreads.Size = new System.Drawing.Size(201, 60);
            this.btnStopThreads.TabIndex = 3;
            this.btnStopThreads.Text = "Stop Threads";
            this.btnStopThreads.UseVisualStyleBackColor = true;
            this.btnStopThreads.Click += new System.EventHandler(this.btnStopThreads_Click);
            // 
            // btnShowStatus
            // 
            this.btnShowStatus.Location = new System.Drawing.Point(23, 305);
            this.btnShowStatus.Name = "btnShowStatus";
            this.btnShowStatus.Size = new System.Drawing.Size(201, 57);
            this.btnShowStatus.TabIndex = 4;
            this.btnShowStatus.Text = "Show Status";
            this.btnShowStatus.UseVisualStyleBackColor = true;
            this.btnShowStatus.Click += new System.EventHandler(this.btnShowStatus_Click);
            // 
            // btnViewThread
            // 
            this.btnViewThread.Location = new System.Drawing.Point(23, 388);
            this.btnViewThread.Name = "btnViewThread";
            this.btnViewThread.Size = new System.Drawing.Size(201, 50);
            this.btnViewThread.TabIndex = 5;
            this.btnViewThread.Text = "View Thread";
            this.btnViewThread.UseVisualStyleBackColor = true;
            this.btnViewThread.Click += new System.EventHandler(this.btnViewThread_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnViewThread);
            this.Controls.Add(this.btnShowStatus);
            this.Controls.Add(this.btnStopThreads);
            this.Controls.Add(this.btnStartThreads);
            this.Controls.Add(this.lb);
            this.Controls.Add(this.btnShowContent);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnShowContent;
        private System.Windows.Forms.ListBox lb;
        private System.Windows.Forms.Button btnStartThreads;
        private System.Windows.Forms.Button btnStopThreads;
        private System.Windows.Forms.Button btnShowStatus;
        private System.Windows.Forms.Button btnViewThread;
    }
}

