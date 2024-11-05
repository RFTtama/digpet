namespace digpet
{
    internal static class APP_SETTINGS
    {
        public const string APPLICATION_VERSION = "1.00.02" + DEBUG_APPENDANCE;         //アプリバージョン
        public const string CHAR_FORMAT_VERSION = "1.00.00";                            //キャラフォーマットのバージョン

        public const string CONFIG_FILE_PATH    = "config.json";                        //コンフィグファイルのパス
        public const string ERRORLOG_PATH       = "errorLog.txt";                       //エラーログのパス
        public const string LOG_PATH            = "Log.txt";                            //ログファイルのパス
        public const string TOKEN_PATH          = "TOKENS.dig";                         //トークンファイルのパス

#if DEBUG
        public const string DEBUG_APPENDANCE    = "-debug";                             //デバッグ判別用
#else
        public const string DEBUG_APPENDANCE    = "";                         //デバッグ判別用
#endif
    }
}
