using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digpet.Modules
{
    /// <summary>
    /// リングバッファのフラグ保持モジュール
    /// </summary>
    internal class RingFlagMemClass
    {
        //変数宣言
        private bool[] ringBuf = Array.Empty<bool>();
        private int ind = 0;

        //ゲッター
        public int BufferSize
        {
            get
            {
                return ringBuf.Length;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="size"></param>
        public RingFlagMemClass(int size)
        {
            ind = 0;
            ringBuf = new bool[size];
        }

        /// <summary>
        /// リングバッファにブール値を記録する
        /// </summary>
        /// <param name="flag">記録するブール値</param>
        public void Add(bool flag)
        {
            if (ringBuf.Length > 0)
            {
                ringBuf[ind] = flag;
                ind++;
                ind = ind % ringBuf.Length;
            }
        }

        /// <summary>
        /// trueの合計数を算出する
        /// </summary>
        /// <returns>リングバッファに格納されているtrue要素の数</returns>
        public int GetTotalOfTrue()
        {
            int ret = 0;
            for (int i = 0; i < ringBuf.Length; i++)
            {
                if (ringBuf[i])
                {
                    ret++;
                }
            }

            return ret;
        }
    }
}
