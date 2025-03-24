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
    }
}
