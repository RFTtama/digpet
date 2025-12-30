using digpet.Managers;
using digpet.Modules;
using digpet.TaskTimerClass;

namespace digpet._Controller
{
    public partial class Controller
    {
        private System.Threading.Timer? timer;

        // timer関連

        private CpuAvgCalcTimer? cpuAvgCalcTimer;
        private CameraTimer? cameraTimer;
        private LogTimer? logTimer;

        private int backUpCnt;

        /// <summary>
        /// タイマ類の初期化
        /// </summary>
        private void InitTimers()
        {
            logTimer = LogTimer.Instance;
            cpuAvgCalcTimer = CpuAvgCalcTimer.Instance;
            cameraTimer = CameraTimer.Instance;
            timer = new System.Threading.Timer(General1sTimerFunc, null, 0, 1000);
        }

        /// <summary>
        /// 全体の1s毎処理
        /// </summary>
        private void General1sTimerFunc(object? obj)
        {
            if (!CameraTimer.CameraDisable)
            {
                CameraProcess();
            }
            else
            {
                CpuProcess();
            }

            backUpCnt++;

            if (backUpCnt > SettingManager.PublicSettings.TokenBackupInterval)
            {
                TokenManager tm = TokenManager.Instance;
                tm.Write(SettingManager.PrivateSettings.TOKEN_CALC_PATH);
                backUpCnt = 0;
            }

            ErrorLogLib er = ErrorLogLib.Instance;
            er.Export();
        }

        /// <summary>
        /// カメラ用のトークン算出処理など
        /// </summary>
        private void CameraProcess()
        {
            if (null == cameraTimer) return;
            if (cameraTimer.AvgCalcFlg)
            {
                TokenManager tm = TokenManager.Instance;
                tm.AddTokens(cameraTimer.DetectAvg);
                cameraTimer.ClearDetectAvg();
            }

            string txt;
            int detectNum = cameraTimer.FaceDetected;

            if (detectNum < 0)
            {
                txt = "検出: エラー";
                logTimer?.SaveLog("detect", "error");
            }
            else if (detectNum == 0)
            {
                txt = "検出: なし";
                logTimer?.SaveLog("detect", "false");
            }
            else
            {
                txt = "検出: あり";
                logTimer?.SaveLog("detect", "true");
            }

            SetCpuUsageLabel(txt);
        }

        /// <summary>
        /// CPUのトークン算出処理など
        /// </summary>
        private void CpuProcess()
        {
            if (null == cpuAvgCalcTimer) return;
            if (cpuAvgCalcTimer.AvgCalcFlg)
            {
                TokenManager tm = TokenManager.Instance;
                tm.AddTokens(cpuAvgCalcTimer.CpuAvg);
                cpuAvgCalcTimer.ClearCpuAvg();
            }
            SetCpuUsageLabel("CPU: " + cpuAvgCalcTimer.CpuUsage.ToString("n2") + "%");
            logTimer?.SaveLog("cpuUsage", cpuAvgCalcTimer.CpuUsage.ToString("n2"));
        }

        /// <summary>
        /// CpuUsegeLabelを上書きするためにInvokeする
        /// </summary>
        /// <param name="label"></param>
        private void SetCpuUsageLabel(string label)
        {
            _view?.UpdateCpuLabel(label);
        }
    }
}
