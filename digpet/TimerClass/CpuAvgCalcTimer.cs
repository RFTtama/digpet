using digpet.Interface;
using digpet.Modules;
using System.Configuration;

namespace digpet.TimerClass
{
    /// <summary>
    /// CPU使用率を管理するためのマネージャ
    /// </summary>
    /// 
    internal class CpuAvgCalcTimer : TaskClassInterface
    {
        //変数宣言
        private uint cpuCnt = 0;
        private double _cpuAvg = 0.0;
        private bool _avgCalcFlg = false;
        private double _cpuUsage = 0.0;

        //クラス宣言
        private CpuAvgManager cpuAvgManager = new CpuAvgManager();
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

        /// <summary>
        /// タスク処理
        /// </summary>
        /// <returns>ステータス</returns>
        public override TaskClassRet TaskFunc()
        {
            double cpuUsage = (double)cpuWatcher.GetCpuUsage();

            //60秒に1回処理を行う
            if (cpuCnt > 0 && cpuCnt % 60 == 0)
            {
                try
                {
                    //CPU使用率の平均を取得し、トークンを計算する
                    cpuCnt = 0;
                    GetCpuAvg();
                }
                catch (Exception ex)
                {
                    ErrorLog.ErrorOutput("CPU使用率平均計算エラー", ex.Message);
                    return new TaskClassRet(TaskReturn.TASK_FAILURE, string.Empty);
                }
            }
            else
            {
                //CPU使用率を加算
                cpuAvgManager.SetCpuSum(cpuUsage);
            }

            _cpuUsage = cpuUsage;

            cpuCnt++;
            return new TaskClassRet(TaskReturn.TASK_SUCCESS, string.Empty);
        }

        /// <summary>
        /// CPU使用率の平均をクリア
        /// </summary>
        public void ClearCpuAvg()
        {
            _cpuAvg = 0.0;
            _avgCalcFlg = false;

        }

        /// <summary>
        /// 戻り値処理
        /// </summary>
        /// <param name="ret">戻り値</param>
        public override void TaskCheckRet(TaskClassRet ret)
        {
            switch (ret.taskReturn)
            {
                default:
                    break;
            }
        }

        /// <summary>
        /// CPU使用率の平均を求めcpuAvgに代入する
        /// </summary>
        private void GetCpuAvg()
        {
            _cpuAvg = cpuAvgManager.GetCpuAvg();

            _avgCalcFlg = true;
            cpuAvgManager.Clear();
            LogManager.LogOutput("分毎トークンの算出完了");
        }

        /// <summary>
        /// CPUの使用率の平均算出用クラス
        /// </summary>
        private class CpuAvgManager
        {
            //変数関連の宣言
            private double _cpuSum;                 //現在のCPU使用率の合計
            private uint _cpuCount;                 //合計を足した回数

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public CpuAvgManager()
            {
                Clear();
            }

            /// <summary>
            /// 変数の中身などを初期化する
            /// </summary>
            public void Clear()
            {
                _cpuSum = 0;
                _cpuCount = 0;
            }

            /// <summary>
            /// CPU使用率を加算する
            /// </summary>
            /// <param name="cpuUsage">CPU使用率</param>
            public void SetCpuSum(double cpuUsage)
            {
                if (_cpuCount >= uint.MaxValue)
                {
                    _cpuCount = uint.MaxValue;
                }
                else
                {
                    _cpuCount++;
                }

                if (_cpuSum >= double.MaxValue)
                {
                    _cpuSum = double.MaxValue;
                }
                else
                {
                    _cpuSum += cpuUsage;
                }
            }

            /// <summary>
            /// CPU使用率の平均を取得
            /// </summary>
            /// <returns>CPU使用率の平均(double)</returns>
            public double GetCpuAvg()
            {
                if (_cpuCount == 0) return 0.0;
                return _cpuSum / _cpuCount;
            }
        }
    }
}
