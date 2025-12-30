using digpet._View;
using digpet.Managers;

namespace digpet._Controller
{
    public partial class Controller : IController
    {
        private static Lazy<Controller> _lazy = new(() => new Controller(), isThreadSafe: true);
        public static Controller Instance => _lazy.Value;

        // 変数関連
        private IDigpet? _view;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private Controller()
        {
            InitTimers();
            cameraTimer?.Init();
            backUpCnt = 0;
        }
    }
}
