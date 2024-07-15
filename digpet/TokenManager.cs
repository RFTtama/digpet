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

        /// <summary>
        /// コンストラクタ
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
            _dailyTokens = 0.0;
            ReadTokens();
        }

        /// <summary>
        /// トークンを追加
        /// </summary>
        /// <param name="minToken">時間毎のトークン(未計算)</param>
        public void AddTokens(double minToken)
        {
            TokenExist();
            _dailyTokens += (Math.Sqrt(minToken) * 10.0) * TOKEN_CALC_WEIGHT;
            WriteTokens();
        }

        /// <summary>
        /// トークンを読み取る
        /// </summary>
        private void ReadTokens()
        {
            djm.ReadJsonFile(TOKEN_PATH);
            TokenExist();
        }

        /// <summary>
        /// トークンを書き出す
        /// </summary>
        private void WriteTokens()
        {
            djm.dict[DateTime.Today.ToString()] = _dailyTokens;
            djm.WriteJsonFile(TOKEN_PATH);
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
                djm.dict.Add(DateTime.Today.ToString(), 0.0);
                djm.WriteJsonFile(TOKEN_PATH);
            }
        }
    }
}
