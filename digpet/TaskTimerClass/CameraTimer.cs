using digpet.Managers;
using digpet.Managers.GenerakManager;
using digpet.Modules;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using System.Diagnostics;

namespace digpet.TaskTimerClass
{

    /// <summary>
    /// カメラ用管理クラス
    /// </summary>
    public class CameraTimer: IDisposable
    {
        private static Lazy<CameraTimer> _lazy = new(() => new CameraTimer(), isThreadSafe: true);
        public static CameraTimer Instance => _lazy.Value;
        private System.Threading.Timer? _timer;

        //定数宣言
        private const int SmoothCount = 5;
        private const int ImageSizeX = 640;
        private const int ImageSizeY = 640;

        //変数宣言
        private static bool _cameraDisable;
        private int _faceDetected;
        private int cameraCnt;
        private double _detectAvg;
        private bool _avgCalcFlg;
        private bool isDetectBySmooth;

        //クラス宣言
        private RingFlagMemClass ringMem;
        private AvgManager detectAvgManager;
        private VideoCapture capture;
        private RingFlagMemClass smoothMem;
        private RingFlagMemClass neglectMem;
        private InferenceSession? session;

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
        public bool IsNeglect
        {
            get { return ((neglectMem.GetTotalOfTrue() == 0) && (CheckCameraModeEnable())); }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private CameraTimer()
        {
            //変数宣言
            _cameraDisable = false;
            _faceDetected = -1;
            cameraCnt = 0;
            _detectAvg = 0.0;
            _avgCalcFlg = false;
            isDetectBySmooth = false;

            //クラス宣言
            ringMem = new RingFlagMemClass(10);
            detectAvgManager = new AvgManager();
            capture = new VideoCapture();
            smoothMem = new RingFlagMemClass(SmoothCount);
            neglectMem = new RingFlagMemClass(0);

            Init();
            _timer = new(TaskFunc, null, 0, 1000);
        }

        /// <summary>
        /// カメラ関連の初期化
        /// </summary>
        public void Init()
        {
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
            try
            {
                session = new InferenceSession("yolov5n-0.5.onnx");
            }
            catch (Exception ex)
            {
                ErrorLogLib er = ErrorLogLib.Instance;
                er.ErrorOutput("CameraTimer初期化エラー", ex.Message);
                DisposeCapture();
            }
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
        private void DisposeCapture()
        {
            if (!capture.IsDisposed)
            {
                capture.Dispose();
                session?.Dispose();
                _cameraDisable = true;
            }
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        public void Dispose()
        {
            DisposeCapture();
        }

        /// <summary>
        /// 1s毎に実行される関数
        /// </summary>
        /// <returns></returns>
        private void TaskFunc(object? obj)
        {
            if (!CheckCameraModeEnable())
            {
                _faceDetected = -1;
                ringMem.Add(false);
                return;
            }

            _faceDetected = TakePhotoAndDetectFace();

            if (SettingManager.PublicSettings.EnableCameraDetectSmoothingMode)
            {
                _faceDetected = DetectSmoothing(FaceDetected);
            }
            CalcNeglect(FaceDetected);

            if (FaceDetected < 0)
            {
                ringMem.Add(true);
                return;
            }

            CalcProcess();

            ringMem.Add(false);
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
                ErrorLogLib er = ErrorLogLib.Instance;
                if (!capture.Read(flame))
                {
                    er.ErrorOutput("写真撮影エラー", "写真の撮影に失敗しました");
                    return null;
                }

                if (flame.Empty())
                {
                    er.ErrorOutput("写真撮影エラー", "写真が空です");
                    return null;
                }

                return flame.Clone();
            }
        }

        /// <summary>
        /// 画像から顔を検出する
        /// </summary>
        /// <param name="mat"></param>
        /// <returns>異常: -1, 0<: 顔の検出数</returns>
        private int DetectFace(Mat mat)
        {
            using Mat det = mat.Clone();

            //matがnullならfalseを返却
            if (det == null)
            {
                ErrorLogLib er = ErrorLogLib.Instance;
                er.ErrorOutput("顔検出エラー", "渡された画像がnullです");
                return -1;
            }

            Cv2.CvtColor(src: det, dst: det, code: ColorConversionCodes.BGR2RGB);
            Cv2.Resize(det, det, new OpenCvSharp.Size(ImageSizeX, ImageSizeY));

            float[] imageData = new float[3 * ImageSizeX * ImageSizeY];
            for (int y = 0; y < ImageSizeY; y++)
            {
                for (int x = 0; x < ImageSizeX; x++)
                {
                    Vec3b pixel = det.At<Vec3b>(y, x);
                    int offset = y * 640 + x;
                    imageData[0 * ImageSizeX * ImageSizeY + offset] = pixel.Item0 / 255.0f;
                    imageData[1 * ImageSizeX * ImageSizeY + offset] = pixel.Item1 / 255.0f;
                    imageData[2 * ImageSizeX * ImageSizeY + offset] = pixel.Item2 / 255.0f;
                }
            }

            var inputData = new DenseTensor<float>(imageData, new[] { 1, 3, ImageSizeX, ImageSizeY });

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input", inputData)
            };

            if (session == null)
            {
                ErrorLogLib er = ErrorLogLib.Instance;
                er.ErrorOutput("顔検出エラー", "モデルが読み込まれていません");
                return -1;
            }

            using var results = session.Run(inputs);
            var output = results.First(x => x.Name == "output").AsTensor<float>();

            float[] outValues = output.ToArray();

            int detNum = 0;

            //出力結果の選定
            for (int i = 0; i < outValues.Length; i += 16)
            {
                if (outValues[i + 4] >= (SettingManager.PublicSettings.DetectThreshold / 100.0f))
                {
                    detNum++;
                }
            }

            Debug.Print($"{detNum.ToString()}\n");

            return detNum;
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
