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
            CpuUsageTimer = new System.Windows.Forms.Timer(components);
            TestLabel1 = new Label();
            TestLabel2 = new Label();
            KibunLabel = new Label();
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
            TestLabel1.Location = new Point(12, 371);
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
            TestLabel2.Location = new Point(12, 421);
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
            KibunLabel.Location = new Point(12, 396);
            KibunLabel.Name = "KibunLabel";
            KibunLabel.Size = new Size(52, 25);
            KibunLabel.TabIndex = 2;
            KibunLabel.Text = "CPU:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(509, 455);
            Controls.Add(KibunLabel);
            Controls.Add(TestLabel2);
            Controls.Add(TestLabel1);
            Name = "Form1";
            Text = "digpet";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer CpuUsageTimer;
        private Label TestLabel1;
        private Label TestLabel2;
        private Label KibunLabel;
    }
}
