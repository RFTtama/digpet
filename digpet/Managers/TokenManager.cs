using digpet.Modules;
using System.Diagnostics;
using System.Text.Json;

namespace digpet.Managers
{
    public class TokenManager
    {
        //クラス宣言
        private CompressTokenAvgManager avgManager;                                         //token平均値管理用クラス

        //固定値関連の宣言
        private const double HANDOVER_PERCENT = 0.99;                                       //tokenの引継ぎ割合
        private const string AVG_MANAGER_ID = "ID_COMPRESS";                                //token平均値管理用クラスのID

        //変数関連の宣言
        private long _tokens;                                                               //token

        /// <summary>
        /// 獲得しているtokenの合計
        /// </summary>
        public long Tokens
        {
            get { return _tokens; }
        }

        /// <summary>
        /// 現在の感情値
        /// </summary>
        public double Feeling
        {
            get
            {
                long avg = avgManager.GetThreshold(SettingManager.PublicSettings.TokenCompressArrayElementIndex);
                if (avg <= 0) avg = 1;
                long diff = Tokens - avg;
                Debug.Print("Diff: " + diff.ToString());
                double ret = (double)diff / avg;
                Debug.Print("Ret: " + ret.ToString());
                return ret;
            }
        }

        /// <summary>
        /// token計算用クラスをファイルに書き出す
        /// </summary>
        /// <param name="path">パス</param>
        public void Write(string path)
        {
            WriteToFile(path);
        }

        /// <summary>
        /// token計算用クラスをファイルに書き出す
        /// </summary>
        /// <param name="path">パス</param>
        private void WriteToFile(string path)
        {
            try
            {
                string json = JsonSerializer.Serialize(avgManager, SettingManager.JSON_OPTIONS);
                using (StreamWriter sw = new StreamWriter(path, false))
                {
                    sw.Write(json);
                }
                LogLib.LogOutput("token計算用ファイルが正常に書き込まれました");
            }
            catch (Exception ex)
            {
                LogLib.LogOutput("token計算用ファイルの書き込み失敗");
                ErrorLogLib.ErrorOutput("token計算用ファイル初期化エラー", ex.Message);
            }
        }

        /// <summary>
        /// ファイルからtoken計算用クラスを読みだす
        /// </summary>
        /// <param name="path">パス</param>
        public void Read(string path)
        {
            if (File.Exists(path))
            {
                ReadFromFile(path);
            }
            else
            {
                WriteToFile(path);
            }
        }

        /// <summary>
        /// ファイルからtoken計算用クラスを読みだす
        /// </summary>
        /// <param name="path">パス</param>
        private void ReadFromFile(string path)
        {
            try
            {
                string json = string.Empty;
                using (StreamReader sr = new StreamReader(path))
                {
                    json = sr.ReadToEnd();
                }
                avgManager = JsonSerializer.Deserialize<CompressTokenAvgManager>(json) ?? new CompressTokenAvgManager(AVG_MANAGER_ID);
                _tokens = avgManager.GetTokens();
                LogLib.LogOutput("token計算用ファイルが読み込まれました");
            }
            catch (Exception ex)
            {
                LogLib.LogOutput("token計算用ファイルの読み込みに失敗しました");
                ErrorLogLib.ErrorOutput("token計算用ファイル読み取りエラー", ex.Message);
            }
        }

        /// <summary>
        /// コンストラクタ
        /// FLOPSの計算も行う
        /// </summary>
        public TokenManager()
        {
            _tokens = 0;
            avgManager = new CompressTokenAvgManager(AVG_MANAGER_ID);
        }

        /// <summary>
        /// トークンを追加
        /// </summary>
        /// <param name="minToken">時間毎のトークン(未計算)</param>
        public void AddTokens(double minToken)
        {
            long appendToken = (long)(minToken);
            LogLib.LogOutput("DailyTokenに" + appendToken.ToString() + "が足されました。");
            _tokens = (long)(Tokens * HANDOVER_PERCENT) + appendToken;
            Debug.Print("Token: " + Tokens.ToString());

            avgManager.Add(Tokens);
        }

        /// <summary>
        /// TokenAvgManagerのインタフェース
        /// </summary>
        private interface TokenAvgManagerInterface
        {
            public string Id { get; }
            public long GetThreshold(int ind);
            public void Add(long token);
            public long GetTokens();
        }

        /// <summary>
        /// tokenの平均値(基準値)計算用クラス
        /// 管理はこのクラスに任せる
        /// </summary>
        private class CompressTokenAvgManager : TokenAvgManagerInterface
        {
            public string Id { get; set; }

            private const int TOKEN_COMPRESS_ARRAY_LENGTH = 10000;      //token圧縮配列のサイズ

            public long TokenMax { get; set; }                          //tokenの最大値

            public long[] TokenCompressArray { get; set; }             //token圧縮配列


            public CompressTokenAvgManager(string id)
            {
                Id = id;
                TokenMax = 0;
                TokenCompressArray = new long[TOKEN_COMPRESS_ARRAY_LENGTH];
                for (int i = 0; i < TOKEN_COMPRESS_ARRAY_LENGTH; i++)
                {
                    TokenCompressArray[i] = 0;
                }
            }

            /// <summary>
            /// 平均値を計算する
            /// </summary>
            /// <param name="token">token値</param>
            public void Add(long token)
            {
                if (token > TokenMax)
                {
                    TokenMax = token;
                }

                CompressDimention();
                TokenCompressArray[TOKEN_COMPRESS_ARRAY_LENGTH - 1] = token;
            }

            /// <summary>
            /// 感情判定の閾値を取得
            /// 0～9999まである 各要素はind(min)経過時に影響を受け始める
            /// </summary>
            /// <returns></returns>
            public long GetThreshold(int ind)
            {
                long threshold = ((TokenMax + TokenCompressArray[((TOKEN_COMPRESS_ARRAY_LENGTH - 1) - ind)]) / 2);
                return threshold;
            }

            /// <summary>
            /// token圧縮配列の要素を圧縮する
            /// </summary>
            private void CompressDimention()
            {
                for (int i = 0; i < TOKEN_COMPRESS_ARRAY_LENGTH - 1; i++)
                {
                    TokenCompressArray[i] = TokenCompressArray[i] + TokenCompressArray[i + 1];
                }
            }

            /// <summary>
            /// 現在のtoken値を返却する
            /// </summary>
            /// <returns>現在のtoken値</returns>
            public long GetTokens()
            {
                return TokenCompressArray[(TOKEN_COMPRESS_ARRAY_LENGTH - 1)];
            }
        }
    }
}
