namespace digpet
{
    public partial class Form1 : Form
    {
        //�N���X�֘A�̐錾
        private CpuAvgManager cpuAvgManager = new CpuAvgManager();
        private TokenManager tokenManager = new TokenManager();
        private SettingManager settingManager = new SettingManager();

        //�ϐ��֘A�̐錾
        private int cpuCnt;
        private double cpuAvg;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            Init();
            CpuUsageTimer.Enabled = true;
            OutTokenLabel();
        }

        /// <summary>
        /// ������
        /// </summary>
        private void Init()
        {
            cpuCnt = 0;
            cpuAvg = 0.0;
        }

        /// <summary>
        /// 1��������CPU�g�p���̕��ς����߂āA�ϐ��ɑ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CpuUsageTimer_Tick(object sender, EventArgs e)
        {
            //60�b��1�񏈗����s��
            if ((cpuCnt > 0) && (cpuCnt % 60 == 0))
            {
                try
                {
                    //CPU�g�p���̕��ς��擾���A�g�[�N�����v�Z����
                    GetCpuAvg();
                }
                catch (Exception ex)
                {
                    ErrorLog.ErrorOutput("CPU�g�p�����όv�Z�G���[", ex.Message, true);
                    CpuUsageTimer.Enabled = false;
                }
            }
            else
            {
                //CPU�g�p�������Z
                SumCpuAvg();
            }
            cpuCnt++;
        }

        /// <summary>
        /// CPU�g�p���̕��ς�����cpuAvg�ɑ������
        /// </summary>
        private void GetCpuAvg()
        {
            cpuCnt = 0;
            cpuAvg = cpuAvgManager.GetCpuAvg();
            tokenManager.AddTokens(cpuAvg);
            OutTokenLabel();
            cpuAvgManager.Clear();
        }

        /// <summary>
        /// CPU�g�p���̕��ς����߂���悤�ɐ��l�𑫂�
        /// </summary>
        private void SumCpuAvg()
        {
            double cpuUsage = (double)CpuWatcher.GetCpuUsage();
            cpuAvgManager.SetCpuSum(cpuUsage);
            OutCpuLabel(cpuUsage);
        }

        /// <summary>
        /// �����̋C���𕶎��ɕϊ�����
        /// </summary>
        /// <param name="feeling">�����̋C��(double)</param>
        /// <returns></returns>
        private string GetFeeling(double feeling)
        {
            double feel = feeling;
            if (feel > 1.0) feel = 1.0;
            if (feel < -1.0) feel = -1.0;

            return settingManager.GetFeelingString(feel);
        }

        /// <summary>
        /// ���݂̐e���x�𕶎��ɕϊ�����
        /// </summary>
        /// <param name="intimacy">�e���x</param>
        /// <returns></returns>
        private string GetIntimacy(double intimacy)
        {
            double inti = intimacy;
            if (inti < 0.0) inti = 0.0;

            return settingManager.GetIntimacyString(inti);
        }

        /// <summary>
        /// �����̃g�[�N�����o�͂���(�e�X�g)
        /// </summary>
        /// <param name="value">�o�͂���g�[�N��</param>
        private void OutTokenLabel()
        {
            EmoStringLabel.Text = GetFeeling(tokenManager.Feeling);
            DailyTokenLabel.Text = "�����̊l���g�[�N��: " + tokenManager.DailyTokens.ToString("n2");
            EmoTokenLabel.Text = "�����̊���g�[�N��: " + tokenManager.EmotionTokens.ToString("n2");
            AverageEmotionTokensLabel.Text = "���ϊ���g�[�N��: " + tokenManager.AverageEmotionTokens.ToString("n2");
            FeelingLabel.Text = "�����̊���: " + tokenManager.Feeling.ToString("n2");
            FlopsLabel.Text = "FLOPS: " + tokenManager.Flops.ToString();
            TotalTokenLabel.Text = "�݌v�g�[�N��: " + (tokenManager.TotalTokens).ToString("n2");
            IntimacyLabel.Text = GetIntimacy(tokenManager.TotalTokens);
        }

        /// <summary>
        /// ���݂�CPU�g�p�����o�͂���(�e�X�g)
        /// </summary>
        /// <param name="value">�o�͂���CPU�g�p��</param>
        private void OutCpuLabel(double value)
        {
            CpuUsageLabel.Text = "CPU: " + value.ToString("n2") + "%";
        }

        /// <summary>
        /// ���v���̕\����؂�ւ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleShowButton_Click(object sender, EventArgs e)
        {
            StatsPanel.Visible = !StatsPanel.Visible;
            ImportButton.Visible = !ImportButton.Visible;
        }
    }
}
