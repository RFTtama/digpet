using System.Text.Json;

namespace digpet
{
    /// <summary>
    /// キーがdouble型の辞書管理用クラス
    /// </summary>
    internal class DoubleJsonManager
    {
        /// <summary>
        /// 辞書
        /// </summary>
        public Dictionary<string, double> dict = new Dictionary<string, double>();

        private string password = string.Empty;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DoubleJsonManager(string password)
        {
            Init();
            this.password = password;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void Init()
        {
            if (dict != null)
            {
                dict.Clear();
            }
            else
            {
                dict = new Dictionary<string, double>();
            }

            password = string.Empty;
        }

        /// <summary>
        /// JSONファイルから辞書を読み取る
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <param name="initValue">初期値</param>
        public void ReadJsonFile(string path, (string, double) initValue)
        {
            if (!File.Exists(path))
            {
                InitJsonFile(path, initValue);
                return;
            }

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
#if true
                    string planeText = Crypter.DecryptString(sr.ReadToEnd(), password);
#else
                    string planeText = sr.ReadToEnd();
#endif
                    dict = JsonSerializer.Deserialize<Dictionary<string, double>>(planeText) ?? new Dictionary<string, double>();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorOutput("Jsonファイル読み取りエラー", ex.Message, true);
            }
        }

        /// <summary>
        /// 辞書をJSONに書き込む
        /// </summary>
        /// <param name="path">ファイルパス</param>
        public void WriteJsonFile(string path)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(path, false))
                {
                    string jsonText = JsonSerializer.Serialize(dict);
#if true
                    sw.Write(Crypter.EncryptString(jsonText, password));
#else
                    sw.Write(jsonText);
#endif
                }
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorOutput("Jsonファイル書き込みエラー", ex.Message, true);
            }
        }

        /// <summary>
        /// JSONファイルの中身と辞書を空にする
        /// </summary>
        /// <param name="path">ファイルパス</param>
        private void InitJsonFile(string path, (string, double) initValue)
        {
            dict.Add(initValue.Item1, initValue.Item2);

            try
            {
                using (StreamWriter sw = new StreamWriter(path, false))
                {
                    string jsonText = JsonSerializer.Serialize(dict);
#if true
                    sw.Write(Crypter.EncryptString(jsonText, password));
#else
                    sw.Write(jsonText);
#endif
                }
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorOutput("Jsonファイル初期化エラー", ex.Message, true);
            }
        }
    }
}
