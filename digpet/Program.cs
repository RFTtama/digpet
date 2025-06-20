using digpet.Managers;
using digpet.Modules;

namespace digpet
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

#if !DEBUG
            Mutex mutex = new Mutex(false, "digpet");                           //Mutex(Release用)
#endif
#if DEBUG
            Mutex mutex = new Mutex(false, "digpet_debug");                     //Mutex(Debug用)
# endif

            bool hasHandle = false;                                             //ハンドルされているか

            try
            {
                try
                {
                    hasHandle = mutex.WaitOne(0, false);                        //Mutexがすでにハンドルされているか確かめる
                }
                catch (System.Threading.AbandonedMutexException)                //ハンドルされていない
                {
                    hasHandle = true;                                           //このアプリでハンドルする
                }
                if (hasHandle == false)                                         //このアプリでハンドルしない
                {
                    MessageBox.Show("Digpetは既に実行されています",           //エラー
                        "Digpet実行エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ApplicationConfiguration.Initialize();

                SettingManager.ReadSettingFile(SettingManager.PrivateSettings.SETTING_PATH);

                ClassManager cm = new ClassManager();
                Digpet digpet = new Digpet(cm.arg);

                Application.Run(digpet);

                cm.Terminator();
            }
            catch (Exception ex)
            {
                ErrorLogLib.ErrorOutput("未定義のエラー", ex.Message);

                if (hasHandle)                                                  //Mutexの解放
                {
                    mutex.ReleaseMutex();
                }
                mutex.Close();
            }
            finally
            {
                if (hasHandle)                                                  ////Mutexの解放
                {
                    mutex.ReleaseMutex();
                }
                mutex.Close();
            }
        }
    }
}