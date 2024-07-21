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
            TestLabel1 = new Label();
            TestLabel2 = new Label();
            KibunLabel = new Label();
            DailyTokenLabel = new Label();
            FeelingLabel = new Label();
            AverageEmotionTokensLabel = new Label();
            EmoTokenLabel = new Label();
            ToggleShowButton = new Button();
            FlopsLabel = new Label();
            StatsLabel = new Label();
            SuspendLayout();
            // 
            // CpuUsageTimer
            // 
            CpuUsageTimer.Interval = 1000;
            CpuUsageTimer.Tick += CpuUsageTimer_Tick;
            // 
            // TestLabel1
            // 
            TestLabel1.AutoSize = true;
            TestLabel1.Font = new Font("Yu Gothic UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            TestLabel1.ForeColor = Color.Orange;
            TestLabel1.Location = new Point(12, 377);
            TestLabel1.Name = "TestLabel1";
            TestLabel1.Size = new Size(41, 25);
            TestLabel1.TabIndex = 0;
            TestLabel1.Text = "なし";
            // 
            // TestLabel2
            // 
            TestLabel2.AutoSize = true;
            TestLabel2.Font = new Font("Yu Gothic UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            TestLabel2.ForeColor = Color.Orange;
            TestLabel2.Location = new Point(12, 427);
            TestLabel2.Name = "TestLabel2";
            TestLabel2.Size = new Size(107, 25);
            TestLabel2.TabIndex = 1;
            TestLabel2.Text = "累計トークン:";
            // 
            // KibunLabel
            // 
            KibunLabel.AutoSize = true;
            KibunLabel.Font = new Font("Yu Gothic UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            KibunLabel.ForeColor = Color.Orange;
            KibunLabel.Location = new Point(12, 402);
            KibunLabel.Name = "KibunLabel";
            KibunLabel.Size = new Size(52, 25);
            KibunLabel.TabIndex = 2;
            KibunLabel.Text = "CPU:";
            // 
            // DailyTokenLabel
            // 
            DailyTokenLabel.AutoSize = true;
            DailyTokenLabel.Font = new Font("Yu Gothic UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            DailyTokenLabel.ForeColor = Color.Orange;
            DailyTokenLabel.Location = new Point(12, 34);
            DailyTokenLabel.Name = "DailyTokenLabel";
            DailyTokenLabel.Size = new Size(41, 25);
            DailyTokenLabel.TabIndex = 3;
            DailyTokenLabel.Text = "なし";
            // 
            // FeelingLabel
            // 
            FeelingLabel.AutoSize = true;
            FeelingLabel.Font = new Font("Yu Gothic UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            FeelingLabel.ForeColor = Color.Orange;
            FeelingLabel.Location = new Point(12, 109);
            FeelingLabel.Name = "FeelingLabel";
            FeelingLabel.Size = new Size(41, 25);
            FeelingLabel.TabIndex = 5;
            FeelingLabel.Text = "なし";
            // 
            // AverageEmotionTokensLabel
            // 
            AverageEmotionTokensLabel.AutoSize = true;
            AverageEmotionTokensLabel.Font = new Font("Yu Gothic UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            AverageEmotionTokensLabel.ForeColor = Color.Orange;
            AverageEmotionTokensLabel.Location = new Point(12, 84);
            AverageEmotionTokensLabel.Name = "AverageEmotionTokensLabel";
            AverageEmotionTokensLabel.Size = new Size(41, 25);
            AverageEmotionTokensLabel.TabIndex = 6;
            AverageEmotionTokensLabel.Text = "なし";
            // 
            // EmoTokenLabel
            // 
            EmoTokenLabel.AutoSize = true;
            EmoTokenLabel.Font = new Font("Yu Gothic UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            EmoTokenLabel.ForeColor = Color.Orange;
            EmoTokenLabel.Location = new Point(12, 59);
            EmoTokenLabel.Name = "EmoTokenLabel";
            EmoTokenLabel.Size = new Size(41, 25);
            EmoTokenLabel.TabIndex = 8;
            EmoTokenLabel.Text = "なし";
            // 
            // ToggleShowButton
            // 
            ToggleShowButton.Location = new Point(381, 423);
            ToggleShowButton.Name = "ToggleShowButton";
            ToggleShowButton.Size = new Size(91, 27);
            ToggleShowButton.TabIndex = 9;
            ToggleShowButton.Text = "統計表示切替";
            ToggleShowButton.UseVisualStyleBackColor = true;
            ToggleShowButton.Click += ToggleShowButton_Click;
            // 
            // FlopsLabel
            // 
            FlopsLabel.AutoSize = true;
            FlopsLabel.Font = new Font("Yu Gothic UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            FlopsLabel.ForeColor = Color.Orange;
            FlopsLabel.Location = new Point(12, 134);
            FlopsLabel.Name = "FlopsLabel";
            FlopsLabel.Size = new Size(41, 25);
            FlopsLabel.TabIndex = 10;
            FlopsLabel.Text = "なし";
            // 
            // StatsLabel
            // 
            StatsLabel.AutoSize = true;
            StatsLabel.Font = new Font("Yu Gothic UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 128);
            StatsLabel.ForeColor = Color.Orange;
            StatsLabel.Location = new Point(12, 9);
            StatsLabel.Name = "StatsLabel";
            StatsLabel.Size = new Size(50, 25);
            StatsLabel.TabIndex = 11;
            StatsLabel.Text = "統計";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(484, 461);
            Controls.Add(StatsLabel);
            Controls.Add(FlopsLabel);
            Controls.Add(ToggleShowButton);
            Controls.Add(EmoTokenLabel);
            Controls.Add(AverageEmotionTokensLabel);
            Controls.Add(FeelingLabel);
            Controls.Add(DailyTokenLabel);
            Controls.Add(KibunLabel);
            Controls.Add(TestLabel2);
            Controls.Add(TestLabel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "digpet";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer CpuUsageTimer;
        private Label TestLabel1;
        private Label TestLabel2;
        private Label KibunLabel;
        private Label DailyTokenLabel;
        private Label FeelingLabel;
        private Label AverageEmotionTokensLabel;
        private Label EmoTokenLabel;
        private Button ToggleShowButton;
        private Label FlopsLabel;
        private Label StatsLabel;
    }
}
