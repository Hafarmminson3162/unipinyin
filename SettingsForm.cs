using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace UniPinyin
{
    public class SettingsForm : Form
    {
        private readonly Settings _settings;
        private readonly Action _onSettingsUpdated;

        // Header controls
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblBadge;
        private ComboBox cboUiLang;

        // Card 1: Input & Hotkey
        private Panel cardInput;
        private Label lblSecInput;
        private Label lblStatusTitle;
        private Label lblStatusBadge;
        private Button btnQuickToggle;
        private Label lblCharSet;
        private ComboBox cboCharSet;
        private Label lblHotkey;
        private ComboBox cboHotkey;

        // Card 2: Suggestions & UI
        private Panel cardOptions;
        private Label lblSecOptions;
        private Label lblSource;
        private ComboBox cboDataSource;
        private Label lblCandCount;
        private ComboBox cboCandidateCount;
        private CheckBox chkSpaceSelectFirst;
        private CheckBox chkAutoFollowCaret;

        // Card 3: System & Audio
        private Panel cardSystem;
        private Label lblSecSystem;
        private CheckBox chkStartWithWindows;
        private CheckBox chkShowNotifications;
        private CheckBox chkPlaySoundOnToggle;

        // Card 4: Live Test Field
        private Panel cardTest;
        private Label lblSecTest;
        private Label lblTestHint;
        private TextBox txtTestInput;

        // Footer buttons
        private Panel pnlFooter;
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
            this.Text = "UniPinyin - Cấu hình Bộ Gõ Tiếng Trung";
            this.ClientSize = new Size(590, 715);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            this.BackColor = Color.FromArgb(242, 242, 247); // macOS System Settings background

            // ================= 1. HEADER (APPLE TOOLBAR STYLE) =================
            Panel pnlHeader = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(590, 68),
                BackColor = Color.White
            };
            pnlHeader.Paint += (s, e) =>
            {
                using (Pen sep = new Pen(Color.FromArgb(229, 229, 234), 1f))
                {
                    e.Graphics.DrawLine(sep, 0, 67, 590, 67);
                }
            };

            int logoX = 18;
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_icon.png");
            if (File.Exists(iconPath))
            {
                try
                {
                    PictureBox picLogo = new PictureBox
                    {
                        Image = Image.FromFile(iconPath),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Location = new Point(16, 12),
                        Size = new Size(42, 42)
                    };
                    pnlHeader.Controls.Add(picLogo);
                    logoX = 66;
                }
                catch { }
            }

            lblTitle = new Label
            {
                Text = "UniPinyin",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 28, 30),
                Location = new Point(logoX, 12),
                AutoSize = true
            };

            lblBadge = new Label
            {
                Text = "v2.0",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 122, 255),
                BackColor = Color.FromArgb(235, 245, 255),
                Location = new Point(logoX + 96, 15),
                Size = new Size(38, 18),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblSubtitle = new Label
            {
                Text = "Bộ gõ Tiếng Trung Pinyin phong cách UniKey trên Windows",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(142, 142, 147),
                Location = new Point(logoX + 1, 38),
                AutoSize = true
            };

            cboUiLang = new ComboBox
            {
                Location = new Point(445, 20),
                Width = 125,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };
            cboUiLang.Items.Add("🇻🇳 Tiếng Việt");
            cboUiLang.Items.Add("🇬🇧 English");
            cboUiLang.SelectedIndexChanged += (s, e) =>
            {
                _settings.UiLanguage = (cboUiLang.SelectedIndex == 1) ? "en" : "vi";
                ApplyLanguageUI();
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblBadge);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(cboUiLang);
            this.Controls.Add(pnlHeader);

            int cardX = 18;
            int cardW = 554;
            int curY = 80;

            // ================= 2. CARD 1: INPUT & HOTKEY =================
            lblSecInput = new Label
            {
                Text = "CHẾ ĐỘ GÕ & BÀN PHÍM",
                Font = new Font("Segoe UI", 8.2f, FontStyle.Bold),
                ForeColor = Color.FromArgb(110, 110, 115),
                Location = new Point(cardX + 4, curY),
                AutoSize = true
            };
            this.Controls.Add(lblSecInput);
            curY += 20;

            cardInput = CreateAppleCard(cardX, curY, cardW, 116);

            // Row 1: Status
            lblStatusTitle = new Label { Text = "Trạng thái:", Location = new Point(16, 12), AutoSize = true, ForeColor = Color.FromArgb(60, 60, 67) };
            lblStatusBadge = new Label
            {
                Text = _settings.IsEnabled ? "● Đang BẬT [Tiếng Trung - 中]" : "○ Đang TẮT [Tiếng Anh - E]",
                ForeColor = _settings.IsEnabled ? Color.FromArgb(52, 199, 89) : Color.FromArgb(142, 142, 147),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Location = new Point(145, 12),
                AutoSize = true
            };

            btnQuickToggle = new Button
            {
                Text = "Bật / Tắt",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                Location = new Point(445, 8),
                Size = new Size(95, 26),
                BackColor = Color.FromArgb(242, 242, 247),
                FlatStyle = FlatStyle.Flat
            };
            btnQuickToggle.FlatAppearance.BorderColor = Color.FromArgb(209, 209, 214);
            btnQuickToggle.Click += (s, e) =>
            {
                _settings.IsEnabled = !_settings.IsEnabled;
                RefreshStatus(_settings.IsEnabled);
                if (_onSettingsUpdated != null) _onSettingsUpdated();
            };

            // Row 2: Character Set
            lblCharSet = new Label { Text = "Bảng mã chữ Hán:", Location = new Point(16, 46), AutoSize = true, ForeColor = Color.FromArgb(60, 60, 67) };
            cboCharSet = new ComboBox
            {
                Location = new Point(145, 43),
                Width = 395,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };

            // Row 3: Hotkey
            lblHotkey = new Label { Text = "Phím chuyển đổi:", Location = new Point(16, 80), AutoSize = true, ForeColor = Color.FromArgb(60, 60, 67) };
            cboHotkey = new ComboBox
            {
                Location = new Point(145, 77),
                Width = 395,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };

            cardInput.Controls.Add(lblStatusTitle);
            cardInput.Controls.Add(lblStatusBadge);
            cardInput.Controls.Add(btnQuickToggle);
            cardInput.Controls.Add(lblCharSet);
            cardInput.Controls.Add(cboCharSet);
            cardInput.Controls.Add(lblHotkey);
            cardInput.Controls.Add(cboHotkey);
            this.Controls.Add(cardInput);

            curY += 126;

            // ================= 3. CARD 2: OPTIONS & SUGGESTIONS =================
            lblSecOptions = new Label
            {
                Text = "NGUỒN TỪ ĐIỂN & GỢI Ý",
                Font = new Font("Segoe UI", 8.2f, FontStyle.Bold),
                ForeColor = Color.FromArgb(110, 110, 115),
                Location = new Point(cardX + 4, curY),
                AutoSize = true
            };
            this.Controls.Add(lblSecOptions);
            curY += 20;

            cardOptions = CreateAppleCard(cardX, curY, cardW, 142);

            lblSource = new Label { Text = "Nguồn dữ liệu:", Location = new Point(16, 12), AutoSize = true, ForeColor = Color.FromArgb(60, 60, 67) };
            cboDataSource = new ComboBox
            {
                Location = new Point(145, 9),
                Width = 395,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };

            lblCandCount = new Label { Text = "Số từ gợi ý / trang:", Location = new Point(16, 44), AutoSize = true, ForeColor = Color.FromArgb(60, 60, 67) };
            cboCandidateCount = new ComboBox
            {
                Location = new Point(145, 41),
                Width = 395,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular)
            };

            chkSpaceSelectFirst = new CheckBox
            {
                Text = "Phím Space (Cách) để chọn nhanh từ gợi ý số 1",
                Location = new Point(16, 78),
                AutoSize = true,
                ForeColor = Color.FromArgb(28, 28, 30)
            };

            chkAutoFollowCaret = new CheckBox
            {
                Text = "Thanh gợi ý tự động bám theo vị trí con trỏ chuột / Caret",
                Location = new Point(16, 108),
                AutoSize = true,
                ForeColor = Color.FromArgb(28, 28, 30)
            };

            cardOptions.Controls.Add(lblSource);
            cardOptions.Controls.Add(cboDataSource);
            cardOptions.Controls.Add(lblCandCount);
            cardOptions.Controls.Add(cboCandidateCount);
            cardOptions.Controls.Add(chkSpaceSelectFirst);
            cardOptions.Controls.Add(chkAutoFollowCaret);
            this.Controls.Add(cardOptions);

            curY += 152;

            // ================= 4. CARD 3: SYSTEM & AUDIO =================
            lblSecSystem = new Label
            {
                Text = "HỆ THỐNG & ÂM BÁO",
                Font = new Font("Segoe UI", 8.2f, FontStyle.Bold),
                ForeColor = Color.FromArgb(110, 110, 115),
                Location = new Point(cardX + 4, curY),
                AutoSize = true
            };
            this.Controls.Add(lblSecSystem);
            curY += 20;

            cardSystem = CreateAppleCard(cardX, curY, cardW, 106);

            chkStartWithWindows = new CheckBox
            {
                Text = "Tự động khởi động cùng Windows (Start with Windows)",
                Location = new Point(16, 12),
                AutoSize = true,
                ForeColor = Color.FromArgb(28, 28, 30)
            };

            chkShowNotifications = new CheckBox
            {
                Text = "Hiện thông báo góc màn hình khi chuyển đổi ngôn ngữ",
                Location = new Point(16, 42),
                AutoSize = true,
                ForeColor = Color.FromArgb(28, 28, 30)
            };

            chkPlaySoundOnToggle = new CheckBox
            {
                Text = "Phát âm báo (beep) thông minh khi bật / tắt tiếng Trung",
                Location = new Point(16, 72),
                AutoSize = true,
                ForeColor = Color.FromArgb(28, 28, 30)
            };

            cardSystem.Controls.Add(chkStartWithWindows);
            cardSystem.Controls.Add(chkShowNotifications);
            cardSystem.Controls.Add(chkPlaySoundOnToggle);
            this.Controls.Add(cardSystem);

            curY += 116;

            // ================= 5. CARD 4: LIVE TEST FIELD =================
            lblSecTest = new Label
            {
                Text = "GÕ THỬ NGHIỆM",
                Font = new Font("Segoe UI", 8.2f, FontStyle.Bold),
                ForeColor = Color.FromArgb(110, 110, 115),
                Location = new Point(cardX + 4, curY),
                AutoSize = true
            };
            this.Controls.Add(lblSecTest);
            curY += 20;

            cardTest = CreateAppleCard(cardX, curY, cardW, 76);

            lblTestHint = new Label
            {
                Text = "Nhập thử tại đây (ví dụ: nihao, xiexie, woshizhongguoren, xi'an):",
                Location = new Point(16, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(110, 110, 115)
            };

            txtTestInput = new TextBox
            {
                Location = new Point(16, 34),
                Width = 522,
                Font = new Font("Microsoft YaHei UI", 11f, FontStyle.Regular),
                BackColor = Color.FromArgb(249, 249, 251)
            };

            cardTest.Controls.Add(lblTestHint);
            cardTest.Controls.Add(txtTestInput);
            this.Controls.Add(cardTest);

            // ================= 6. FOOTER (ACTION BUTTONS BAR) =================
            pnlFooter = new Panel
            {
                Location = new Point(0, 655),
                Size = new Size(590, 60),
                BackColor = Color.White
            };
            pnlFooter.Paint += (s, e) =>
            {
                using (Pen sep = new Pen(Color.FromArgb(229, 229, 234), 1f))
                {
                    e.Graphics.DrawLine(sep, 0, 0, 590, 0);
                }
            };

            btnSave = new Button
            {
                Text = "Lưu & Thu nhỏ",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Location = new Point(18, 12),
                Size = new Size(155, 36),
                BackColor = Color.FromArgb(0, 122, 255), // Apple System Blue
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnWeb = new Button
            {
                Text = "Trình Gõ Web",
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                Location = new Point(185, 12),
                Size = new Size(130, 36),
                BackColor = Color.FromArgb(242, 242, 247),
                ForeColor = Color.FromArgb(28, 28, 30),
                FlatStyle = FlatStyle.Flat
            };
            btnWeb.FlatAppearance.BorderColor = Color.FromArgb(209, 209, 214);
            btnWeb.Click += BtnWeb_Click;

            btnHelp = new Button
            {
                Text = "Hướng Dẫn",
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                Location = new Point(325, 12),
                Size = new Size(125, 36),
                BackColor = Color.FromArgb(242, 242, 247),
                ForeColor = Color.FromArgb(28, 28, 30),
                FlatStyle = FlatStyle.Flat
            };
            btnHelp.FlatAppearance.BorderColor = Color.FromArgb(209, 209, 214);
            btnHelp.Click += BtnHelp_Click;

            btnExit = new Button
            {
                Text = "Thoát",
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                Location = new Point(460, 12),
                Size = new Size(110, 36),
                BackColor = Color.FromArgb(242, 242, 247),
                ForeColor = Color.FromArgb(215, 0, 21), // Apple Red
                FlatStyle = FlatStyle.Flat
            };
            btnExit.FlatAppearance.BorderColor = Color.FromArgb(209, 209, 214);
            btnExit.Click += (s, e) => Application.Exit();

            pnlFooter.Controls.Add(btnSave);
            pnlFooter.Controls.Add(btnWeb);
            pnlFooter.Controls.Add(btnHelp);
            pnlFooter.Controls.Add(btnExit);
            this.Controls.Add(pnlFooter);

            this.FormClosing += SettingsForm_FormClosing;
        }

        private static Panel CreateAppleCard(int x, int y, int w, int h)
        {
            Panel p = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = Color.White
            };
            p.Paint += (s, e) =>
            {
                using (Pen border = new Pen(Color.FromArgb(229, 229, 234), 1f))
                {
                    e.Graphics.DrawRectangle(border, 0, 0, w - 1, h - 1);
                }
            };
            return p;
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

            this.Text = isEn ? "UniPinyin - Chinese IME Control Panel" : "UniPinyin - Bảng Điều Khiển Bộ Gõ Tiếng Trung";
            lblTitle.Text = "UniPinyin";
            lblSubtitle.Text = isEn 
                ? "Lightweight Chinese Pinyin IME for Windows (UniKey style)" 
                : "Bộ gõ Tiếng Trung Pinyin phong cách UniKey trên Windows";

            lblSecInput.Text = isEn ? "INPUT MODE & KEYBOARD" : "CHẾ ĐỘ GÕ & BÀN PHÍM";
            lblStatusTitle.Text = isEn ? "IME Status:" : "Trạng thái:";
            btnQuickToggle.Text = isEn ? "Toggle ON/OFF" : "Bật / Tắt";
            lblCharSet.Text = isEn ? "Character Set:" : "Bảng mã chữ Hán:";
            lblHotkey.Text = isEn ? "Toggle Hotkey:" : "Phím chuyển đổi:";

            lblSecOptions.Text = isEn ? "DICTIONARY & SUGGESTIONS" : "NGUỒN TỪ ĐIỂN & GỢI Ý";
            lblSource.Text = isEn ? "Prediction Source:" : "Nguồn dữ liệu:";
            lblCandCount.Text = isEn ? "Candidates / Page:" : "Số từ gợi ý / trang:";
            chkSpaceSelectFirst.Text = isEn 
                ? "Press Space to select Candidate #1 immediately" 
                : "Phím Space (Cách) để chọn nhanh từ gợi ý số 1";
            chkAutoFollowCaret.Text = isEn 
                ? "Candidate bar follows text caret / mouse cursor" 
                : "Thanh gợi ý tự động bám theo con trỏ chuột / Caret";

            lblSecSystem.Text = isEn ? "SYSTEM & SOUNDS" : "HỆ THỐNG & ÂM BÁO";
            chkStartWithWindows.Text = isEn 
                ? "Start automatically with Windows" 
                : "Tự động khởi động cùng Windows (Start with Windows)";
            chkShowNotifications.Text = isEn 
                ? "Show desktop balloon notifications on toggle" 
                : "Hiện thông báo góc màn hình khi chuyển đổi ngôn ngữ";
            chkPlaySoundOnToggle.Text = isEn 
                ? "Play smart audio beep on language switch" 
                : "Phát âm báo (beep) thông minh khi bật / tắt tiếng Trung";

            lblSecTest.Text = isEn ? "LIVE TEST FIELD" : "GÕ THỬ NGHIỆM";
            lblTestHint.Text = isEn 
                ? "Type Pinyin here to test (e.g. nihao, xiexie, woshizhongguoren, xi'an):" 
                : "Nhập thử tại đây (ví dụ: nihao, xiexie, woshizhongguoren, xi'an):";

            btnSave.Text = isEn ? "Save & Minimize" : "Lưu & Thu nhỏ";
            btnWeb.Text = isEn ? "Open Web IME" : "Trình Gõ Web";
            btnHelp.Text = isEn ? "User Guide" : "Hướng Dẫn";
            btnExit.Text = isEn ? "Quit" : "Thoát";

            // Repopulate combo boxes
            int oldCharSet = cboCharSet.SelectedIndex;
            cboCharSet.Items.Clear();
            if (isEn)
            {
                cboCharSet.Items.Add("Simplified Chinese (简体中文) - Default");
                cboCharSet.Items.Add("Traditional Chinese (繁體中文)");
            }
            else
            {
                cboCharSet.Items.Add("Chữ Giản thể (Simplified - 简体) - Mặc định");
                cboCharSet.Items.Add("Chữ Phồn thể (Traditional - 繁體)");
            }
            cboCharSet.SelectedIndex = (oldCharSet >= 0) ? oldCharSet : (_settings.IsSimplified ? 0 : 1);

            int oldHotkey = cboHotkey.SelectedIndex;
            cboHotkey.Items.Clear();
            if (isEn)
            {
                cboHotkey.Items.Add("Alt + Z (Recommended / Default)");
                cboHotkey.Items.Add("Ctrl + Shift");
                cboHotkey.Items.Add("Ctrl + Space");
            }
            else
            {
                cboHotkey.Items.Add("Alt + Z (Khuyên dùng / Mặc định)");
                cboHotkey.Items.Add("Ctrl + Shift");
                cboHotkey.Items.Add("Ctrl + Space");
            }
            if (oldHotkey >= 0)
            {
                cboHotkey.SelectedIndex = oldHotkey;
            }
            else
            {
                if (_settings.HotkeyMode == 0) cboHotkey.SelectedIndex = 1;
                else if (_settings.HotkeyMode == 1) cboHotkey.SelectedIndex = 2;
                else cboHotkey.SelectedIndex = 0;
            }

            int oldSource = cboDataSource.SelectedIndex;
            cboDataSource.Items.Clear();
            if (isEn)
            {
                cboDataSource.Items.Add("Google Cloud Input Tools + Offline Fallback (Recommended)");
                cboDataSource.Items.Add("Local Dictionary Only (Completely Offline)");
            }
            else
            {
                cboDataSource.Items.Add("Google Cloud Input Tools + Dự phòng Ngoại tuyến (Khuyên dùng)");
                cboDataSource.Items.Add("Chỉ dùng Từ điển nội bộ (Hoàn toàn Offline)");
            }
            cboDataSource.SelectedIndex = (oldSource >= 0) ? oldSource : (_settings.UseGoogleOnline ? 0 : 1);

            int oldCand = cboCandidateCount.SelectedIndex;
            cboCandidateCount.Items.Clear();
            if (isEn)
            {
                cboCandidateCount.Items.Add("5 candidates per page (Compact & Clean)");
                cboCandidateCount.Items.Add("9 candidates per page (Extended List)");
            }
            else
            {
                cboCandidateCount.Items.Add("5 từ trên 1 trang (Gọn gàng & Chuẩn)");
                cboCandidateCount.Items.Add("9 từ trên 1 trang (Danh sách đầy đủ)");
            }
            cboCandidateCount.SelectedIndex = (oldCand >= 0) ? oldCand : (_settings.CandidateCount == 9 ? 1 : 0);

            RefreshStatus(_settings.IsEnabled);
        }

        public void RefreshStatus(bool isEnabled)
        {
            bool isEn = (_settings.UiLanguage == "en");
            if (isEnabled)
            {
                lblStatusBadge.Text = isEn ? "● ACTIVE [Chinese - 中]" : "● Đang BẬT [Tiếng Trung - 中]";
                lblStatusBadge.ForeColor = Color.FromArgb(52, 199, 89); // Apple Green
            }
            else
            {
                lblStatusBadge.Text = isEn ? "○ INACTIVE [English - E]" : "○ Đang TẮT [Tiếng Anh - E]";
                lblStatusBadge.ForeColor = Color.FromArgb(142, 142, 147); // Apple Gray
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            _settings.IsSimplified = (cboCharSet.SelectedIndex == 0);

            if (cboHotkey.SelectedIndex == 1) _settings.HotkeyMode = 0;
            else if (cboHotkey.SelectedIndex == 2) _settings.HotkeyMode = 1;
            else _settings.HotkeyMode = 2;

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
            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UniPinyin_Web.html");
            if (File.Exists(htmlPath))
            {
                Process.Start(new ProcessStartInfo(htmlPath) { UseShellExecute = true });
            }
        }

        private void BtnHelp_Click(object sender, EventArgs e)
        {
            bool isEn = (_settings.UiLanguage == "en");
            string docFile = isEn ? "USER_GUIDE.md" : "HUONG_DAN_SU_DUNG.md";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, docFile);
            if (File.Exists(path))
            {
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            else
            {
                string hotkeyText = (_settings.HotkeyMode == 0) ? "Ctrl+Shift" : (_settings.HotkeyMode == 1 ? "Ctrl+Space" : "Alt+Z");
                string msg = isEn
                    ? "UniPinyin Quick Guide:\n\n- Toggle hotkey: " + hotkeyText + " (or click tray icon)\n- Type 'nihao' -> press Space or 1-9 to select candidates\n- TELEX tones: x=Tone 1, s=Tone 2, r=Tone 3, f=Tone 4, w/u+w=ü\n- Double 'w' produces 'w' (not 'uw')\n- Use apostrophe ' to separate syllables (e.g. xi'an, wo'shi)\n- Press Enter to commit raw toned Pinyin text\n- Press Esc to cancel composition"
                    : "Hướng Dẫn Gõ Nhanh UniPinyin:\n\n- Phím tắt bật/tắt: " + hotkeyText + " (hoặc nhấp chuột vào icon khay hệ thống)\n- Gõ Pinyin -> nhấn Space hoặc phím 1-9 để chọn từ\n- Gõ dấu thanh TELEX: x=Thanh 1, s=Thanh 2, r=Thanh 3, f=Thanh 4, w=âm ü\n- Ấn 2 lần 'w' ra chữ 'w' (chứ không phải 'uw')\n- Đánh dấu nháy đơn ' để ngăn cách âm tiết (xi'an, wo'shi)\n- Nhấn Enter để lấy thẳng chữ Pinyin có dấu\n- Nhấn Esc để xóa bộ đệm";
                MessageBox.Show(msg, "UniPinyin", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
