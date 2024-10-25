using System.Security.Cryptography;
using System.Windows.Forms;

namespace digpet
{
    public partial class Form1 : Form
    {
        //�N���X�֘A�̐錾
        private CpuAvgManager cpuAvgManager = new CpuAvgManager();
        private TokenManager tokenManager = new TokenManager();
        private SettingManager settingManager = new SettingManager();
        private CharZipFileManager charZipFileManager = new CharZipFileManager();

        //�ϐ��֘A�̐錾
        private int cpuCnt;
        private double cpuAvg;

        //�萔�֘A�̐錾
        private const string SETTING_PATH = "settings.json";
        private const int FONT_MARGIN_SIZE = 5;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            Text += "   Ver." + APP_SETTINGS.APPLICATION_VERSION;
            MouseWheel += new MouseEventHandler(MouseWheelEvent);
            Init();
            settingManager.ReadSettingFile(SETTING_PATH);
            CheckResetTime();
            tokenManager.ReadTokens();
            ReadCharConfig();
            SetNowWindowState();
            CpuUsageTimer.Enabled = true;
            ImageChangeTimer.Enabled = true;
            LogManager.LogOutput("���������������܂���");
        }

        /// <summary>
        /// ������
        /// </summary>
        private void Init()
        {
            cpuCnt = 0;
            cpuAvg = 0.0;
        }

        /// <summary>
        /// �}�E�X�z�C�[�������邭�邵�����̃C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseWheelEvent(object? sender, MouseEventArgs e)
        {
            if (CharPictureBox.Image == null)
            {
                return;
            }
            Size picSize = new Size(CharPictureBox.Width, CharPictureBox.Height);
            const float MINUS_MAGN = 0.9f;
            const float PLUS_MAGN = 1.1f;

            if (e.Delta > 0)
            {
                picSize.Width = (int)(picSize.Width * PLUS_MAGN);
                picSize.Height = (int)(picSize.Height * PLUS_MAGN);
            }
            else if (e.Delta < 0)
            {
                picSize.Width = (int)(picSize.Width * MINUS_MAGN);
                picSize.Height = (int)(picSize.Height * MINUS_MAGN);
            }

            settingManager.Settings.ImageSize = picSize;
            settingManager.WriteSettingFile(SETTING_PATH);

            UpdateImageSize();
        }

        /// <summary>
        /// �g�[�N�������Z�b�g���鎞�����ݒ肳��Ă��邩�m�F����
        /// </summary>
        private void CheckResetTime()
        {
            int resetHour = settingManager.Settings.ResetHour;

            if (resetHour < 0)
            {
                resetHour = SetResetTime();
                settingManager.WriteSettingFile(SETTING_PATH);
            }

            tokenManager.ResetHour = resetHour;
        }

        /// <summary>
        /// ���Z�b�g������V�����ݒ肷��
        /// </summary>
        /// <returns>���Z�b�g����</returns>
        private int SetResetTime()
        {
            while (true)
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox("�g�[�N�������Z�b�g���鎞����0�`23�Őݒ肵�Ă�������",
                    "���Z�b�g������", "0");

                int hour = -1;

                if (int.TryParse(input, out hour) == true)
                {
                    if (hour >= 0 && hour < 24)
                    {
                        settingManager.Settings.ResetHour = hour;
                        LogManager.LogOutput("���Z�b�g������" + hour.ToString() + "�ɐݒ肵�܂���");
                        return hour;
                    }
                }
            }
        }

        /// <summary>
        /// �L�����N�^�[�̃R���t�B�O�f�[�^��ǂݎ��
        /// </summary>
        private void ReadCharConfig()
        {
            if (settingManager.Settings.CharSettingPath == null)
            {
                ErrorLog.ErrorOutput("�L�����t�@�C���ǂݎ��G���[", "�ݒ肳��Ă���L�����t�@�C���̃p�X��null����ł�");
            }
            else if (settingManager.Settings.CharSettingPath == string.Empty)
            {
                return;
            }
            else
            {
                charZipFileManager.ReadCharSettings(settingManager.Settings.CharSettingPath);
                ImageChangeTimer.Interval = charZipFileManager.GetPictureTurnOverPeriod();
                SetControlColor(charZipFileManager.GetControlColor());
            }
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
                    cpuCnt = 0;
                    GetCpuAvg();
                }
                catch (Exception ex)
                {
                    ErrorLog.ErrorOutput("CPU�g�p�����όv�Z�G���[", ex.Message);
                    CpuUsageTimer.Enabled = false;
                }
            }
            else
            {
                //CPU�g�p�������Z
                SumCpuAvg();
            }
            cpuCnt++;
        }

        /// <summary>
        /// CPU�g�p���̕��ς�����cpuAvg�ɑ������
        /// </summary>
        private void GetCpuAvg()
        {
            cpuAvg = cpuAvgManager.GetCpuAvg();
            tokenManager.AddTokens(cpuAvg);
            OutTokenLabel();
            cpuAvgManager.Clear();
            LogManager.LogOutput("�����g�[�N���̎Z�o����");
        }

        /// <summary>
        /// CPU�g�p���̕��ς����߂���悤�ɐ��l�𑫂�
        /// </summary>
        private void SumCpuAvg()
        {
            double cpuUsage = (double)CpuWatcher.GetCpuUsage();
            cpuAvgManager.SetCpuSum(cpuUsage);
            OutCpuLabel(cpuUsage);
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

            return charZipFileManager.GetFeelingString(feel);
        }

        /// <summary>
        /// ���݂̐e���x�𕶎��ɕϊ�����
        /// </summary>
        /// <param name="intimacy">�e���x</param>
        /// <returns></returns>
        private string GetIntimacy(double intimacy)
        {
            double inti = intimacy;
            if (inti < 0.0) inti = 0.0;

            return charZipFileManager.GetIntimacyString(inti);
        }

        /// <summary>
        /// �����̃g�[�N�����o�͂���(�e�X�g)
        /// </summary>
        /// <param name="value">�o�͂���g�[�N��</param>
        private void OutTokenLabel()
        {
            EmoStringLabel.Text = charZipFileManager.GetFeelingTag() + GetFeeling(tokenManager.Feeling);
            DailyTokenLabel.Text = "�����̊l���g�[�N��: " + tokenManager.DailyTokens.ToString("n2");
            EmoTokenLabel.Text = "�����̊���g�[�N��: " + tokenManager.EmotionTokens.ToString("n2");
            AverageEmotionTokensLabel.Text = "���ϊ���g�[�N��: " + tokenManager.AverageEmotionTokens.ToString("n2");
            FeelingLabel.Text = "�����̊���: " + tokenManager.Feeling.ToString("n2");
            FlopsLabel.Text = "FLOPS: " + tokenManager.Flops.ToString();
            TotalTokenLabel.Text = "�݌v�g�[�N��: " + (tokenManager.TotalTokens).ToString("n2");
            IntimacyLabel.Text = charZipFileManager.GetIntimacyTag() + GetIntimacy(tokenManager.TotalTokens);
            LogManager.LogOutput("�\������Ă���g�[�N�����̍X�V����");
        }

        /// <summary>
        /// ���݂̏�Ԃɉ����ĉ摜��؂�ւ���
        /// </summary>
        private void ChangeImage()
        {
            string intimacy = GetIntimacy(tokenManager.TotalTokens);
            string feeling = GetFeeling(tokenManager.Feeling);

            Image? image = charZipFileManager.GetCharImage(intimacy, feeling);

            if (image == null)
            {
                LogManager.LogOutput("�摜���ݒ肳��܂���ł���");
                return;
            }

            LogManager.LogOutput("�摜��" + image.ToString() + "�ɐݒ肳��܂���");

            CharPictureBox.Image = image;
        }

        /// <summary>
        /// ���݂�CPU�g�p�����o�͂���(�e�X�g)
        /// </summary>
        /// <param name="value">�o�͂���CPU�g�p��</param>
        private void OutCpuLabel(double value)
        {
            CpuUsageLabel.Text = "CPU: " + value.ToString("n2") + "%";
        }

        /// <summary>
        /// ���v���̕\����؂�ւ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleShowButton_Click(object sender, EventArgs e)
        {
            if ((StatsPanel.Visible == true) && (DefaultPanel.Visible == true))
            {
                StatsPanel.Visible = !StatsPanel.Visible;
                ClearButton.Visible = !ClearButton.Visible;
                ImportButton.Visible = !ImportButton.Visible;
            }
            else if ((StatsPanel.Visible == false) && (DefaultPanel.Visible == true))
            {
                DefaultPanel.Visible = !DefaultPanel.Visible;
            }
            else
            {
                StatsPanel.Visible = !StatsPanel.Visible;
                ClearButton.Visible = !ClearButton.Visible;
                ImportButton.Visible = !ImportButton.Visible;
                DefaultPanel.Visible = !DefaultPanel.Visible;
            }
        }

        /// <summary>
        /// �L�����t�@�C���̃p�X����������ł���ēǂݍ��݂���
        /// </summary>
        /// <param name="path">�L�����t�@�C���p�X</param>
        private void ReWriteCharConfig(string path)
        {
            settingManager.Settings.CharSettingPath = path;
            settingManager.WriteSettingFile(SETTING_PATH);
            ReadCharConfig();
        }

        /// <summary>
        /// �C���|�[�g�{�^���̃N���b�N
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportButton_Click(object sender, EventArgs e)
        {
            LogManager.LogOutput("�C���|�[�g�{�^�����N���b�N����܂���");
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "ZIP�t�@�C��(*.zip)|*.zip;";
            ofd.Title = "�C���|�[�g����L�����f�[�^��I�����Ă�������";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ReWriteCharConfig(ofd.FileName);
            }
            else
            {
                LogManager.LogOutput("�L�����f�[�^�̃C���|�[�g���s");
                MessageBox.Show("�L�����f�[�^�̃C���|�[�g�Ɏ��s���܂���", "�C���|�[�g�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// �L�����f�[�^�̎Q�Ƃ��N���A
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_Click(object sender, EventArgs e)
        {
            DialogResult result =
                MessageBox.Show("�L�����ݒ���N���A���܂�\n�N���A��A�v�����I�����܂�����낵���ł���?", "�L�����ݒ���N���A���܂���?",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
            {
                return;
            }
            LogManager.LogOutput("�N���A�{�^�����N���b�N����܂���");
            ReWriteCharConfig(string.Empty);
        }

        /// <summary>
        /// �t�H�[���I�����̓���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveNowWindowState();
            settingManager.WriteSettingFile(SETTING_PATH);
            LogManager.LogOutput("�A�v���̏I�������I��");
        }

        /// <summary>
        /// ���݂̃E�B���h�E�̏�Ԃ�ݒ�t�@�C���ɂ܂Ƃ߂�
        /// </summary>
        private void SaveNowWindowState()
        {
            settingManager.Settings.WindowLocation = Location;
            settingManager.Settings.WindowSize = Size;
            settingManager.Settings.WindowState = GetWindowStateId();
        }

        /// <summary>
        /// ���݂̃E�B���h�E�̏�Ԃ�ݒ肷��
        /// </summary>
        private void SetNowWindowState()
        {
            Location = settingManager.Settings.WindowLocation;
            Size = settingManager.Settings.WindowSize;
            WindowState = GetWindowState();
            SetControlFontSize();
            LogManager.LogOutput("�ݒ�𕜌����܂���");
        }

        /// <summary>
        /// �t�H���g�̃T�C�Y��ݒ�ɉ����Ċg�傷��
        /// </summary>
        private void SetControlFontSize()
        {
            int enlarge = settingManager.Settings.FontEnlargeSize;

            if (enlarge > 0)
            {
                SetSatsPanelControlSize(enlarge);
                SetGeneralLabelControlSize(enlarge);
                SetButtonControlSize(enlarge);
                LogManager.LogOutput("�t�H���g�̑傫���Đݒ�I��");
            }
        }

        /// <summary>
        /// StatsPanel�̃R���g���[���ɑ΂���t�H���g�T�C�Y�ݒ�
        /// </summary>
        /// <param name="enlarge">�g��T�C�Y</param>
        private void SetSatsPanelControlSize(int enlarge)
        {
            Control[] controls =
                {
                    StatsLabel,
                    DailyTokenLabel,
                    EmoTokenLabel,
                    AverageEmotionTokensLabel,
                    TotalTokenLabel,
                    FeelingLabel,
                    FlopsLabel
                };

            for (int panelInd = 0; panelInd < controls.Length; panelInd++)
            {
                Control nowCont = controls[panelInd];

                nowCont.Font = new Font(nowCont.Font.Name, nowCont.Font.Size + enlarge);

                if (panelInd > 0)
                {
                    Control befCont = controls[panelInd - 1];

                    nowCont.Location = new Point(befCont.Location.X, befCont.Location.Y + nowCont.Height + FONT_MARGIN_SIZE);
                }
                else
                {
                    //�ʒu��[�ɍ��킹��
                    nowCont.Top = 0;
                    nowCont.Left = 0;
                }
            }
        }

        /// <summary>
        /// ���x���̃R���g���[���ɑ΂���t�H���g�T�C�Y�ݒ�
        /// </summary>
        /// <param name="enlarge">�g��T�C�Y</param>
        private void SetGeneralLabelControlSize(int enlarge)
        {
            Control[] controls =
            {
                CpuUsageLabel,
                IntimacyLabel,
                EmoStringLabel
            };

            for (int panelInd = 0; panelInd < controls.Length; panelInd++)
            {
                Control nowCont = controls[panelInd];

                nowCont.Font = new Font(nowCont.Font.Name, nowCont.Font.Size + enlarge);

                if (panelInd > 0)
                {
                    Control befCont = controls[panelInd - 1];

                    nowCont.Location = new Point(befCont.Location.X, befCont.Location.Y + nowCont.Height + FONT_MARGIN_SIZE);
                }
                else
                {
                    //�ʒu��[�ɍ��킹��
                    nowCont.Top = 0;
                    nowCont.Left = 0;
                }
            }
        }

        /// <summary>
        /// �{�^���̃R���g���[���ɑ΂���t�H���g�T�C�Y�ݒ�
        /// </summary>
        /// <param name="enlarge">�g��T�C�Y</param>
        private void SetButtonControlSize(int enlarge)
        {
            Control[] controls =
            {
                ToggleShowButton,
                ClearButton,
                ImportButton,
            };

            int maxWidth = 0;

            for (int panelInd = 0; panelInd < controls.Length; panelInd++)
            {
                Control nowCont = controls[panelInd];

                nowCont.Font = new Font(nowCont.Font.Name, nowCont.Font.Size + enlarge);

                if (nowCont.Width > maxWidth)
                {
                    maxWidth = nowCont.Width;
                }

                if (panelInd > 0)
                {
                    Control befCont = controls[panelInd - 1];

                    nowCont.Location = new Point(nowCont.Location.X, befCont.Location.Y - nowCont.Height - FONT_MARGIN_SIZE);
                }
                else
                {
                    nowCont.Top -= enlarge;
                }
            }

            if (maxWidth > 0)
            {
                foreach (Control control in controls)
                {
                    control.Width = maxWidth;
                    control.Left = Size.Width - maxWidth - 28;
                }
            }
        }

        /// <summary>
        /// �E�B���h�E�̏��ID��ԋp����
        /// </summary>
        /// <returns>0: �ʏ�, 1: �ő剻, 2: �ŏ���</returns>
        private int GetWindowStateId()
        {
            int wstate = 0;

            switch (WindowState)
            {
                case FormWindowState.Normal:
                    wstate = 0;
                    break;

                case FormWindowState.Maximized:
                    wstate = 1;
                    break;

                case FormWindowState.Minimized:
                    wstate = 2;
                    break;

                default:
                    wstate = 0;
                    break;
            }

            return wstate;
        }

        /// <summary>
        /// �E�B���h�E�̏�Ԃ�ԋp����
        /// </summary>
        /// <returns>�ʏ�A�ő剻�A�ŏ���</returns>
        private FormWindowState GetWindowState()
        {
            FormWindowState loadState = FormWindowState.Normal;

            switch (settingManager.Settings.WindowState)
            {
                case 0:
                    loadState = FormWindowState.Normal;
                    break;

                case 1:
                    loadState = FormWindowState.Maximized;
                    break;


                case 2:
                    loadState = FormWindowState.Minimized;
                    break;

                default:
                    loadState = FormWindowState.Normal;
                    break;
            }

            return loadState;
        }

        /// <summary>
        /// �R���g���[���̐F��ݒ肷��
        /// </summary>
        /// <param name="color">�F</param>
        private void SetControlColor(Color color)
        {
            LogManager.LogOutput("�R���g���[���̐F��" + color.ToString() + "�ɐݒ肵�܂���");
            BackColor = color;
        }

        /// <summary>
        /// �摜�̍X�V�^�C�}����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageChangeTimer_Tick(object sender, EventArgs e)
        {
            ChangeImage();
        }

        /// <summary>
        /// �E�B���h�E�̃T�C�Y�ύX��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            UpdateImageSize();
        }

        /// <summary>
        /// �摜�̃T�C�Y�ƈʒu���X�V����
        /// </summary>
        private void UpdateImageSize()
        {
            CharPictureBox.Size = settingManager.Settings.ImageSize;
            CharPictureBox.Left = (Width / 2) - (CharPictureBox.Width / 2) - 10;
            CharPictureBox.Top = (Height / 2) - (CharPictureBox.Height / 2) - 10;
        }
    }
}
