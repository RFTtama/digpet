using digpet.Interface;
using digpet.Managers;
using digpet.Modules;
using OpenCvSharp;

namespace digpet.TimerClass
{

    /// <summary>
    /// カメラ用管理クラス
    /// </summary>
    internal class CameraTimer : TaskClassInterface
    {
        //変数宣言
        private bool cameraDisable = false;

        //クラス宣言
        private readonly RingFlagMemClass ringMem = new RingFlagMemClass(10);

        /// <summary>
        /// 1s毎に実行される関数
        /// </summary>
        /// <returns></returns>
        public override TaskReturn TaskFunc()
        {
            if (!CheckCameraModeEnable()) return TaskReturn.TASK_SUCCESS;

            //処理をここに書く

            return TaskReturn.TASK_SUCCESS;
        }

        /// <summary>
        /// カメラモードが有効か調べる
        /// </summary>
        /// <returns>true: 有効, false: 無効</returns>
        private bool CheckCameraModeEnable()
        {
            if (!SettingManager.PublicSettings.EnableCameraMode)
            {
                return false;
            }

            if (cameraDisable)
            {
                return false;
            }

            if (ringMem.GetTotalOfTrue() >= SettingManager.PublicSettings.CameraDisableThreshold)
            {
                cameraDisable = true;
                LogManager.LogOutput("カメラタスクの実行に複数回失敗したため、機能を無効にしました");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 戻り値確認
        /// </summary>
        /// <param name="ret"></param>
        public override void TaskCheckRet(TaskReturn ret)
        {
            switch (ret)
            {
                case TaskReturn.TASK_SUCCESS:
                    ringMem.Add(true);
                    break;

                case TaskReturn.TASK_BLOCKED:
                case TaskReturn.TASK_FAILURE:
                    ringMem.Add(false);
                    break;

                default:
                    ringMem.Add(false);
                    break;
            }
        }
    }
}
