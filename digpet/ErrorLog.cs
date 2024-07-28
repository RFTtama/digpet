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
                using (StreamWriter sw = new StreamWriter("errorLog.txt", true))
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
    }
}
