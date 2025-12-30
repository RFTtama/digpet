using digpet._View;
using digpet.Modules;

namespace digpet._Controller
{
    public partial class Controller
    {
        public void Init(IDigpet view)
        {
            _view = view;
            _view.InitRequest(arg);
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        public void Terminator()
        {
            ErrorLogLib.Export();
            logTimer?.Export();
            cameraTimer?.Dispose();

            timer?.Dispose();
        }
    }
}
