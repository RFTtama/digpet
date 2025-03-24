using digpet.Models.AbstractModels;

namespace digpet.Managers
{
    internal class TaskManager
    {
        public readonly TASK[] Tasks;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="maxTaskNum">登録可能なタスクの最大数</param>
        public TaskManager(int maxTaskNum)
        {
            Tasks = new TASK[maxTaskNum];
        }

        /// <summary>
        /// 登録されているTASKを一斉に実行する
        /// </summary>
        public void TaskRun()
        {
            //複数回実行してもタスクが重複することはないが、注意すること
            foreach (TASK task in Tasks)
            {
                Task.Run(async () =>
                {
                    await task.Program();
                });
            }
        }
    }
}
