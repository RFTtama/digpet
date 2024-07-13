namespace digpet
{
    public partial class Form1 : Form
    {
        //�N���X�֘A�̐錾
        private CpuAvgManager cpuAvgManager;
        private TokenManager tokenManager;

        //�ϐ��֘A�̐錾
        private int cpuCnt;
        private double cpuAvg;

        public Form1()
        {
            InitializeComponent();
            cpuCnt = 0;
            cpuAvg = 0.0;
            cpuAvgManager = new CpuAvgManager();
            tokenManager = new TokenManager();
            CpuUsageTimer.Enabled = true;
        }


        /// <summary>
        /// 1��������CPU�g�p���̕��ς����߂āA�ϐ��ɑ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CpuUsageTimer_Tick(object sender, EventArgs e)
        {
            if ((cpuCnt > 0) && (cpuCnt % 60 == 0))
            {
                try
                {
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
                SumCpuAvg();
            }
        }

        /// <summary>
        /// CPU�g�p���̕��ς�����cpuAvg�ɑ������
        /// </summary>
        private void GetCpuAvg()
        {
            cpuCnt = 0;
            cpuAvg = cpuAvgManager.GetCpuAvg();
            TestLabel1.Text = tokenManager.CalcMinTokens(cpuAvg).ToString();
            cpuAvgManager.Clear();
        }

        /// <summary>
        /// CPU�g�p���̕��ς����߂���悤�ɐ��l�𑫂�
        /// </summary>
        private void SumCpuAvg()
        {
            double cpuUsage = (double)CpuWatcher.GetCpuUsage();
            cpuAvgManager.SetCpuSum(cpuUsage);
            TestLabel2.Text = cpuUsage.ToString();
            cpuCnt++;
        }
    }
}
