using digpet.Interface;
using digpet.Managers;
using digpet.Modules;
using digpet.Properties;
using OpenCvSharp;
using OpenCvSharp.ML;
using System.Diagnostics;
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
        private bool init = false;

        //クラス宣言
        private readonly RingFlagMemClass ringMem = new RingFlagMemClass(10);
        private CascadeClassifier classifier = new CascadeClassifier();
        private AvgManager detectAvgManager = new AvgManager();
        private VideoCapture capture = new VideoCapture();

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
        /// カメラ関連の初期化
        /// </summary>
        public void Init()
        {
            Task.Run(() =>
            {
                InitClassifier();

                if (CheckCameraModeEnable())
                {
                    capture.Open(SettingManager.PublicSettings.CameraId);
                    if (!capture.IsOpened())
                    {
                        _cameraDisable = true;
                    }
                }
                else
                {
                    _cameraDisable = true;
                }
                init = true;
            });
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
                LogManager.LogOutput("カスケードファイルの読み取りに失敗しました");
                _cameraDisable = true;
            }
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~CameraTimer()
        {
            classifier.Dispose();
            capture.Dispose();
        }

        /// <summary>
        /// 1s毎に実行される関数
        /// </summary>
        /// <returns></returns>
        public override TaskReturn TaskFunc()
        {
#if DEBUG
            Stopwatch stopwatch = Stopwatch.StartNew();
#endif
            if (!init) return TaskReturn.TASK_SUCCESS;

            if (!CheckCameraModeEnable())
            {
                _faceDetected = -1;
                return TaskReturn.TASK_SUCCESS;
            }

            _faceDetected = TakePhotoAndDetectFace();

            Debug.Print(FaceDetected.ToString());

            if (FaceDetected < 0) return TaskReturn.TASK_FAILURE;

            if ((cameraCnt > 0) && ((cameraCnt % 60) == 0))
            {
                try
                {
                    //検出の平均を取得し、トークンを計算する
                    cameraCnt = 0;
                    GetDetectAvg();
                }
                catch (Exception ex)
                {
                    ErrorLog.ErrorOutput("検出平均計算エラー", ex.Message);
                    return TaskReturn.TASK_FAILURE;
                }
            }
            else
            {
                detectAvgManager.Sum(100.0);
            }

            cameraCnt++;

#if DEBUG
            Debug.Print("camera fin: " + stopwatch.ToString());
#endif
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

            if (CameraDisable)
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
        /// 写真を撮って顔検出
        /// </summary>
        /// <returns>1: 顔を検出, 0: 顔検出失敗, else: エラー</returns>
        private int TakePhotoAndDetectFace()
        {
            int detect = 0;

            using (Mat? flame = TakePhoto())
            {
                if (flame == null)
                {
                    LogManager.LogOutput("写真の撮影に失敗しました");
                    _faceDetected = -1;
                    return -1;
                }

#if false
                Cv2.ImWrite("Sapmle.png", flame);
#endif

                detect = DetectFace(flame);
            }

            return detect;
        }

        /// <summary>
        /// カメラから画像を取得し、返却する
        /// </summary>
        /// <returns>画像配列(Mat)</returns>
        private Mat? TakePhoto()
        {
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

        /// <summary>
        /// 戻り値確認
        /// </summary>
        /// <param name="ret"></param>
        public override void TaskCheckRet(TaskReturn ret)
        {
            switch (ret)
            {
                case TaskReturn.TASK_SUCCESS:
                    ringMem.Add(false);
                    break;

                case TaskReturn.TASK_BLOCKED:
                case TaskReturn.TASK_FAILURE:
                    ringMem.Add(true);
                    break;

                default:
                    ringMem.Add(true);
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
                    ErrorLog.ErrorOutput("顔検出エラー", "顔の検出が失敗しました");
                    return -1;
                }

                return faces.Length;
            }
        }

        /// <summary>
        /// 検出平均を求めcpuAvgに代入する
        /// </summary>
        private void GetDetectAvg()
        {
            _detectAvg = detectAvgManager.GetAvg();

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
