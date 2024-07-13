namespace digpet
{
    public partial class Form1 : Form
    {
        //クラス関連の宣言
        private CpuAvgManager cpuAvgManager;
        private TokenManager tokenManager;

        //変数関連の宣言
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
        /// 1分おきにCPU使用率の平均を求めて、変数に代入する
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
                    ErrorLog.ErrorOutput("CPU使用率平均計算エラー", ex.Message, true);
                    CpuUsageTimer.Enabled = false;
                }
            }
            else
            {
                SumCpuAvg();
            }
        }

        /// <summary>
        /// CPU使用率の平均を求めcpuAvgに代入する
        /// </summary>
        private void GetCpuAvg()
        {
            cpuCnt = 0;
            cpuAvg = cpuAvgManager.GetCpuAvg();
            TestLabel1.Text = tokenManager.CalcMinTokens(cpuAvg).ToString();
            cpuAvgManager.Clear();
        }

        /// <summary>
        /// CPU使用率の平均を求められるように数値を足す
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
