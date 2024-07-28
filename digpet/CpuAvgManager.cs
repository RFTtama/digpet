namespace digpet
{
    /// <summary>
    /// CPU使用率を管理するためのマネージャ
    /// </summary>
    internal class CpuAvgManager
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
            if (((_cpuSum + cpuUsage) > double.PositiveInfinity) || (_cpuCount + 1 > int.MaxValue))
            {
                ErrorLog.ErrorOutput("CPU値のオーバーフローエラー", "CPU値がオーバーフローしています", true);
                return;
            }

            _cpuSum += cpuUsage;
            _cpuCount++;
        }

        /// <summary>
        /// CPU使用率の平均を取得
        /// </summary>
        /// <returns>CPU使用率の平均(double)</returns>
        public double GetCpuAvg()
        {
            if (_cpuCount == 0) return 0.0;
            return (_cpuSum / _cpuCount);
        }
    }
}
