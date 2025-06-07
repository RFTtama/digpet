using digpet.Managers;
using System.Globalization;

namespace digpet.Modules
{
    internal static class LogLib
    {
        private static string _logDump = string.Empty;

        /// <summary>
        /// ログを出力する
        /// </summary>
        /// <param name="msg">メッセージ</param>
        public static void LogOutput(string msg)
        {
            _logDump += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + msg + "\r\n\n";
        }

        /// <summary>
        /// ログの保存パスを返却する
        /// </summary>
        /// <returns>ログ保存先のパス</returns>
        private static string GetLogDirectroy()
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
        private static void DeleteTooMuchLogs()
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

        public static void Export()
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
    }
}
