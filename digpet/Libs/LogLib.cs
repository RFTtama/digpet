using digpet.Managers;

namespace digpet.Modules
{
    internal static class LogLib
    {
        /// <summary>
        /// ログを出力する
        /// </summary>
        /// <param name="msg">メッセージ</param>
        public static void LogOutput(string msg)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(SettingManager.PrivateSettings.LOG_PATH, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + msg + "\r\n");
                }
            }
            catch (Exception ex)
            {
                ErrorLogLib.ErrorOutput("ログ書き込みエラー", ex.Message);
            }
        }
    }
}
