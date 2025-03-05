using digpet.Managers;
using digpet.Managers.GenerakManager;
using digpet.Models.AbstractModels;
using digpet.Modules;
using digpet.Properties;
using OpenCvSharp;
using OpenCvSharp.ML;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;

namespace digpet.TimerClass
{

    /// <summary>
    /// カメラ用管理クラス
    /// </summary>
    internal class CameraTimer : TaskClassModel
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
                init = true;
            });
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
                LogLib.LogOutput("カスケードファイルの読み取りに失敗しました");
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

            if (FaceDetected < 0) return TaskReturn.TASK_FAILURE;

            CalcProcess();

            return TaskReturn.TASK_SUCCESS;
        }

        /// <summary>
        /// 平均値の算出処理
        /// </summary>
        private void CalcProcess()
        {
            Debug.Print(cameraCnt.ToString());
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
                LogLib.LogOutput("カメラタスクの実行に複数回失敗したため、機能を無効にしました");
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
        /// カメラから画像を取得し、返却する
        /// </summary>
        /// <returns>画像配列(Mat)</returns>
        private Mat? TakePhoto()
        {
            using (Mat flame = new Mat())
            {
                if(!capture.Read(flame))
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
        /// <returns></returns>
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
            LogLib.LogOutput("分毎トークンの算出完了");
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
