using digpet.Models.AbstractModels;
using digpet.Modules;
using digpet.TaskTimerClass;
using digpet.TimerClass;
using System.Xml.Xsl;

namespace digpet.Managers
{
    public class ClassManager
    {
        // Digpetにわたすやつ
        public ClassManagerArg arg;

        // timer関連
        private AutoResetEvent autoEvent = new AutoResetEvent(true);
        private System.Threading.Timer timer;

        private CpuAvgCalcTimer cpuAvgCalcTimer = new CpuAvgCalcTimer();
        private CameraTimer cameraTimer = new CameraTimer();
        private GCTimer gcTimer = new GCTimer();
        private LogTimer logTimer = new LogTimer();

        // 変数関連
        private List<TaskClassModel> taskQueue = new List<TaskClassModel>();
        private int backUpCnt;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ClassManager()
        {
            InitQueue();
            arg = new ClassManagerArg();
            cameraTimer.Init();
            timer = new System.Threading.Timer(TaskRunTimer1s, autoEvent, 0, 1000);
            backUpCnt = 0;
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        public void Terminator()
        {
            ErrorLogLib.Export();
            logTimer.Export();

            timer.Dispose();
        }

        /// <summary>
        /// タスクのキューを初期化する
        /// </summary>
        private void InitQueue()
        {
            //1sタスク関数テーブルを設定する
            taskQueue =
                [
                    gcTimer,
                    cpuAvgCalcTimer,
                    cameraTimer,
                    logTimer,
                ];
        }

        private void TaskRunTimer1s(object? state)
        {
            try
            {
                if (taskQueue == null) return;
                General1sTimerFunc();
            }
            catch (ObjectDisposedException)
            {
                //Digpet終了時のエラー回避用
            }
            catch (Exception ex)
            {
                ErrorLogLib.ErrorOutput("共通定期関数実行エラー", ex.Message);
            }

            try 
            {

                //タスクテーブルに設定されているタスククラスの関数を順番に実行する
                for (int i = 0; i < taskQueue.Count; i++)
                {
                    if (taskQueue[i] == null) continue;

                    switch (taskQueue[i].ClassTask.Status)
                    {
                        case TaskStatus.Running:
                            taskQueue[i].TaskCheckRet(TaskReturn.TASK_BLOCKED);
                            continue;

                        default:
                            break;
                    }

                    TaskClassModel sendTask = taskQueue[i];

                    taskQueue[i].ClassTask = Task.Run(() =>
                    {
                        sendTask.TaskCheckRet(sendTask.TaskFunc());
                    });
                }
            }
            catch (ObjectDisposedException)
            {
                //Digpet終了時のエラー回避用
            }
            catch (Exception ex)
            {
                ErrorLogLib.ErrorOutput("タスク実行エラー", ex.Message);
            }
        }

        /// <summary>
        /// 全体の1s毎処理
        /// </summary>
        private void General1sTimerFunc()
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
                arg.tokenManager.Write(SettingManager.PrivateSettings.TOKEN_CALC_PATH);
                backUpCnt = 0;
            }

            ErrorLogLib.Export();
        }

        /// <summary>
        /// カメラ用のトークン算出処理など
        /// </summary>
        private void CameraProcess()
        {
            if (cameraTimer.AvgCalcFlg)
            {
                arg.tokenManager.AddTokens(cameraTimer.DetectAvg);
                cameraTimer.ClearDetectAvg();
            }

            string txt;
            int detectNum = cameraTimer.FaceDetected;

            if (detectNum < 0)
            {
                txt = "検出: エラー";
                LogTimer.SaveLog("detect", "error");
            }
            else if (detectNum == 0)
            {
                txt = "検出: なし";
                LogTimer.SaveLog("detect", "false");
            }
            else
            {
                txt = "検出: あり";
                LogTimer.SaveLog("detect", "true");
            }

            SetCpuUsageLabel(txt);
        }

        /// <summary>
        /// CPUのトークン算出処理など
        /// </summary>
        private void CpuProcess()
        {
            if (cpuAvgCalcTimer.AvgCalcFlg)
            {
                arg.tokenManager.AddTokens(cpuAvgCalcTimer.CpuAvg);
                cpuAvgCalcTimer.ClearCpuAvg();
            }
            SetCpuUsageLabel("CPU: " + cpuAvgCalcTimer.CpuUsage.ToString("n2") + "%");
            LogTimer.SaveLog("cpuUsage", cpuAvgCalcTimer.CpuUsage.ToString("n2"));
        }

        /// <summary>
        /// CpuUsegeLabelを上書きするためにInvokeする
        /// </summary>
        /// <param name="label"></param>
        private void SetCpuUsageLabel(string label)
        {
            arg.CpuUsageLabelUpdate?.Invoke(label);
        }
    }

    public class ClassManagerArg
    {
        public TokenManager tokenManager = new TokenManager();
        public CharZipFileManager charZipFileManager = new CharZipFileManager();
        public SetCpuUsageLabel? CpuUsageLabelUpdate;

        public delegate void SetCpuUsageLabel(string label);
    }
}
