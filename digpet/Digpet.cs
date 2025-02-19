using digpet.Interface;
using digpet.Managers;
using digpet.Modules;
using digpet.TaskTimerClass;
using digpet.TimerClass;

namespace digpet
{
    public partial class Digpet : Form
    {
        //クラス関連の宣言
        private TokenManager tokenManager = new TokenManager();
        private CharZipFileManager charZipFileManager = new CharZipFileManager();

        //変数関連の宣言
        private bool gotNormalImage;                                    //正常に画像を切り替えることができたか

        //定数関連の宣言
        private const string SETTING_PATH = "settings.json";
        private const int FONT_MARGIN_SIZE = 5;

        //テーブル宣言
        private TaskClassInterface[]? TaskRun1sTable;

        //タスククラス宣言
        private CpuAvgCalcTimer cpuAvgCalcTimer = new CpuAvgCalcTimer();
        private CameraTimer cameraTimer = new CameraTimer();
        private GCTimer gcTimer = new GCTimer();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Digpet()
        {
            InitializeComponent();
            Init();
            LogManager.LogOutput("初期化が完了しました");
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void Init()
        {
            Text += "   Ver." + SettingManager.PrivateSettings.APPLICATION_VERSION;
            MouseWheel += new MouseEventHandler(MouseWheelEvent);
            gotNormalImage = true;

            //1sタスク関数テーブルを設定する
            TaskRun1sTable =
                [
                    gcTimer,
                    cpuAvgCalcTimer,
                    cameraTimer,
                ];

            SettingManager.ReadSettingFile(SETTING_PATH);
            CheckResetTime();
            tokenManager.ReadTokens();
            ReadCharConfig();
            SetNowWindowState();
            cameraTimer.Init();

            //タイマは設定ファイルの読み取りが終わるまで開始しない
            TimerStart();
        }

        /// <summary>
        /// タイマ開始処理
        /// </summary>
        private void TimerStart()
        {
            ImageChangeTimer.Enabled = true;
            TaskRunTimer1s.Enabled = true;
        }

        /// <summary>
        /// マウスホイールをくるくるした時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseWheelEvent(object? sender, MouseEventArgs e)
        {
            if (CharPictureBox.Image == null)
            {
                return;
            }
            Size picSize = new Size(CharPictureBox.Width, CharPictureBox.Height);
            const float MINUS_MAGN = 0.9f;
            const float PLUS_MAGN = 1.1f;

            if (e.Delta > 0)
            {
                picSize.Width = (int)(picSize.Width * PLUS_MAGN);
                picSize.Height = (int)(picSize.Height * PLUS_MAGN);
            }
            else if (e.Delta < 0)
            {
                picSize.Width = (int)(picSize.Width * MINUS_MAGN);
                picSize.Height = (int)(picSize.Height * MINUS_MAGN);
            }

            SettingManager.PublicSettings.ImageSize = picSize;
            SettingManager.WriteSettingFile(SETTING_PATH);

            UpdateImageSize();
        }

        /// <summary>
        /// トークンをリセットする時刻が設定されているか確認する
        /// </summary>
        private void CheckResetTime()
        {
            int resetHour = SettingManager.PublicSettings.ResetHour;

            if (resetHour < 0)
            {
                resetHour = SetResetTime();
                SettingManager.WriteSettingFile(SETTING_PATH);
            }

            tokenManager.ResetHour = resetHour;
        }

        /// <summary>
        /// リセット時刻を新しく設定する
        /// </summary>
        /// <returns>リセット時刻</returns>
        private int SetResetTime()
        {
            while (true)
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox("トークンをリセットする時刻を0〜23で設定してください",
                    "リセット時刻設", "0");

                int hour = -1;

                if (int.TryParse(input, out hour) == true)
                {
                    if (hour >= 0 && hour < 24)
                    {
                        SettingManager.PublicSettings.ResetHour = hour;
                        LogManager.LogOutput("リセット時刻を" + hour.ToString() + "に設定しました");
                        return hour;
                    }
                }
            }
        }

        /// <summary>
        /// キャラクターのコンフィグデータを読み取る
        /// </summary>
        private void ReadCharConfig()
        {
            if (SettingManager.PublicSettings.CharSettingPath == null)
            {
                ErrorLog.ErrorOutput("キャラファイル読み取りエラー", "設定されているキャラファイルのパスがnullか空です");
            }
            else if (SettingManager.PublicSettings.CharSettingPath == string.Empty)
            {
                return;
            }
            else
            {
                charZipFileManager.ReadCharSettings(SettingManager.PublicSettings.CharSettingPath);
                ImageChangeTimer.Interval = charZipFileManager.GetPictureTurnOverPeriod();
                SetControlColor(charZipFileManager.GetControlColor());
            }
        }

        /// <summary>
        /// 今日の気分を文字に変換する
        /// </summary>
        /// <param name="feeling">今日の気分(double)</param>
        /// <returns></returns>
        private string GetFeeling(double feeling)
        {
            double feel = feeling;
            if (feel > 1.0) feel = 1.0;
            if (feel < -1.0) feel = -1.0;

            return charZipFileManager.GetFeelingString(feel);
        }

        /// <summary>
        /// 現在の親密度を文字に変換する
        /// </summary>
        /// <param name="intimacy">親密度</param>
        /// <returns></returns>
        private string GetIntimacy(double intimacy)
        {
            double inti = intimacy;
            if (inti < 0.0) inti = 0.0;

            return charZipFileManager.GetIntimacyString(inti);
        }

        /// <summary>
        /// 今日のトークンを出力する(テスト)
        /// </summary>
        /// <param name="value">出力するトークン</param>
        private void OutTokenLabel()
        {
            EmoStringLabel.Text = charZipFileManager.GetFeelingTag() + GetFeeling(tokenManager.Feeling);
            DailyTokenLabel.Text = "今日の獲得トークン: " + tokenManager.DailyTokens.ToString("n2");
            EmoTokenLabel.Text = "今日の感情トークン: " + tokenManager.EmotionTokens.ToString("n2");
            AverageEmotionTokensLabel.Text = "平均感情トークン: " + tokenManager.AverageEmotionTokens.ToString("n2");
            FeelingLabel.Text = "今日の感情: " + tokenManager.Feeling.ToString("n2");
            FlopsLabel.Text = "FLOPS: " + tokenManager.Flops.ToString();
            TotalTokenLabel.Text = "累計トークン: " + (tokenManager.TotalTokens).ToString("n2");
            IntimacyLabel.Text = charZipFileManager.GetIntimacyTag() + GetIntimacy(tokenManager.TotalTokens);
            LogManager.LogOutput("表示されているトークン情報の更新完了");
        }

        /// <summary>
        /// 現在の状態に応じて画像を切り替える
        /// </summary>
        private void ChangeImage()
        {
            string intimacy = GetIntimacy(tokenManager.TotalTokens);
            string feeling = GetFeeling(tokenManager.Feeling);

            Image? image = charZipFileManager.GetCharImage(intimacy, feeling);

            if ((image == null) && (gotNormalImage == true))
            {
                LogManager.LogOutput("画像が設定されませんでした");
                return;
            }

            //nullの場合は正常な画像ではないので、フラグをオフに
            if (image == null)
            {
                gotNormalImage = false;
            }
            else
            {
                gotNormalImage = true;
            }

            CharPictureBox.Image = image;
        }

        /// <summary>
        /// 現在のCPU使用率を出力する(テスト)
        /// </summary>
        /// <param name="value">出力するCPU使用率</param>
        private void OutCpuLabel(double value)
        {
            CpuUsageLabel.Text = "CPU: " + value.ToString("n2") + "%";
        }

        /// <summary>
        /// 検出状態を設定
        /// </summary>
        /// <param name="detectNum"></param>
        private void OutDetectLabel(int detectNum)
        {
            string txt;
            if (detectNum < 0)
            {
                txt = "検出: エラー";
            }
            else if (detectNum == 0)
            {
                txt = "検出: なし";
            }
            else
            {
                txt = "検出: あり";
            }

            CpuUsageLabel.Text = txt;
        }

        /// <summary>
        /// 統計情報の表示を切り替える
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleShowButton_Click(object sender, EventArgs e)
        {
            if ((StatsPanel.Visible == true) && (DefaultPanel.Visible == true))
            {
                StatsPanel.Visible = !StatsPanel.Visible;
                ClearButton.Visible = !ClearButton.Visible;
                ImportButton.Visible = !ImportButton.Visible;
            }
            else if ((StatsPanel.Visible == false) && (DefaultPanel.Visible == true))
            {
                DefaultPanel.Visible = !DefaultPanel.Visible;
            }
            else
            {
                StatsPanel.Visible = !StatsPanel.Visible;
                ClearButton.Visible = !ClearButton.Visible;
                ImportButton.Visible = !ImportButton.Visible;
                DefaultPanel.Visible = !DefaultPanel.Visible;
            }
        }

        /// <summary>
        /// キャラファイルのパスを書き込んでから再読み込みする
        /// </summary>
        /// <param name="path">キャラファイルパス</param>
        private void ReWriteCharConfig(string path)
        {
            SettingManager.PublicSettings.CharSettingPath = path;
            SettingManager.WriteSettingFile(SETTING_PATH);
            ReadCharConfig();
        }

        /// <summary>
        /// インポートボタンのクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportButton_Click(object sender, EventArgs e)
        {
            LogManager.LogOutput("インポートボタンがクリックされました");
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "ZIPファイル(*.zip)|*.zip;";
            ofd.Title = "インポートするキャラデータを選択してください";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ReWriteCharConfig(ofd.FileName);
            }
            else
            {
                LogManager.LogOutput("キャラデータのインポート失敗");
                MessageBox.Show("キャラデータのインポートに失敗しました", "インポートエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// キャラデータの参照をクリア
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_Click(object sender, EventArgs e)
        {
            DialogResult result =
                MessageBox.Show("キャラ設定をクリアします\nクリア後アプリを終了しますがよろしいですか?", "キャラ設定をクリアしますか?",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
            {
                return;
            }
            LogManager.LogOutput("クリアボタンがクリックされました");
            ReWriteCharConfig(string.Empty);
            this.Close();
        }

        /// <summary>
        /// フォーム終了時の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveNowWindowState();
            SettingManager.WriteSettingFile(SETTING_PATH);
            LogManager.LogOutput("アプリの終了処理終了");
        }

        /// <summary>
        /// 現在のウィンドウの状態を設定ファイルにまとめる
        /// </summary>
        private void SaveNowWindowState()
        {
            SettingManager.PublicSettings.WindowLocation = Location;
            SettingManager.PublicSettings.WindowSize = Size;
            SettingManager.PublicSettings.WindowState = GetWindowStateId();
        }

        /// <summary>
        /// 現在のウィンドウの状態を設定する
        /// </summary>
        private void SetNowWindowState()
        {
            Location = SettingManager.PublicSettings.WindowLocation;
            Size = SettingManager.PublicSettings.WindowSize;
            WindowState = GetWindowState();
            this.TopMost = SettingManager.PublicSettings.TopMost;
            SetControlFontSize();
            LogManager.LogOutput("設定を復元しました");
        }

        /// <summary>
        /// フォントのサイズを設定に応じて拡大する
        /// </summary>
        private void SetControlFontSize()
        {
            int enlarge = SettingManager.PublicSettings.FontEnlargeSize;

            if (enlarge > 0)
            {
                SetSatsPanelControlSize(enlarge);
                SetGeneralLabelControlSize(enlarge);
                SetButtonControlSize(enlarge);
                LogManager.LogOutput("フォントの大きさ再設定終了");
            }
        }

        /// <summary>
        /// StatsPanelのコントロールに対するフォントサイズ設定
        /// </summary>
        /// <param name="enlarge">拡大サイズ</param>
        private void SetSatsPanelControlSize(int enlarge)
        {
            Control[] controls =
                {
                    StatsLabel,
                    DailyTokenLabel,
                    EmoTokenLabel,
                    AverageEmotionTokensLabel,
                    TotalTokenLabel,
                    FeelingLabel,
                    FlopsLabel
                };

            for (int panelInd = 0; panelInd < controls.Length; panelInd++)
            {
                Control nowCont = controls[panelInd];

                nowCont.Font = new Font(nowCont.Font.Name, nowCont.Font.Size + enlarge);

                if (panelInd > 0)
                {
                    Control befCont = controls[panelInd - 1];

                    nowCont.Location = new Point(befCont.Location.X, befCont.Location.Y + nowCont.Height + FONT_MARGIN_SIZE);
                }
                else
                {
                    //位置を端に合わせる
                    nowCont.Top = 0;
                    nowCont.Left = 0;
                }
            }
        }

        /// <summary>
        /// ラベルのコントロールに対するフォントサイズ設定
        /// </summary>
        /// <param name="enlarge">拡大サイズ</param>
        private void SetGeneralLabelControlSize(int enlarge)
        {
            Control[] controls =
            {
                CpuUsageLabel,
                IntimacyLabel,
                EmoStringLabel
            };

            for (int panelInd = 0; panelInd < controls.Length; panelInd++)
            {
                Control nowCont = controls[panelInd];

                nowCont.Font = new Font(nowCont.Font.Name, nowCont.Font.Size + enlarge);

                if (panelInd > 0)
                {
                    Control befCont = controls[panelInd - 1];

                    nowCont.Location = new Point(befCont.Location.X, befCont.Location.Y + nowCont.Height + FONT_MARGIN_SIZE);
                }
                else
                {
                    //位置を端に合わせる
                    nowCont.Top = 0;
                    nowCont.Left = 0;
                }
            }
        }

        /// <summary>
        /// ボタンのコントロールに対するフォントサイズ設定
        /// </summary>
        /// <param name="enlarge">拡大サイズ</param>
        private void SetButtonControlSize(int enlarge)
        {
            Control[] controls =
            {
                ToggleShowButton,
                ClearButton,
                ImportButton,
            };

            int maxWidth = 0;

            for (int panelInd = 0; panelInd < controls.Length; panelInd++)
            {
                Control nowCont = controls[panelInd];

                nowCont.Font = new Font(nowCont.Font.Name, nowCont.Font.Size + enlarge);

                if (nowCont.Width > maxWidth)
                {
                    maxWidth = nowCont.Width;
                }

                if (panelInd > 0)
                {
                    Control befCont = controls[panelInd - 1];

                    nowCont.Location = new Point(nowCont.Location.X, befCont.Location.Y - nowCont.Height - FONT_MARGIN_SIZE);
                }
                else
                {
                    nowCont.Top -= enlarge;
                }
            }

            if (maxWidth > 0)
            {
                foreach (Control control in controls)
                {
                    control.Width = maxWidth;
                    control.Left = Size.Width - maxWidth - 28;
                }
            }
        }

        /// <summary>
        /// ウィンドウの状態IDを返却する
        /// </summary>
        /// <returns>0: 通常, 1: 最大化, 2: 最小化</returns>
        private int GetWindowStateId()
        {
            int wstate = 0;

            switch (WindowState)
            {
                case FormWindowState.Normal:
                    wstate = 0;
                    break;

                case FormWindowState.Maximized:
                    wstate = 1;
                    break;

                case FormWindowState.Minimized:
                    wstate = 2;
                    break;

                default:
                    wstate = 0;
                    break;
            }

            return wstate;
        }

        /// <summary>
        /// ウィンドウの状態を返却する
        /// </summary>
        /// <returns>通常、最大化、最小化</returns>
        private FormWindowState GetWindowState()
        {
            FormWindowState loadState = FormWindowState.Normal;

            switch (SettingManager.PublicSettings.WindowState)
            {
                case 0:
                    loadState = FormWindowState.Normal;
                    break;

                case 1:
                    loadState = FormWindowState.Maximized;
                    break;


                case 2:
                    loadState = FormWindowState.Minimized;
                    break;

                default:
                    loadState = FormWindowState.Normal;
                    break;
            }

            return loadState;
        }

        /// <summary>
        /// コントロールの色を設定する
        /// </summary>
        /// <param name="color">色</param>
        private void SetControlColor(Color color)
        {
            LogManager.LogOutput("コントロールの色を" + color.ToString() + "に設定しました");
            BackColor = color;
        }

        /// <summary>
        /// 画像の更新タイマ処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageChangeTimer_Tick(object sender, EventArgs e)
        {
            ChangeImage();
        }

        /// <summary>
        /// ウィンドウのサイズ変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            UpdateImageSize();
        }

        /// <summary>
        /// 画像のサイズと位置を更新する
        /// </summary>
        private void UpdateImageSize()
        {
            CharPictureBox.Size = SettingManager.PublicSettings.ImageSize;
            CharPictureBox.Left = (Width / 2) - (CharPictureBox.Width / 2) - 10;
            CharPictureBox.Top = (Height / 2) - (CharPictureBox.Height / 2) - 10;
        }

        /// <summary>
        /// 1s周期のタスクを実行する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskRunTimer1s_Tick(object sender, EventArgs e)
        {
            try
            {
                if (TaskRun1sTable == null) return;

                General1sTimerFunc();

                //タスクテーブルに設定されているタスククラスの関数を順番に実行する
                for (int i = 0; i < TaskRun1sTable.Length; i++)
                {
                    if (TaskRun1sTable[i] == null) continue;

                    switch (TaskRun1sTable[i].ClassTask.Status)
                    {
                        case TaskStatus.Running:
                            TaskRun1sTable[i].TaskCheckRet(TaskReturn.TASK_BLOCKED);
                            continue;

                        default:
                            break;
                    }

                    TaskClassInterface sendTask = TaskRun1sTable[i];

                    TaskRun1sTable[i].ClassTask = Task.Run(() =>
                    {
                        sendTask.TaskCheckRet(sendTask.TaskFunc());
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorOutput("タスク実行エラー", ex.Message);
            }
        }

        /// <summary>
        /// 全体の1s毎処理
        /// </summary>
        private void General1sTimerFunc()
        {
            if (!cameraTimer.CameraDisable)
            {
                CameraProcess();
            }
            else
            {
                CpuProcess();
            }


        }

        /// <summary>
        /// カメラ用のトークン算出処理など
        /// </summary>
        private void CameraProcess()
        {
            if (cameraTimer.AvgCalcFlg)
            {
                tokenManager.AddTokens(cameraTimer.DetectAvg, true);
                OutTokenLabel();
                cameraTimer.ClearDetectAvg();
            }
            OutDetectLabel(cameraTimer.FaceDetected);
        }

        /// <summary>
        /// CPUのトークン算出処理など
        /// </summary>
        private void CpuProcess()
        {
            if (cpuAvgCalcTimer.AvgCalcFlg)
            {
                tokenManager.AddTokens(cpuAvgCalcTimer.CpuAvg, false);
                OutTokenLabel();
                cpuAvgCalcTimer.ClearCpuAvg();
            }
            OutCpuLabel(cpuAvgCalcTimer.CpuUsage);
        }
    }
}
