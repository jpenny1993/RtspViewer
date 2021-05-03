namespace RtspViewer.Forms.Windows
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuBarTop = new System.Windows.Forms.MenuStrip();
            this.menuStripFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBtnPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBtnSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBtnQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.statusLblConnection = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLblTimer = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLblFps = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.videoView = new RtspViewer.Forms.Controls.VideoView();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.menuBarTop.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuBarTop
            // 
            this.menuBarTop.BackColor = System.Drawing.SystemColors.Control;
            this.menuBarTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuStripFile});
            this.menuBarTop.Location = new System.Drawing.Point(0, 0);
            this.menuBarTop.Name = "menuBarTop";
            this.menuBarTop.Size = new System.Drawing.Size(646, 24);
            this.menuBarTop.TabIndex = 0;
            // 
            // menuStripFile
            // 
            this.menuStripFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuBtnPlay,
            this.menuBtnSettings,
            this.menuBtnQuit});
            this.menuStripFile.Name = "menuStripFile";
            this.menuStripFile.Size = new System.Drawing.Size(37, 20);
            this.menuStripFile.Text = "File";
            // 
            // menuBtnPlay
            // 
            this.menuBtnPlay.Enabled = false;
            this.menuBtnPlay.Name = "menuBtnPlay";
            this.menuBtnPlay.Size = new System.Drawing.Size(116, 22);
            this.menuBtnPlay.Text = "Play";
            this.menuBtnPlay.Click += new System.EventHandler(this.MenuBtnPlay_Click);
            // 
            // menuBtnSettings
            // 
            this.menuBtnSettings.Name = "menuBtnSettings";
            this.menuBtnSettings.Size = new System.Drawing.Size(116, 22);
            this.menuBtnSettings.Text = "Settings";
            this.menuBtnSettings.Click += new System.EventHandler(this.MenuBtnSettings_Click);
            // 
            // menuBtnQuit
            // 
            this.menuBtnQuit.Name = "menuBtnQuit";
            this.menuBtnQuit.Size = new System.Drawing.Size(116, 22);
            this.menuBtnQuit.Text = "Quit";
            this.menuBtnQuit.Click += new System.EventHandler(this.MenuBtnQuit_Click);
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLblConnection,
            this.statusLblTimer,
            this.statusLblFps});
            this.statusBar.Location = new System.Drawing.Point(0, 394);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(646, 22);
            this.statusBar.TabIndex = 1;
            // 
            // statusLblConnection
            // 
            this.statusLblConnection.Name = "statusLblConnection";
            this.statusLblConnection.Size = new System.Drawing.Size(210, 17);
            this.statusLblConnection.Spring = true;
            this.statusLblConnection.Text = "Disconnected";
            this.statusLblConnection.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusLblTimer
            // 
            this.statusLblTimer.Margin = new System.Windows.Forms.Padding(2, 3, 0, 2);
            this.statusLblTimer.Name = "statusLblTimer";
            this.statusLblTimer.Size = new System.Drawing.Size(208, 17);
            this.statusLblTimer.Spring = true;
            this.statusLblTimer.Text = "00:00:00";
            // 
            // statusLblFps
            // 
            this.statusLblFps.Margin = new System.Windows.Forms.Padding(2, 3, 0, 2);
            this.statusLblFps.Name = "statusLblFps";
            this.statusLblFps.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.statusLblFps.Size = new System.Drawing.Size(208, 17);
            this.statusLblFps.Spring = true;
            this.statusLblFps.Text = "0 fps";
            this.statusLblFps.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Controls.Add(this.videoView, 0, 0);
            this.tableLayoutPanel.Location = new System.Drawing.Point(2, 25);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 366F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(640, 366);
            this.tableLayoutPanel.TabIndex = 2;
            // 
            // videoView
            // 
            this.videoView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.videoView.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.videoView.Location = new System.Drawing.Point(3, 3);
            this.videoView.Name = "videoView";
            this.videoView.Size = new System.Drawing.Size(640, 360);
            this.videoView.TabIndex = 0;
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(646, 416);
            this.Controls.Add(this.tableLayoutPanel);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.menuBarTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuBarTop;
            this.MinimumSize = new System.Drawing.Size(662, 455);
            this.Name = "MainForm";
            this.Text = "RTSP Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuBarTop.ResumeLayout(false);
            this.menuBarTop.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuBarTop;
        private System.Windows.Forms.ToolStripMenuItem menuStripFile;
        private System.Windows.Forms.ToolStripMenuItem menuBtnSettings;
        private System.Windows.Forms.ToolStripMenuItem menuBtnQuit;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel statusLblConnection;
        private System.Windows.Forms.ToolStripStatusLabel statusLblTimer;
        private System.Windows.Forms.ToolStripStatusLabel statusLblFps;
        private System.Windows.Forms.ToolStripMenuItem menuBtnPlay;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private RtspViewer.Forms.Controls.VideoView videoView;
        private System.Windows.Forms.Timer timer;
    }
}

