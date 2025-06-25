using digpet.Modules;
using digpet.TaskTimerClass;
using ScottPlot;
using ScottPlot.Plottables;
using System.Text.Json;

namespace digpet.Managers
{
    public class TokenManager
    {
        //クラス宣言
        private CompressTokenAvgManager avgManager;                                         //token平均値管理用クラス

        //固定値関連の宣言
        private const string AVG_MANAGER_ID = "ID_DCAbMA";                                  //token平均値管理用クラスのID
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
                return ((avgManager.JoyFeeling + avgManager.HappyFeeling) / 2.0) - ((avgManager.SadFeeling + avgManager.AngryFeeling) / 2.0);
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
                return avgManager.HappyFeeling;
            }
        }

        public double SadFeeling
        {
            get
            {
                return avgManager.SadFeeling;
            }
        }

        public double AngryFeeling
        {
            get
            {
                return avgManager.AngryFeeling;
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
            }
            catch (Exception ex)
            {
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
                avgManager = JsonSerializer.Deserialize<CompressTokenAvgManager>(json) ?? new CompressTokenAvgManager();
                CheckBankId();
                CheckTokenCompressArray();
            }
            catch (Exception ex)
            {
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
        /// BANK IDのチェック処理
        /// </summary>
        private void CheckBankId()
        {
            if (avgManager.Id != TokenManager.AVG_MANAGER_ID)
            {
                avgManager = new CompressTokenAvgManager();
                avgManager.Id = TokenManager.AVG_MANAGER_ID;
            }
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
        }

        /// <summary>
        /// コンストラクタ
        /// FLOPSの計算も行う
        /// </summary>
        public TokenManager()
        {
            avgManager = new CompressTokenAvgManager();
            avgManager.Id = TokenManager.AVG_MANAGER_ID;
        }

        /// <summary>
        /// トークンを追加
        /// </summary>
        /// <param name="minToken">時間毎のトークン(未計算)</param>
        public void AddTokens(double minToken)
        {
            LogTimer.SaveLog("minToken", minToken.ToString());

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

            public const int TOKEN_COMPRESS_ARRAY_LENGTH = 1;           //token圧縮配列のサイズ
            public const int SECOND_DIMENTION_SIZE = 10000;             //2次元目のサイズ
            private const double SAD_MAGN = 100.0;                      //哀token計算用の閾値
            private const double SAD_TOKEN_MAX = 10000;                 //哀tokenの最大値
            private const double ANGRY_TOKEN_MAX = 500;                 //怒tokenの最大値
            private const double HAPPY_TOKEN_MAX = 120;                 //喜tokenの最大値
            private const double MAX_TOKEN_DEC_MAGN = 0.99999;

            public double TokenMax { get; set; }                        //tokenの最大値

            public double[] TokenCompressArray { get; set; }            //token圧縮配列

            public double[][] CascadeArray { get; set; }                //カスケード配列

            public int SadTokenBoost { get; set; }

            public double JoyToken { get; set; }                        //楽token
            public double SadToken { get; set; }                        //哀token
            public double HappyToken { get; set; }                      //喜token
            public double AngryToken { get; set; }                      //怒token

            public int SecondDimIndex { get; set; }

            private bool IsGeneralZero
            {
                get
                {
                    return (GetTokens() <= (CalcSecondDimAvg(0) + SAD_MAGN));
                }
            }


            /// <summary>
            /// 楽の感情
            /// </summary>
            public double JoyFeeling
            {
                get
                {
                    double avg = GetThreshold();
                    if (avg <= 0) avg = 1;
                    double diff = GetTokens() - avg;
#if false
                Debug.Print("Diff: " + diff.ToString());
#endif
                    double ret = (double)diff / avg;
                    if (ret < 0.0) ret = 0.0;
                    if (ret > 1.0) ret = 1.0;
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

            /// <summary>
            /// 喜の感情
            /// </summary>
            public double HappyFeeling
            {
                get
                {
                    return (HappyToken / HAPPY_TOKEN_MAX);
                }
            }


            public CompressTokenAvgManager()
            {
                Id = string.Empty;
                TokenMax = 0.0;
                SadToken = 0.0;
                HappyToken = 0.0;
                AngryToken = 0.0;
                SadTokenBoost = 0;
                SecondDimIndex = 0;

                TokenCompressArray = new double[TOKEN_COMPRESS_ARRAY_LENGTH];
                CascadeArray = new double[TOKEN_COMPRESS_ARRAY_LENGTH][];

                ClearArrays();
            }

            /// <summary>
            /// 配列の初期化
            /// </summary>
            private void ClearArrays()
            {
                for (int i = 0; i < TOKEN_COMPRESS_ARRAY_LENGTH; i++)
                {
                    TokenCompressArray[i] = 0;
                }

                for (int i = 0; i < TOKEN_COMPRESS_ARRAY_LENGTH; i++)
                {
                    CascadeArray[i] = new double[SECOND_DIMENTION_SIZE];

                    for (int j = 0; j < SECOND_DIMENTION_SIZE; j++)
                    {
                        CascadeArray[i][j] = 0;
                    }
                }
            }

            /// <summary>
            /// 平均値を計算する
            /// </summary>
            /// <param name="token">token値</param>
            public void Add(double token)
            {
                TokenMax = TokenMax * MAX_TOKEN_DEC_MAGN;

                if (token > TokenMax)
                {
                    TokenMax = token;
                }

                CompressDimention();

                CalcJoyToken(token);
                CalcSadToken(token);
                CalcAngryToken(token);
                CalcHappyToken(token);

                SecondDimIndex++;
                SecondDimIndex = SecondDimIndex % SECOND_DIMENTION_SIZE;
            }

            /// <summary>
            /// 楽のトークン計算
            /// </summary>
            /// <param name="token"></param>
            private void CalcJoyToken(double token)
            {
                JoyToken = token;
                AddToSecondDim(TOKEN_COMPRESS_ARRAY_LENGTH - 1, token);
                TokenCompressArray[TOKEN_COMPRESS_ARRAY_LENGTH - 1] = CalcSecondDimAvg(TOKEN_COMPRESS_ARRAY_LENGTH - 1);
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
                    if (SadToken > 0.0)
                    {
                        SadTokenBoost++;
                    }
                    else
                    {
                        SadTokenBoost = 0;
                    }

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
            private double GetThreshold()
            {
                double threshold = ((TokenMax + TokenCompressArray[TOKEN_COMPRESS_ARRAY_LENGTH - 1]) / 2.0);
                return threshold;
            }

            /// <summary>
            /// token圧縮配列の要素を圧縮する
            /// </summary>
            private void CompressDimention()
            {
                for (int i = 0; i < TOKEN_COMPRESS_ARRAY_LENGTH - 1; i++)
                {
                    AddToSecondDim(i, ((CalcSecondDimAvg(i) + CalcSecondDimAvg(i + 1)) / 2.0));
                    TokenCompressArray[i] = CalcSecondDimAvg(i);
                }
            }

            /// <summary>
            /// 2次元要素に値を代入する
            /// </summary>
            /// <param name="index"></param>
            /// <param name="val"></param>
            private void AddToSecondDim(int index, double val)
            {
                if ((index < 0) || (index >= TOKEN_COMPRESS_ARRAY_LENGTH))
                {
                    return;
                }

                if ((SecondDimIndex < 0) || (SecondDimIndex >= SECOND_DIMENTION_SIZE))
                {
                    return;
                }

                CascadeArray[index][SecondDimIndex] = val;
            }

            /// <summary>
            /// 指定したインデックスの2次元要素の平均値を求める
            /// </summary>
            /// <param name="index">インデックス</param>
            /// <returns>平均値</returns>
            private double CalcSecondDimAvg(int index)
            {
                if ((index < 0) || (index >= TOKEN_COMPRESS_ARRAY_LENGTH))
                {
                    return 0.0;
                }

                double sum = 0.0;

                for (int i = 0; i < SECOND_DIMENTION_SIZE; i++)
                {
                    sum += CascadeArray[index][i];
                }

                return (sum / (double)SECOND_DIMENTION_SIZE);
            }

            /// <summary>
            /// 現在のtoken値を返却する
            /// </summary>
            /// <returns>現在のtoken値</returns>
            public double GetTokens()
            {
                return JoyToken;
            }
        }
    }
}
