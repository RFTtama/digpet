using System.Diagnostics;
using System.Runtime.InteropServices;

namespace digpet.Modules
{
    /// <summary>
    /// CPU使用率を取得するためのstaticクラス
    /// </summary>
    internal class CpuWatcher
    {
        //クラス
        private CpuWatcherInterface? _interface;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CpuWatcher()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _interface = new WindowsCpuWatcher();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _interface = new LinuxCpuWatcher();
            }
            else
            {
                _interface = new OtherCpuWatcher();
                ErrorLog.ErrorOutput("使用OS取得エラー", "サポートされていないOSでの起動です");
            }
        }

        /// <summary>
        /// cpu利用率を取得する
        /// </summary>
        /// <returns>cpu利用率(float)</returns>
        public float GetCpuUsage()
        {
            if (_interface == null)
            {
                return 0.0f;
            }
            return _interface.GetCpuUsage();
        }

        /// <summary>
        /// CPU監視用インタフェース
        /// </summary>
        private interface CpuWatcherInterface
        {
            float GetCpuUsage();
        }

        /// <summary>
        /// Windows用CPU監視クラス
        /// </summary>
        private class WindowsCpuWatcher: CpuWatcherInterface
        {
            //cpu用のパフォーマンスカウンタ
            private static PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", ".");

            /// <summary>
            /// CPU使用率の取得
            /// </summary>
            /// <returns></returns>
            public float GetCpuUsage()
            {
                return cpuCounter.NextValue();
            }
        }

        /// <summary>
        /// Linux用CPU監視クラス
        /// </summary>
        private class LinuxCpuWatcher: CpuWatcherInterface
        {
            //変数の宣言
            private long idleTimeStart = 0;
            private long totalTimeStart = 0;

            public LinuxCpuWatcher()
            {
                string[] firstLine = File.ReadAllLines("/proc/stat");
                string[] firstCpuStats = firstLine[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                idleTimeStart = long.Parse(firstCpuStats[4]);
                totalTimeStart = 0;
                for (int i = 1; i < firstCpuStats.Length; i++)
                {
                    totalTimeStart += long.Parse(firstCpuStats[i]);
                }
            }

            /// <summary>
            /// CPU使用率の取得
            /// </summary>
            /// <returns></returns>
            public float GetCpuUsage()
            {
                string[] secondLine = File.ReadAllLines("/proc/stat");
                string[] secondCpuStats = secondLine[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                long idleTimeEnd = long.Parse(secondCpuStats[4]);
                long totalTimeEnd = 0;
                for (int i = 1; i < secondCpuStats.Length; i++)
                {
                    totalTimeEnd += long.Parse(secondCpuStats[i]);
                }

                long idleDelta = idleTimeEnd - idleTimeStart;
                long totalDelta = totalTimeEnd - totalTimeStart;

                float cpuUsage = (1.0f - (float)idleDelta / totalDelta) * 100;

                idleTimeStart = idleTimeEnd;
                totalTimeStart = totalTimeEnd;

                return cpuUsage;
            }
        }

        private class OtherCpuWatcher : CpuWatcherInterface
        {
            /// <summary>
            /// CPU使用率の取得
            /// </summary>
            /// <returns></returns>
            public float GetCpuUsage()
            {
                //対応していないため、0を返却
                return 0.0f;
            }
        }
    }
}
