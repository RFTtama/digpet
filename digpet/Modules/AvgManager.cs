using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digpet.Modules
{
    /// <summary>
    /// 平均値算出用モジュール
    /// </summary>
    internal class AvgManager
    {
        //変数関連の宣言
        private double _cpuSum;                 //現在のCPU使用率の合計
        private uint _cpuCount;                 //合計を足した回数

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
