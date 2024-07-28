namespace digpet
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

        //変数関連の宣言
        private double _dailyTokens;
        private const string TOKEN_PATH = "TOKENS.dig";                                     //トークンのファイル名
        private const string TOKEN_PASS = "qK6Nvgjfn8aa6oy2tDtYw17Lz0zePJMnXdiAnfXO";       //トークンの暗号化キー(このキーでテキストが複合できるのはないしょ)

        //リスト関連の宣言
        private List<double> _emotionTokens = new List<double>();                           //日別感情トークンの獲得量リスト
        private List<double> _totalTokens = new List<double>();                             //日別累計トークンの獲得量リスト

        /// <summary>
        /// 今日の累計トークン
        /// </summary>
        public double DailyTokens
        {
            get { return _dailyTokens; }
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

        /// <summary>
        /// 平均獲得感情トークン
        /// </summary>
        public double AverageEmotionTokens
        {
            get
            {
                if (_emotionTokens.Count <= 0) return 0;
                return _emotionTokens.ToArray().Sum() / _emotionTokens.Count;
            }
        }

        /// <summary>
        /// 今日の気分
        /// </summary>
        public double Feeling
        {
            get
            {
                if (AverageEmotionTokens <= 0) return 0;
                return ((EmotionTokens - AverageEmotionTokens) / AverageEmotionTokens);
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
            ClearCalcTokens();
            ReadTokens();
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
        private double GetCpuWeight()
        {
            double log = Math.Log10(flops.Flops);
            double reci = 1.0 / log;
            double rev = 1.0 - reci;
            return (Math.Pow(rev, 3.0));
        }

        /// <summary>
        /// トークンを追加
        /// </summary>
        /// <param name="minToken">時間毎のトークン(未計算)</param>
        public void AddTokens(double minToken)
        {
            TokenExist();
            _dailyTokens += (Math.Sqrt((minToken * GetCpuWeight())) * 10.0) * TOKEN_CALC_WEIGHT;
            WriteTokens();
        }

        /// <summary>
        /// トークンを読み取る
        /// </summary>
        private void ReadTokens()
        {
            djm.ReadJsonFile(TOKEN_PATH, (DateTime.Today.ToString(), 0.0));
            TokenExist();
            CalcAllToken();
        }

        /// <summary>
        /// トークンを書き出す
        /// </summary>
        private void WriteTokens()
        {
            djm.dict[DateTime.Today.ToString()] = _dailyTokens;
            djm.WriteJsonFile(TOKEN_PATH);
            CalcAllToken();
        }

        /// <summary>
        /// トークンリセットする,トークンファイルがない場合は作成する
        /// </summary>
        private void TokenExist()
        {
            if (djm.dict.ContainsKey(DateTime.Today.ToString()))
            {
                _dailyTokens = djm.dict[DateTime.Today.ToString()];
            }
            else
            {
                _dailyTokens = 0.0;
                djm.dict.Add(DateTime.Today.ToString(), _dailyTokens);
                djm.WriteJsonFile(TOKEN_PATH);
            }
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
                    double emoMem = (djm.dict[keys[i - 1]] * HANDOVER_PERCENT) + djm.dict[keys[i]];
                    double totalMem = _totalTokens[i - 1] + emoMem;

                    _emotionTokens.Add(emoMem);
                    _totalTokens.Add(totalMem);
                }
            }
        }

        /// <summary>
        /// 複数び跨いだ時の処理
        /// </summary>
        /// <param name="days">跨いだ日数(1以下は機能しない)</param>
        /// <param name="lastDate">最後の日付</param>
        private void CrossDatesProcess(int days, DateTime lastDate)
        {
            for (int j = 1; j < days - 1; j++)
            {
                double emoMem = 0.0;

                //最後の日付のデータはあるので、その日のトークンを足さないといけない
                if (j == days - 2)
                {
                    emoMem = (_emotionTokens[_emotionTokens.Count - 1] * HANDOVER_PERCENT) + djm.dict[lastDate.ToString()];
                }
                else
                {
                    emoMem = (_emotionTokens[_emotionTokens.Count - 1] * HANDOVER_PERCENT);
                }

                double totalMem = (_totalTokens[_totalTokens.Count - 1] + emoMem) * HANDOVER_PENALTY;

                _emotionTokens.Add(emoMem);
                _totalTokens.Add(totalMem);
            }
        }
    }
}
