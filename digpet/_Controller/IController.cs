using digpet._View;

namespace digpet._Controller
{
    internal interface IController
    {
        public void Init(IDigpet view);
        public void Terminator();
    }
}
