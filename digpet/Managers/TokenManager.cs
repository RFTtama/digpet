using digpet.Modules;
using ScottPlot.Plottables;
using ScottPlot;
using System.Diagnostics;
using System.Text.Json;

namespace digpet.Managers
{
    public class TokenManager
    {
        //クラス宣言
        private CompressTokenAvgManager avgManager;                                         //token平均値管理用クラス

        //固定値関連の宣言
        private const string AVG_MANAGER_ID = "ID_COMPRESS";                                //token平均値管理用クラスのID
        private const double HANDOVER_PERCENT = 0.99;

        /// <summary>
        /// 獲得しているtokenの合計
        /// </summary>
        public double Tokens
        {
            get { return avgManager.GetTokens(); }
        }

        /// <summary>
        /// 現在の感情値
        /// </summary>
        public double Feeling
        {
            get
            {
                return ((avgManager.JoyFeeling + avgManager.HappyToken) - (avgManager.SadFeeling + avgManager.AngryFeeling));
            }
        }

        public double JoyFeeling
        {
            get
            {
                return avgManager.JoyFeeling;
            }
        }

        public double HappyFeeling
        {
            get
            {
                return avgManager.HappyToken;
            }
        }

        public double SadFeeling
        {
            get
            {
                return avgManager.SadToken;
            }
        }

        public double AngryFeeling
        {
            get
            {
                return avgManager.AngryToken;
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
                CheckTokenCompressArray();
                LogLib.LogOutput("token計算用ファイルが読み込まれました");
            }
            catch (Exception ex)
            {
                LogLib.LogOutput("token計算用ファイルの読み込みに失敗しました");
                ErrorLogLib.ErrorOutput("token計算用ファイル読み取りエラー", ex.Message);
            }
        }

        /// <summary>
        /// トークン圧縮配列の整合性を確認
        /// </summary>
        private void CheckTokenCompressArray()
        {
            double[] arr = avgManager.TokenCompressArray;
            if (arr.Length < CompressTokenAvgManager.TOKEN_COMPRESS_ARRAY_LENGTH)
            {
                List<double> list = arr.ToList();
                for (int ind = 0; ind < CompressTokenAvgManager.TOKEN_COMPRESS_ARRAY_LENGTH - arr.Length; ind++)
                {
                    list.Insert(0, 0);
                }
                arr = list.ToArray();
            }

            avgManager.TokenCompressArray = arr;
        }

        /// <summary>
        /// トークンのプロットを保存する
        /// </summary>
        /// <param name="picName">保存する画像名</param>
        /// <returns>true: 正常, false: 異常</returns>
        private void SaveTokenPlot(string picName)
        {
            Plot plot = new Plot();
            int[] x = new int[avgManager.TokenCompressArray.Length];

            for (int i = 0; i < avgManager.TokenCompressArray.Length; i++)
            {
                x[i] = i;
            }

            Signal s1 = plot.Add.Signal(avgManager.TokenCompressArray);

            s1.LegendText = "Token Compress Array";

            plot.XLabel("Days");
            plot.YLabel("Tokens Amount");

            plot.Title("Token Compress Array Token Amount Transition");

            plot.GetImage(1024, 512).Save(picName);

            LogLib.LogOutput("トークンプロットを保存しました");
        }

        /// <summary>
        /// コンストラクタ
        /// FLOPSの計算も行う
        /// </summary>
        public TokenManager()
        {
            avgManager = new CompressTokenAvgManager(AVG_MANAGER_ID);
        }

        /// <summary>
        /// トークンを追加
        /// </summary>
        /// <param name="minToken">時間毎のトークン(未計算)</param>
        public void AddTokens(double minToken)
        {
            LogLib.LogOutput("DailyTokenに" + minToken.ToString() + "が足されました。");

            double token = (Tokens * HANDOVER_PERCENT) + minToken;
#if false
            Debug.Print("Token: " + Tokens.ToString());
#endif

            avgManager.Add(token);
            if (SettingManager.PublicSettings.SaveTokenPlot)
            {
                SaveTokenPlot(SettingManager.PrivateSettings.PLOT_PATH);
            }
        }

        /// <summary>
        /// TokenAvgManagerのインタフェース
        /// </summary>
        private interface TokenAvgManagerInterface
        {
            public string Id { get; }
            public void Add(double token);
            public double GetTokens();
        }

        /// <summary>
        /// tokenの平均値(基準値)計算用クラス
        /// 管理はこのクラスに任せる
        /// </summary>
        private class CompressTokenAvgManager : TokenAvgManagerInterface
        {
            public string Id { get; set; }

            public const int TOKEN_COMPRESS_ARRAY_LENGTH = 10080;       //token圧縮配列のサイズ
            private const double SAD_MAGN = 100.0;                      //哀token計算用の閾値
            private const double SAD_TOKEN_MAX = 10000;                 //哀tokenの最大値
            private const double ANGRY_TOKEN_MAX = 500;                 //怒tokenの最大値
            private const double HAPPY_TOKEN_MAX = 120;                 //喜tokenの最大値

            public double TokenMax { get; set; }                        //tokenの最大値

            public double[] TokenCompressArray { get; set; }            //token圧縮配列

            public int SadTokenBoost {  get; set; }

            public double SadToken {  get; set; }                       //哀token
            public double HappyToken { get; set; }                      //喜token
            public double AngryToken { get; set; }                      //怒token

            private bool IsGeneralZero
            {
                get
                {
                    return GetTokens() <= (TokenCompressArray[0] + SAD_MAGN);
                }
            }


            /// <summary>
            /// 楽の感情
            /// </summary>
            public double JoyFeeling
            {
                get
                {
                    double avg = GetThreshold(SettingManager.PublicSettings.TokenCompressArrayElementIndex);
                    if (avg <= 0) avg = 1;
                    double diff = GetTokens() - avg;
#if false
                Debug.Print("Diff: " + diff.ToString());
#endif
                    double ret = (double)diff / avg;
                    if (ret < 0) ret = 0;
#if false
                Debug.Print("Ret: " + ret.ToString());
#endif
                    return ret;
                }
            }

            /// <summary>
            /// 哀の感情
            /// </summary>
            public double SadFeeling
            {
                get
                {
                    return (SadToken / SAD_TOKEN_MAX);
                }
            }

            /// <summary>
            /// 怒の感情
            /// </summary>
            public double AngryFeeling
            {
                get
                {
                    return (AngryToken / ANGRY_TOKEN_MAX);
                }
            }


            public CompressTokenAvgManager(string id)
            {
                Id = id;
                TokenMax = 0.0;
                SadToken = 0.0;
                HappyToken = 0.0;
                AngryToken = 0.0;
                SadTokenBoost = 0;
                TokenCompressArray = new double[TOKEN_COMPRESS_ARRAY_LENGTH];
                for (int i = 0; i < TOKEN_COMPRESS_ARRAY_LENGTH; i++)
                {
                    TokenCompressArray[i] = 0;
                }
            }

            /// <summary>
            /// 平均値を計算する
            /// </summary>
            /// <param name="token">token値</param>
            public void Add(double token)
            {
                if (token > TokenMax)
                {
                    TokenMax = token;
                }

                CompressDimention();

                CalcJoyToken(token);
                CalcSadToken(token);
                CalcAngryToken(token);
                CalcHappyToken(token);
            }

            /// <summary>
            /// 楽のトークン計算
            /// </summary>
            /// <param name="token"></param>
            private void CalcJoyToken(double token)
            {
                TokenCompressArray[TOKEN_COMPRESS_ARRAY_LENGTH - 1] = token;
            }

            /// <summary>
            /// 哀のトークン計算
            /// </summary>
            private void CalcSadToken(double token)
            {
                if (IsGeneralZero)
                {
                    SadTokenBoost = 0;
                    SadToken += 3.0;
                    if (SadToken > SAD_TOKEN_MAX)
                    {
                        SadToken = SAD_TOKEN_MAX;
                    }
                }
                else if (AngryToken <= 0.0)
                {
                    SadTokenBoost++;
                    SadToken = SadToken - (100.0 * SadTokenBoost);

                    if (SadToken < 0.0)
                    {
                        SadToken = 0.0;
                    }
                }
            }

            /// <summary>
            /// 怒のトークン計算
            /// </summary>
            /// <param name="token"></param>
            private void CalcAngryToken(double token)
            {
                if (!IsGeneralZero)
                {
                    AngryToken -= 10.0;
                    if (AngryToken < 0.0)
                    {
                        AngryToken = 0.0;
                    }
                }
                else if ((SadToken / SAD_TOKEN_MAX) == 1.0)
                {
                    AngryToken += 1.0;
                    if (AngryToken > ANGRY_TOKEN_MAX)
                    {
                        AngryToken = ANGRY_TOKEN_MAX;
                    }
                }
            }

            /// <summary>
            /// 喜のトークン計算
            /// </summary>
            /// <param name="token"></param>
            private void CalcHappyToken(double token)
            {
                if (JoyFeeling >= 1.0)
                {
                    HappyToken += 1.0;
                    if (HappyToken > HAPPY_TOKEN_MAX)
                    {
                        HappyToken = HAPPY_TOKEN_MAX;
                    }
                }
                else if (IsGeneralZero)
                {
                    HappyToken -= 2;
                    if (HappyToken < 0.0)
                    {
                        HappyToken = 0.0;
                    }
                }
            }

            /// <summary>
            /// 感情判定の閾値を取得
            /// 0～9999まである 各要素はind(min)経過時に影響を受け始める
            /// </summary>
            /// <returns></returns>
            private double GetThreshold(int ind)
            {
                int index = ind;
                if (index < 0)
                {
                    index = 0;
                }
                if (index >= TOKEN_COMPRESS_ARRAY_LENGTH)
                {
                    index = TOKEN_COMPRESS_ARRAY_LENGTH - 1;
                }
                double threshold = ((TokenMax + TokenCompressArray[((TOKEN_COMPRESS_ARRAY_LENGTH - 1) - ind)]) / 2.0);
                return threshold;
            }

            /// <summary>
            /// token圧縮配列の要素を圧縮する
            /// </summary>
            private void CompressDimention()
            {
                for (int i = 0; i < TOKEN_COMPRESS_ARRAY_LENGTH - 1; i++)
                {
                    TokenCompressArray[i] = (TokenCompressArray[i] + TokenCompressArray[i + 1]) / 2.0;
                }
            }

            /// <summary>
            /// 現在のtoken値を返却する
            /// </summary>
            /// <returns>現在のtoken値</returns>
            public double GetTokens()
            {
                return TokenCompressArray[(TOKEN_COMPRESS_ARRAY_LENGTH - 1)];
            }
        }
    }
}
