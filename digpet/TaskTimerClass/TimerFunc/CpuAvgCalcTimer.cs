using digpet.Managers.GenerakManager;

namespace digpet.TaskTimerClass.TimerFunc
{
    /// <summary>
    /// CPU使用率を管理するためのマネージャ
    /// </summary>
    /// 
    public class CpuAvgCalcTimer
    {
        private static Lazy<CpuAvgCalcTimer> _lazy = new(() => new CpuAvgCalcTimer(), isThreadSafe: true);
        public static CpuAvgCalcTimer Instance => _lazy.Value;

        private System.Threading.Timer _timer;

        //変数宣言
        private uint cpuCnt;
        private double _cpuAvg;
        private bool _avgCalcFlg;
        private double _cpuUsage;

        //クラス宣言
        private AvgManager cpuAvgManager = new AvgManager();
        private CpuWatcher cpuWatcher = new CpuWatcher();

        //ゲッター
        public bool AvgCalcFlg
        {
            get { return _avgCalcFlg; }
        }
        public double CpuAvg
        {
            get { return _cpuAvg; }
        }
        public double CpuUsage
        {
            get { return _cpuUsage; }
        }

        private CpuAvgCalcTimer()
        {
            cpuCnt = 0;
            _cpuAvg = 0.0;
            _avgCalcFlg = false;
            _cpuUsage = 0.0;
            _timer = new(TaskFunc, null, 0, 1000);
        }

        /// <summary>
        /// タスク処理
        /// </summary>
        /// <returns>ステータス</returns>
        private void TaskFunc(object? obj)
        {
            _cpuUsage = (double)cpuWatcher.GetCpuUsage();

            //60秒に1回処理を行う
            if ((cpuCnt > 0) && ((cpuCnt % 60) == 0))
            {
                //CPU使用率の平均を取得し、トークンを計算する
                cpuCnt = 0;
                CalcCpuAvg();
            }

            //CPU使用率を加算
            cpuAvgManager.Sum(CpuUsage);

            cpuCnt++;
        }

        /// <summary>
        /// CPU使用率の平均をクリア
        /// </summary>
        public void ClearCpuAvg()
        {
            cpuCnt = 0;
            _cpuAvg = 0.0;
            _avgCalcFlg = false;
            _cpuUsage = 0.0;
        }

        /// <summary>
        /// CPU使用率の平均を求めcpuAvgに代入する
        /// </summary>
        private void CalcCpuAvg()
        {
            _cpuAvg = cpuAvgManager.GetAvg();

            _avgCalcFlg = true;
            cpuAvgManager.Clear();
        }
    }
}
