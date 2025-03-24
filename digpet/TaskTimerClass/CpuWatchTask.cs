using digpet.Managers.GenerakManager;
using digpet.Models.AbstractModels;

namespace digpet.TaskTimerClass
{
    class CpuWatchTask : TASK
    {
        //クラス関連
        private CpuWatcher cpuWatcher = new CpuWatcher();

        //delegate関連
        public delegate void CpuWatchDelegate(float cpuUsage);
        CpuWatchDelegate? cpuWatchDelegate;

        public CpuWatchTask(int interval, int waitTime) : base(interval, waitTime)
        {
        }

        protected override TaskReturn Main()
        {
            cpuWatchDelegate?.Invoke(cpuWatcher.GetCpuUsage());
            return TaskReturn.Normal;
        }

        protected override void Terminator()
        {
        }
    }
}
