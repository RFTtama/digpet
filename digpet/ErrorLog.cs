namespace digpet
{
    /// <summary>
    /// エラー出力クラス
    /// </summary>
    static class ErrorLog
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
                using (StreamWriter sw = new StreamWriter(APP_SETTINGS.ERRORLOG_PATH, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss [") + name + "]" + msg + "\r\n");
                }
                if (show) MessageBox.Show(msg, name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
