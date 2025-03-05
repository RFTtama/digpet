using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using digpet.Modules;

namespace digpet.Managers
{
    internal class VersionManager
    {
        /// <summary>
        /// メジャーバージョン
        /// </summary>
        public int major
        {
            get
            {
                return GetVersionByIndex(0);
            }
            set
            {
                versionArray[0] = value;
            }
        }

        /// <summary>
        /// マイナーバージョン
        /// </summary>
        public int minor
        {
            get
            {
                return GetVersionByIndex(1);
            }
            set
            {
                versionArray[1] = value;
            }
        }

        /// <summary>
        /// パッチバージョン
        /// </summary>
        public int patch
        {
            get
            {
                return GetVersionByIndex(2);
            }
            set
            {
                versionArray[2] = value;
            }
        }

        //内部変数
        private int[] versionArray = new int[3];

        //固定値関連
        private const int INITIALIZE_NUM = -1;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public VersionManager()
        {
            Init();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="versionStr">バージョン文字列</param>
        public VersionManager(string versionStr)
        {
            Init();
            Parse(versionStr);
        }

        /// <summary>
        /// 初期化する
        /// </summary>
        private void Init()
        {
            for (int i = 0; i < versionArray.Length; i++)
            {
                versionArray[i] = -1;
            }
        }

        /// <summary>
        /// 文字列からバージョン情報を抜き出す
        /// </summary>
        /// <param name="versionStr">バージョン文字列</param>
        private void Parse(string versionStr)
        {
            string[] parts = versionStr.Split('.');

            if (parts.Length != versionArray.Length)
            {
                ErrorLogLib.ErrorOutput("バージョン情報変換エラー", "バージョン情報の入力フォーマットが正しくありません");
                return;
            }
            for (int i = 0; i < versionArray.Length; i++)
            {
                int mem;
                if (int.TryParse(parts[i], out mem))
                {
                    versionArray[i] = mem;
                }
            }
        }

        /// <summary>
        /// インデックスからバージョンの数値を返却する
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <returns>設定されている数値 設定されていない項目が1個でもある場合は-1を返却する</returns>
        private int GetVersionByIndex(int index)
        {
            int ret = -1;
            int minNum = int.MaxValue;

            for (int i = 0; i < versionArray.Length; i++)
            {
                if (minNum > versionArray[i])
                {
                    minNum = versionArray[i];
                }
            }

            if (index >= 0 && index < versionArray.Length && minNum >= 0)
            {
                ret = versionArray[index];
            }

            return ret;
        }

        /// <summary>
        /// バージョン情報を比較する
        /// </summary>
        /// <param name="compareVersion">比較先のバージョン</param>
        /// <returns>1: 元の方が大きい, 0: 同じ: -1: 対象の方が大きい</returns>
        public int Compare(VersionManager compareVersion)
        {
            int ret = 0;
            for (int i = 0; i < versionArray.Length; i++)
            {
                if (versionArray[i] > compareVersion.versionArray[i])
                {
                    ret = 1;
                    break;
                }
                else if (versionArray[i] < compareVersion.versionArray[i])
                {
                    ret = -1;
                    break;
                }
            }

            return ret;
        }
    }
}
