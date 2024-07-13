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
        public const double TOKEN_CALC_WEIGHT = 0.0694;

        //変数関連の宣言
        private double _dailyTokens;

        public TokenManager() 
        {
            
        }

        public void Clear()
        {
            _dailyTokens = 0.0;
        }

        public double CalcMinTokens(double minToken)
        {
            return minToken * TOKEN_CALC_WEIGHT;
        }
    }
}
