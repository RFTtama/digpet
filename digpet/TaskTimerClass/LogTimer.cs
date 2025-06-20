using digpet.Managers;
using digpet.Models.AbstractModels;
using digpet.Modules;
using System.Globalization;

namespace digpet.TaskTimerClass
{
    public class LogTimer : TaskClassModel
    {
        // 変数宣言
        private static string _logDump = string.Empty;
        private static int step = 0;

        // 辞書系
        private static Dictionary<string, string> logDict = new Dictionary<string, string>();

        // 関数テーブル
        private Action[] stepFuncTable =
        {
            Step01,
        };

        /// <summary>
        /// 周期関数
        /// </summary>
        /// <returns></returns>
        public override TaskReturn TaskFunc()
        {
            if ((step >= 0) && (step < stepFuncTable.Length))
            {
                stepFuncTable[step]();
                return TaskReturn.TASK_SUCCESS;
            }

            return TaskReturn.TASK_FAILURE;
        }

        public override void TaskCheckRet(TaskReturn ret)
        {

        }

        /// <summary>
        /// ログの保存依頼
        /// </summary>
        /// <param name="logKey">キー</param>
        /// <param name="logValue">バリュー</param>
        /// <returns></returns>
        public static bool SaveLog(string logKey, string logValue)
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
        private static void Step01()
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

        /// <summary>
        /// 書き出し
        /// </summary>
        private static void ExportInClass()
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
