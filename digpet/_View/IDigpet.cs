using digpet._Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digpet._View
{
    public interface IDigpet
    {
        public void InitRequest(ClassManagerArg arg);
        public void UpdateCpuLabel(string label);
    }
}
