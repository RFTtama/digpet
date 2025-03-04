using digpet.Abstract;

namespace digpet.TaskTimerClass
{
    /// <summary>
    /// ガーベジコレクタ
    /// </summary>
    internal class GCTimer : TaskClassAbstract
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
