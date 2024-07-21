using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digpet
{
    /// <summary>
    /// FLOPSの管理クラス
    /// </summary>
    internal class FlopsManager
    {
        private const int CALC_SECOND = 10;
        private const ulong CALC_MILLISECONDS = CALC_SECOND * 1000;
        private ulong _flops;

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
        /// FLOPSを計算する(非同期)
        /// </summary>
        public void CalcFlops()
        {
            Task.Run(() =>
            {
                ulong processNum = 0;
                DateTime startTime = DateTime.Now;

                while ((DateTime.Now - startTime).TotalMilliseconds < CALC_MILLISECONDS)
                {
                    _ = -1 / 3.0f;
                    processNum++;
                }

                _flops = processNum / CALC_SECOND;
            });
        }
    }
}
