using digpet.Managers;
using System.Xml.Linq;

namespace digpet.Modules
{
    /// <summary>
    /// エラー出力クラス
    /// </summary>
    public class ErrorLogLib
    {
        private static Lazy<ErrorLogLib> _lazy = new(() => new ErrorLogLib(), isThreadSafe: true);
        public static ErrorLogLib Instance => _lazy.Value;

        private string _logDump = string.Empty;

        private ErrorLogLib() { }

        /// <summary>
        /// エラーを出力する
        /// </summary>
        /// <param name="name">エラー名</param>
        /// <param name="msg">エラーメッセージ</param>
        /// <param name="show">表示するか</param>
        public void ErrorOutput(string name, string msg, bool show)
        {
            _logDump += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss [") + name + "]" + msg + "\r\n\n";
            if (show) TaskMessageLib.OutputMessage(msg, name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// エラーを出力する
        /// </summary>
        /// <param name="name">エラー名</param>
        /// <param name="msg">エラーメッセージ</param>
        public void ErrorOutput(string name, string msg)
        {
            ErrorOutput(name, msg, true);
        }

        /// <summary>
        /// エラーログをファイルに書き出す
        /// </summary>
        public void Export()
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
