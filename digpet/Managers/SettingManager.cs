using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using digpet.Modules;

namespace digpet.Managers
{
    /// <summary>
    /// 設定ファイル管理クラス
    /// Digpetの別ソースファイルでも使用できるようにstaticにしてある
    /// </summary>
    public static class SettingManager
    {
        //外部で変更可能な設定
        public static DigpetSettings PublicSettings = new DigpetSettings();

        //外部で変更可能でない設定
        public static class PrivateSettings
        {
            public const string APPLICATION_VERSION = "1.01.00" + DEBUG_APPENDANCE;         //アプリバージョン
            public const string CHAR_FORMAT_VERSION = "1.00.00";                            //キャラフォーマットのバージョン

            public const string CONFIG_FILE_PATH = "config.json";                           //コンフィグファイルのパス
            public const string ERRORLOG_PATH = "errorLog.txt";                             //エラーログのパス
            public const string LOG_PATH = "Log.txt";                                       //ログファイルのパス
            public const string TOKEN_PATH = "TOKENS.dig";                                  //トークンファイルのパス
            public const string CASCADE_PATH = "haarcascade_frontalface_default.xml";       //カスケードファイルのパス

#if DEBUG
            public const string DEBUG_APPENDANCE = "-debug";                                //デバッグ判別用
#else
        public const string DEBUG_APPENDANCE    = "";                                   //デバッグ判別用
#endif
        }

        //JSONの設定
        private static readonly JsonSerializerOptions JSON_OPTIONS = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        /// <summary>
        /// 設定を読み取る
        /// </summary>
        /// <param name="path">設定ファイルパス</param>
        public static void ReadSettingFile(string path)
        {
            if (File.Exists(path))
            {
                ReadSettings(path);
            }
            else
            {
                WriteSettings(path);
            }
        }

        /// <summary>
        /// 設定を書き込む
        /// </summary>
        /// <param name="path">設定ファイルパス</param>
        public static void WriteSettingFile(string path)
        {
            WriteSettings(path);
        }

        /// <summary>
        /// 設定ファイルを書き込む
        /// </summary>
        /// <param name="path"></param>
        private static void WriteSettings(string path)
        {
            try
            {
                string settingString = JsonSerializer.Serialize(PublicSettings, JSON_OPTIONS);
                using (StreamWriter sw = new StreamWriter(path, false))
                {
                    sw.Write(settingString);
                }
                LogManager.LogOutput("設定ファイルが正常に書き込まれました");
            }
            catch (Exception ex)
            {
                LogManager.LogOutput("設定ファイルの書き込み失敗");
                ErrorLog.ErrorOutput("設定ファイル初期化エラー", ex.Message);
            }
        }

        /// <summary>
        /// 設定を読み取る
        /// </summary>
        /// <param name="path"></param>
        private static void ReadSettings(string path)
        {
            try
            {
                string settingString = string.Empty;
                using (StreamReader sr = new StreamReader(path))
                {
                    settingString = sr.ReadToEnd();
                }
                PublicSettings = JsonSerializer.Deserialize<DigpetSettings>(settingString) ?? new DigpetSettings();
                LogManager.LogOutput("設定ファイルが読み込まれました");
            }
            catch (Exception ex)
            {
                LogManager.LogOutput("設定ファイルの読み込みに失敗しました");
                ErrorLog.ErrorOutput("設定ファイル読み取りエラー", ex.Message);
            }
        }

        /// <summary>
        /// 設定保持クラス
        /// 設定の項目が追加されたら増やすこと!!
        /// </summary>
        public class DigpetSettings
        {
            //キャラ設定ファイルのパス
            public string CharSettingPath { get; set; }

            //リセット時間
            public int ResetHour { get; set; }

            //ウィンドウの状態 0: 通常, 1: 最大化, 2: 最小化
            public int WindowState { get; set; }

            //ウィンドウを常に前面に配置するか
            public bool TopMost { get; set; }

            //ウィンドウサイズ
            public Size WindowSize { get; set; }

            //ウィンドウロケーション
            public Point WindowLocation { get; set; }

            //フォントの拡大サイズ
            public int FontEnlargeSize { get; set; }

            //キャラ画像サイズ
            public Size ImageSize { get; set; }

            //カメラモードのオンオフ
            public bool EnableCameraMode { get; set; }

            //使用するカメラのID
            public int CameraId { get; set; }

            //10回中何回タスクの実行遅延したら機能を無効にするか
            public uint CameraDisableThreshold { get; set; }

            /// <summary>
            /// コンストラクタ
            /// 初期値に初期化する
            /// </summary>
            public DigpetSettings()
            {
                CharSettingPath = string.Empty;
                ResetHour = -1;
                WindowState = 0;
                TopMost = false;
                WindowSize = new Size(500, 500);
                WindowLocation = new Point(0, 0);
                FontEnlargeSize = 0;
                ImageSize = new Size(400, 400);
                EnableCameraMode = false;
                CameraId = 0;
                CameraDisableThreshold = 1;
            }
        }
    }
}
