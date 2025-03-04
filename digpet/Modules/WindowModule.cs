using digpet.Interface;
using digpet.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digpet.Modules
{
    internal static class WindowModule
    {
        /// <summary>
        /// ウィンドウの状態を保存する
        /// </summary>
        public static void SaveNowWindowState(WindowStateClass stateClass)
        {
            SettingManager.PublicSettings.WindowLocation = stateClass.Location;
            SettingManager.PublicSettings.WindowSize = stateClass.Size;
            SettingManager.PublicSettings.WindowState = GetWindowStateId(stateClass.State);
        }

        /// <summary>
        /// ウィンドウの状態IDを返却する
        /// </summary>
        /// <returns>0: 通常, 1: 最大化, 2: 最小化</returns>
        private static int GetWindowStateId(FormWindowState state)
        {
            int wstate = 0;

            switch (state)
            {
                case FormWindowState.Normal:
                    wstate = 0;
                    break;

                case FormWindowState.Maximized:
                    wstate = 1;
                    break;

                case FormWindowState.Minimized:
                    wstate = 2;
                    break;

                default:
                    wstate = 0;
                    break;
            }

            return wstate;
        }

        /// <summary>
        /// ウィンドウの状態を返却する
        /// </summary>
        /// <returns>通常、最大化、最小化</returns>
        private static FormWindowState GetWindowState()
        {
            FormWindowState loadState = FormWindowState.Normal;

            switch (SettingManager.PublicSettings.WindowState)
            {
                case 0:
                    loadState = FormWindowState.Normal;
                    break;

                case 1:
                    loadState = FormWindowState.Maximized;
                    break;


                case 2:
                    loadState = FormWindowState.Minimized;
                    break;

                default:
                    loadState = FormWindowState.Normal;
                    break;
            }

            return loadState;
        }

        /// <summary>
        /// 保存されたウィンドウの状態を取得する
        /// </summary>
        public static WindowStateClass GetSavedWindowState()
        {
            WindowStateClass ret = new WindowStateClass();
            ret.Location = SettingManager.PublicSettings.WindowLocation;
            ret.Size = SettingManager.PublicSettings.WindowSize;
            ret.State = GetWindowState();

            return ret;
        }
    }
}
