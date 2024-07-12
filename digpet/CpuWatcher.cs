using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digpet
{
    /// <summary>
    /// CPU使用率を取得するためのstaticクラス
    /// </summary>
    internal static class CpuWatcher
    {
        //cpu用のパフォーマンスカウンタ
        private static PerformanceCounter cpuCounter = new (".", "Processor", "% Processor Time", "_Total");

        /// <summary>
        /// cpu利用率を取得する
        /// </summary>
        /// <returns>cpu利用率(float)</returns>
        public static float GetCpuUsage()
        {
            return cpuCounter.NextValue();
        }
    }
}
