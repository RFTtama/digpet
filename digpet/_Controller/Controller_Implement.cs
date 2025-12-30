using digpet._View;
using digpet.Modules;

namespace digpet._Controller
{
    public partial class Controller
    {
        public void Init(IDigpet view)
        {
            _view = view;
            _view.InitRequest();
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        public void Terminator()
        {
            ErrorLogLib er = ErrorLogLib.Instance;
            er.Export();
            logTimer?.Export();
            cameraTimer?.Dispose();

            timer?.Dispose();
        }
    }
}
