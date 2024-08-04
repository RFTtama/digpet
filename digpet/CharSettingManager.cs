using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;

namespace digpet
{
    internal class CharSettingManager
    {
        //クラス宣言
        public Settings settings;       //設定クラス(ややこしいが設定用クラスとは別物)

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CharSettingManager()
        {
            settings = new Settings();
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
                settings = JsonSerializer.Deserialize<Settings>(jsonText) ?? new Settings();
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorOutput("コンフィグ読み取りエラー", ex.Message, true);
            }
        }

        /// <summary>
        /// 大まかな設定管理クラス
        /// </summary>
        public class Settings
        {
            //クラス宣言
            public FeelingManager feelingSetting = new FeelingManager();
            public IntimacyManager intimacySetting = new IntimacyManager();
            public CharSettings charSettings = new CharSettings();


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
                    intimacies = Array.Empty<Intimacy>();
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
