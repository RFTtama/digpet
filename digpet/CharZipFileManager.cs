using System.IO.Compression;

namespace digpet
{
    internal class CharZipFileManager
    {
        //クラス宣言
        private CharSettingManager _charSettingManager;         //キャラクターファイルの設定クラス

        //定数宣言
        private const string CONFIG_PATH = "config.json";       //設定ファイルのパス

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CharZipFileManager()
        {
            _charSettingManager = new CharSettingManager();
        }

        /// <summary>
        /// 感情のテキストを取得する
        /// </summary>
        /// <param name="feeling">感情</param>
        /// <returns></returns>
        public string GetFeelingString(double feeling)
        {
            return _charSettingManager.settings.feelingSetting.GetFeelingString(feeling);
        }

        /// <summary>
        /// 親密度のテキストを取得する
        /// </summary>
        /// <param name="intimacy">親密度</param>
        /// <returns></returns>
        public string GetIntimacyString(double intimacy)
        {
            return _charSettingManager.settings.intimacySetting.GetIntimacygString(intimacy);
        }

        /// <summary>
        /// キャラクターのコンフィグファイルを読み取る
        /// </summary>
        /// <param name="path"></param>
        public void ReadCharSettings(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    using (ZipArchive zip = ZipFile.OpenRead(path))
                    {
                        ZipArchiveEntry? entry = zip.GetEntry(CONFIG_PATH);

                        if (entry != null)
                        {
                            _charSettingManager.ReadEntry(entry);
                        }
                        else
                        {
                            ErrorLog.ErrorOutput("コンフィグファイル読み取りエラー", "キャラデータにコンフィグファイルが含まれていません", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.ErrorOutput("コンフィグファイル読み取りエラー", ex.Message, true);
                }
            }
            else
            {
                ErrorLog.ErrorOutput("コンフィグファイル確認エラー", "キャラデータが見つかりません", true);
            }
        }
    }
}