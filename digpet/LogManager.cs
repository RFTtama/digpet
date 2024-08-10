using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digpet
{
    internal static class LogManager
    {
        /// <summary>
        /// ログを出力する
        /// </summary>
        /// <param name="msg">メッセージ</param>
        public static void LogOutput(string msg)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(APP_SETTINGS.LOG_PATH, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + msg + "\r\n");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorOutput("ログ書き込みエラー", ex.Message, true);
            }
        }
    }
}
