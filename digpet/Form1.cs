namespace digpet
{
    public partial class Form1 : Form
    {
        //クラス関連の宣言
        private CpuAvgManager cpuAvgManager = new CpuAvgManager();
        private TokenManager tokenManager = new TokenManager();

        //変数関連の宣言
        private int cpuCnt;
        private double cpuAvg;
        private Label[] labelArray = Array.Empty<Label>();

        //定数の宣言
        private readonly string[] FEELING_STRING =
        {
            "悪い", "普通", "良い", "最高"
        };

        public Form1()
        {
            InitializeComponent();
            Init();
            CpuUsageTimer.Enabled = true;
            Label1Out(tokenManager.DailyTokens);
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void Init()
        {
            InitLabelArray();
            cpuCnt = 0;
            cpuAvg = 0.0;
        }

        /// <summary>
        /// ラベルアレイの要素を初期化する
        /// </summary>
        private void InitLabelArray()
        {
            labelArray =
            [
                StatsLabel,
                DailyTokenLabel,
                EmoTokenLabel,
                AverageEmotionTokensLabel,
                FeelingLabel,
                FlopsLabel
            ];
        }


        /// <summary>
        /// 1分おきにCPU使用率の平均を求めて、変数に代入する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CpuUsageTimer_Tick(object sender, EventArgs e)
        {
            //60秒に1回処理を行う
            if ((cpuCnt > 0) && (cpuCnt % 60 == 0))
            {
                try
                {
                    //CPU使用率の平均を取得し、トークンを計算する
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
                //CPU使用率を加算
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
            tokenManager.AddTokens(cpuAvg);
            Label1Out(tokenManager.DailyTokens);
            cpuAvgManager.Clear();
        }

        /// <summary>
        /// CPU使用率の平均を求められるように数値を足す
        /// </summary>
        private void SumCpuAvg()
        {
            double cpuUsage = (double)CpuWatcher.GetCpuUsage();
            cpuAvgManager.SetCpuSum(cpuUsage);
            Label2Out(cpuUsage);
            cpuCnt++;
        }

        /// <summary>
        /// 今日の気分を文字に変換する
        /// </summary>
        /// <param name="feeling">今日の気分(double)</param>
        /// <returns></returns>
        private string GetFeeling(double feeling)
        {
            double feel = feeling;
            if (feel > 1.0) feel = 1.0;
            if (feel < -1.0) feel = -1.0;

            string feelingText;

            if (feel < -0.49)
            {
                feelingText = FEELING_STRING[0];
            }
            else if (feel < 0.0)
            {
                feelingText = FEELING_STRING[1];
            }
            else if (feel < 0.3)
            {
                feelingText = FEELING_STRING[2];
            }
            else
            {
                feelingText = FEELING_STRING[3];
            }

            return feelingText;
        }

        /// <summary>
        /// 今日のトークンを出力する(テスト)
        /// </summary>
        /// <param name="value">出力するトークン</param>
        private void Label1Out(double value)
        {
            TestLabel1.Text = GetFeeling(tokenManager.Feeling);
            DailyTokenLabel.Text = "今日の獲得トークン: " + tokenManager.DailyTokens.ToString("n2");
            EmoTokenLabel.Text = "今日の感情トークン: " + tokenManager.EmotionTokens.ToString("n2");
            AverageEmotionTokensLabel.Text = "平均感情トークン: " + tokenManager.AverageEmotionTokens.ToString("n2");
            FeelingLabel.Text = "今日の感情: " + tokenManager.Feeling.ToString("n2");
            FlopsLabel.Text = "FLOPS: " + tokenManager.Flops.ToString();
            KibunLabelOut(value);
        }

        /// <summary>
        /// 現在のCPU使用率を出力する(テスト)
        /// </summary>
        /// <param name="value">出力するCPU使用率</param>
        private void Label2Out(double value)
        {
            TestLabel2.Text = "CPU: " + value.ToString("n2") + "%";
        }

        /// <summary>
        /// トークンの獲得量を出力する(テスト)
        /// </summary>
        /// <param name="value">出力するトークンの獲得量</param>
        private void KibunLabelOut(double value)
        {
            KibunLabel.Text = "累計トークン: " + (tokenManager.TotalTokens).ToString("n2");
        }

        private void ToggleShowButton_Click(object sender, EventArgs e)
        {
            foreach (Label targetLabel in labelArray)
            {
                targetLabel.Visible = !targetLabel.Visible;
            }
        }
    }
}
