using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digpet.Interface
{
    /// <summary>
    /// タスク処理用基底クラス
    /// </summary>
    public abstract class TaskClassInterface
    {
        //タスク保持用変数
        public required Task ClassTask;

        //タスクとして送信する関数
        public abstract TaskClassRet TaskFunc();

        //タスク戻り値チェック用関数
        public abstract void TaskCheckRet(TaskClassRet ret);
    }

    /// <summary>
    /// タスク実行時の戻り値
    /// </summary>
    public class TaskClassRet
    {
        public TaskReturn taskReturn;
        public string msg = string.Empty;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TaskClassRet(TaskReturn ret, string message)
        {
            taskReturn = ret;
            msg = message;
        }
    }

    //タスク戻り値のenum
    public enum TaskReturn
    {
        TASK_SUCCESS,
        TASK_SUCCESS_MSG,
        TASK_FAILURE,
        TASK_FAILURE_MSG,
        TASK_BLOCKED
    }
}
