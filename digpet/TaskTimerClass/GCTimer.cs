﻿using digpet.Models.AbstractModels;

namespace digpet.TaskTimerClass
{
    /// <summary>
    /// ガーベジコレクタ
    /// </summary>
    internal class GCTimer : TaskClassModel
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
