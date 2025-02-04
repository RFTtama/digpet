using digpet.Interface;
using digpet.Managers;
using digpet.Modules;
using digpet.Properties;
using OpenCvSharp;
using OpenCvSharp.ML;
using System.Reflection;

namespace digpet.TimerClass
{

    /// <summary>
    /// カメラ用管理クラス
    /// </summary>
    internal class CameraTimer : TaskClassInterface
    {
        //変数宣言
        private bool _cameraDisable = false;
        private int _faceDetected = -1;
        private int cameraCnt = 0;
        private double _detectAvg = 0.0;
        private bool _avgCalcFlg = false;

        //クラス宣言
        private readonly RingFlagMemClass ringMem = new RingFlagMemClass(10);
        private CascadeClassifier classifier = new CascadeClassifier();
        private AvgManager detectAvgManager = new AvgManager();

        //ゲッターなど
        public int FaceDetected
        {
            get { return _faceDetected; }
        }
        public bool AvgCalcFlg
        {
            get { return _avgCalcFlg; }
        }
        public double DetectAvg
        { 
            get { return _detectAvg; }
        }
        public bool CameraDisable
        {
            get { return _cameraDisable; }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CameraTimer()
        {
            InitClassifier();
        }

        /// <summary>
        /// クラス分類器を初期化
        /// </summary>
        private void InitClassifier()
        {
            if (File.Exists(SettingManager.PrivateSettings.CASCADE_PATH))
            {
                classifier.Load(SettingManager.PrivateSettings.CASCADE_PATH);
            }
            else
            {
                _cameraDisable = true;
            }
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~CameraTimer()
        {
            classifier.Dispose();
        }

        /// <summary>
        /// 1s毎に実行される関数
        /// </summary>
        /// <returns></returns>
        public override TaskReturn TaskFunc()
        {
            if (!CheckCameraModeEnable())
            {
                _faceDetected = -1;
                return TaskReturn.TASK_SUCCESS;
            }

            using (Mat? flame = TakePhoto())
            {
                if (flame == null)
                {
                    LogManager.LogOutput("写真の撮影に失敗しました");
                    _faceDetected = -1;
                    return TaskReturn.TASK_FAILURE;
                }

                _faceDetected = DetectFace(flame);
            }

            if (FaceDetected < 0) return TaskReturn.TASK_FAILURE;

            if ((cameraCnt > 0) && ((cameraCnt % 60) == 0))
            {
                try
                {
                    //CPU使用率の平均を取得し、トークンを計算する
                    cameraCnt = 0;
                    GetCpuAvg();
                }
                catch (Exception ex)
                {
                    ErrorLog.ErrorOutput("CPU使用率平均計算エラー", ex.Message);
                    return TaskReturn.TASK_FAILURE;
                }
            }
            else
            {
                detectAvgManager.SetCpuSum(100.0);
            }

            cameraCnt++;

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

            if (_cameraDisable)
            {
                return false;
            }

            if (ringMem.GetTotalOfTrue() >= SettingManager.PublicSettings.CameraDisableThreshold)
            {
                _cameraDisable = true;
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

                using (Mat flame = new Mat())
                {
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

                    return flame.Clone();
                }
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

        /// <summary>
        /// 画像から顔を検出する
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        private int DetectFace(Mat mat)
        {
            //matがnullならfalseを返却
            if (mat == null)
            {
                ErrorLog.ErrorOutput("顔検出エラー", "渡された画像がnullです");
                return -1;
            }

            using (Mat gray = new Mat())
            {
                Cv2.CvtColor(src: mat.Clone(), dst: gray, code: ColorConversionCodes.BGR2GRAY);

                //画像のサイズを統一する必要があるかも
                Rect[] faces = classifier.DetectMultiScale(image: gray, scaleFactor: 1.1, minNeighbors: 3, minSize: new OpenCvSharp.Size(100, 100));

                if (faces == null)
                {
                    ErrorLog.ErrorOutput("顔検出エラー", "顔の検出がしっぱいしました");
                    return -1;
                }

                return faces.Length;
            }
        }

        /// <summary>
        /// 平均を求めcpuAvgに代入する
        /// </summary>
        private void GetCpuAvg()
        {
            _detectAvg = detectAvgManager.GetCpuAvg();

            _avgCalcFlg = true;
            detectAvgManager.Clear();
            LogManager.LogOutput("分毎トークンの算出完了");
        }

        /// <summary>
        /// トークンクリア
        /// </summary>
        public void ClearDetectAvg()
        {
            cameraCnt = 0;
            _detectAvg = 0.0;
            _avgCalcFlg = false;
        }
    }
}
