using digpet.Modules;

namespace digpet.Managers
{
    public class TokenManager
    {
        //固定値関連の宣言
        private const double HANDOVER_PERCENT = 0.99;                                       //tokenの引継ぎ割合

        //変数関連の宣言
        private long _tokens;                                                              //token

        private bool isIncrease;                                                            //tokenの値が増加し続けているか?
        private long tokenBef;                                                             //過去のtoken値
        private long peakTokenSum;                                                         //peak tokenの合計値
        private long peakTokenCount;                                                       //peak token数

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
                if (PeakTokenAvg <= 0.0)
                {
                    return -1.0;
                }
                long diff = Tokens - PeakTokenAvg;
                double ret = diff / PeakTokenAvg;
                return ret;
            }
        }

        /// <summary>
        /// token peak値の平均
        /// </summary>
        public long PeakTokenAvg
        {
            get
            {
                if (peakTokenCount <= 0)
                {
                    return Tokens;
                }
                return (peakTokenSum / peakTokenCount);
            }
        }

        /// <summary>
        /// コンストラクタ
        /// FLOPSの計算も行う
        /// </summary>
        public TokenManager()
        {
            Clear();
        }

        /// <summary>
        /// トークン情報をクリア
        /// </summary>
        public void Clear()
        {
            _tokens = 0;
            isIncrease = true;
            tokenBef = 0;
            peakTokenSum = 0;
            peakTokenCount = 0;
        }

        /// <summary>
        /// トークンを読み取る
        /// </summary>
        public void ReadTokens()
        {
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

            if ((tokenBef > Tokens) && (isIncrease == true))
            {
                PeacProcess();
            }
            else if (Tokens >= tokenBef)
            {
                isIncrease = true;
            }

                tokenBef = Tokens;
        }

        /// <summary>
        /// tokenのpeak計算処理
        /// </summary>
        private void PeacProcess()
        {
            peakTokenSum += Tokens;
            peakTokenCount++;
            isIncrease = false;
        }
    }
}
