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

        private Label lblTitle;
        private Label lblSubtitle;
        private ComboBox cboUiLang;

        private GroupBox grpMain;
        private Label lblStatusTitle;
        private Label lblStatus;
        private Label lblCharSet;
        private ComboBox cboCharSet;
        private Label lblHotkey;
        private ComboBox cboHotkey;

        private GroupBox grpOptions;
        private Label lblSource;
        private ComboBox cboDataSource;
        private Label lblCandCount;
        private ComboBox cboCandidateCount;
        private CheckBox chkSpaceSelectFirst;
        private CheckBox chkAutoFollowCaret;

        private GroupBox grpSystem;
        private CheckBox chkStartWithWindows;
        private CheckBox chkShowNotifications;
        private CheckBox chkPlaySoundOnToggle;
        private Label lblTest;
        private TextBox txtTestInput;

        private Button btnSave;
        private Button btnWeb;
        private Button btnHelp;
        private Button btnExit;

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
            ApplyLanguageUI();
            LoadDataFromSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "UniPinyin - Bảng Điều Khiển Bộ Gõ Tiếng Trung";
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

            lblTitle = new Label
            {
                Text = "UniPinyin - Bộ Gõ Tiếng Trung Pinyin (Google / US Keyboard)",
                Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(titleX, 8),
                AutoSize = true
            };

            lblSubtitle = new Label
            {
                Text = "Nhẹ, mượt mà, gợi ý trực tuyến từ Google, hoạt động trên mọi ứng dụng",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(210, 230, 255),
                Location = new Point(titleX + 1, 32),
                AutoSize = true
            };

            cboUiLang = new ComboBox
            {
                Location = new Point(365, 14),
                Width = 115,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular)
            };
            cboUiLang.Items.Add("🇻🇳 Tiếng Việt");
            cboUiLang.Items.Add("🇬🇧 English");
            cboUiLang.SelectedIndexChanged += (s, e) =>
            {
                _settings.UiLanguage = (cboUiLang.SelectedIndex == 1) ? "en" : "vi";
                ApplyLanguageUI();
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(cboUiLang);
            this.Controls.Add(pnlHeader);

            startY = 68;

            // Group 1: Điều khiển chính
            grpMain = new GroupBox
            {
                Text = "Điều khiển chính",
                Location = new Point(12, startY),
                Size = new Size(465, 115)
            };

            lblStatusTitle = new Label { Text = "Trạng thái:", Location = new Point(16, 24), AutoSize = true };
            lblStatus = new Label
            {
                Text = _settings.IsEnabled ? "● Đang BẬT [Tiếng Trung - 中]" : "○ Đang TẮT [Tiếng Anh - EN]",
                ForeColor = _settings.IsEnabled ? Color.DarkGreen : Color.Gray,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Location = new Point(140, 24),
                AutoSize = true
            };

            lblCharSet = new Label { Text = "Bảng mã chữ:", Location = new Point(16, 52), AutoSize = true };
            cboCharSet = new ComboBox
            {
                Location = new Point(140, 49),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            lblHotkey = new Label { Text = "Phím chuyển đổi:", Location = new Point(16, 82), AutoSize = true };
            cboHotkey = new ComboBox
            {
                Location = new Point(140, 79),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            grpMain.Controls.Add(lblStatusTitle);
            grpMain.Controls.Add(lblStatus);
            grpMain.Controls.Add(lblCharSet);
            grpMain.Controls.Add(cboCharSet);
            grpMain.Controls.Add(lblHotkey);
            grpMain.Controls.Add(cboHotkey);
            this.Controls.Add(grpMain);

            startY += 122;

            // Group 2: Nguồn gợi ý & Giao diện
            grpOptions = new GroupBox
            {
                Text = "Nguồn từ điển & Thanh gợi ý",
                Location = new Point(12, startY),
                Size = new Size(465, 125)
            };

            lblSource = new Label { Text = "Nguồn gợi ý chữ:", Location = new Point(16, 24), AutoSize = true };
            cboDataSource = new ComboBox
            {
                Location = new Point(140, 21),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            lblCandCount = new Label { Text = "Số từ trên 1 trang:", Location = new Point(16, 54), AutoSize = true };
            cboCandidateCount = new ComboBox
            {
                Location = new Point(140, 51),
                Width = 140,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            chkSpaceSelectFirst = new CheckBox
            {
                Text = "Phím Space (Cách) để chọn từ gợi ý số 1",
                Location = new Point(18, 85),
                AutoSize = true
            };

            chkAutoFollowCaret = new CheckBox
            {
                Text = "Thanh gợi ý tự động bám theo con trỏ chuột/caret",
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

            // Group 3: Hệ thống & Gõ thử nghiệm
            grpSystem = new GroupBox
            {
                Text = "Hệ thống & Gõ thử nghiệm",
                Location = new Point(12, startY),
                Size = new Size(465, 135)
            };

            chkStartWithWindows = new CheckBox
            {
                Text = "Khởi động cùng Windows",
                Location = new Point(18, 22),
                AutoSize = true
            };

            chkShowNotifications = new CheckBox
            {
                Text = "Hiện thông báo khi chuyển đổi",
                Location = new Point(230, 22),
                AutoSize = true
            };

            chkPlaySoundOnToggle = new CheckBox
            {
                Text = "Âm báo (beep) khi chuyển đổi ngôn ngữ",
                Location = new Point(18, 48),
                AutoSize = true
            };

            lblTest = new Label
            {
                Text = "Khung gõ thử (Gõ 'nihao', 'xiexie', 'zhongwen' tại đây):",
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
            btnSave = new Button
            {
                Text = "Lưu & Đóng vào Tray",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Location = new Point(12, startY),
                Size = new Size(155, 34),
                BackColor = Color.FromArgb(24, 103, 192),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;

            btnWeb = new Button
            {
                Text = "Mở Trình Gõ Web",
                Location = new Point(175, startY),
                Size = new Size(130, 34)
            };
            btnWeb.Click += BtnWeb_Click;

            btnHelp = new Button
            {
                Text = "Hướng Dẫn",
                Location = new Point(313, startY),
                Size = new Size(82, 34)
            };
            btnHelp.Click += BtnHelp_Click;

            btnExit = new Button
            {
                Text = "Thoát",
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
            cboUiLang.SelectedIndex = (_settings.UiLanguage == "en") ? 1 : 0;
            chkSpaceSelectFirst.Checked = _settings.SpaceSelectFirst;
            chkAutoFollowCaret.Checked = _settings.AutoFollowCaret;
            chkStartWithWindows.Checked = _settings.StartWithWindows;
            chkShowNotifications.Checked = _settings.ShowNotifications;
            chkPlaySoundOnToggle.Checked = _settings.PlaySoundOnToggle;
        }

        public void ApplyLanguageUI()
        {
            bool isEn = (_settings.UiLanguage == "en");

            // Form Title & Header
            this.Text = isEn ? "UniPinyin - Chinese IME Control Panel" : "UniPinyin - Bảng Điều Khiển Bộ Gõ Tiếng Trung";
            lblTitle.Text = isEn ? "UniPinyin - Chinese Pinyin IME (Google / US Keyboard)" : "UniPinyin - Bộ Gõ Tiếng Trung Pinyin (Google / US Keyboard)";
            lblSubtitle.Text = isEn ? "Lightweight, smooth, Google Cloud suggestions, system-wide support" : "Nhẹ, mượt mà, gợi ý trực tuyến từ Google, hoạt động trên mọi ứng dụng";

            // Group 1: Main Controls
            grpMain.Text = isEn ? "Main Controls" : "Điều khiển chính";
            lblStatusTitle.Text = isEn ? "Status:" : "Trạng thái:";
            RefreshStatus(_settings.IsEnabled);

            lblCharSet.Text = isEn ? "Character Set:" : "Bảng mã chữ:";
            int charIdx = cboCharSet.SelectedIndex >= 0 ? cboCharSet.SelectedIndex : (_settings.IsSimplified ? 0 : 1);
            cboCharSet.Items.Clear();
            if (isEn)
            {
                cboCharSet.Items.Add("Simplified Chinese (简体) - Mainland Standard");
                cboCharSet.Items.Add("Traditional Chinese (繁體) - Taiwan / HK");
            }
            else
            {
                cboCharSet.Items.Add("Chữ Hán Giản thể (Simplified - 简体) - Phổ thông");
                cboCharSet.Items.Add("Chữ Hán Phồn thể (Traditional - 繁體) - Đài Loan/Hồng Kông");
            }
            cboCharSet.SelectedIndex = Math.Max(0, Math.Min(cboCharSet.Items.Count - 1, charIdx));

            lblHotkey.Text = isEn ? "Toggle Hotkey:" : "Phím chuyển đổi:";
            int hotkeyIdx = cboHotkey.SelectedIndex >= 0 ? cboHotkey.SelectedIndex : Math.Min(2, Math.Max(0, _settings.HotkeyMode));
            cboHotkey.Items.Clear();
            if (isEn)
            {
                cboHotkey.Items.Add("Ctrl + Shift (UniKey style)");
                cboHotkey.Items.Add("Ctrl + Space");
                cboHotkey.Items.Add("Alt + Z (Default)");
            }
            else
            {
                cboHotkey.Items.Add("Ctrl + Shift (Kiểu UniKey)");
                cboHotkey.Items.Add("Ctrl + Space");
                cboHotkey.Items.Add("Alt + Z (Mặc định)");
            }
            cboHotkey.SelectedIndex = Math.Max(0, Math.Min(cboHotkey.Items.Count - 1, hotkeyIdx));

            // Group 2: Dictionary & Candidate Bar
            grpOptions.Text = isEn ? "Dictionary & Candidate Bar" : "Nguồn từ điển & Thanh gợi ý";
            lblSource.Text = isEn ? "Suggestion Source:" : "Nguồn gợi ý chữ:";
            int srcIdx = cboDataSource.SelectedIndex >= 0 ? cboDataSource.SelectedIndex : (_settings.UseGoogleOnline ? 0 : 1);
            cboDataSource.Items.Clear();
            if (isEn)
            {
                cboDataSource.Items.Add("Google Cloud Suggestion + Offline Dict (Recommended)");
                cboDataSource.Items.Add("Offline Dictionary Only (No internet)");
            }
            else
            {
                cboDataSource.Items.Add("Google Cloud Suggestion + Từ điển Offline (Khuyên dùng)");
                cboDataSource.Items.Add("Chỉ từ điển Offline (Không kết nối mạng)");
            }
            cboDataSource.SelectedIndex = Math.Max(0, Math.Min(cboDataSource.Items.Count - 1, srcIdx));

            lblCandCount.Text = isEn ? "Candidates per page:" : "Số từ trên 1 trang:";
            int candIdx = cboCandidateCount.SelectedIndex >= 0 ? cboCandidateCount.SelectedIndex : (_settings.CandidateCount == 9 ? 1 : 0);
            cboCandidateCount.Items.Clear();
            if (isEn)
            {
                cboCandidateCount.Items.Add("5 words (Compact)");
                cboCandidateCount.Items.Add("9 words (Full)");
            }
            else
            {
                cboCandidateCount.Items.Add("5 từ (Gọn gàng)");
                cboCandidateCount.Items.Add("9 từ (Đầy đủ)");
            }
            cboCandidateCount.SelectedIndex = Math.Max(0, Math.Min(cboCandidateCount.Items.Count - 1, candIdx));

            chkSpaceSelectFirst.Text = isEn ? "Press Space key to select 1st candidate" : "Phím Space (Cách) để chọn từ gợi ý số 1";
            chkAutoFollowCaret.Text = isEn ? "Candidate bar automatically follows caret/cursor" : "Thanh gợi ý tự động bám theo con trỏ chuột/caret";

            // Group 3: System & Testing
            grpSystem.Text = isEn ? "System & Live Typing Test" : "Hệ thống & Gõ thử nghiệm";
            chkStartWithWindows.Text = isEn ? "Start with Windows" : "Khởi động cùng Windows";
            chkShowNotifications.Text = isEn ? "Show notification on toggle" : "Hiện thông báo khi chuyển đổi";
            chkPlaySoundOnToggle.Text = isEn ? "Play beep sound on toggle" : "Âm báo (beep) khi chuyển đổi ngôn ngữ";
            lblTest.Text = isEn ? "Test typing here (e.g. type 'nihao', 'xiexie', 'zhongguo'):" : "Khung gõ thử (Gõ 'nihao', 'xiexie', 'zhongwen' tại đây):";

            // Buttons
            btnSave.Text = isEn ? "Save & Close to Tray" : "Lưu & Đóng vào Tray";
            btnWeb.Text = isEn ? "Open Web IME" : "Mở Trình Gõ Web";
            btnHelp.Text = isEn ? "Help / Guide" : "Hướng Dẫn";
            btnExit.Text = isEn ? "Exit" : "Thoát";
        }

        public void RefreshStatus(bool isEnabled)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => RefreshStatus(isEnabled)));
                return;
            }
            bool isEn = (_settings.UiLanguage == "en");
            if (isEn)
            {
                lblStatus.Text = isEnabled ? "● ON [Chinese - 中]" : "○ OFF [English - EN]";
            }
            else
            {
                lblStatus.Text = isEnabled ? "● Đang BẬT [Tiếng Trung - 中]" : "○ Đang TẮT [Tiếng Anh - EN]";
            }
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
            _settings.UiLanguage = (cboUiLang.SelectedIndex == 1) ? "en" : "vi";

            _settings.Save();

            if (_onSettingsUpdated != null)
            {
                _onSettingsUpdated();
            }

            this.Hide();
        }

        private void BtnWeb_Click(object sender, EventArgs e)
        {
            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UniPinyin_Web.html");
            if (File.Exists(htmlPath))
            {
                Process.Start(new ProcessStartInfo(htmlPath) { UseShellExecute = true });
            }
            else
            {
                bool isEn = (_settings.UiLanguage == "en");
                string msg = isEn ? "Could not find UniPinyin_Web.html file." : "Không tìm thấy tệp UniPinyin_Web.html cạnh file ứng dụng.";
                string title = isEn ? "Notice" : "Thông báo";
                MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnHelp_Click(object sender, EventArgs e)
        {
            bool isEn = (_settings.UiLanguage == "en");

            if (isEn)
            {
                string msgEn = "UNIPINYIN USER GUIDE:\n\n" +
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

                MessageBox.Show(msgEn, "UniPinyin User Guide", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string msgVi = "HƯỚNG DẪN GÕ TIẾNG TRUNG UNIPINYIN:\n\n" +
                               "1. Gõ Pinyin có dấu thanh (Kiểu TELEX tiện lợi):\n" +
                               "   - Âm 'ü' (u có dấu : trên đầu): gõ 'w' hoặc 'u+w' (VD: 'lw' -> 'lü', 'nw' -> 'nü', 'luw' -> 'lü').\n" +
                               "   - Thanh 1 (ngang - ā): gõ phím 'x' (VD: 'lax' -> 'lā')\n" +
                               "   - Thanh 2 (sắc - á): gõ phím 's' (VD: 'mas' -> 'má')\n" +
                               "   - Thanh 3 (hỏi/caron - ǎ): gõ phím 'r' (VD: 'nir' -> 'nǐ')\n" +
                               "   - Thanh 4 (huyền/nặng - à): gõ phím 'f' (VD: 'haf' -> 'hà')\n" +
                               "   - Gõ lặp lại phím dấu để hủy dấu & lấy chữ cái gốc (VD: 'laxx' -> 'lax', 'haff' -> 'haf').\n\n" +
                               "2. Xuất chữ & Chọn gợi ý:\n" +
                               "   - Nhấn phím ENTER: Xác nhận lấy chuỗi Pinyin có dấu (VD: 'nǐhǎo', 'lǜ', 'hànzì').\n" +
                               "   - Nhấn phím CÁCH (Space): Chọn chữ Hán gợi ý số 1 (VD: '你好', '绿').\n" +
                               "   - Nhấn phím số 1-9: Chọn chữ Hán tương ứng trên thanh gợi ý.\n" +
                               "   - Nhấn phím '.' / PageDown: Sang trang gợi ý tiếp theo.\n" +
                               "   - Nhấn phím ',' / PageUp: Lùi lại trang trước.\n" +
                               "   - Nhấn phím Esc: Hủy bỏ chuỗi đang gõ.\n\n" +
                               "3. Phím tắt Bật / Tắt & Âm báo:\n" +
                               "   - Phím tắt mặc định: Alt + Z (hoặc click chuột trái icon khay hệ thống).\n" +
                               "   - Âm báo tiếng bíp khi chuyển đổi: Tiếng thanh cao khi BẬT, tiếng trầm khi TẮT (tùy chỉnh bật/tắt trong Bảng điều khiển).\n" +
                               "   - Biểu tượng [中]: Đang bật Tiếng Trung\n" +
                               "   - Biểu tượng [EN]: Đang ở chế độ Tiếng Anh (bàn phím US gốc)";

                MessageBox.Show(msgVi, "Hướng Dẫn Sử Dụng UniPinyin", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
