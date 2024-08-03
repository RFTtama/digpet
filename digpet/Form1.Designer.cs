namespace digpet
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            CpuUsageTimer = new System.Windows.Forms.Timer(components);
            EmoStringLabel = new Label();
            CpuUsageLabel = new Label();
            TotalTokenLabel = new Label();
            DailyTokenLabel = new Label();
            FeelingLabel = new Label();
            AverageEmotionTokensLabel = new Label();
            EmoTokenLabel = new Label();
            ToggleShowButton = new Button();
            FlopsLabel = new Label();
            StatsLabel = new Label();
            StatsPanel = new Panel();
            ImportButton = new Button();
            IntimacyLabel = new Label();
            ClearButton = new Button();
            StatsPanel.SuspendLayout();
            SuspendLayout();
            // 
            // CpuUsageTimer
            // 
            CpuUsageTimer.Interval = 1000;
            CpuUsageTimer.Tick += CpuUsageTimer_Tick;
            // 
            // EmoStringLabel
            // 
            resources.ApplyResources(EmoStringLabel, "EmoStringLabel");
            EmoStringLabel.ForeColor = Color.Orange;
            EmoStringLabel.Name = "EmoStringLabel";
            // 
            // CpuUsageLabel
            // 
            resources.ApplyResources(CpuUsageLabel, "CpuUsageLabel");
            CpuUsageLabel.ForeColor = Color.Orange;
            CpuUsageLabel.Name = "CpuUsageLabel";
            // 
            // TotalTokenLabel
            // 
            resources.ApplyResources(TotalTokenLabel, "TotalTokenLabel");
            TotalTokenLabel.ForeColor = Color.Orange;
            TotalTokenLabel.Name = "TotalTokenLabel";
            // 
            // DailyTokenLabel
            // 
            resources.ApplyResources(DailyTokenLabel, "DailyTokenLabel");
            DailyTokenLabel.ForeColor = Color.Orange;
            DailyTokenLabel.Name = "DailyTokenLabel";
            // 
            // FeelingLabel
            // 
            resources.ApplyResources(FeelingLabel, "FeelingLabel");
            FeelingLabel.ForeColor = Color.Orange;
            FeelingLabel.Name = "FeelingLabel";
            // 
            // AverageEmotionTokensLabel
            // 
            resources.ApplyResources(AverageEmotionTokensLabel, "AverageEmotionTokensLabel");
            AverageEmotionTokensLabel.ForeColor = Color.Orange;
            AverageEmotionTokensLabel.Name = "AverageEmotionTokensLabel";
            // 
            // EmoTokenLabel
            // 
            resources.ApplyResources(EmoTokenLabel, "EmoTokenLabel");
            EmoTokenLabel.ForeColor = Color.Orange;
            EmoTokenLabel.Name = "EmoTokenLabel";
            // 
            // ToggleShowButton
            // 
            resources.ApplyResources(ToggleShowButton, "ToggleShowButton");
            ToggleShowButton.Name = "ToggleShowButton";
            ToggleShowButton.UseVisualStyleBackColor = true;
            ToggleShowButton.Click += ToggleShowButton_Click;
            // 
            // FlopsLabel
            // 
            resources.ApplyResources(FlopsLabel, "FlopsLabel");
            FlopsLabel.ForeColor = Color.Orange;
            FlopsLabel.Name = "FlopsLabel";
            // 
            // StatsLabel
            // 
            resources.ApplyResources(StatsLabel, "StatsLabel");
            StatsLabel.ForeColor = Color.Orange;
            StatsLabel.Name = "StatsLabel";
            // 
            // StatsPanel
            // 
            resources.ApplyResources(StatsPanel, "StatsPanel");
            StatsPanel.BackColor = Color.AliceBlue;
            StatsPanel.BorderStyle = BorderStyle.FixedSingle;
            StatsPanel.Controls.Add(StatsLabel);
            StatsPanel.Controls.Add(FlopsLabel);
            StatsPanel.Controls.Add(DailyTokenLabel);
            StatsPanel.Controls.Add(TotalTokenLabel);
            StatsPanel.Controls.Add(EmoTokenLabel);
            StatsPanel.Controls.Add(FeelingLabel);
            StatsPanel.Controls.Add(AverageEmotionTokensLabel);
            StatsPanel.Name = "StatsPanel";
            // 
            // ImportButton
            // 
            resources.ApplyResources(ImportButton, "ImportButton");
            ImportButton.Name = "ImportButton";
            ImportButton.UseVisualStyleBackColor = true;
            ImportButton.Click += ImportButton_Click;
            // 
            // IntimacyLabel
            // 
            resources.ApplyResources(IntimacyLabel, "IntimacyLabel");
            IntimacyLabel.ForeColor = Color.Orange;
            IntimacyLabel.Name = "IntimacyLabel";
            // 
            // ClearButton
            // 
            resources.ApplyResources(ClearButton, "ClearButton");
            ClearButton.Name = "ClearButton";
            ClearButton.UseVisualStyleBackColor = true;
            ClearButton.Click += ClearButton_Click;
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(ClearButton);
            Controls.Add(IntimacyLabel);
            Controls.Add(ImportButton);
            Controls.Add(StatsPanel);
            Controls.Add(ToggleShowButton);
            Controls.Add(CpuUsageLabel);
            Controls.Add(EmoStringLabel);
            HelpButton = true;
            Name = "Form1";
            TopMost = true;
            FormClosing += Form1_FormClosing;
            StatsPanel.ResumeLayout(false);
            StatsPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer CpuUsageTimer;
        private Label EmoStringLabel;
        private Label CpuUsageLabel;
        private Label TotalTokenLabel;
        private Label DailyTokenLabel;
        private Label FeelingLabel;
        private Label AverageEmotionTokensLabel;
        private Label EmoTokenLabel;
        private Button ToggleShowButton;
        private Label FlopsLabel;
        private Label StatsLabel;
        private Panel StatsPanel;
        private Button ImportButton;
        private Label IntimacyLabel;
        private Button ClearButton;
    }
}
