namespace digpet
{
    public partial class Form1 : Form
    {
        //�N���X�֘A�̐錾
        private CpuAvgManager cpuAvgManager = new CpuAvgManager();
        private TokenManager tokenManager = new TokenManager();

        //�ϐ��֘A�̐錾
        private int cpuCnt;
        private double cpuAvg;
        private Label[] labelArray = Array.Empty<Label>();

        //�萔�̐錾
        private readonly string[] FEELING_STRING =
        {
            "����", "����", "�ǂ�", "�ō�"
        };

        public Form1()
        {
            InitializeComponent();
            Init();
            CpuUsageTimer.Enabled = true;
            Label1Out(tokenManager.DailyTokens);
        }

        /// <summary>
        /// ������
        /// </summary>
        private void Init()
        {
            InitLabelArray();
            cpuCnt = 0;
            cpuAvg = 0.0;
        }

        /// <summary>
        /// ���x���A���C�̗v�f������������
        /// </summary>
        private void InitLabelArray()
        {
            labelArray =
            [
                StatsLabel,
                DailyTokenLabel,
                EmoTokenLabel,
                AverageEmotionTokensLabel,
                FeelingLabel,
                FlopsLabel
            ];
        }


        /// <summary>
        /// 1��������CPU�g�p���̕��ς����߂āA�ϐ��ɑ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CpuUsageTimer_Tick(object sender, EventArgs e)
        {
            //60�b��1�񏈗����s��
            if ((cpuCnt > 0) && (cpuCnt % 60 == 0))
            {
                try
                {
                    //CPU�g�p���̕��ς��擾���A�g�[�N�����v�Z����
                    GetCpuAvg();
                }
                catch (Exception ex)
                {
                    ErrorLog.ErrorOutput("CPU�g�p�����όv�Z�G���[", ex.Message, true);
                    CpuUsageTimer.Enabled = false;
                }
            }
            else
            {
                //CPU�g�p�������Z
                SumCpuAvg();
            }
        }

        /// <summary>
        /// CPU�g�p���̕��ς�����cpuAvg�ɑ������
        /// </summary>
        private void GetCpuAvg()
        {
            cpuCnt = 0;
            cpuAvg = cpuAvgManager.GetCpuAvg();
            tokenManager.AddTokens(cpuAvg);
            Label1Out(tokenManager.DailyTokens);
            cpuAvgManager.Clear();
        }

        /// <summary>
        /// CPU�g�p���̕��ς����߂���悤�ɐ��l�𑫂�
        /// </summary>
        private void SumCpuAvg()
        {
            double cpuUsage = (double)CpuWatcher.GetCpuUsage();
            cpuAvgManager.SetCpuSum(cpuUsage);
            Label2Out(cpuUsage);
            cpuCnt++;
        }

        /// <summary>
        /// �����̋C���𕶎��ɕϊ�����
        /// </summary>
        /// <param name="feeling">�����̋C��(double)</param>
        /// <returns></returns>
        private string GetFeeling(double feeling)
        {
            double feel = feeling;
            if (feel > 1.0) feel = 1.0;
            if (feel < -1.0) feel = -1.0;

            string feelingText;

            if (feel < -0.49)
            {
                feelingText = FEELING_STRING[0];
            }
            else if (feel < 0.0)
            {
                feelingText = FEELING_STRING[1];
            }
            else if (feel < 0.3)
            {
                feelingText = FEELING_STRING[2];
            }
            else
            {
                feelingText = FEELING_STRING[3];
            }

            return feelingText;
        }

        /// <summary>
        /// �����̃g�[�N�����o�͂���(�e�X�g)
        /// </summary>
        /// <param name="value">�o�͂���g�[�N��</param>
        private void Label1Out(double value)
        {
            TestLabel1.Text = GetFeeling(tokenManager.Feeling);
            DailyTokenLabel.Text = "�����̊l���g�[�N��: " + tokenManager.DailyTokens.ToString("n2");
            EmoTokenLabel.Text = "�����̊���g�[�N��: " + tokenManager.EmotionTokens.ToString("n2");
            AverageEmotionTokensLabel.Text = "���ϊ���g�[�N��: " + tokenManager.AverageEmotionTokens.ToString("n2");
            FeelingLabel.Text = "�����̊���: " + tokenManager.Feeling.ToString("n2");
            FlopsLabel.Text = "FLOPS: " + tokenManager.Flops.ToString();
            KibunLabelOut(value);
        }

        /// <summary>
        /// ���݂�CPU�g�p�����o�͂���(�e�X�g)
        /// </summary>
        /// <param name="value">�o�͂���CPU�g�p��</param>
        private void Label2Out(double value)
        {
            TestLabel2.Text = "CPU: " + value.ToString("n2") + "%";
        }

        /// <summary>
        /// �g�[�N���̊l���ʂ��o�͂���(�e�X�g)
        /// </summary>
        /// <param name="value">�o�͂���g�[�N���̊l����</param>
        private void KibunLabelOut(double value)
        {
            KibunLabel.Text = "�݌v�g�[�N��: " + (tokenManager.TotalTokens).ToString("n2");
        }

        private void ToggleShowButton_Click(object sender, EventArgs e)
        {
            foreach (Label targetLabel in labelArray)
            {
                targetLabel.Visible = !targetLabel.Visible;
            }
        }
    }
}
