﻿using System.Text.Json;
using digpet.Modules;

namespace digpet.Managers
{
    /// <summary>
    /// 設定ファイル管理クラス
    /// </summary>
    internal class SettingManager
    {
        //内包クラス
        public DigpetSettings Settings;

        //JSONの設定
        private readonly JsonSerializerOptions JSON_OPTIONS = new JsonSerializerOptions
        {
            WriteIndented = true
        };

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
                string settingString = JsonSerializer.Serialize(Settings, JSON_OPTIONS);
                using (StreamWriter sw = new StreamWriter(path, false))
                {
                    sw.Write(settingString);
                }
                LogManager.LogOutput("設定ファイルが正常に書き込まれました");
            }
            catch (Exception ex)
            {
                LogManager.LogOutput("設定ファイルの書き込み失敗");
                ErrorLog.ErrorOutput("設定ファイル初期化エラー", ex.Message);
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
                LogManager.LogOutput("設定ファイルが読み込まれました");
            }
            catch (Exception ex)
            {
                LogManager.LogOutput("設定ファイルの読み込みに失敗しました");
                ErrorLog.ErrorOutput("設定ファイル読み取りエラー", ex.Message);
            }
        }

        /// <summary>
        /// 設定保持クラス
        /// 設定の項目が追加されたら増やすこと!!
        /// </summary>
        public class DigpetSettings
        {
            //キャラ設定ファイルのパス
            public string CharSettingPath { get; set; }

            //リセット時間
            public int ResetHour { get; set; }

            //ウィンドウの状態 0: 通常, 1: 最大化, 2: 最小化
            public int WindowState { get; set; }

            //ウィンドウサイズ
            public Size WindowSize { get; set; }

            //ウィンドウロケーション
            public Point WindowLocation { get; set; }

            //フォントの拡大サイズ
            public int FontEnlargeSize { get; set; }

            //キャラ画像サイズ
            public Size ImageSize { get; set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public DigpetSettings()
            {
                CharSettingPath = string.Empty;
                ResetHour = -1;
                WindowState = 0;
                WindowSize = new Size(500, 500);
                WindowLocation = new Point(0, 0);
                FontEnlargeSize = 0;
                ImageSize = new Size(400, 400);
            }
        }
    }
}
