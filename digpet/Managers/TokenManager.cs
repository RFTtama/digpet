﻿using digpet.Modules;

namespace digpet.Managers
{
    internal class TokenManager
    {
        //クラス関連の宣言
        private DoubleJsonManager djm = new DoubleJsonManager(TOKEN_PASS);                  //(string, double)のJSONファイルの管理クラス
        private FlopsManager flops = new FlopsManager(1000);                                //FLOPS計算クラス

        //固定値関連の宣言
        private const double TOKEN_CALC_WEIGHT = 1000.0 / (60.0 * 24.0 * 100.0);            //トークンのウェイト
        private const double HANDOVER_PERCENT = 0.5;                                        //感情トークンの引継ぎ割合
        private const double HANDOVER_PENALTY = 0.95;                                       //複数日跨いだ際の累計トークンペナルティ割合
        private const string TOKEN_PASS = "qK6Nvgjfn8aa6oy2tDtYw17Lz0zePJMnXdiAnfXO";       //トークンの暗号化キー(このキーでテキストが複合できるのはないしょ)

        //変数関連の宣言
        private double _dailyTokens;                                                        //その日のトークン
        private int _resetHour;                                                             //トークンをリセットする時刻

        //リスト関連の宣言
        private List<double> _emotionTokens = new List<double>();                           //日別感情トークンの獲得量リスト
        private List<double> _totalTokens = new List<double>();                             //日別累計トークンの獲得量リスト

        /// <summary>
        /// 今日の累計トークン
        /// </summary>
        public double DailyTokens
        {
            get
            {
                return _dailyTokens;
            }
        }

        /// <summary>
        /// 今日の感情トークン
        /// </summary>
        public double EmotionTokens
        {
            get
            {
                if (_emotionTokens.Count <= 0) return 0;
                return _emotionTokens[_emotionTokens.Count - 1];
            }
        }

        /// <summary>
        /// 今日までの累計トークン
        /// </summary>
        public double TotalTokens
        {
            get
            {
                if (_totalTokens.Count <= 0) return 0;
                return _totalTokens[_totalTokens.Count - 1];
            }
        }

        private double _averageEmotionTokens = 0.0;

        /// <summary>
        /// 平均獲得感情トークンV2
        /// トークン獲得値のピークだけを計算対象としている
        /// </summary>
        public double AverageEmotionTokens
        {
            get
            {
                return _averageEmotionTokens;
            }
        }

        private double _feeling = 0.0;

        /// <summary>
        /// 今日の気分
        /// </summary>
        public double Feeling
        {
            get
            {
                return _feeling;
            }
        }

        /// <summary>
        /// 計算したFLOPS
        /// </summary>
        public ulong Flops
        {
            get
            {
                return flops.Flops;
            }
        }

        /// <summary>
        /// トークンリセット時刻
        /// </summary>
        public int ResetHour
        {
            get
            {
                return _resetHour;
            }

            set
            {
                _resetHour = value;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// FLOPSの計算も行う
        /// </summary>
        public TokenManager()
        {
            Clear();
            flops.CalcFlops();
        }

        /// <summary>
        /// トークン情報をクリア
        /// </summary>
        public void Clear()
        {
            _dailyTokens = 0.0;
            _resetHour = -1;
            ClearCalcTokens();
        }

        /// <summary>
        /// トークンを読み取る
        /// </summary>
        public void ReadTokens()
        {
            ReadTokensFromFile();
        }

        /// <summary>
        /// 計算して求めるトークンをクリア
        /// </summary>
        private void ClearCalcTokens()
        {
            _emotionTokens.Clear();
            _totalTokens.Clear();
        }

        /// <summary>
        /// CPUの性能による重みを取得する
        /// </summary>
        /// <returns></returns>
        private double GetCpuWeight(bool isCameraMode)
        {
            double rev;

            if (isCameraMode)
            {
                rev = 1.0;
            }
            else
            {
                double log = Math.Log10(flops.Flops);
                double reci = 1.0 / log;
                rev = 1.0 - reci;
            }

            double ret = Math.Pow(rev, 3.0);

            return ret;
        }

        /// <summary>
        /// トークンを追加
        /// </summary>
        /// <param name="minToken">時間毎のトークン(未計算)</param>
        public void AddTokens(double minToken, bool isCameraMode)
        {
            TokenExist();
            double appendToken = Math.Sqrt(minToken * GetCpuWeight(isCameraMode)) * 10.0 * TOKEN_CALC_WEIGHT;
            LogManager.LogOutput("DailyTokenに" + appendToken.ToString() + "が足されました。");
            _dailyTokens += appendToken;
            WriteTokens();
        }

        /// <summary>
        /// トークンを読み取る
        /// </summary>
        private void ReadTokensFromFile()
        {
            djm.ReadJsonFile(SettingManager.PrivateSettings.TOKEN_PATH, (CalcTokenResetTime().ToString(), 0.0));
            TokenExist();
            CalcAllToken();
        }

        /// <summary>
        /// トークンを書き出す
        /// </summary>
        private void WriteTokens()
        {
            //リセット時間が設定されていなかったら処理をしない
            if (ResetHour >= 0)
            {
                DateTime resetTime = CalcTokenResetTime();

                if (djm.dict.ContainsKey(resetTime.ToString()))
                {
                    djm.dict[resetTime.ToString()] = _dailyTokens;
                    djm.WriteJsonFile(SettingManager.PrivateSettings.TOKEN_PATH);
                    CalcAllToken();
                }
                else
                {
                    ErrorLog.ErrorOutput("トークン書き込みエラー", "指定された時間のデータが辞書に存在しません");
                }
            }
        }

        /// <summary>
        /// トークンリセットする,トークンファイルがない場合は作成する
        /// </summary>
        private void TokenExist()
        {
            //リセット時間が設定されていなかったら処理をしない
            if (ResetHour >= 0)
            {
                DateTime resetTime = CalcTokenResetTime();

                if (djm.dict.ContainsKey(resetTime.ToString()))
                {
                    _dailyTokens = djm.dict[resetTime.ToString()];
                    DeleteInvalidTokens(resetTime);
                }
                else
                {
                    _dailyTokens = 0.0;
                    djm.dict.Add(resetTime.ToString(), _dailyTokens);
                    djm.WriteJsonFile(SettingManager.PrivateSettings.TOKEN_PATH);
                }
            }
        }

        /// <summary>
        /// 無効な日付のトークンを削除する
        /// </summary>
        private void DeleteInvalidTokens(DateTime validDay)
        {
            int validIndex;
            for (validIndex = 0; validIndex < djm.dict.Keys.Count; validIndex++)
            {
                if (validDay.ToString() == djm.dict.Keys.ToArray()[validIndex])
                {
                    break;
                }
            }

            if (validIndex >= djm.dict.Keys.Count)
            {
                return;
            }

            string[] deleteArray = djm.dict.Keys.ToArray();

            for (int i = validIndex + 1; i < djm.dict.Count; i++)
            {
                djm.dict.Remove(deleteArray[i]);
            }
        }

        /// <summary>
        /// トークンのリセット時間を計算して返却する
        /// </summary>
        /// <returns>トークンリセット時間</returns>
        private DateTime CalcTokenResetTime()
        {
            DateTime tokenDate = DateTime.Today;

            if (ResetHour < 0)
            {
                ErrorLog.ErrorOutput("トークンリセット時刻読み取りエラー", "トークンのリセット時刻が設定されていません");
            }
            else
            {
                DateTime resetTime = DateTime.Today;
                resetTime = resetTime.AddHours(ResetHour);
                if ((DateTime.Now - resetTime).TotalHours >= 0)
                {
                    //today
                    tokenDate = DateTime.Today;
                }
                else
                {
                    //yesterday
                    tokenDate = DateTime.Today.AddDays(-1);
                }
            }

            return tokenDate;
        }

        /// <summary>
        /// ペナルティを考慮して、感情トークンと合計トークンを計算する
        /// </summary>
        private void CalcAllToken()
        {
            if (djm.dict.Count <= 0) return;

            string[] keys = djm.dict.Keys.ToArray();

            ClearCalcTokens();
            _emotionTokens.Add(djm.dict[keys[0]]);
            _totalTokens.Add(djm.dict[keys[0]]);

#if false
            LogManager.LogOutput("トークン計算過程出力");
#endif

            for (int i = 1; i < keys.Length; i++)
            {
                DateTime newDay = DateTime.Parse(keys[i]);
                DateTime befDay = DateTime.Parse(keys[i - 1]);
                TimeSpan spanDay = newDay - befDay;

                //日付が1日以上経過している場合
                if (spanDay.Days > 1)
                {
                    CrossDatesProcess(spanDay.Days, newDay);
                }
                else//日付が経過していない
                {
                    double emoMem = djm.dict[keys[i - 1]] * HANDOVER_PERCENT + djm.dict[keys[i]];
                    double totalMem = _totalTokens[i - 1] + emoMem;

                    _emotionTokens.Add(emoMem);
                    _totalTokens.Add(totalMem);

#if false
                    LogManager.LogOutput("emoMem: " + emoMem.ToString());
                    LogManager.LogOutput("totalMem: " + totalMem.ToString());
#endif

                }
            }

#if false
            LogManager.LogOutput("トークン計算過程出力終了");
#endif

            _averageEmotionTokens = CalcAverageEmotionTokensV2();
            _feeling = CalcFeeling();
        }

        /// <summary>
        /// 平均感情トークン(旧)を算出し、返却する
        /// </summary>
        /// <returns>平均感情トークン</returns>
        private double CalcAverageEmotionTokens()
        {
            if (_emotionTokens.Count <= 0) return 0.0;
            return _emotionTokens.ToArray().Sum() / _emotionTokens.Count;
        }

        /// <summary>
        /// 平均感情トークン(新)を算出し、返却する
        /// </summary>
        /// <returns>平均感情トークン</returns>
        private double CalcAverageEmotionTokensV2()
        {
            if (_emotionTokens.Count <= 0) return 0.0;
            // トークン数が1なら、そのトークンを返却する
            if (_emotionTokens.Count == 1) return _emotionTokens[0];

            double[] tokenPeaks = CalcTokenPeaksForAverageEmotionTokensV2();

            if (tokenPeaks.Length == 1)
            {
                //トークンのピークが1個しかないなら、通常の平均算出ロジックを使う
                return CalcAverageEmotionTokens();
            }
            else
            {
                return tokenPeaks.Sum() / tokenPeaks.Length;
            }
        }

        private double[] CalcTokenPeaksForAverageEmotionTokensV2()
        {
            bool upFlg = true;
            List<double> tokenPeaks = new List<double>();

            //ピークの計算
            double tokenBef = _emotionTokens[0];
            for (int i = 1; i < _emotionTokens.Count; i++)
            {
                double tokenNow = _emotionTokens[i];

                if ((upFlg == true) && (tokenBef > tokenNow))
                {
                    tokenPeaks.Add(tokenBef);

                    upFlg = false;
                }

                if (tokenBef < tokenNow) upFlg = true;

                tokenBef = tokenNow;
            }

            //最後のトークンがまだ上昇中だった場合は、それもピークとして加算する
            if (upFlg == true)
            {
                tokenPeaks.Add(_emotionTokens[_emotionTokens.Count - 1]);
            }

            return tokenPeaks.ToArray();
        }

        /// <summary>
        /// 感情を計算し、返却する
        /// </summary>
        /// <returns>感情</returns>
        private double CalcFeeling()
        {
            double avgToken = AverageEmotionTokens;

            if (avgToken <= 0 || EmotionTokens <= 0) return 0.0;
            return (EmotionTokens - avgToken) / avgToken;
        }

        /// <summary>
        /// 複数び跨いだ時の処理
        /// </summary>
        /// <param name="days">跨いだ日数(1以下は機能しない)</param>
        /// <param name="lastDate">最後の日付</param>
        private void CrossDatesProcess(int days, DateTime lastDate)
        {
            if (djm.dict.ContainsKey(lastDate.ToString()))
            {

                for (int j = 1; j < days - 1; j++)
                {
                    double emoMem = 0.0;

                    //最後の日付のデータはあるので、その日のトークンを足さないといけない
                    if (j == days - 2)
                    {
                        emoMem = _emotionTokens[_emotionTokens.Count - 1] * HANDOVER_PERCENT + djm.dict[lastDate.ToString()];
                    }
                    else
                    {
                        emoMem = _emotionTokens[_emotionTokens.Count - 1] * HANDOVER_PERCENT;
                    }

                    double totalMem = (_totalTokens[_totalTokens.Count - 1] + emoMem) * HANDOVER_PENALTY;

                    _emotionTokens.Add(emoMem);
                    _totalTokens.Add(totalMem);

#if DEBUG
                    LogManager.LogOutput("emoMem: " + emoMem.ToString());
                    LogManager.LogOutput("totalMem: " + totalMem.ToString());
#endif

                }
            }
            else
            {
                ErrorLog.ErrorOutput("トークン計算エラー", "指定された日付が辞書に登録されていません");
            }
        }
    }
}
