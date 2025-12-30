using digpet.Managers;
using digpet.Modules;
using System.Globalization;

namespace digpet.TaskTimerClass.TimerFunc
{
    public class LogTimer
    {
        private static Lazy<LogTimer> _lazy = new(() => new LogTimer(), isThreadSafe: true);
        public static LogTimer Instance => _lazy.Value;

        // 変数宣言
        private string _logDump;

        // 辞書系
        private Dictionary<string, string> logDict;

        private System.Threading.Timer? _timer;

        private LogTimer()
        {
            _logDump = string.Empty;
            logDict = new Dictionary<string, string>();
            _timer = new System.Threading.Timer(TaskFunc, null, 0, 1000);
        }

        /// <summary>
        /// 周期関数
        /// </summary>
        /// <returns></returns>
        private void TaskFunc(object? obj)
        {
            Step01();
        }

        /// <summary>
        /// ログの保存依頼
        /// </summary>
        /// <param name="logKey">キー</param>
        /// <param name="logValue">バリュー</param>
        /// <returns></returns>
        public bool SaveLog(string logKey, string logValue)
        {
            string value = "→\"" + logValue + "\"";

            try
            {
                logDict.Add(logKey, value);
            }
            catch (ArgumentException)
            {
                logDict[logKey] += value;
            }
            catch (Exception ex)
            {
                ErrorLogLib.ErrorOutput("ログ保存エラー", ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// ログの保存
        /// </summary>
        private void Step01()
        {
            if (logDict.Count > 0)
            {
                string mem = string.Empty;
                mem += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ");

                string[] keys = logDict.Keys.ToArray();
                Array.Sort(keys);

                foreach (string key in keys)
                {
                    mem += "\"" + key + "\": " + logDict[key] + ", ";
                }
                mem += "\r\n\n";

                _logDump += mem;
            }

            logDict.Clear();

            ExportInClass();
        }

        /// <summary>
        /// ログの保存パスを返却する
        /// </summary>
        /// <returns>ログ保存先のパス</returns>
        private string GetLogDirectroy()
        {
            string path = string.Empty;
            path += SettingManager.PrivateSettings.LOG_DIRECTORY + "/";
            path += DateTime.Now.ToString("yyyyMMdd_");
            path += SettingManager.PrivateSettings.LOG_PATH;

            return path;
        }

        /// <summary>
        /// 保存日数を超えたログを削除する
        /// </summary>
        private void DeleteTooMuchLogs()
        {
            string[] files = Directory.GetFiles(SettingManager.PrivateSettings.LOG_DIRECTORY);

            foreach (string file in files)
            {
                string[] clean = Path.GetFileName(file).Split('_');
                DateTime fileDate;
                if (DateTime.TryParseExact(clean[0], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fileDate))
                {
                    if ((DateTime.Now - fileDate).TotalDays >= SettingManager.PublicSettings.LogDeleteDays)
                    {
                        File.Delete(file);
                    }
                }
            }
        }

        /// <summary>
        /// 書き出し
        /// </summary>
        private void ExportInClass()
        {
            try
            {
                if (!Path.Exists(SettingManager.PrivateSettings.LOG_DIRECTORY))
                {
                    Directory.CreateDirectory(SettingManager.PrivateSettings.LOG_DIRECTORY);
                }

                string logPath = GetLogDirectroy();

                if (!File.Exists(logPath))
                {
                    DeleteTooMuchLogs();
                }

                if (!string.IsNullOrEmpty(_logDump))
                {
                    using (StreamWriter sw = new StreamWriter(logPath, true))
                    {
                        sw.Write(_logDump);
                    }
                    _logDump = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorLogLib.ErrorOutput("ログ書き込みエラー", ex.Message);
            }
        }

        /// <summary>
        /// 書き出し
        /// </summary>
        public void Export()
        {
            ExportInClass();
        }
    }
}
