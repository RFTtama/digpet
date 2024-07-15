using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digpet
{
    internal class TokenManager
    {
        //クラス関連の宣言
        private DoubleJsonManager djm = new DoubleJsonManager(TOKEN_PASS);

        //固定値関連の宣言
        public const double TOKEN_CALC_WEIGHT = 100.0 / (60.0 * 24.0);

        //変数関連の宣言
        private double _dailyTokens;
        private const string TOKEN_PATH = "TOKENS.dig";
        private const string TOKEN_PASS = "qK6Nvgjfn8aa6oy2tDtYw17Lz0zePJMnXdiAnfXO";

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
            ReadTokens();
        }

        public void AddTokens(double minToken)
        {
            TokenExist();
            _dailyTokens += Math.Sqrt(minToken) * TOKEN_CALC_WEIGHT;
            WriteTokens();
        }

        private void ReadTokens()
        {
            djm.ReadJsonFile(TOKEN_PATH);
            TokenExist();
        }

        private void WriteTokens()
        {
            djm.dict[DateTime.Today.ToString()] = _dailyTokens;
            djm.WriteJsonFile(TOKEN_PATH);
        }

        private void TokenExist()
        {
            if (djm.dict.ContainsKey(DateTime.Today.ToString()))
            {
                _dailyTokens = djm.dict[DateTime.Today.ToString()];
            }
            else
            {
                djm.dict.Add(DateTime.Today.ToString(), 0.0);
                djm.WriteJsonFile(TOKEN_PATH);
            }
        }
    }
}
