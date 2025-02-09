using System.Collections.Immutable;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using digpet.Modules;

namespace digpet.Managers
{
    internal class CharZipFileManager
    {
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

        public Image? GetCharImage(string intimacy, string feeling)
        {
            CharSettingManager.Settings.CharSettings.Intimacy? target = null;

            //ワイルドカード<ALL>を走査する
            target = SearchWildcardIntimacy();

            //ワイルドカードなし
            if (target == null)
            {
                target = SearchIntimacy(intimacy);
            }

            //対象intimacyなし
            if (target == null)
            {
                return null;
            }

            string imageName = GetImageNameFromFeeling(target, feeling);
            return GetImageFromImageName(imageName);
        }

        /// <summary>
        /// ワイルドカードのintimacyを走査する
        /// </summary>
        /// <returns>intimacy なかった場合はnull</returns>
        private CharSettingManager.Settings.CharSettings.Intimacy? SearchWildcardIntimacy()
        {
            CharSettingManager.Settings.CharSettings.Intimacy? ret = null;

            foreach (CharSettingManager.Settings.CharSettings.Intimacy _intimacy in _charSettingManager.CharSettings.charSettings.intimacies)
            {
                if (_intimacy.name == "<ALL>")
                {
                    ret = _intimacy;
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// 引数と同名のintimacyを返却する
        /// </summary>
        /// <param name="intimacyString">検索するintimacy</param>
        /// <returns>あった場合同名のintimacy なかった場合null</returns>
        private CharSettingManager.Settings.CharSettings.Intimacy? SearchIntimacy(string intimacyString)
        {
            CharSettingManager.Settings.CharSettings.Intimacy? ret = null;

            foreach (CharSettingManager.Settings.CharSettings.Intimacy _intimacy in _charSettingManager.CharSettings.charSettings.intimacies)
            {
                if (_intimacy.name == intimacyString)
                {
                    ret = _intimacy;
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// feelingとintimacyから対象の画像名を取り出す
        /// transitionの割合で計算される
        /// </summary>
        /// <param name="target">ターゲットのintimacy</param>
        /// <param name="feelingString">feelingのstring</param>
        /// <returns>画像名</returns>
        private string GetImageNameFromFeeling(CharSettingManager.Settings.CharSettings.Intimacy target, string feelingString)
        {
            string ret = string.Empty;
            int tranSum = 0;
            Dictionary<string, int> transDict = new Dictionary<string, int>();

            foreach (CharSettingManager.Settings.CharSettings.Intimacy.Feeling feeling in target.feelings)
            {
                if (feeling.name == feelingString && !transDict.ContainsKey(feelingString))
                {
                    transDict.Add(feeling.filePath, feeling.transition);
                    tranSum += feeling.transition;
                }
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
        /// Intimacyのタグを取得する
        /// </summary>
        /// <returns>Intimacyのタグ(string)</returns>
        public string GetIntimacyTag()
        {
            string? ret = string.Empty;

            ret = _charSettingManager.CharSettings.charSettings.intimacyTag;

            if (string.IsNullOrWhiteSpace(ret))
            {
                ret = string.Empty;
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
        /// 親密度のテキストを取得する
        /// </summary>
        /// <param name="intimacy">親密度</param>
        /// <returns></returns>
        public string GetIntimacyString(double intimacy)
        {
            return _charSettingManager.CharSettings.intimacySetting.GetIntimacygString(intimacy);
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
            if (!File.Exists(path))
            {
                ErrorLog.ErrorOutput("コンフィグファイル確認エラー", "キャラデータが見つかりません");
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
                        ErrorLog.ErrorOutput("コンフィグファイル読み取りエラー", "キャラデータにコンフィグファイルが含まれていません");
                        return;
                    }

                    if (ReadConfig(entry) == 0)
                    {
                        SetImageList(zip);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorOutput("コンフィグファイル読み取りエラー", ex.Message);
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
                if (charVersion.Compare(availableVersion) <= 0)
                {
                    ret = true;
                }
                else
                {
                    ErrorLog.ErrorOutput("キャラファイルバージョンエラー", "キャラファイルのバージョンがサポートされている最大のバージョン("
                        + SettingManager.PrivateSettings.CHAR_FORMAT_VERSION + ")より大きいです");
                }
            }
            else
            {
                ErrorLog.ErrorOutput("キャラファイルバージョンエラー", "バージョン情報が正常に設定されませんでした");
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
                ErrorLog.ErrorOutput("イメージ読み取りエラー", ex.Message);
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
                        LogManager.LogOutput("キャラファイルのコンフィグデータ読み込みに失敗しました");
                        ErrorLog.ErrorOutput("コンフィグ読み取りエラー", "コンフィグデータがNULLです");
                    }
                    else if (string.IsNullOrEmpty(settings_tmp.charSettings.name))
                    {
                        LogManager.LogOutput("設定ファイルが正しく読み取られませんでした");
                        ErrorLog.ErrorOutput("コンフィグ読み取りエラー", "キャラファイルのコンフィグデータが正しく設定されていない可能性があります");
                    }
                    else
                    {
                        LogManager.LogOutput("キャラファイルのコンフィグデータが正常に読み込まれました");
                        _settings = settings_tmp;
                        ret = 0;
                    }
                }
                catch (Exception ex)
                {
                    LogManager.LogOutput("キャラファイルのコンフィグデータ読み込みに失敗しました");
                    ErrorLog.ErrorOutput("コンフィグ読み取りエラー", ex.Message);
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
                public IntimacyManager intimacySetting { get; set; }
                public CharSettings charSettings { get; set; }

                /// <summary>
                /// コンストラクタ
                /// </summary>
                public Settings()
                {
                    feelingSetting = new FeelingManager();
                    intimacySetting = new IntimacyManager();
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
                    public string intimacyTag { get; set; }
                    public string feelingTag { get; set; }
                    public Intimacy[] intimacies { get; set; }

                    /// <summary>
                    /// コンストラクタ
                    /// </summary>
                    public CharSettings()
                    {
                        name = string.Empty;
                        intimacyTag = string.Empty;
                        feelingTag = string.Empty;
#if false
                    intimacies = [new Intimacy()
                    {
                        name = string.Empty,
                        feelings = [new Intimacy.Feeling() { name = string.Empty, filePath = string.Empty, transition = -1 }]
                    }];
#else
                        intimacies = Array.Empty<Intimacy>();
#endif
                    }

                    public class Intimacy
                    {
                        public string name { get; set; }
                        public Feeling[] feelings { get; set; }

                        /// <summary>
                        /// コンストラクタ
                        /// </summary>
                        public Intimacy()
                        {
                            name = string.Empty;
                            feelings = Array.Empty<Feeling>();
                        }

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
                    }
                }
            }
        }
    }
}