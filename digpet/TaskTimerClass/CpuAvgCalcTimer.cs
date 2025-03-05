using digpet.Managers.GenerakManager;
using digpet.Models.AbstractModels;
using digpet.Modules;

namespace digpet.TimerClass
{
    /// <summary>
    /// CPU使用率を管理するためのマネージャ
    /// </summary>
    /// 
    internal class CpuAvgCalcTimer : TaskClassModel
    {
        //変数宣言
        private uint cpuCnt = 0;
        private double _cpuAvg = 0.0;
        private bool _avgCalcFlg = false;
        private double _cpuUsage = 0.0;

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

        /// <summary>
        /// タスク処理
        /// </summary>
        /// <returns>ステータス</returns>
        public override TaskReturn TaskFunc()
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
            return TaskReturn.TASK_SUCCESS;
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
        /// 戻り値処理
        /// </summary>
        /// <param name="ret">戻り値</param>
        public override void TaskCheckRet(TaskReturn ret)
        {
            switch (ret)
            {
                default:
                    break;
            }
        }

        /// <summary>
        /// CPU使用率の平均を求めcpuAvgに代入する
        /// </summary>
        private void CalcCpuAvg()
        {
            _cpuAvg = cpuAvgManager.GetAvg();

            _avgCalcFlg = true;
            cpuAvgManager.Clear();
            LogLib.LogOutput("分毎トークンの算出完了");
        }
    }
}
