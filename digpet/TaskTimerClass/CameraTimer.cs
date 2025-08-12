using digpet.Managers;
using digpet.Managers.GenerakManager;
using digpet.Models.AbstractModels;
using digpet.Modules;
using OpenCvSharp;

namespace digpet.TimerClass
{

    /// <summary>
    /// カメラ用管理クラス
    /// </summary>
    public class CameraTimer : TaskClassModel
    {
        //定数宣言
        private const int SmoothCount = 5;

        //変数宣言
        private static bool _cameraDisable = false;
        private int _faceDetected = -1;
        private int cameraCnt = 0;
        private double _detectAvg = 0.0;
        private bool _avgCalcFlg = false;
        private bool init = false;
        private bool isDetectBySmooth = false;

        //クラス宣言
        private static readonly RingFlagMemClass ringMem = new RingFlagMemClass(10);
        private CascadeClassifier classifier = new CascadeClassifier();
        private AvgManager detectAvgManager = new AvgManager();
        private static VideoCapture capture = new VideoCapture();
        private RingFlagMemClass smoothMem = new RingFlagMemClass(SmoothCount);
        private static RingFlagMemClass neglectMem = new RingFlagMemClass(0);

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
        public static bool CameraDisable
        {
            get { return _cameraDisable; }
        }
        public static bool IsNeglect
        {
            get { return ((neglectMem.GetTotalOfTrue() == 0) && (CheckCameraModeEnable())); }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CameraTimer()
        {
            Init();
        }

        /// <summary>
        /// カメラ関連の初期化
        /// </summary>
        public void Init()
        {
            InitClassifier();

            if (CheckCameraModeEnable())
            {
                SetCaptureSettings();
                capture.Open(SettingManager.PublicSettings.CameraId);
                if (!capture.IsOpened())
                {
                    DisposeCapture();
                }
            }
            else
            {
                DisposeCapture();
            }

            neglectMem = new RingFlagMemClass(SettingManager.PublicSettings.NeglectActiveTime);

            init = true;
        }

        /// <summary>
        /// キャプチャの設定を行う
        /// </summary>
        private void SetCaptureSettings()
        {
            capture.AutoFocus = true;
        }

        /// <summary>
        /// キャプチャを破棄する
        /// </summary>
        private static void DisposeCapture()
        {
            if (!capture.IsDisposed)
            {
                capture.Dispose();
                _cameraDisable = true;
            }
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
                DisposeCapture();
            }
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~CameraTimer()
        {
            classifier.Dispose();
            DisposeCapture();
        }

        /// <summary>
        /// 1s毎に実行される関数
        /// </summary>
        /// <returns></returns>
        public override TaskReturn TaskFunc()
        {
            if (!init) return TaskReturn.TASK_SUCCESS;

            if (!CheckCameraModeEnable())
            {
                _faceDetected = -1;
                return TaskReturn.TASK_SUCCESS;
            }

            _faceDetected = TakePhotoAndDetectFace();

            if (SettingManager.PublicSettings.EnableCameraDetectSmoothingMode)
            {
                _faceDetected = DetectSmoothing(FaceDetected);
            }
            CalcNeglect(FaceDetected);

            if (FaceDetected < 0) return TaskReturn.TASK_FAILURE;

            CalcProcess();

            return TaskReturn.TASK_SUCCESS;
        }

        /// <summary>
        /// 平均値の算出処理
        /// </summary>
        private void CalcProcess()
        {
            if ((cameraCnt > 0) && ((cameraCnt % 60) == 0))
            {
                //検出の平均を取得し、トークンを計算する
                cameraCnt = 0;
                CalcDetectAvg();
            }
            double sumValue = 0.0;

            if (FaceDetected > 0)
            {
                sumValue = 100.0;
            }

            detectAvgManager.Sum(sumValue);

            cameraCnt++;
        }

        /// <summary>
        /// カメラモードが有効か調べる
        /// </summary>
        /// <returns>true: 有効, false: 無効</returns>
        private static bool CheckCameraModeEnable()
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
                DisposeCapture();
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
        /// 検出の平滑化処理
        /// </summary>
        /// <param name="detect">平滑化前の検出結果</param>
        /// <returns>平滑化後の検出結果</returns>
        private int DetectSmoothing(int detect)
        {
            if (detect < 0) return detect;

            smoothMem.Add(detect);

            if (isDetectBySmooth)
            {
                if (smoothMem.GetTotalOfTrue() == 0)
                {
                    isDetectBySmooth = false;
                    return 0;
                }
                if (detect == 0)
                {
                    return 1;
                }
                return detect;
            }

            if (smoothMem.GetTotalOfTrue() >= SmoothCount)
            {
                isDetectBySmooth = true;
                return detect;
            }
            return 0;
        }

        /// <summary>
        /// 放置されているかの算出
        /// </summary>
        /// <param name="detect"></param>
        private void CalcNeglect(int detect)
        {
            if (detect > 0)
            {
                neglectMem.Add(true);
            }
            else
            {
                neglectMem.Add(false);
            }
        }

        /// <summary>
        /// カメラから画像を取得し、返却する
        /// </summary>
        /// <returns>画像配列(Mat)</returns>
        private Mat? TakePhoto()
        {
            using (Mat flame = new Mat())
            {
                if (!capture.Read(flame))
                {
                    ErrorLogLib.ErrorOutput("写真撮影エラー", "写真の撮影に失敗しました");
                    return null;
                }

                if (flame.Empty())
                {
                    ErrorLogLib.ErrorOutput("写真撮影エラー", "写真が空です");
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
        /// <returns>異常: -1, 0<: 顔の検出数</returns>
        private int DetectFace(Mat mat)
        {
            //matがnullならfalseを返却
            if (mat == null)
            {
                ErrorLogLib.ErrorOutput("顔検出エラー", "渡された画像がnullです");
                return -1;
            }

            using (Mat gray = new Mat())
            {
                Cv2.CvtColor(src: mat, dst: gray, code: ColorConversionCodes.BGR2GRAY);
                Cv2.EqualizeHist(gray, gray);

                //画像のサイズを統一する必要があるかも
                Rect[] faces = classifier.DetectMultiScale(image: gray, scaleFactor: 1.1, minNeighbors: 10, minSize: new OpenCvSharp.Size());

                if (faces == null)
                {
                    ErrorLogLib.ErrorOutput("顔検出エラー", "顔の検出が失敗しました");
                    return -1;
                }

                return faces.Length;
            }
        }

        /// <summary>
        /// 検出平均を求めcpuAvgに代入する
        /// </summary>
        private void CalcDetectAvg()
        {
            _detectAvg = detectAvgManager.GetAvg();

            _avgCalcFlg = true;
            detectAvgManager.Clear();
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
