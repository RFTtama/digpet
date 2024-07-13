using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digpet
{
    internal class TokenManager
    {
        //固定値関連の宣言
        public const double TOKEN_CALC_WEIGHT = 0.00694;

        //変数関連の宣言
        private double _dailyTokens;

        /// <summary>
        /// 今日の累計トークン
        /// </summary>
        public double DailyTokens
        {
            get { return _dailyTokens; }
        }

        public TokenManager() 
        {
            Clear();
        }

        public void Clear()
        {
            _dailyTokens = 0.0;
        }

        public void AddTokens(double minToken)
        {
            _dailyTokens += minToken * TOKEN_CALC_WEIGHT;
        }
    }
}
