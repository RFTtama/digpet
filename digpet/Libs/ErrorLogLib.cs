using digpet.Managers;

namespace digpet.Modules
{
    /// <summary>
    /// エラー出力クラス
    /// </summary>
    static class ErrorLogLib
    {
        /// <summary>
        /// エラーを出力する
        /// </summary>
        /// <param name="name">エラー名</param>
        /// <param name="msg">エラーメッセージ</param>
        /// <param name="show">表示するか</param>
        public static void ErrorOutput(string name, string msg, bool show)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(SettingManager.PrivateSettings.ERRORLOG_PATH, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss [") + name + "]" + msg + "\r\n");
                }
                if (show) TaskMessageLib.OutputMessage(msg, name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                TaskMessageLib.OutputMessage(ex.Message, "エラー出力エラー");
            }
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
    }
}
