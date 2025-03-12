namespace digpet.Models.AbstractModels
{
    /// <summary>
    /// タスク処理定義用abstract
    /// </summary>
    abstract class TASK
    {
        //変数関連
        private bool _init;
        private int _interval;
        private int _waitTime;
        private DateTime _timeStamp;

        //getter
        public bool Init
        {
            get { return _init; }
        }

        public int Interval
        {
            get { return _interval; }
        }

        public int WaitTime
        {
            get { return _waitTime; }
        }

        public DateTime TimeStamp
        {
            get { return _timeStamp; }
        }

        //Mainの戻り値
        protected enum TaskReturn
        {
            Normal,
            End
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="interval">タイマの間隔</param>
        /// <param name="waitTime">待ち時間(基本intervalより小さい数値)</param>
        public TASK(int interval, int waitTime)
        {
            _init = false;
            _interval = interval;
            _waitTime = waitTime;
            _timeStamp = DateTime.Now;
        }

        /// <summary>
        /// Taskに投げる用の処理
        /// </summary>
        /// <returns></returns>
        public async Task Program()
        {
            if (!Init) return;

            while (true)
            {
                while ((DateTime.Now - TimeStamp).TotalMilliseconds < Interval)
                {
                    await Task.Delay(WaitTime);
                }

                TaskReturn ret = Main();

                if (ret == TaskReturn.End) break;

                _timeStamp = DateTime.Now;
            }

            Terminator();
        }

        /// <summary>
        /// タスクで実行したい処理を書いておく関数
        /// </summary>
        /// <returns></returns>
        protected abstract TaskReturn Main();

        /// <summary>
        /// タスク処理を終了する際に実行する関数
        /// </summary>
        protected abstract void Terminator();
    }
}
