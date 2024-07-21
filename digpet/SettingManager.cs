using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digpet
{
    internal class SettingManager
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SettingManager()
        {
            Init();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void Init()
        {
            FeelingManager.Init();
            IntimacyManager.Init();
        }

        /// <summary>
        /// 感情のテキストを取得する
        /// </summary>
        /// <param name="feeling">感情</param>
        /// <returns></returns>
        public string GetFeelingString(double feeling)
        {
            return FeelingManager.GetFeelingString(feeling);
        }

        /// <summary>
        /// 親密度のテキストを取得する
        /// </summary>
        /// <param name="intimacy">親密度</param>
        /// <returns></returns>
        public string GetIntimacyString(double intimacy)
        {
            return IntimacyManager.GetIntimacygString(intimacy);
        }

        /// <summary>
        /// 感情の管理クラス(テキストとか)
        /// </summary>
        private static class FeelingManager
        {
            //変数関連
            private static Dictionary<double, string> feelingDict = new Dictionary<double, string>();

            /// <summary>
            /// 初期化
            /// </summary>
            public static void Init()
            {
                //初期値(設定ファイルで変更可能にする)
                feelingDict = new Dictionary<double, string>()
                {
                    [-1.00]                     = "エラー",
                    [-0.49]                     = "悪い",
                    [0.0]                       = "普通",
                    [0.3]                       = "良い",
                    [1.0]                       = "最高",
                    [double.PositiveInfinity]   = "エラー"
                };
            }

            /// <summary>
            /// 感情のテキストを取得する
            /// </summary>
            /// <param name="feeling">感情</param>
            /// <returns></returns>
            public static string GetFeelingString(double feeling)
            {
                double[] keys = feelingDict.Keys.ToArray();

                if (keys.Length > 0)
                {
                    foreach (double threshold in keys)
                    {
                        if (threshold == keys[0])
                        {
                            if (feeling < threshold)
                            {
                                return feelingDict[threshold];
                            }
                        }
                        else
                        {
                            if (feeling <= threshold)
                            {
                                return feelingDict[threshold];
                            }
                        }
                    }
                }

                return "エラー";
            }
        }

        /// <summary>
        /// 親密度の管理クラス(テキストとか)
        /// </summary>
        private static class IntimacyManager
        {
            //変数関連
            private static Dictionary<double, string> intimacyDict = new Dictionary<double, string>();

            /// <summary>
            /// 初期化
            /// </summary>
            public static void Init()
            {
                //初期値(設定ファイルで変更可能にする)
                intimacyDict = new Dictionary<double, string>()
                {
                    [double.PositiveInfinity]   = "設定なし"
                };
            }

            /// <summary>
            /// 親密度のテキストを取得する
            /// </summary>
            /// <param name="intimacy">親密度</param>
            /// <returns></returns>
            public static string GetIntimacygString(double intimacy)
            {
                double[] keys = intimacyDict.Keys.ToArray();

                if (keys.Length > 0)
                {
                    foreach (double threshold in keys)
                    {
                        if (threshold == keys[0])
                        {
                            if (intimacy < threshold)
                            {
                                return intimacyDict[threshold];
                            }
                        }
                        else
                        {
                            if (intimacy <= threshold)
                            {
                                return intimacyDict[threshold];
                            }
                        }
                    }
                }

                return "エラー";
            }
        }
    }
}
