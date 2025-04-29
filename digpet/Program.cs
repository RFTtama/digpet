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
            Mutex mutex = new Mutex(false, "digpet");                           //Mutex(Release�p)
#endif
#if DEBUG
            Mutex mutex = new Mutex(false, "digpet_debug");                     //Mutex(Debug�p)
# endif

            bool hasHandle = false;                                             //�n���h������Ă��邩

            try
            {
                try
                {
                    hasHandle = mutex.WaitOne(0, false);                        //Mutex�����łɃn���h������Ă��邩�m���߂�
                }
                catch (System.Threading.AbandonedMutexException)                //�n���h������Ă��Ȃ�
                {
                    hasHandle = true;                                           //���̃A�v���Ńn���h������
                }
                if (hasHandle == false)                                         //���̃A�v���Ńn���h�����Ȃ�
                {
                    MessageBox.Show("Digpet�͊��Ɏ��s����Ă��܂�",           //�G���[
                        "Digpet���s�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ApplicationConfiguration.Initialize();

                SettingManager.ReadSettingFile(SettingManager.PrivateSettings.SETTING_PATH);

                ClassManager cm = new ClassManager();
                Digpet digpet = new Digpet(cm.arg);

                Application.Run(digpet);
            }
            catch (Exception ex)
            {
                ErrorLogLib.ErrorOutput("����`�̃G���[", ex.Message);

                if (hasHandle)                                                  //Mutex�̉��
                {
                    mutex.ReleaseMutex();
                }
                mutex.Close();
            }
            finally
            {
                if (hasHandle)                                                  ////Mutex�̉��
                {
                    mutex.ReleaseMutex();
                }
                mutex.Close();
            }
        }
    }
}