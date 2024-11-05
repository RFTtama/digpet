using System.Diagnostics;

namespace digpet.Modules
{
    /// <summary>
    /// CPU使用率を取得するためのstaticクラス
    /// </summary>
    internal static class CpuWatcher
    {
        //cpu用のパフォーマンスカウンタ
        private static PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", ".");

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
