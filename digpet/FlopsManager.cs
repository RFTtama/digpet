namespace digpet
{
    /// <summary>
    /// FLOPSの管理クラス
    /// </summary>
    internal class FlopsManager
    {
        private const uint CALC_SECOND = 10;                            //FLOPSの計算時間(s)
        private const ulong CALC_MILLISECONDS = CALC_SECOND * 1000;     //1秒間に含まれるミリセカンド数(変更するな)
        private ulong _flops;                                           //計算したFLOPを入れておくための変数

        /// <summary>
        /// FLOPS数値
        /// </summary>
        public ulong Flops
        {
            get { return _flops; }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="initNum">FLOPSの初期値</param>
        public FlopsManager(ulong initNum)
        {
            _flops = initNum;
        }

        /// <summary>
        /// 10秒間FLOPSを計算する(非同期)
        /// 計算後にFlopsが置き換わる
        /// </summary>
        public void CalcFlops()
        {
            //Task処理でFLOPSの計算を行う
            Task.Run(() =>
            {
                ulong processNum = 0;
                DateTime startTime = DateTime.Now;

                //指定した秒数が経過するまで処理を続ける
                while ((DateTime.Now - startTime).TotalMilliseconds < CALC_MILLISECONDS)
                {
                    //FLOPS計算
                    _ = -1 / 3.0f;
                    processNum++;
                }

                _flops = processNum / CALC_SECOND;
            });
        }
    }
}
