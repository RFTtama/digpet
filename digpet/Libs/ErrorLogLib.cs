using digpet.Managers;
using System.Xml.Linq;

namespace digpet.Modules
{
    /// <summary>
    /// エラー出力クラス
    /// </summary>
    static class ErrorLogLib
    {
        private static string _logDump = string.Empty;

        /// <summary>
        /// エラーを出力する
        /// </summary>
        /// <param name="name">エラー名</param>
        /// <param name="msg">エラーメッセージ</param>
        /// <param name="show">表示するか</param>
        public static void ErrorOutput(string name, string msg, bool show)
        {
            _logDump += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss [") + name + "]" + msg + "\r\n\n";
            if (show) TaskMessageLib.OutputMessage(msg, name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// エラーを出力する
        /// </summary>
        /// <param name="name">エラー名</param>
        /// <param name="msg">エラーメッセージ</param>
        public static void ErrorOutput(string name, string msg)
        {
            ErrorOutput(name, msg, true);
        }

        /// <summary>
        /// エラーログをファイルに書き出す
        /// </summary>
        public static void Export()
        {
            try
            {
                if (!string.IsNullOrEmpty(_logDump))
                {
                    using (StreamWriter sw = new StreamWriter(SettingManager.PrivateSettings.ERRORLOG_PATH, true))
                    {
                        sw.Write(_logDump);
                    }
                    _logDump = string.Empty;
                }
            }
            catch (Exception ex)
            {
                TaskMessageLib.OutputMessage(ex.Message, "エラー出力エラー");
            }
        }
    }
}
