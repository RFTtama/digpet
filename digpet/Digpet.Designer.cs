namespace digpet
{
    partial class Digpet
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Digpet));
            EmoStringLabel = new Label();
            CpuUsageLabel = new Label();
            DailyTokenLabel = new Label();
            ToggleShowButton = new Button();
            StatsLabel = new Label();
            StatsPanel = new Panel();
            FeelingLabel = new Label();
            ImportButton = new Button();
            ClearButton = new Button();
            DefaultPanel = new Panel();
            CharPictureBox = new PictureBox();
            ImageChangeTimer = new System.Windows.Forms.Timer(components);
            StatsPanel.SuspendLayout();
            DefaultPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)CharPictureBox).BeginInit();
            SuspendLayout();
            // 
            // EmoStringLabel
            // 
            resources.ApplyResources(EmoStringLabel, "EmoStringLabel");
            EmoStringLabel.BackColor = Color.Transparent;
            EmoStringLabel.ForeColor = Color.Orange;
            EmoStringLabel.Name = "EmoStringLabel";
            // 
            // CpuUsageLabel
            // 
            resources.ApplyResources(CpuUsageLabel, "CpuUsageLabel");
            CpuUsageLabel.BackColor = Color.Transparent;
            CpuUsageLabel.ForeColor = Color.Orange;
            CpuUsageLabel.Name = "CpuUsageLabel";
            // 
            // DailyTokenLabel
            // 
            resources.ApplyResources(DailyTokenLabel, "DailyTokenLabel");
            DailyTokenLabel.BackColor = Color.Transparent;
            DailyTokenLabel.ForeColor = Color.Orange;
            DailyTokenLabel.Name = "DailyTokenLabel";
            // 
            // ToggleShowButton
            // 
            resources.ApplyResources(ToggleShowButton, "ToggleShowButton");
            ToggleShowButton.Name = "ToggleShowButton";
            ToggleShowButton.UseVisualStyleBackColor = true;
            ToggleShowButton.Click += ToggleShowButton_Click;
            // 
            // StatsLabel
            // 
            resources.ApplyResources(StatsLabel, "StatsLabel");
            StatsLabel.BackColor = Color.Transparent;
            StatsLabel.ForeColor = Color.Orange;
            StatsLabel.Name = "StatsLabel";
            // 
            // StatsPanel
            // 
            resources.ApplyResources(StatsPanel, "StatsPanel");
            StatsPanel.BackColor = Color.AliceBlue;
            StatsPanel.BorderStyle = BorderStyle.FixedSingle;
            StatsPanel.Controls.Add(FeelingLabel);
            StatsPanel.Controls.Add(StatsLabel);
            StatsPanel.Controls.Add(DailyTokenLabel);
            StatsPanel.Name = "StatsPanel";
            // 
            // FeelingLabel
            // 
            resources.ApplyResources(FeelingLabel, "FeelingLabel");
            FeelingLabel.BackColor = Color.Transparent;
            FeelingLabel.ForeColor = Color.Orange;
            FeelingLabel.Name = "FeelingLabel";
            // 
            // ImportButton
            // 
            resources.ApplyResources(ImportButton, "ImportButton");
            ImportButton.Name = "ImportButton";
            ImportButton.UseVisualStyleBackColor = true;
            ImportButton.Click += ImportButton_Click;
            // 
            // ClearButton
            // 
            resources.ApplyResources(ClearButton, "ClearButton");
            ClearButton.Name = "ClearButton";
            ClearButton.UseVisualStyleBackColor = true;
            ClearButton.Click += ClearButton_Click;
            // 
            // DefaultPanel
            // 
            resources.ApplyResources(DefaultPanel, "DefaultPanel");
            DefaultPanel.BackColor = Color.AliceBlue;
            DefaultPanel.BorderStyle = BorderStyle.FixedSingle;
            DefaultPanel.Controls.Add(EmoStringLabel);
            DefaultPanel.Controls.Add(CpuUsageLabel);
            DefaultPanel.Name = "DefaultPanel";
            // 
            // CharPictureBox
            // 
            resources.ApplyResources(CharPictureBox, "CharPictureBox");
            CharPictureBox.BackColor = Color.Transparent;
            CharPictureBox.Name = "CharPictureBox";
            CharPictureBox.TabStop = false;
            // 
            // ImageChangeTimer
            // 
            ImageChangeTimer.Tick += ImageChangeTimer_Tick;
            // 
            // Digpet
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            Controls.Add(StatsPanel);
            Controls.Add(DefaultPanel);
            Controls.Add(ClearButton);
            Controls.Add(ImportButton);
            Controls.Add(ToggleShowButton);
            Controls.Add(CharPictureBox);
            HelpButton = true;
            Name = "Digpet";
            FormClosing += Form1_FormClosing;
            SizeChanged += Form1_SizeChanged;
            StatsPanel.ResumeLayout(false);
            StatsPanel.PerformLayout();
            DefaultPanel.ResumeLayout(false);
            DefaultPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)CharPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label EmoStringLabel;
        private Label CpuUsageLabel;
        private Label DailyTokenLabel;
        private Button ToggleShowButton;
        private Label StatsLabel;
        private Panel StatsPanel;
        private Button ImportButton;
        private Button ClearButton;
        private Panel DefaultPanel;
        private PictureBox CharPictureBox;
        private System.Windows.Forms.Timer ImageChangeTimer;
        private Label FeelingLabel;
    }
}
