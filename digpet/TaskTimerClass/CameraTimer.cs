using digpet.Managers;
using digpet.Managers.GenerakManager;
using digpet.Models.AbstractModels;
using digpet.Modules;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace digpet.TimerClass
{

    /// <summary>
    /// カメラ用管理クラス
    /// </summary>
    public class CameraTimer : TaskClassModel
    {
        //定数宣言
        private const int SmoothCount = 5;
        private const int ImageSizeX = 640;
        private const int ImageSizeY = 640;

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
        private AvgManager detectAvgManager = new AvgManager();
        private static VideoCapture capture = new VideoCapture();
        private RingFlagMemClass smoothMem = new RingFlagMemClass(SmoothCount);
        private static RingFlagMemClass neglectMem = new RingFlagMemClass(0);
        private static InferenceSession? session;

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
                ErrorLogLib.ErrorOutput("CameraTimer初期化エラー", ex.Message);
                DisposeCapture();
            }

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
                session?.Dispose();
                _cameraDisable = true;
            }
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~CameraTimer()
        {
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
            using Mat det = mat.Clone();

            //matがnullならfalseを返却
            if (det == null)
            {
                ErrorLogLib.ErrorOutput("顔検出エラー", "渡された画像がnullです");
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
                ErrorLogLib.ErrorOutput("顔検出エラー", "モデルが読み込まれていません");
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
