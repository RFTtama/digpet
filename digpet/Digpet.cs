using digpet.Managers;
using digpet.Models.AbstractModels;
using digpet.Modules;
using digpet.TaskTimerClass;
using digpet.TimerClass;

namespace digpet
{
    public partial class Digpet : Form
    {
        //�N���X�֘A�̐錾
        private TokenManager tokenManager;
        private CharZipFileManager charZipFileManager;

        //�ϐ��֘A�̐錾
        private bool gotNormalImage;                                    //����ɉ摜��؂�ւ��邱�Ƃ��ł�����

        //�萔�֘A�̐錾
        private const int FONT_MARGIN_SIZE = 5;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public Digpet(ClassManagerArg arg)
        {
            InitializeComponent();

            tokenManager = arg.tokenManager;
            charZipFileManager = arg.charZipFileManager;
            arg.CpuUsageLabelUpdate = UpdateCpuLabel;

            Init();
            LogLib.LogOutput("���������������܂���");
        }

        /// <summary>
        /// ������
        /// </summary>
        private void Init()
        {
            Text += "   Ver." + SettingManager.PrivateSettings.APPLICATION_VERSION;
            MouseWheel += new MouseEventHandler(MouseWheelEvent);
            gotNormalImage = true;

            CheckResetTime();
            tokenManager.ReadTokens();
            ReadCharConfig();
            SetNowWindowState();

            ImageChangeTimer.Enabled = true;
        }

        /// <summary>
        /// CpuUsageLabel�̍X�V����
        /// </summary>
        /// <param name="text">�X�V����e�L�X�g</param>
        public void UpdateCpuLabel(string label)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => CpuUsageLabel.Text = label));
                Invoke(new Action(UpdateDetailLabels));
            }
            else
            {
                CpuUsageLabel.Text = label;
                UpdateDetailLabels();
            }
        }

        /// <summary>
        /// �����̃g�[�N�����o�͂���
        /// </summary>
        private void UpdateDetailLabels()
        {
            EmoStringLabel.Text = charZipFileManager.GetFeelingTag() + GetFeeling(tokenManager.Feeling);
            DailyTokenLabel.Text = "�����̊l���g�[�N��: " + tokenManager.DailyTokens.ToString("n2");
            EmoTokenLabel.Text = "�����̊���g�[�N��: " + tokenManager.EmotionTokens.ToString("n2");
            AverageEmotionTokensLabel.Text = "���ϊ���g�[�N��: " + tokenManager.AverageEmotionTokens.ToString("n2");
            FeelingLabel.Text = "�����̊���: " + tokenManager.Feeling.ToString("n2");
            FlopsLabel.Text = "FLOPS: " + tokenManager.Flops.ToString();
            TotalTokenLabel.Text = "�݌v�g�[�N��: " + (tokenManager.TotalTokens).ToString("n2");
            IntimacyLabel.Text = charZipFileManager.GetIntimacyTag() + GetIntimacy(tokenManager.TotalTokens);
            LogLib.LogOutput("�\������Ă���g�[�N�����̍X�V����");
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

            SettingManager.PublicSettings.ImageSize = picSize;
            SettingManager.WriteSettingFile(SettingManager.PrivateSettings.SETTING_PATH);

            UpdateImageSize();
        }

        /// <summary>
        /// �g�[�N�������Z�b�g���鎞�����ݒ肳��Ă��邩�m�F����
        /// </summary>
        private void CheckResetTime()
        {
            int resetHour = SettingManager.PublicSettings.ResetHour;

            if (resetHour < 0)
            {
                resetHour = SetResetTime();
                SettingManager.WriteSettingFile(SettingManager.PrivateSettings.SETTING_PATH);
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
                    "���Z�b�g�����ݒ�", "0");

                int hour = -1;

                if (int.TryParse(input, out hour) == true)
                {
                    if (hour >= 0 && hour < 24)
                    {
                        SettingManager.PublicSettings.ResetHour = hour;
                        LogLib.LogOutput("���Z�b�g������" + hour.ToString() + "�ɐݒ肵�܂���");
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
            if (SettingManager.PublicSettings.CharSettingPath == null)
            {
                ErrorLogLib.ErrorOutput("�L�����t�@�C���ǂݎ��G���[", "�ݒ肳��Ă���L�����t�@�C���̃p�X��null����ł�");
            }
            else if (SettingManager.PublicSettings.CharSettingPath == string.Empty)
            {
                return;
            }
            else
            {
                charZipFileManager.ReadCharSettings(SettingManager.PublicSettings.CharSettingPath);
                ImageChangeTimer.Interval = charZipFileManager.GetPictureTurnOverPeriod();
                SetControlColor(charZipFileManager.GetControlColor());
            }
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
        /// ���݂̏�Ԃɉ����ĉ摜��؂�ւ���
        /// </summary>
        private void ChangeImage()
        {
            string intimacy = GetIntimacy(tokenManager.TotalTokens);
            string feeling = GetFeeling(tokenManager.Feeling);

            Image? image = charZipFileManager.GetCharImage(intimacy, feeling);

            if ((image == null) && (gotNormalImage == true))
            {
                LogLib.LogOutput("�摜���ݒ肳��܂���ł���");
                return;
            }

            //null�̏ꍇ�͐���ȉ摜�ł͂Ȃ��̂ŁA�t���O���I�t��
            if (image == null)
            {
                gotNormalImage = false;
            }
            else
            {
                gotNormalImage = true;
            }

            CharPictureBox.Image = image;
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
            SettingManager.PublicSettings.CharSettingPath = path;
            SettingManager.WriteSettingFile(SettingManager.PrivateSettings.SETTING_PATH);
            ReadCharConfig();
        }

        /// <summary>
        /// �C���|�[�g�{�^���̃N���b�N
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportButton_Click(object sender, EventArgs e)
        {
            LogLib.LogOutput("�C���|�[�g�{�^�����N���b�N����܂���");
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "ZIP�t�@�C��(*.zip)|*.zip;";
            ofd.Title = "�C���|�[�g����L�����f�[�^��I�����Ă�������";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ReWriteCharConfig(ofd.FileName);
            }
            else
            {
                LogLib.LogOutput("�L�����f�[�^�̃C���|�[�g���s");
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
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
            {
                return;
            }
            LogLib.LogOutput("�N���A�{�^�����N���b�N����܂���");
            ReWriteCharConfig(string.Empty);
            this.Close();
        }

        /// <summary>
        /// �t�H�[���I�����̓���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveNowWindowState();
            SettingManager.WriteSettingFile(SettingManager.PrivateSettings.SETTING_PATH);
            LogLib.LogOutput("�A�v���̏I�������I��");
        }

        /// <summary>
        /// ���݂̃E�B���h�E�̏�Ԃ�ݒ�t�@�C���ɂ܂Ƃ߂�
        /// </summary>
        private void SaveNowWindowState()
        {
            SettingManager.PublicSettings.WindowLocation = Location;
            SettingManager.PublicSettings.WindowSize = Size;
            SettingManager.PublicSettings.WindowState = GetWindowStateId();
        }

        /// <summary>
        /// ���݂̃E�B���h�E�̏�Ԃ�ݒ肷��
        /// </summary>
        private void SetNowWindowState()
        {
            Location = SettingManager.PublicSettings.WindowLocation;
            Size = SettingManager.PublicSettings.WindowSize;
            WindowState = GetWindowState();
            this.TopMost = SettingManager.PublicSettings.TopMost;
            SetControlFontSize();
            LogLib.LogOutput("�ݒ�𕜌����܂���");
        }

        /// <summary>
        /// �t�H���g�̃T�C�Y��ݒ�ɉ����Ċg�傷��
        /// </summary>
        private void SetControlFontSize()
        {
            int enlarge = SettingManager.PublicSettings.FontEnlargeSize;

            if (enlarge > 0)
            {
                SetSatsPanelControlSize(enlarge);
                SetGeneralLabelControlSize(enlarge);
                SetButtonControlSize(enlarge);
                LogLib.LogOutput("�t�H���g�̑傫���Đݒ�I��");
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

            switch (SettingManager.PublicSettings.WindowState)
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
            LogLib.LogOutput("�R���g���[���̐F��" + color.ToString() + "�ɐݒ肵�܂���");
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
            CharPictureBox.Size = SettingManager.PublicSettings.ImageSize;
            CharPictureBox.Left = (Width / 2) - (CharPictureBox.Width / 2) - 10;
            CharPictureBox.Top = (Height / 2) - (CharPictureBox.Height / 2) - 10;
        }
    }
}
