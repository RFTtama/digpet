using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace digpet
{
    /// <summary>
    /// 設定ファイル管理クラス
    /// </summary>
    internal class SettingManager
    {
        public DigpetSettings Settings;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SettingManager()
        {
            Settings = new DigpetSettings();
        }

        /// <summary>
        /// 設定を読み取る
        /// </summary>
        /// <param name="path">設定ファイルパス</param>
        public void ReadSettingFile(string path)
        {
            if (File.Exists(path))
            {
                ReadSettings(path);
            }
            else
            {
                WriteSettings(path);
            }
        }

        /// <summary>
        /// 設定を書き込む
        /// </summary>
        /// <param name="path">設定ファイルパス</param>
        public void WriteSettingFile(string path)
        {
            WriteSettings(path);
        }

        /// <summary>
        /// 設定ファイルを書き込む
        /// </summary>
        /// <param name="path"></param>
        private void WriteSettings(string path)
        {
            try
            {
                string settingString = JsonSerializer.Serialize(Settings);
                using (StreamWriter sw = new StreamWriter(path, false))
                {
                    sw.Write(settingString);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorOutput("設定ファイル初期化エラー", ex.Message, true);
            }
        }

        /// <summary>
        /// 設定を読み取る
        /// </summary>
        /// <param name="path"></param>
        private void ReadSettings(string path)
        {
            try
            {
                string settingString = string.Empty;
                using (StreamReader sr = new StreamReader(path))
                {
                    settingString = sr.ReadToEnd();
                }
                Settings = JsonSerializer.Deserialize<DigpetSettings>(settingString) ?? new DigpetSettings();
            }
            catch (Exception ex)
            {
                ErrorLog.ErrorOutput("設定ファイル読み取りエラー", ex.Message, true);
            }
        }

        /// <summary>
        /// 設定保持クラス
        /// </summary>
        public class DigpetSettings
        {
            public string CharSettingPath { get; set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public DigpetSettings()
            {
                CharSettingPath = string.Empty;
            }
        }
    }
}
