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
            helpProvider = new HelpProvider();
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
            helpProvider.SetHelpKeyword(EmoStringLabel, resources.GetString("EmoStringLabel.HelpKeyword"));
            helpProvider.SetHelpNavigator(EmoStringLabel, (HelpNavigator)resources.GetObject("EmoStringLabel.HelpNavigator"));
            helpProvider.SetHelpString(EmoStringLabel, resources.GetString("EmoStringLabel.HelpString"));
            EmoStringLabel.Name = "EmoStringLabel";
            helpProvider.SetShowHelp(EmoStringLabel, (bool)resources.GetObject("EmoStringLabel.ShowHelp"));
            // 
            // CpuUsageLabel
            // 
            resources.ApplyResources(CpuUsageLabel, "CpuUsageLabel");
            CpuUsageLabel.ForeColor = Color.Orange;
            helpProvider.SetHelpKeyword(CpuUsageLabel, resources.GetString("CpuUsageLabel.HelpKeyword"));
            helpProvider.SetHelpNavigator(CpuUsageLabel, (HelpNavigator)resources.GetObject("CpuUsageLabel.HelpNavigator"));
            helpProvider.SetHelpString(CpuUsageLabel, resources.GetString("CpuUsageLabel.HelpString"));
            CpuUsageLabel.Name = "CpuUsageLabel";
            helpProvider.SetShowHelp(CpuUsageLabel, (bool)resources.GetObject("CpuUsageLabel.ShowHelp"));
            // 
            // TotalTokenLabel
            // 
            resources.ApplyResources(TotalTokenLabel, "TotalTokenLabel");
            TotalTokenLabel.ForeColor = Color.Orange;
            helpProvider.SetHelpKeyword(TotalTokenLabel, resources.GetString("TotalTokenLabel.HelpKeyword"));
            helpProvider.SetHelpNavigator(TotalTokenLabel, (HelpNavigator)resources.GetObject("TotalTokenLabel.HelpNavigator"));
            helpProvider.SetHelpString(TotalTokenLabel, resources.GetString("TotalTokenLabel.HelpString"));
            TotalTokenLabel.Name = "TotalTokenLabel";
            helpProvider.SetShowHelp(TotalTokenLabel, (bool)resources.GetObject("TotalTokenLabel.ShowHelp"));
            // 
            // DailyTokenLabel
            // 
            resources.ApplyResources(DailyTokenLabel, "DailyTokenLabel");
            DailyTokenLabel.ForeColor = Color.Orange;
            helpProvider.SetHelpKeyword(DailyTokenLabel, resources.GetString("DailyTokenLabel.HelpKeyword"));
            helpProvider.SetHelpNavigator(DailyTokenLabel, (HelpNavigator)resources.GetObject("DailyTokenLabel.HelpNavigator"));
            helpProvider.SetHelpString(DailyTokenLabel, resources.GetString("DailyTokenLabel.HelpString"));
            DailyTokenLabel.Name = "DailyTokenLabel";
            helpProvider.SetShowHelp(DailyTokenLabel, (bool)resources.GetObject("DailyTokenLabel.ShowHelp"));
            // 
            // FeelingLabel
            // 
            resources.ApplyResources(FeelingLabel, "FeelingLabel");
            FeelingLabel.ForeColor = Color.Orange;
            helpProvider.SetHelpKeyword(FeelingLabel, resources.GetString("FeelingLabel.HelpKeyword"));
            helpProvider.SetHelpNavigator(FeelingLabel, (HelpNavigator)resources.GetObject("FeelingLabel.HelpNavigator"));
            helpProvider.SetHelpString(FeelingLabel, resources.GetString("FeelingLabel.HelpString"));
            FeelingLabel.Name = "FeelingLabel";
            helpProvider.SetShowHelp(FeelingLabel, (bool)resources.GetObject("FeelingLabel.ShowHelp"));
            // 
            // AverageEmotionTokensLabel
            // 
            resources.ApplyResources(AverageEmotionTokensLabel, "AverageEmotionTokensLabel");
            AverageEmotionTokensLabel.ForeColor = Color.Orange;
            helpProvider.SetHelpKeyword(AverageEmotionTokensLabel, resources.GetString("AverageEmotionTokensLabel.HelpKeyword"));
            helpProvider.SetHelpNavigator(AverageEmotionTokensLabel, (HelpNavigator)resources.GetObject("AverageEmotionTokensLabel.HelpNavigator"));
            helpProvider.SetHelpString(AverageEmotionTokensLabel, resources.GetString("AverageEmotionTokensLabel.HelpString"));
            AverageEmotionTokensLabel.Name = "AverageEmotionTokensLabel";
            helpProvider.SetShowHelp(AverageEmotionTokensLabel, (bool)resources.GetObject("AverageEmotionTokensLabel.ShowHelp"));
            // 
            // EmoTokenLabel
            // 
            resources.ApplyResources(EmoTokenLabel, "EmoTokenLabel");
            EmoTokenLabel.ForeColor = Color.Orange;
            helpProvider.SetHelpKeyword(EmoTokenLabel, resources.GetString("EmoTokenLabel.HelpKeyword"));
            helpProvider.SetHelpNavigator(EmoTokenLabel, (HelpNavigator)resources.GetObject("EmoTokenLabel.HelpNavigator"));
            helpProvider.SetHelpString(EmoTokenLabel, resources.GetString("EmoTokenLabel.HelpString"));
            EmoTokenLabel.Name = "EmoTokenLabel";
            helpProvider.SetShowHelp(EmoTokenLabel, (bool)resources.GetObject("EmoTokenLabel.ShowHelp"));
            // 
            // ToggleShowButton
            // 
            resources.ApplyResources(ToggleShowButton, "ToggleShowButton");
            helpProvider.SetHelpKeyword(ToggleShowButton, resources.GetString("ToggleShowButton.HelpKeyword"));
            helpProvider.SetHelpNavigator(ToggleShowButton, (HelpNavigator)resources.GetObject("ToggleShowButton.HelpNavigator"));
            helpProvider.SetHelpString(ToggleShowButton, resources.GetString("ToggleShowButton.HelpString"));
            ToggleShowButton.Name = "ToggleShowButton";
            helpProvider.SetShowHelp(ToggleShowButton, (bool)resources.GetObject("ToggleShowButton.ShowHelp"));
            ToggleShowButton.UseVisualStyleBackColor = true;
            ToggleShowButton.Click += ToggleShowButton_Click;
            // 
            // FlopsLabel
            // 
            resources.ApplyResources(FlopsLabel, "FlopsLabel");
            FlopsLabel.ForeColor = Color.Orange;
            helpProvider.SetHelpKeyword(FlopsLabel, resources.GetString("FlopsLabel.HelpKeyword"));
            helpProvider.SetHelpNavigator(FlopsLabel, (HelpNavigator)resources.GetObject("FlopsLabel.HelpNavigator"));
            helpProvider.SetHelpString(FlopsLabel, resources.GetString("FlopsLabel.HelpString"));
            FlopsLabel.Name = "FlopsLabel";
            helpProvider.SetShowHelp(FlopsLabel, (bool)resources.GetObject("FlopsLabel.ShowHelp"));
            // 
            // StatsLabel
            // 
            resources.ApplyResources(StatsLabel, "StatsLabel");
            StatsLabel.ForeColor = Color.Orange;
            helpProvider.SetHelpKeyword(StatsLabel, resources.GetString("StatsLabel.HelpKeyword"));
            helpProvider.SetHelpNavigator(StatsLabel, (HelpNavigator)resources.GetObject("StatsLabel.HelpNavigator"));
            helpProvider.SetHelpString(StatsLabel, resources.GetString("StatsLabel.HelpString"));
            StatsLabel.Name = "StatsLabel";
            helpProvider.SetShowHelp(StatsLabel, (bool)resources.GetObject("StatsLabel.ShowHelp"));
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
            helpProvider.SetHelpKeyword(StatsPanel, resources.GetString("StatsPanel.HelpKeyword"));
            helpProvider.SetHelpNavigator(StatsPanel, (HelpNavigator)resources.GetObject("StatsPanel.HelpNavigator"));
            helpProvider.SetHelpString(StatsPanel, resources.GetString("StatsPanel.HelpString"));
            StatsPanel.Name = "StatsPanel";
            helpProvider.SetShowHelp(StatsPanel, (bool)resources.GetObject("StatsPanel.ShowHelp"));
            // 
            // helpProvider
            // 
            resources.ApplyResources(helpProvider, "helpProvider");
            // 
            // ImportButton
            // 
            resources.ApplyResources(ImportButton, "ImportButton");
            helpProvider.SetHelpKeyword(ImportButton, resources.GetString("ImportButton.HelpKeyword"));
            helpProvider.SetHelpNavigator(ImportButton, (HelpNavigator)resources.GetObject("ImportButton.HelpNavigator"));
            helpProvider.SetHelpString(ImportButton, resources.GetString("ImportButton.HelpString"));
            ImportButton.Name = "ImportButton";
            helpProvider.SetShowHelp(ImportButton, (bool)resources.GetObject("ImportButton.ShowHelp"));
            ImportButton.UseVisualStyleBackColor = true;
            ImportButton.Click += ImportButton_Click;
            // 
            // IntimacyLabel
            // 
            resources.ApplyResources(IntimacyLabel, "IntimacyLabel");
            IntimacyLabel.ForeColor = Color.Orange;
            helpProvider.SetHelpKeyword(IntimacyLabel, resources.GetString("IntimacyLabel.HelpKeyword"));
            helpProvider.SetHelpNavigator(IntimacyLabel, (HelpNavigator)resources.GetObject("IntimacyLabel.HelpNavigator"));
            helpProvider.SetHelpString(IntimacyLabel, resources.GetString("IntimacyLabel.HelpString"));
            IntimacyLabel.Name = "IntimacyLabel";
            helpProvider.SetShowHelp(IntimacyLabel, (bool)resources.GetObject("IntimacyLabel.ShowHelp"));
            // 
            // ClearButton
            // 
            resources.ApplyResources(ClearButton, "ClearButton");
            helpProvider.SetHelpKeyword(ClearButton, resources.GetString("ClearButton.HelpKeyword"));
            helpProvider.SetHelpNavigator(ClearButton, (HelpNavigator)resources.GetObject("ClearButton.HelpNavigator"));
            helpProvider.SetHelpString(ClearButton, resources.GetString("ClearButton.HelpString"));
            ClearButton.Name = "ClearButton";
            helpProvider.SetShowHelp(ClearButton, (bool)resources.GetObject("ClearButton.ShowHelp"));
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
            FormBorderStyle = FormBorderStyle.FixedSingle;
            HelpButton = true;
            helpProvider.SetHelpKeyword(this, resources.GetString("$this.HelpKeyword"));
            helpProvider.SetHelpNavigator(this, (HelpNavigator)resources.GetObject("$this.HelpNavigator"));
            helpProvider.SetHelpString(this, resources.GetString("$this.HelpString"));
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            helpProvider.SetShowHelp(this, (bool)resources.GetObject("$this.ShowHelp"));
            TopMost = true;
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
        private HelpProvider helpProvider;
        private Button ImportButton;
        private Label IntimacyLabel;
        private Button ClearButton;
    }
}
