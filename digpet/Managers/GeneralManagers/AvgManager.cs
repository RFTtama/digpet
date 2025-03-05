namespace digpet.Managers.GenerakManager
{
    /// <summary>
    /// 平均値算出用モジュール
    /// </summary>
    internal class AvgManager
    {
        //変数関連の宣言
        private double _sum;                 //数値合計
        private uint _count;                 //合計を足した回数

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AvgManager()
        {
            Clear();
        }

        /// <summary>
        /// 変数の中身などを初期化する
        /// </summary>
        public void Clear()
        {
            _sum = 0;
            _count = 0;
        }

        /// <summary>
        /// 数値加算する
        /// </summary>
        /// <param name="value">CPU使用率</param>
        public void Sum(double value)
        {
            _count++;
            _sum += value;
        }

        /// <summary>
        /// 平均を取得
        /// </summary>
        /// <returns>平均(double)</returns>
        public double GetAvg()
        {
            if (_count == 0) return 0.0;
            return _sum / _count;
        }
    }
}
