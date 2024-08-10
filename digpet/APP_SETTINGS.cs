using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digpet
{
    internal static class APP_SETTINGS
    {
        public const string APPLICATION_VERSION = "1.00.00" + DEBUG_APPENDANCE;        //アプリバージョン
        public const string CHAR_FORMAT_VERSION = "1.00.00";                            //キャラフォーマットのバージョン

#if DEBUG
        public const string DEBUG_APPENDANCE = "-debug";                                //デバッグ判別用
#else
        public const string DEBUG_APPENDANCE = string.empty;                            //デバッグ判別用
#endif
    }
}
