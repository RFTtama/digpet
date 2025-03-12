using digpet.Models.AbstractModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digpet.TaskTimerClass
{
    class CpuWatchTask : TASK
    {
        public CpuWatchTask(int interval, int waitTime) : base(interval, waitTime)
        {
        }

        protected override TaskReturn Main()
        {
            throw new NotImplementedException();
        }

        protected override void Terminator()
        {
            throw new NotImplementedException();
        }
    }
}
