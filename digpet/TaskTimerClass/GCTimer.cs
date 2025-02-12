using digpet.Interface;

namespace digpet.TaskTimerClass
{
    /// <summary>
    /// ガーベジコレクタ
    /// </summary>
    internal class GCTimer : TaskClassInterface
    {

        public override TaskReturn TaskFunc()
        {
            GC.Collect();
            return TaskReturn.TASK_SUCCESS;
        }

        public override void TaskCheckRet(TaskReturn ret)
        {

        }
    }
}
