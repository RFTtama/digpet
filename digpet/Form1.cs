using System.Security.Cryptography;
using System.Windows.Forms;

namespace digpet
{
    public partial class Form1 : Form
    {
        //クラス関連の宣言
        private CpuAvgManager cpuAvgManager = new CpuAvgManager();
        private TokenManager tokenManager = new TokenManager();
        private SettingManager settingManager = new SettingManager();
        private CharZipFileManager charZipFileManager = new CharZipFileManager();

        //変数関連の宣言
        private int cpuCnt;
        private double cpuAvg;

        //定数関連の宣言
        private const string SETTING_PATH = "settings.json";
        private const int FONT_MARGIN_SIZE = 5;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            Text += "   Ver." + APP_SETTINGS.APPLICATION_VERSION;
            Init();
            ReadSettings();
            SetNowWindowState();
            CpuUsageTimer.Enabled = true;
            LogManager.LogOutput("初期化が完了しました");
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void Init()
        {
            cpuCnt = 0;
            cpuAvg = 0.0;
        }

        /// <summary>
        /// トークンをリセットする時刻が設定されているか確認する
        /// </summary>
        private void CheckResetTime()
        {
            int resetHour = settingManager.Settings.ResetHour;

            if (resetHour < 0)
            {
                resetHour = SetResetTime();
                settingManager.WriteSettingFile(SETTING_PATH);
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
                string input = Microsoft.VisualBasic.Interaction.InputBox("トークンをリセットする時刻を0～23で設定してください",
                    "リセット時刻設", "0");

                int hour = -1;

                if (int.TryParse(input, out hour) == true)
                {
                    if (hour >= 0 && hour < 24)
                    {
                        settingManager.Settings.ResetHour = hour;
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
            if (!string.IsNullOrEmpty(settingManager.Settings.CharSettingPath))
            {
                charZipFileManager.ReadCharSettings(settingManager.Settings.CharSettingPath);
                SetControlColor(charZipFileManager.GetControlColor());
            }
            else
            {
                ErrorLog.ErrorOutput("キャラファイル読み取りエラー", "設定されているキャラファイルのパスがnullか空です");
            }
        }

        /// <summary>
        /// 設定ファイル関連読み取り
        /// </summary>
        private void ReadSettings()
        {
            settingManager.ReadSettingFile(SETTING_PATH);
            CheckResetTime();
            ReadCharConfig();
        }

        /// <summary>
        /// 1分おきにCPU使用率の平均を求めて、変数に代入する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CpuUsageTimer_Tick(object sender, EventArgs e)
        {
            //60秒に1回処理を行う
            if ((cpuCnt > 0) && (cpuCnt % 60 == 0))
            {
                try
                {
                    //CPU使用率の平均を取得し、トークンを計算する
                    GetCpuAvg();
                }
                catch (Exception ex)
                {
                    ErrorLog.ErrorOutput("CPU使用率平均計算エラー", ex.Message);
                    CpuUsageTimer.Enabled = false;
                }
            }
            else
            {
                //CPU使用率を加算
                SumCpuAvg();
            }
            cpuCnt++;
        }

        /// <summary>
        /// CPU使用率の平均を求めcpuAvgに代入する
        /// </summary>
        private void GetCpuAvg()
        {
            cpuCnt = 0;
            cpuAvg = cpuAvgManager.GetCpuAvg();
            tokenManager.AddTokens(cpuAvg);
            OutTokenLabel();
            cpuAvgManager.Clear();
            LogManager.LogOutput("分毎トークンの算出完了");
        }

        /// <summary>
        /// CPU使用率の平均を求められるように数値を足す
        /// </summary>
        private void SumCpuAvg()
        {
            double cpuUsage = (double)CpuWatcher.GetCpuUsage();
            cpuAvgManager.SetCpuSum(cpuUsage);
            OutCpuLabel(cpuUsage);
            ChangeImage();
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
            EmoStringLabel.Text = GetFeeling(tokenManager.Feeling);
            DailyTokenLabel.Text = "今日の獲得トークン: " + tokenManager.DailyTokens.ToString("n2");
            EmoTokenLabel.Text = "今日の感情トークン: " + tokenManager.EmotionTokens.ToString("n2");
            AverageEmotionTokensLabel.Text = "平均感情トークン: " + tokenManager.AverageEmotionTokens.ToString("n2");
            FeelingLabel.Text = "今日の感情: " + tokenManager.Feeling.ToString("n2");
            FlopsLabel.Text = "FLOPS: " + tokenManager.Flops.ToString();
            TotalTokenLabel.Text = "累計トークン: " + (tokenManager.TotalTokens).ToString("n2");
            IntimacyLabel.Text = GetIntimacy(tokenManager.TotalTokens);
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

            if (image == null)
            {
                LogManager.LogOutput("画像が設定されませんでした");
                return;
            }

            LogManager.LogOutput("画像が" + image.ToString() + "に設定されました");

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
        /// 統計情報の表示を切り替える
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleShowButton_Click(object sender, EventArgs e)
        {
            StatsPanel.Visible = !StatsPanel.Visible;
            ClearButton.Visible = !ClearButton.Visible;
            ImportButton.Visible = !ImportButton.Visible;
        }

        /// <summary>
        /// キャラファイルのパスを書き込んでから再読み込みする
        /// </summary>
        /// <param name="path">キャラファイルパス</param>
        private void ReWriteCharConfig(string path)
        {
            settingManager.Settings.CharSettingPath = path;
            settingManager.WriteSettingFile(SETTING_PATH);
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
            LogManager.LogOutput("クリアボタンがクリックされました");
            ReWriteCharConfig(string.Empty);
        }

        /// <summary>
        /// フォーム終了時の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveNowWindowState();
            settingManager.WriteSettingFile(SETTING_PATH);
            LogManager.LogOutput("アプリの終了処理終了");
        }

        /// <summary>
        /// 現在のウィンドウの状態を設定ファイルにまとめる
        /// </summary>
        private void SaveNowWindowState()
        {
            settingManager.Settings.WindowLocation = Location;
            settingManager.Settings.WindowSize = Size;
            settingManager.Settings.WindowState = GetWindowStateId();
        }

        /// <summary>
        /// 現在のウィンドウの状態を設定する
        /// </summary>
        private void SetNowWindowState()
        {
            Location = settingManager.Settings.WindowLocation;
            Size = settingManager.Settings.WindowSize;
            WindowState = GetWindowState();
            SetControlFontSize();
            LogManager.LogOutput("設定を復元しました");
        }

        /// <summary>
        /// フォントのサイズを設定に応じて拡大する
        /// </summary>
        private void SetControlFontSize()
        {
            int enlarge = settingManager.Settings.FontEnlargeSize;

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

            switch (settingManager.Settings.WindowState)
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
    }
}
