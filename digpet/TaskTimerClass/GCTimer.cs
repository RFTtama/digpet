using digpet.Managers;
using digpet.Models.AbstractModels;
using digpet.Modules;

namespace digpet.TaskTimerClass
{
    /// <summary>
    /// ガーベジコレクタ
    /// </summary>
    public class GCTimer : TaskClassModel
    {
        public override TaskReturn TaskFunc()
        {
            long memory;
            memory = GC.GetTotalMemory(false);

            if (memory >= SettingManager.PublicSettings.GcThreshold)
            {
                GC.Collect();
                LogLib.LogOutput("ガーベジコレクタを手動実行しました Mem:" + memory.ToString());
            }

            return TaskReturn.TASK_SUCCESS;
        }

        public override void TaskCheckRet(TaskReturn ret)
        {

        }
    }
}
