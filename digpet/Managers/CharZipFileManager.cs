using digpet.Modules;
using digpet.TaskTimerClass.TimerFunc;
using System.Collections.Immutable;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace digpet.Managers
{
    public class CharZipFileManager
    {
        private static Lazy<CharZipFileManager> _lazy = new(() => new CharZipFileManager(), isThreadSafe: true);
        public static CharZipFileManager Instance => _lazy.Value;

        //クラス宣言
        private CharSettingManager _charSettingManager = new CharSettingManager();          //キャラクターファイルの設定クラス

        //変数宣言
        private Dictionary<string, Image> imageList = new Dictionary<string, Image>();      //画像リスト

        //固定値宣言
        private readonly string[] IMAGE_EXTENSION =
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".tif"
        };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CharZipFileManager()
        {
            Init();
        }

        /// <summary>
        /// 初期化/クリアする
        /// </summary>
        private void Init()
        {
            _charSettingManager = new CharSettingManager();
            imageList = new Dictionary<string, Image>();
        }

        /// <summary>
        /// 画像切替周期を取得する
        /// </summary>
        /// <returns>切替周期(ms)</returns>
        public int GetPictureTurnOverPeriod()
        {
            int ret = 200;

            if (_charSettingManager.CharSettings.pictureTurnOverPeriod > 0)
            {
                ret = _charSettingManager.CharSettings.pictureTurnOverPeriod;
            }

            return ret;
        }

        public Image? GetCharImage(string feeling)
        {
            string imageName = string.Empty;
            if (CheckNeglectEnable())
            {
                imageName = _charSettingManager.CharSettings.charSettings.leavingImgPath;
            }

            if (string.IsNullOrEmpty(imageName))
            {
                imageName = GetImageNameFromFeeling(feeling);
            }
            return GetImageFromImageName(imageName);
        }

        private bool CheckNeglectEnable()
        {
            if (!SettingManager.PublicSettings.EnablNeglectMode) return false;
            CameraTimer ct = CameraTimer.Instance;
            if (!ct.IsNeglect) return false;
            return true;
        }

        /// <summary>
        /// feelingとintimacyから対象の画像名を取り出す
        /// transitionの割合で計算される
        /// </summary>
        /// <param name="target">ターゲットのintimacy</param>
        /// <param name="feelingString">feelingのstring</param>
        /// <returns>画像名</returns>
        private string GetImageNameFromFeeling(string feelingString)
        {
            string ret = string.Empty;
            int tranSum = 0;
            Dictionary<string, int> transDict = new Dictionary<string, int>();

            foreach (CharSettingManager.Settings.CharSettings.Feeling feeling in _charSettingManager.CharSettings.charSettings.feelings)
            {
                if (feeling.name == feelingString && !transDict.ContainsKey(feelingString))
                {
                    transDict.Add(feeling.filePath, feeling.transition);
                    tranSum += feeling.transition;
                }
            }

            if (tranSum <= 0)
            {
                return ret;
            }

            Random random = new Random();
            int selected = random.Next() % tranSum;

            int selectSum = 0;

            foreach (string key in transDict.Keys)
            {
                selectSum += transDict[key];

                if (selected < selectSum)
                {
                    ret = key;
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// Feelingのタグを取得する
        /// </summary>
        /// <returns>Feelingのタグ</returns>
        public string GetFeelingTag()
        {
            string? ret = string.Empty;

            ret = _charSettingManager.CharSettings.charSettings.feelingTag;

            if (string.IsNullOrWhiteSpace(ret))
            {
                ret = string.Empty;
            }

            return ret;
        }

        /// <summary>
        /// 画像名から対象の画像を取得する
        /// </summary>
        /// <param name="imageName">取得する画像名</param>
        /// <returns>画像 対象が存在しない場合はnull</returns>
        private Image? GetImageFromImageName(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                return null;
            }

            Image? ret = null;

            foreach (string key in imageList.Keys)
            {
                if (key == imageName)
                {
                    ret = imageList[key];
                }
            }

            return ret;
        }

        /// <summary>
        /// 感情のテキストを取得する
        /// </summary>
        /// <param name="feeling">感情</param>
        /// <returns></returns>
        public string GetFeelingString(double feeling)
        {
            return _charSettingManager.CharSettings.feelingSetting.GetFeelingString(feeling);
        }

        /// <summary>
        /// コントロールの色を取得する
        /// </summary>
        /// <returns></returns>
        public Color GetControlColor()
        {
            ushort red = _charSettingManager.CharSettings.backgroundColor.red;
            ushort green = _charSettingManager.CharSettings.backgroundColor.green;
            ushort blue = _charSettingManager.CharSettings.backgroundColor.blue;

            return Color.FromArgb(red, green, blue);
        }

        /// <summary>
        /// キャラクターのコンフィグファイルを読み取る
        /// </summary>
        /// <param name="path">Zipファイルパス</param>
        public void ReadCharSettings(string path)
        {
            ErrorLogLib er = ErrorLogLib.Instance;
            if (!File.Exists(path))
            {
                er.ErrorOutput("コンフィグファイル確認エラー", "キャラデータが見つかりません");
                return;
            }

            Init();

            try
            {
                using (ZipArchive zip = ZipFile.OpenRead(path))
                {
                    ZipArchiveEntry? entry = zip.GetEntry(SettingManager.PrivateSettings.CONFIG_FILE_PATH);

                    if (entry == null)
                    {
                        er.ErrorOutput("コンフィグファイル読み取りエラー", "キャラデータにコンフィグファイルが含まれていません");
                        return;
                    }

                    if (ReadConfig(entry) == 0)
                    {
                        SetImageList(zip);
                    }
                    else
                    {
                        er.ErrorOutput("コンフィグファイル読み取りエラー", "キャラデータのバージョンがサポートされていません");
                    }
                }
            }
            catch (Exception ex)
            {
                er.ErrorOutput("コンフィグファイル読み取りエラー", ex.Message);
            }
        }

        /// <summary>
        /// バージョンが対応しているか
        /// </summary>
        /// <returns>true: 対応, false: 非対応</returns>
        private bool IsHandleVersion()
        {
            bool ret = false;
            VersionManager charVersion = new VersionManager(_charSettingManager.CharSettings.version);
            VersionManager availableVersion = new VersionManager(SettingManager.PrivateSettings.CHAR_FORMAT_VERSION);

            if (charVersion.major != -1 && availableVersion.major != -1)
            {
                int cp = VersionManager.Compare(charVersion, availableVersion);
                if ((cp < 100)
                    && (-100 < cp))
                {
                    ret = true;
                }
                else
                {
                    ret = false;
                }
            }
            else
            {
                ErrorLogLib er = ErrorLogLib.Instance;
                er.ErrorOutput("キャラファイルバージョンエラー", "バージョン情報が正常に設定されませんでした");
            }

            return ret;
        }

        /// <summary>
        /// コンフィグを読む
        /// </summary>
        /// <param name="entry">エントリ</param>
        /// <return>0: 正常, else: 異常</return>
        private int ReadConfig(ZipArchiveEntry entry)
        {
            int ret = 0;
            int readStatus = _charSettingManager.ReadEntry(entry);
            if (readStatus != 0 || !IsHandleVersion())
            {
                _charSettingManager = new CharSettingManager();
                ret = -1;
            }

            return ret;
        }

        //画像ファイルパスと画像を回帰的に取得し、辞書に登録する
        private void SetImageList(ZipArchive zip)
        {
            try
            {
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    string entryExtension = Path.GetExtension(entry.Name);
                    if (IMAGE_EXTENSION.Contains(entryExtension))
                    {
                        using (Stream imageStream = entry.Open())
                        {
                            Image image = Image.FromStream(imageStream);

                            string fileName = entry.FullName;

                            if (!imageList.ContainsKey(fileName))
                            {
                                imageList.Add(fileName, image);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogLib er = ErrorLogLib.Instance;
                er.ErrorOutput("イメージ読み取りエラー", ex.Message);
            }
        }

        private class CharSettingManager
        {
            //クラス宣言
            private Settings _settings;       //設定クラス(ややこしいが設定用クラスとは別物)

            //読み取り用
            public Settings CharSettings
            {
                get
                {
                    return _settings;
                }
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public CharSettingManager()
            {
                _settings = new Settings();
#if false
            using (StreamWriter sw = new StreamWriter("config.json", false))
            {
                string json = JsonSerializer.Serialize(_settings);
                sw.Write(json);
            }
#endif
            }

            /// <summary>
            /// エントリから設定を読み取る
            /// </summary>
            /// <param name="entry">0: 正常, else: 異常</param>
            /// <returns></returns>
            public int ReadEntry(ZipArchiveEntry entry)
            {
                ErrorLogLib er = ErrorLogLib.Instance;
                int ret = -1;
                string jsonText = string.Empty;

                try
                {
                    using (StreamReader sr = new StreamReader(entry.Open(), Encoding.UTF8))
                    {
                        jsonText = sr.ReadToEnd();
                    }
                    Settings? settings_tmp = JsonSerializer.Deserialize<Settings>(jsonText);
                    if (settings_tmp == null)
                    {
                        er.ErrorOutput("コンフィグ読み取りエラー", "コンフィグデータがNULLです");
                    }
                    else if (string.IsNullOrEmpty(settings_tmp.charSettings.name))
                    {
                        er.ErrorOutput("コンフィグ読み取りエラー", "キャラファイルのコンフィグデータが正しく設定されていない可能性があります");
                    }
                    else
                    {
                        _settings = settings_tmp;
                        ret = 0;
                    }
                }
                catch (Exception ex)
                {
                    er.ErrorOutput("コンフィグ読み取りエラー", ex.Message);
                }

                return ret;
            }

            /// <summary>
            /// 大まかな設定管理クラス
            /// </summary>
            public class Settings
            {
                //変数宣言
                public string version { get; set; }
                public DigColor backgroundColor { get; set; }

                public int pictureTurnOverPeriod { get; set; }

                //クラス宣言
                public FeelingManager feelingSetting { get; set; }
                public CharSettings charSettings { get; set; }

                /// <summary>
                /// コンストラクタ
                /// </summary>
                public Settings()
                {
                    feelingSetting = new FeelingManager();
                    charSettings = new CharSettings();
                    version = string.Empty;
                    backgroundColor = new DigColor(SystemColors.Control);
                    pictureTurnOverPeriod = 200;
                }

                /// <summary>
                /// 色管理用クラス
                /// </summary>
                public class DigColor
                {
                    public byte red
                    {
                        get
                        {
                            return _red;
                        }
                        set
                        {
                            _red = value;
                        }
                    }
                    public byte green
                    {
                        get
                        {
                            return _green;
                        }
                        set
                        {
                            _green = value;
                        }
                    }
                    public byte blue
                    {
                        get
                        {
                            return _blue;
                        }
                        set
                        {
                            _blue = value;
                        }
                    }

                    private byte _red;
                    private byte _green;
                    private byte _blue;

                    /// <summary>
                    /// コンストラクタ
                    /// </summary>
                    /// <param name="color"></param>
                    public DigColor(Color color)
                    {
                        Init();
                        ConvertColor(color);
                    }

                    /// <summary>
                    /// コンストラクタ
                    /// </summary>
                    public DigColor()
                    {
                        Init();
                    }

                    /// <summary>
                    /// 初期化
                    /// </summary>
                    private void Init()
                    {
                        red = (byte)0xff;
                        green = (byte)0xff;
                        blue = (byte)0xff;
                    }

                    /// <summary>
                    /// 色を変換して適用する
                    /// </summary>
                    /// <param name="color"></param>
                    public void ConvertColor(Color color)
                    {
                        red = color.R;
                        green = color.G;
                        blue = color.B;
                    }
                }


                /// <summary>
                /// 感情の管理クラス(テキストとか)
                /// </summary>
                public class FeelingManager
                {
                    //変数関連
                    public Dictionary<string, string> feelingList { get; set; }

                    /// <summary>
                    /// コンストラクタ
                    /// </summary>
                    public FeelingManager()
                    {
                        feelingList = new Dictionary<string, string>()
                        {
                            ["-1.00"] = "エラー",
                            ["-0.49"] = "悪い",
                            ["0.0"] = "普通",
                            ["0.3"] = "良い",
                            ["1.0"] = "最高",
                            ["Infinity"] = "エラー"
                        };
                    }

                    /// <summary>
                    /// 感情のテキストを取得する
                    /// </summary>
                    /// <param name="feeling">感情</param>
                    /// <returns></returns>
                    public string GetFeelingString(double feeling)
                    {
                        string[] keys = feelingList.Keys.ToArray();

                        if (keys.Length <= 0)
                        {
                            return "キーの要素が0以下です";
                        }

                        //0要素目なら未満で比較
                        if (feeling < double.Parse(keys[0], System.Globalization.CultureInfo.InvariantCulture))
                        {
                            return feelingList[keys[0]];
                        }

                        for (int ind = 1; ind < keys.Length; ind++)
                        {
                            if (feeling <= double.Parse(keys[ind], System.Globalization.CultureInfo.InvariantCulture))
                            {
                                return feelingList[keys[ind]];
                            }
                        }

                        return "取得に失敗しました";
                    }
                }

                /// <summary>
                /// 親密度の管理クラス(テキストとか)
                /// </summary>
                public class IntimacyManager
                {
                    //変数関連
                    public Dictionary<string, string> intimacyList { get; set; }

                    /// <summary>
                    /// コンストラクタ
                    /// </summary>
                    public IntimacyManager()
                    {
                        intimacyList = new Dictionary<string, string>()
                        {
                            ["Infinity"] = "設定なし"
                        };
                    }

                    /// <summary>
                    /// 親密度のテキストを取得する
                    /// </summary>
                    /// <param name="intimacy">親密度</param>
                    /// <returns></returns>
                    public string GetIntimacygString(double intimacy)
                    {
                        string[] keys = intimacyList.Keys.ToArray();

                        if (keys.Length <= 0)
                        {
                            return "キーの要素が0以下です";
                        }

                        //最初の要素なら未満で比較
                        if (intimacy < double.Parse(keys[0], System.Globalization.CultureInfo.InvariantCulture))
                        {
                            return intimacyList[keys[0]];
                        }

                        for (int ind = 1; ind < keys.Length; ind++)
                        {
                            if (intimacy <= double.Parse(keys[ind], System.Globalization.CultureInfo.InvariantCulture))
                            {
                                return intimacyList[keys[ind]];
                            }
                        }

                        return "取得に失敗しました";
                    }
                }

                /// <summary>
                /// キャラ設定関連
                /// </summary>
                public class CharSettings
                {
                    public string name { get; set; }
                    public string feelingTag { get; set; }
                    public string leavingImgPath { get; set; }
                    public Feeling[] feelings { get; set; }

                    public class Feeling
                    {
                        public string name { get; set; }
                        public string filePath { get; set; }
                        public int transition { get; set; }

                        /// <summary>
                        /// コンストラクタ
                        /// </summary>
                        public Feeling()
                        {
                            name = string.Empty;
                            filePath = string.Empty;
                            transition = 0;
                        }
                    }

                    /// <summary>
                    /// コンストラクタ
                    /// </summary>
                    public CharSettings()
                    {
                        name = string.Empty;
                        feelingTag = string.Empty;
                        feelings = new Feeling[0];
                        leavingImgPath = string.Empty;
                    }
                }
            }
        }
    }
}