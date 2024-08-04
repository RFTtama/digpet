using System.IO.Compression;
using System.Text.Json;

namespace digpet
{
    internal class CharSettingManager
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
        public void ReadEntry(ZipArchiveEntry entry)
        {
            string jsonText = string.Empty;

            try
            {
                using (StreamReader sr = new StreamReader(entry.Open()))
                {
                    jsonText = sr.ReadToEnd();
                }
                Settings? settings_tmp = JsonSerializer.Deserialize<Settings>(jsonText);
                if (settings_tmp == null)
                {
                    LogManager.LogOutput("キャラファイルのコンフィグデータ読み込みに失敗しました");
                    ErrorLog.ErrorOutput("コンフィグ読み取りエラー", "コンフィグデータがNULLです", true);
                }
                else if (string.IsNullOrEmpty(settings_tmp.charSettings.name))
                {
                    LogManager.LogOutput("設定ファイルが正しく読み取られませんでした");
                    ErrorLog.ErrorOutput("コンフィグ読み取りエラー", "キャラファイルのコンフィグデータが正しく設定されていない可能性があります", true);
                }
                else
                {
                    LogManager.LogOutput("キャラファイルのコンフィグデータが正常に読み込まれました");
                    _settings = settings_tmp;
                }
            }
            catch (Exception ex)
            {
                LogManager.LogOutput("キャラファイルのコンフィグデータ読み込みに失敗しました");
                ErrorLog.ErrorOutput("コンフィグ読み取りエラー", ex.Message, true);
            }
        }

        /// <summary>
        /// 大まかな設定管理クラス
        /// </summary>
        public class Settings
        {
            //変数宣言
            public string version { get; set; }

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
                public Intimacy[] intimacies { get; set; }

                /// <summary>
                /// コンストラクタ
                /// </summary>
                public CharSettings()
                {
                    name = string.Empty;
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
