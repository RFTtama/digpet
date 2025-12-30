using digpet._View;
using digpet.Managers;

namespace digpet._Controller
{
    public partial class Controller : IController
    {
        private static Lazy<Controller> _lazy = new(() => new Controller(), isThreadSafe: true);
        public static Controller Instance => _lazy.Value;


        // Digpetにわたすやつ
        public ClassManagerArg arg;

        // 変数関連
        private IDigpet? _view;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private Controller()
        {
            InitTimers();
            arg = new ClassManagerArg();
            cameraTimer?.Init();
            backUpCnt = 0;
        }
    }

    public class ClassManagerArg
    {
        public TokenManager tokenManager = new TokenManager();
        public CharZipFileManager charZipFileManager = new CharZipFileManager();
    }
}
