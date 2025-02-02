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

            Mat? flame = TakePhoto();

            if (flame == null)
            {
                LogManager.LogOutput("写真の撮影に失敗しました");
                return TaskReturn.TASK_FAILURE;
            }

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
        /// カメラから画像を取得し、返却する
        /// </summary>
        /// <returns>画像配列(Mat)</returns>
        private Mat? TakePhoto()
        {
            using (VideoCapture capture = new VideoCapture())
            {
                capture.Open(SettingManager.PublicSettings.CameraId);
                if (!capture.IsOpened())
                {
                    return null;
                }

                Mat flame = new Mat();

                try
                {
                    capture.Read(flame);
                }
                catch (Exception ex)
                {
                    ErrorLog.ErrorOutput("写真撮影エラー", ex.Message);
                    return null;
                }

                if (flame.Empty())
                {
                    return null;
                }

                return flame;
            }
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
