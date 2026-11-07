using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace UniPinyin
{
    public class SettingsForm : Form
    {
        private readonly Settings _settings;
        private readonly Action _onSettingsUpdated;

        private Label lblStatus;
        private ComboBox cboCharSet;
        private ComboBox cboHotkey;
        private ComboBox cboDataSource;
        private ComboBox cboCandidateCount;
        private CheckBox chkSpaceSelectFirst;
        private CheckBox chkAutoFollowCaret;
        private CheckBox chkStartWithWindows;
        private CheckBox chkShowNotifications;
        private CheckBox chkPlaySoundOnToggle;
        private TextBox txtTestInput;

        public SettingsForm(Settings settings, Action onSettingsUpdated)
        {
            _settings = settings;
            _onSettingsUpdated = onSettingsUpdated;

            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch { }

            InitializeComponent();
            LoadDataFromSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "UniPinyin - Chinese IME Control Panel";
            this.Size = new Size(505, 555);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            this.BackColor = Color.FromArgb(245, 246, 248);

            int startY = 12;

            // Header Banner
            Panel pnlHeader = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(505, 56),
                BackColor = Color.FromArgb(24, 103, 192)
            };

            int titleX = 14;
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_icon.png");
            if (File.Exists(iconPath))
            {
                try
                {
                    PictureBox picLogo = new PictureBox
                    {
                        Image = Image.FromFile(iconPath),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Location = new Point(12, 9),
                        Size = new Size(38, 38)
                    };
                    pnlHeader.Controls.Add(picLogo);
                    titleX = 56;
                }
                catch { }
            }

            Label lblTitle = new Label
            {
                Text = "UniPinyin - Chinese Pinyin IME (Google / US Keyboard)",
                Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(titleX, 8),
                AutoSize = true
            };

            Label lblSubtitle = new Label
            {
                Text = "Lightweight, fast, Google Cloud suggestions, system-wide support",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(210, 230, 255),
                Location = new Point(titleX + 1, 32),
                AutoSize = true
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            this.Controls.Add(pnlHeader);

            startY = 66;

            // Group 1: Main Controls
            GroupBox grpMain = new GroupBox
            {
                Text = "Main Controls",
                Location = new Point(12, startY),
                Size = new Size(465, 115)
            };

            Label lblStatusTitle = new Label { Text = "Status:", Location = new Point(16, 24), AutoSize = true };
            lblStatus = new Label
            {
                Text = _settings.IsEnabled ? "● ON [Chinese - 中]" : "○ OFF [English - EN]",
                ForeColor = _settings.IsEnabled ? Color.DarkGreen : Color.Gray,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Location = new Point(140, 24),
                AutoSize = true
            };

            Label lblCharSet = new Label { Text = "Character Set:", Location = new Point(16, 52), AutoSize = true };
            cboCharSet = new ComboBox
            {
                Location = new Point(140, 49),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboCharSet.Items.Add("Simplified Chinese (简体) - Mainland Standard");
            cboCharSet.Items.Add("Traditional Chinese (繁體) - Taiwan / HK");

            Label lblHotkey = new Label { Text = "Toggle Hotkey:", Location = new Point(16, 82), AutoSize = true };
            cboHotkey = new ComboBox
            {
                Location = new Point(140, 79),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboHotkey.Items.Add("Ctrl + Shift (UniKey style)");
            cboHotkey.Items.Add("Ctrl + Space");
            cboHotkey.Items.Add("Alt + Z (Default)");

            grpMain.Controls.Add(lblStatusTitle);
            grpMain.Controls.Add(lblStatus);
            grpMain.Controls.Add(lblCharSet);
            grpMain.Controls.Add(cboCharSet);
            grpMain.Controls.Add(lblHotkey);
            grpMain.Controls.Add(cboHotkey);
            this.Controls.Add(grpMain);

            startY += 122;

            // Group 2: Suggestion & Candidate Bar
            GroupBox grpOptions = new GroupBox
            {
                Text = "Dictionary & Suggestion Bar",
                Location = new Point(12, startY),
                Size = new Size(465, 125)
            };

            Label lblSource = new Label { Text = "Suggestion Source:", Location = new Point(16, 24), AutoSize = true };
            cboDataSource = new ComboBox
            {
                Location = new Point(140, 21),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboDataSource.Items.Add("Google Cloud Suggestion + Offline Dict (Recommended)");
            cboDataSource.Items.Add("Offline Dictionary Only (No Internet)");

            Label lblCandCount = new Label { Text = "Candidates / Page:", Location = new Point(16, 54), AutoSize = true };
            cboCandidateCount = new ComboBox
            {
                Location = new Point(140, 51),
                Width = 140,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboCandidateCount.Items.Add("5 candidates (Compact)");
            cboCandidateCount.Items.Add("9 candidates (Full)");

            chkSpaceSelectFirst = new CheckBox
            {
                Text = "Press Spacebar to select 1st candidate",
                Location = new Point(18, 85),
                AutoSize = true
            };

            chkAutoFollowCaret = new CheckBox
            {
                Text = "Candidate bar automatically follows caret/cursor",
                Location = new Point(18, 103),
                AutoSize = true
            };

            grpOptions.Controls.Add(lblSource);
            grpOptions.Controls.Add(cboDataSource);
            grpOptions.Controls.Add(lblCandCount);
            grpOptions.Controls.Add(cboCandidateCount);
            grpOptions.Controls.Add(chkSpaceSelectFirst);
            grpOptions.Controls.Add(chkAutoFollowCaret);
            this.Controls.Add(grpOptions);

            startY += 132;

            // Group 3: System & Testing
            GroupBox grpSystem = new GroupBox
            {
                Text = "System & Live Typing Test",
                Location = new Point(12, startY),
                Size = new Size(465, 135)
            };

            chkStartWithWindows = new CheckBox
            {
                Text = "Start with Windows",
                Location = new Point(18, 22),
                AutoSize = true
            };

            chkShowNotifications = new CheckBox
            {
                Text = "Show notification on toggle",
                Location = new Point(230, 22),
                AutoSize = true
            };

            chkPlaySoundOnToggle = new CheckBox
            {
                Text = "Play sound beep on toggle",
                Location = new Point(18, 48),
                AutoSize = true
            };

            Label lblTest = new Label
            {
                Text = "Live typing test (e.g. type 'nihao', 'xiexie', 'zhongguo' here):",
                Location = new Point(16, 76),
                AutoSize = true,
                ForeColor = Color.FromArgb(80, 80, 80)
            };

            txtTestInput = new TextBox
            {
                Location = new Point(18, 98),
                Width = 422,
                Font = new Font("Microsoft YaHei", 10.5f, FontStyle.Regular)
            };

            grpSystem.Controls.Add(chkStartWithWindows);
            grpSystem.Controls.Add(chkShowNotifications);
            grpSystem.Controls.Add(chkPlaySoundOnToggle);
            grpSystem.Controls.Add(lblTest);
            grpSystem.Controls.Add(txtTestInput);
            this.Controls.Add(grpSystem);

            startY += 144;

            // Buttons Bar
            Button btnSave = new Button
            {
                Text = "Save & Close to Tray",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Location = new Point(12, startY),
                Size = new Size(155, 34),
                BackColor = Color.FromArgb(24, 103, 192),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;

            Button btnWeb = new Button
            {
                Text = "Open Web IME",
                Location = new Point(175, startY),
                Size = new Size(130, 34)
            };
            btnWeb.Click += BtnWeb_Click;

            Button btnHelp = new Button
            {
                Text = "Help / Guide",
                Location = new Point(313, startY),
                Size = new Size(82, 34)
            };
            btnHelp.Click += BtnHelp_Click;

            Button btnExit = new Button
            {
                Text = "Exit",
                Location = new Point(402, startY),
                Size = new Size(75, 34)
            };
            btnExit.Click += (s, e) => Application.Exit();

            this.Controls.Add(btnSave);
            this.Controls.Add(btnWeb);
            this.Controls.Add(btnHelp);
            this.Controls.Add(btnExit);

            this.FormClosing += SettingsForm_FormClosing;
        }

        private void LoadDataFromSettings()
        {
            lblStatus.Text = _settings.IsEnabled ? "● ON [Chinese - 中]" : "○ OFF [English - EN]";
            lblStatus.ForeColor = _settings.IsEnabled ? Color.DarkGreen : Color.Gray;

            cboCharSet.SelectedIndex = _settings.IsSimplified ? 0 : 1;
            cboHotkey.SelectedIndex = Math.Min(2, Math.Max(0, _settings.HotkeyMode));
            cboDataSource.SelectedIndex = _settings.UseGoogleOnline ? 0 : 1;
            cboCandidateCount.SelectedIndex = _settings.CandidateCount == 9 ? 1 : 0;
            chkSpaceSelectFirst.Checked = _settings.SpaceSelectFirst;
            chkAutoFollowCaret.Checked = _settings.AutoFollowCaret;
            chkStartWithWindows.Checked = _settings.StartWithWindows;
            chkShowNotifications.Checked = _settings.ShowNotifications;
            chkPlaySoundOnToggle.Checked = _settings.PlaySoundOnToggle;
        }

        public void RefreshStatus(bool isEnabled)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => RefreshStatus(isEnabled)));
                return;
            }
            lblStatus.Text = isEnabled ? "● ON [Chinese - 中]" : "○ OFF [English - EN]";
            lblStatus.ForeColor = isEnabled ? Color.DarkGreen : Color.Gray;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            _settings.IsSimplified = (cboCharSet.SelectedIndex == 0);
            _settings.HotkeyMode = cboHotkey.SelectedIndex;
            _settings.UseGoogleOnline = (cboDataSource.SelectedIndex == 0);
            _settings.CandidateCount = (cboCandidateCount.SelectedIndex == 1) ? 9 : 5;
            _settings.SpaceSelectFirst = chkSpaceSelectFirst.Checked;
            _settings.AutoFollowCaret = chkAutoFollowCaret.Checked;
            _settings.StartWithWindows = chkStartWithWindows.Checked;
            _settings.ShowNotifications = chkShowNotifications.Checked;
            _settings.PlaySoundOnToggle = chkPlaySoundOnToggle.Checked;

            _settings.Save();

            if (_onSettingsUpdated != null)
            {
                _onSettingsUpdated();
            }

            this.Hide();
        }

        private void BtnWeb_Click(object sender, EventArgs e)
        {
            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UniPinyin_Web_EN.html");
            if (!File.Exists(htmlPath))
            {
                htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UniPinyin_Web.html");
            }

            if (File.Exists(htmlPath))
            {
                Process.Start(new ProcessStartInfo(htmlPath) { UseShellExecute = true });
            }
            else
            {
                MessageBox.Show("Could not find web IME HTML file.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnHelp_Click(object sender, EventArgs e)
        {
            string msg = "UNIPINYIN USER GUIDE:\n\n" +
                         "1. Typing Pinyin with Tone Accents (TELEX-style):\n" +
                         "   - 'ü' (u with dots): type 'w' or 'u+w' (e.g. 'lw' -> 'lü', 'nw' -> 'nü', 'luw' -> 'lü').\n" +
                         "   - Tone 1 (flat / macron 'ā'): type 'x' (e.g. 'lax' -> 'lā')\n" +
                         "   - Tone 2 (rising / acute 'á'): type 's' (e.g. 'mas' -> 'má')\n" +
                         "   - Tone 3 (caron / hook 'ǎ'): type 'r' (e.g. 'nir' -> 'nǐ')\n" +
                         "   - Tone 4 (falling / grave 'à'): type 'f' (e.g. 'haf' -> 'hà')\n" +
                         "   - Repeat tone key to undo tone and output raw letter (e.g. 'laxx' -> 'lax').\n\n" +
                         "2. Confirming Input:\n" +
                         "   - Press ENTER: Confirm and output toned Pinyin text directly (e.g. 'nǐhǎo', 'lǜ').\n" +
                         "   - Press SPACE: Select candidate #1 Chinese character (e.g. '你好', '绿').\n" +
                         "   - Press 1-9: Select corresponding Chinese character candidate.\n" +
                         "   - Press '.' / PageDown: Next candidate page.\n" +
                         "   - Press ',' / PageUp: Previous candidate page.\n" +
                         "   - Press ESC: Cancel current composition.\n\n" +
                         "3. Hotkey & Audio Alerts:\n" +
                         "   - Default Toggle Hotkey: Alt + Z (or left-click tray icon).\n" +
                         "   - Audio alert beep: 1000Hz when turning ON, 600Hz when turning OFF (can be toggled in settings).\n" +
                         "   - Icon [中]: Chinese IME ON\n" +
                         "   - Icon [EN]: English Mode (Direct US Keyboard)";

            MessageBox.Show(msg, "UniPinyin User Guide", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}
