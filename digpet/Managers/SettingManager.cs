using digpet.Modules;
using System.Text.Json;

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
        public static DigpetSettingsNew PublicSettingsNew = new DigpetSettingsNew();

        //外部で変更可能でない設定
        public static class PrivateSettings
        {
            public const string APPLICATION_VERSION = "2.03.00" + DEBUG_APPENDANCE;         //アプリバージョン
            public const string CHAR_FORMAT_VERSION = "2.02.00";                            //キャラフォーマットのバージョン

            public const string CONFIG_FILE_PATH = "config.json";                           //コンフィグファイルのパス
            public const string ERRORLOG_PATH = "errorLog.txt";                             //エラーログのパス
            public const string LOG_PATH = "Log.txt";                                       //ログファイルのパス
            public const string TOKEN_CALC_PATH = "TOKEN_BANK";                             //トークンファイルのパス
            public const string SETTING_PATH = "settings.json";                             //設定ファイルのパス
            public const string CASCADE_PATH = "haarcascade_frontalface_default.xml";       //カスケードファイルのパス
            public const string PLOT_PATH = "plot.png";                                     //プロット画像のパス
            public const string LOG_DIRECTORY = "Logs";                                     //ログフォルダのディレクトリ
            public const string SETTINGS_HEADER_VER = "1.0.0";                              //設定ファイルのヘッダ情報

#if DEBUG
            public const string DEBUG_APPENDANCE = "-preview";                              //デバッグ判別用
#else
        public const string DEBUG_APPENDANCE    = "";                                   //デバッグ判別用
#endif
        }

        //JSONの設定
        public static readonly JsonSerializerOptions JSON_OPTIONS = new JsonSerializerOptions
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
                PublicSettingsNew.SettingsHeader.SettingsVersion = PrivateSettings.SETTINGS_HEADER_VER;
                string settingString = JsonSerializer.Serialize(PublicSettingsNew, JSON_OPTIONS);
                using (StreamWriter sw = new StreamWriter(path, false))
                {
                    sw.Write(settingString);
                }
            }
            catch (Exception ex)
            {
                ErrorLogLib.ErrorOutput("設定ファイル初期化エラー", ex.Message);
            }
        }

        /// <summary>
        /// 設定を読み取る
        /// </summary>
        /// <param name="path"></param>
        private static void ReadSettings(string path)
        {
            string settingString = string.Empty;

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    settingString = sr.ReadToEnd();
                }

                DigpetSettingsNew? tmp = JsonSerializer.Deserialize<DigpetSettingsNew>(settingString);

                if (tmp != null)
                {
                    if (tmp.SettingsHeader.SettingsVersion == PrivateSettings.SETTINGS_HEADER_VER)
                    {
                        PublicSettingsNew = tmp;
                        return;
                    }
                    DigpetSettings? tmp2 = JsonSerializer.Deserialize<DigpetSettings>(settingString);
                    if (tmp2 != null)
                    {
                        PublicSettings = tmp2;
                        PublicSettingsNew.SettingsHeader.SettingsVersion = PrivateSettings.SETTINGS_HEADER_VER;
                        return;
                    }
                }

                PublicSettings = new DigpetSettings();
                PublicSettingsNew.SettingsHeader.SettingsVersion = PrivateSettings.SETTINGS_HEADER_VER;

            }
            catch (Exception ex)
            {
                ErrorLogLib.ErrorOutput("設定ファイル読み取りエラー", ex.Message);
            }
        }

        /// <summary>
        /// 設定保持クラス
        /// 設定の項目が追加されたら増やすこと!!
        /// </summary>
        public class DigpetSettings
        {
            //キャラ設定ファイルのパス
            public string CharSettingPath
            {
                get
                {
                    return PublicSettingsNew.CharFileSettings.CharSettingPath;
                }
                set
                {
                    PublicSettingsNew.CharFileSettings.CharSettingPath = value;
                }
            }

            //ウィンドウの状態 0: 通常, 1: 最大化, 2: 最小化
            public int WindowState
            {
                get
                {
                    return PublicSettingsNew.WindowSettings.WindowState;
                }
                set
                {
                    PublicSettingsNew.WindowSettings.WindowState = value;
                }
            }

            //ウィンドウを常に前面に配置するか
            public bool TopMost
            {
                get
                {
                    return PublicSettingsNew.WindowSettings.TopMost;
                }
                set
                {
                    PublicSettingsNew.WindowSettings.TopMost = value;
                }
            }

            //ウィンドウサイズ
            public Size WindowSize
            {
                get
                {
                    return PublicSettingsNew.WindowSettings.WindowSize;
                }
                set
                {
                    PublicSettingsNew.WindowSettings.WindowSize = value;
                }
            }

            //ウィンドウロケーション
            public Point WindowLocation
            {
                get
                {
                    return PublicSettingsNew.WindowSettings.WindowLocation;
                }
                set
                {
                    PublicSettingsNew.WindowSettings.WindowLocation = value;
                }
            }

            //フォントの拡大サイズ
            public int FontEnlargeSize
            {
                get
                {
                    return PublicSettingsNew.AplSettings.FontEnlargeSize;
                }
                set
                {
                    PublicSettingsNew.AplSettings.FontEnlargeSize = value;
                }
            }

            //キャラ画像サイズ
            public Size ImageSize
            {
                get
                {
                    return PublicSettingsNew.AplSettings.ImageSize;
                }
                set
                {
                    PublicSettingsNew.AplSettings.ImageSize = value;
                }
            }

            //カメラモードのオンオフ
            public bool EnableCameraMode
            {
                get
                {
                    return PublicSettingsNew.CameraSettings.EnableCameraMode;
                }
                set
                {
                    PublicSettingsNew.CameraSettings.EnableCameraMode = value;
                }
            }

            //使用するカメラのID
            public int CameraId
            {
                get
                {
                    return PublicSettingsNew.CameraSettings.CameraId;
                }
                set
                {
                    PublicSettingsNew.CameraSettings.CameraId = value;
                }
            }

            //10回中何回タスクの実行遅延したら機能を無効にするか
            public uint CameraDisableThreshold
            {
                get
                {
                    return PublicSettingsNew.CameraSettings.CameraDisableThreshold;
                }
                set
                {
                    PublicSettingsNew.CameraSettings.CameraDisableThreshold = value;
                }
            }

            //トークンプロットの保存
            public bool SaveTokenPlot
            {
                get
                {
                    return PublicSettingsNew.TokenSettings.SaveTokenPlot;
                }
                set
                {
                    PublicSettingsNew.TokenSettings.SaveTokenPlot = value;
                }
            }


            //ログを削除する日数
            public int LogDeleteDays
            {
                get
                {
                    return PublicSettingsNew.LogSettings.LogDeleteDays;
                }
                set
                {
                    PublicSettingsNew.LogSettings.LogDeleteDays = value;
                }
            }

            //Tokenをバックアップする間隔(s)
            public int TokenBackupInterval
            {
                get
                {
                    return PublicSettingsNew.TokenSettings.TokenBackupInterval;
                }
                set
                {
                    PublicSettingsNew.TokenSettings.TokenBackupInterval = value;
                }
            }

            //非アクティブモード有効無効
            public bool EnableNonActiveMode
            {
                get
                {
                    return PublicSettingsNew.AplSettings.EnableNonActiveMode;
                }
                set
                {
                    PublicSettingsNew.AplSettings.EnableNonActiveMode = value;
                }
            }

            //非アクティブモードの開始時間(ms)
            public int NonActiveModeStartTime
            {
                get
                {
                    return PublicSettingsNew.AplSettings.NonActiveModeStartTime;
                }
                set
                {
                    PublicSettingsNew.AplSettings.NonActiveModeStartTime = value;
                }
            }

            //カメラ検出平滑化モードの有効無効
            public bool EnableCameraDetectSmoothingMode
            {
                get
                {
                    return PublicSettingsNew.CameraSettings.EnableCameraDetectSmoothingMode;
                }
                set
                {
                    PublicSettingsNew.CameraSettings.EnableCameraDetectSmoothingMode = value;
                }
            }

            //放置モードの有効無効
            public bool EnablNeglectMode
            {
                get
                {
                    return PublicSettingsNew.AplSettings.EnablNeglectMode;
                }
                set
                {
                    PublicSettingsNew.AplSettings.EnablNeglectMode = value;
                }
            }

            //放置モードの開始までの時間(s)
            public int NeglectActiveTime
            {
                get
                {
                    return PublicSettingsNew.AplSettings.NeglectActiveTime;
                }
                set
                {
                    PublicSettingsNew.AplSettings.NeglectActiveTime = value;
                }
            }

            //検出結果の閾値
            public int DetectThreshold
            {
                get
                {
                    return PublicSettingsNew.CameraSettings.DetectThreshold;
                }
                set
                {
                    PublicSettingsNew.CameraSettings.DetectThreshold = value;
                }
            }
        }

        /// <summary>
        /// 新設定用クラス
        /// </summary>
        public class DigpetSettingsNew
        {
            public _SettingsHeader SettingsHeader { get; set; }
            public _CharFileSettings CharFileSettings { get; set; }
            public _WindowSettings WindowSettings { get; set; }
            public _AplSettings AplSettings { get; set; }
            public _CameraSettings CameraSettings { get; set; }
            public _TokenSettings TokenSettings { get; set; }
            public _LogSettings LogSettings { get; set; }

            /// <summary>
            /// 設定ファイルのヘッダ情報
            /// </summary>
            public class _SettingsHeader
            {
                public string SettingsVersion { get; set; }

                public _SettingsHeader()
                {
                    SettingsVersion = "0.0.0";
                }
            }

            /// <summary>
            /// キャラファイルの設定関連
            /// </summary>
            public class _CharFileSettings
            {
                //キャラ設定ファイルのパス
                public string CharSettingPath { get; set; }

                public _CharFileSettings(string charSettingPath)
                {
                    CharSettingPath = charSettingPath;
                }
            }

            /// <summary>
            /// ウィンドウ状態の設定関連
            /// </summary>
            public class _WindowSettings
            {
                //ウィンドウの状態 0: 通常, 1: 最大化, 2: 最小化
                public int WindowState { get; set; }

                //ウィンドウを常に前面に配置するか
                public bool TopMost { get; set; }

                //ウィンドウサイズ
                public Size WindowSize { get; set; }

                //ウィンドウロケーション
                public Point WindowLocation { get; set; }
            }

            /// <summary>
            /// アプリの設定関連
            /// </summary>
            public class _AplSettings
            {
                //フォントの拡大サイズ
                public int FontEnlargeSize { get; set; }

                //キャラ画像サイズ
                public Size ImageSize { get; set; }

                //非アクティブモード有効無効
                public bool EnableNonActiveMode { get; set; }

                //非アクティブモードの開始時間(ms)
                public int NonActiveModeStartTime { get; set; }

                //放置モードの有効無効
                public bool EnablNeglectMode { get; set; }

                //放置モードの開始までの時間(s)
                public int NeglectActiveTime { get; set; }
            }

            /// <summary>
            /// カメラの設定関連
            /// </summary>
            public class _CameraSettings
            {
                //カメラモードのオンオフ
                public bool EnableCameraMode { get; set; }

                //使用するカメラのID
                public int CameraId { get; set; }

                //10回中何回タスクの実行遅延したら機能を無効にするか
                public uint CameraDisableThreshold { get; set; }

                //カメラ検出平滑化モードの有効無効
                public bool EnableCameraDetectSmoothingMode { get; set; }

                //検出結果の閾値
                public int DetectThreshold { get; set; }
            }

            /// <summary>
            /// token関連設定
            /// </summary>
            public class _TokenSettings
            {
                //トークンプロットの保存
                public bool SaveTokenPlot { get; set; }

                //Tokenをバックアップする間隔(s)
                public int TokenBackupInterval { get; set; }
            }

            /// <summary>
            /// ログ関連設定
            /// </summary>
            public class _LogSettings
            {
                //ログを削除する日数
                public int LogDeleteDays { get; set; }
            }

            public DigpetSettingsNew()
            {
                //コンストラクタ設定
                SettingsHeader = new _SettingsHeader();
                CharFileSettings = new _CharFileSettings(string.Empty);
                WindowSettings = new _WindowSettings();
                AplSettings = new _AplSettings();
                CameraSettings = new _CameraSettings();
                TokenSettings = new _TokenSettings();
                LogSettings = new _LogSettings();

                //初期化
                WindowSettings.WindowState = 0;
                WindowSettings.TopMost = false;
                WindowSettings.WindowSize = new Size(500, 500);
                WindowSettings.WindowLocation = new Point(0, 0);
                AplSettings.FontEnlargeSize = 0;
                AplSettings.ImageSize = new Size(400, 400);
                CameraSettings.EnableCameraMode = false;
                CameraSettings.CameraId = 0;
                CameraSettings.CameraDisableThreshold = 1;
                TokenSettings.SaveTokenPlot = true;
                LogSettings.LogDeleteDays = 31;
                TokenSettings.TokenBackupInterval = 10 * 60;
                AplSettings.EnableNonActiveMode = false;
                AplSettings.NonActiveModeStartTime = 1000;
                CameraSettings.EnableCameraDetectSmoothingMode = true;
                AplSettings.EnablNeglectMode = false;
                AplSettings.NeglectActiveTime = 600;
                CameraSettings.DetectThreshold = 80;
            }
        }
    }
}
