namespace digpet.Models.AbstractModels
{
    /// <summary>
    /// タスク処理用基底クラス
    /// </summary>
    public abstract class TaskClassModel
    {
        //タスク保持用変数
        public Task ClassTask = Task.Run(() => { });

        //タスクとして送信する関数
        public abstract TaskReturn TaskFunc();

        //タスク戻り値チェック用関数
        public abstract void TaskCheckRet(TaskReturn ret);
    }

    //タスク戻り値のenum
    public enum TaskReturn
    {
        TASK_SUCCESS,
        TASK_FAILURE,
        TASK_BLOCKED
    }
}
