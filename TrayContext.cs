using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace UniPinyin
{
    public class TrayContext : ApplicationContext
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly Settings _settings;
        private readonly CandidateManager _candidateManager;
        private readonly FloatingCandidateBar _candidateBar;
        private readonly KeyboardHookManager _hookManager;
        private readonly SettingsForm _settingsForm;

        private Icon _iconChinese;
        private Icon _iconEnglish;
        private SoundPlayer _toggleSound;

        public TrayContext(Settings settings)
        {
            _settings = settings;

            _candidateManager = new CandidateManager();
            _candidateBar = new FloatingCandidateBar();
            _hookManager = new KeyboardHookManager(_settings, _candidateManager, _candidateBar);

            // Kết nối sự kiện cập nhật danh sách gợi ý
            _candidateManager.OnCandidatesUpdated += (pinyin, cands, page, size) =>
            {
                _candidateBar.UpdateDisplay(pinyin, cands, page, size);
            };

            _candidateManager.OnBufferCleared += () =>
            {
                _candidateBar.HideBar();
            };

            _hookManager.OnLanguageToggled += (enabled) =>
            {
                UpdateTrayIcon();
                PlayToggleAudio(enabled);

                if (_settingsForm != null && !_settingsForm.IsDisposed)
                {
                    _settingsForm.RefreshStatus(enabled);
                }

                if (_settings.ShowNotifications)
                {
                    bool isEn = (_settings.UiLanguage == "en");
                    string title = "UniPinyin";
                    string msg = enabled 
                        ? (isEn ? "Chinese IME Enabled [中]" : "Đang bật gõ Tiếng Trung [中]") 
                        : (isEn ? "English Mode Enabled [E]" : "Đang bật gõ Tiếng Anh [E]");
                    _notifyIcon.ShowBalloonTip(1000, title, msg, ToolTipIcon.Info);
                }
            };

            _settingsForm = new SettingsForm(_settings, () =>
            {
                UpdateTrayIcon();
            });

            // Tạo icon động trong bộ nhớ
            CreateDynamicIcons();

            // Khởi tạo NotifyIcon
            _notifyIcon = new NotifyIcon
            {
                Visible = true,
                Text = "UniPinyin"
            };

            _notifyIcon.MouseClick += NotifyIcon_MouseClick;
            _notifyIcon.DoubleClick += (s, e) => ShowSettings();

            BuildContextMenu();
            UpdateTrayIcon();

            // Khởi tạo âm thanh thông báo chuyển đổi (Samsung Galaxy chime)
            InitializeAudio();

            // Khởi động Hook bàn phím
            _hookManager.Start();
        }

        private void InitializeAudio()
        {
            try
            {
                System.Reflection.Assembly asm = typeof(TrayContext).Assembly;
                Stream stream = asm.GetManifestResourceStream("toggle.wav");
                if (stream == null)
                {
                    string localWav = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "toggle.wav");
                    if (File.Exists(localWav))
                    {
                        stream = new FileStream(localWav, FileMode.Open, FileAccess.Read, FileShare.Read);
                    }
                }

                if (stream != null)
                {
                    _toggleSound = new SoundPlayer(stream);
                    _toggleSound.LoadAsync();
                }
            }
            catch { }
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Nhấp chuột trái: chuyển đổi nhanh giữa Tiếng Trung và Tiếng Anh (giống UniKey)
                _settings.IsEnabled = !_settings.IsEnabled;
                _settings.Save();
                _candidateManager.Clear();
                _candidateBar.HideBar();

                UpdateTrayIcon();
                PlayToggleAudio(_settings.IsEnabled);

                if (_settingsForm != null && !_settingsForm.IsDisposed)
                {
                    _settingsForm.RefreshStatus(_settings.IsEnabled);
                }

                if (_settings.ShowNotifications)
                {
                    bool isEn = (_settings.UiLanguage == "en");
                    string msg = _settings.IsEnabled 
                        ? (isEn ? "Chinese IME Enabled [中]" : "Đang bật Tiếng Trung [中]") 
                        : (isEn ? "English Mode Enabled [EN]" : "Đang tắt Tiếng Anh [EN]");
                    _notifyIcon.ShowBalloonTip(1000, "UniPinyin", msg, ToolTipIcon.Info);
                }
            }
        }

        private void PlayToggleAudio(bool enabled)
        {
            if (_settings.PlaySoundOnToggle)
            {
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (_toggleSound != null)
                        {
                            _toggleSound.Play();
                        }
                        else
                        {
                            int freq = enabled ? 1000 : 600;
                            Console.Beep(freq, 50);
                        }
                    }
                    catch
                    {
                        try
                        {
                            int freq = enabled ? 1000 : 600;
                            Console.Beep(freq, 50);
                        }
                        catch
                        {
                            System.Media.SystemSounds.Asterisk.Play();
                        }
                    }
                });
            }
        }

        private void BuildContextMenu()
        {
            bool isEn = (_settings.UiLanguage == "en");
            ContextMenuStrip menu = new ContextMenuStrip();

            ToolStripMenuItem itemToggle = new ToolStripMenuItem(isEn ? "Enable Chinese Input (Pinyin)" : "Bật gõ Tiếng Trung (Pinyin)");
            itemToggle.Checked = _settings.IsEnabled;
            itemToggle.Click += (s, e) =>
            {
                _settings.IsEnabled = !_settings.IsEnabled;
                _settings.Save();
                UpdateTrayIcon();
            };

            ToolStripMenuItem itemSimp = new ToolStripMenuItem(isEn ? "Simplified Chinese (简体)" : "Chữ Giản thể (简体)");
            itemSimp.Checked = _settings.IsSimplified;
            itemSimp.Click += (s, e) =>
            {
                _settings.IsSimplified = true;
                _settings.Save();
                BuildContextMenu();
            };

            ToolStripMenuItem itemTrad = new ToolStripMenuItem(isEn ? "Traditional Chinese (繁體)" : "Chữ Phồn thể (繁體)");
            itemTrad.Checked = !_settings.IsSimplified;
            itemTrad.Click += (s, e) =>
            {
                _settings.IsSimplified = false;
                _settings.Save();
                BuildContextMenu();
            };

            ToolStripMenuItem itemSettings = new ToolStripMenuItem(isEn ? "Control Panel... [Settings]" : "Bảng điều khiển... [Mở cấu hình]");
            itemSettings.Font = new Font(itemSettings.Font, FontStyle.Bold);
            itemSettings.Click += (s, e) => ShowSettings();

            ToolStripMenuItem itemWeb = new ToolStripMenuItem(isEn ? "Open Web IME (Browser Demo)" : "Mở Trình gõ Web (Browser Demo)");
            itemWeb.Click += (s, e) =>
            {
                string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UniPinyin_Web.html");
                if (File.Exists(htmlPath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(htmlPath) { UseShellExecute = true });
                }
            };

            // Menu chuyển ngôn ngữ giao diện (UI Language)
            ToolStripMenuItem itemLangMenu = new ToolStripMenuItem(isEn ? "UI Language (Ngôn ngữ)" : "Ngôn ngữ giao diện (UI Language)");
            ToolStripMenuItem itemLangVi = new ToolStripMenuItem("🇻🇳 Tiếng Việt");
            itemLangVi.Checked = (!isEn);
            itemLangVi.Click += (s, e) =>
            {
                _settings.UiLanguage = "vi";
                _settings.Save();
                _settingsForm.ApplyLanguageUI();
                UpdateTrayIcon();
            };

            ToolStripMenuItem itemLangEn = new ToolStripMenuItem("🇬🇧 English");
            itemLangEn.Checked = isEn;
            itemLangEn.Click += (s, e) =>
            {
                _settings.UiLanguage = "en";
                _settings.Save();
                _settingsForm.ApplyLanguageUI();
                UpdateTrayIcon();
            };

            itemLangMenu.DropDownItems.Add(itemLangVi);
            itemLangMenu.DropDownItems.Add(itemLangEn);

            ToolStripMenuItem itemHelp = new ToolStripMenuItem(isEn ? "Pinyin Typing Guide" : "Hướng dẫn gõ Pinyin");
            itemHelp.Click += (s, e) =>
            {
                if (isEn)
                {
                    string msg = "Quick Toggle Hotkey: Ctrl + Shift or Ctrl + Space\n" +
                                 "- Type 'nihao' -> Press Space or 1 to select 你好\n" +
                                 "- Press '.' or PageDown to next page\n" +
                                 "- Press Enter to commit raw English pinyin text\n" +
                                 "- Press Esc to cancel composition";
                    MessageBox.Show(msg, "UniPinyin - Guide", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string msg = "Phím chuyển đổi nhanh: Ctrl + Shift hoặc Ctrl + Space\n" +
                                 "- Gõ 'nihao' -> Bấm Space hoặc 1 để chọn 你好\n" +
                                 "- Bấm '.' hoặc PageDown để sang trang\n" +
                                 "- Bấm Enter để lấy chữ tiếng Anh nguyên bản\n" +
                                 "- Bấm Esc để hủy soạn thảo";
                    MessageBox.Show(msg, "UniPinyin - Hướng Dẫn", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            ToolStripMenuItem itemExit = new ToolStripMenuItem(isEn ? "Exit UniPinyin" : "Kết thúc (Thoát)");
            itemExit.Click += (s, e) => ExitApplication();

            menu.Items.Add(itemToggle);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(itemSimp);
            menu.Items.Add(itemTrad);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(itemSettings);
            menu.Items.Add(itemWeb);
            menu.Items.Add(itemLangMenu);
            menu.Items.Add(itemHelp);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(itemExit);

            _notifyIcon.ContextMenuStrip = menu;
        }

        private void UpdateTrayIcon()
        {
            bool isEn = (_settings.UiLanguage == "en");
            string hotkeyText = (_settings.HotkeyMode == 0) ? "Ctrl+Shift" : (_settings.HotkeyMode == 1 ? "Ctrl+Space" : "Alt+Z");

            if (_settings.IsEnabled)
            {
                _notifyIcon.Icon = _iconChinese;
                _notifyIcon.Text = isEn 
                    ? "UniPinyin: Chinese [中] (Click or " + hotkeyText + " to toggle)" 
                    : "UniPinyin: Tiếng Trung [中] (Nhấp hoặc " + hotkeyText + " để tắt)";
            }
            else
            {
                _notifyIcon.Icon = _iconEnglish;
                _notifyIcon.Text = isEn 
                    ? "UniPinyin: English [E] (Click or " + hotkeyText + " to toggle)" 
                    : "UniPinyin: Tiếng Anh [E] (Nhấp hoặc " + hotkeyText + " để bật)";
            }
            BuildContextMenu();
        }

        private void CreateDynamicIcons()
        {
            // Thiết kế theo phong cách EVKey / UniKey: chữ '中' và 'E' phóng to full viền, có viền stroke nét đậm
            _iconChinese = GenerateGlyphIcon("中", Color.FromArgb(0, 122, 255), Color.FromArgb(230, 15, 15, 20));
            _iconEnglish = GenerateGlyphIcon("E", Color.White, Color.FromArgb(230, 15, 15, 20));
        }

        private Icon GenerateGlyphIcon(string text, Color fgColor, Color strokeColor)
        {
            int size = 32;
            using (Bitmap bmp = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.Clear(Color.Transparent);

                string fontName = (text == "中") ? "Microsoft YaHei UI" : "Arial";
                FontFamily ff = null;
                try
                {
                    ff = new FontFamily(fontName);
                }
                catch
                {
                    ff = FontFamily.GenericSansSerif;
                }

                using (ff)
                using (GraphicsPath path = new GraphicsPath())
                using (StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                })
                {
                    // Chiều cao mục tiêu trong canvas 32x32 để khi scale xuống taskbar đạt kích thước tương đương EVKey (21-22px)
                    float targetH = (text == "中") ? 24.5f : 25.5f;
                    float testSize = 30f;
                    path.AddString(text, ff, (int)FontStyle.Bold, testSize, new PointF(16f, 16f), sf);
                    RectangleF b = path.GetBounds();
                    float scale = targetH / (b.Height > 0 ? b.Height : 1f);

                    path.Reset();
                    float finalSize = testSize * scale;
                    path.AddString(text, ff, (int)FontStyle.Bold, finalSize, new PointF(16f, 16f), sf);
                    b = path.GetBounds();

                    // Căn giữa tuyệt đối trong khung 32x32
                    float dx = 16f - (b.X + b.Width / 2f);
                    float dy = 16f - (b.Y + b.Height / 2f);
                    using (Matrix m = new Matrix())
                    {
                        m.Translate(dx, dy);
                        path.Transform(m);
                    }

                    // 1. Viền nét đậm tương tự EVKey (Stroke border) để chữ nổi bật trên mọi nền Taskbar sáng/tối
                    float penWidth = (text == "中") ? 2.0f : 2.2f;
                    using (Pen pen = new Pen(strokeColor, penWidth))
                    {
                        pen.LineJoin = LineJoin.Round;
                        g.DrawPath(pen, path);
                    }

                    // 2. Tô màu chính cho chữ
                    using (SolidBrush brush = new SolidBrush(fgColor))
                    {
                        g.FillPath(brush, path);
                    }
                }

                IntPtr hIcon = bmp.GetHicon();
                Icon icon = (Icon)Icon.FromHandle(hIcon).Clone();
                NativeMethods.DestroyIcon(hIcon);
                return icon;
            }
        }

        public void ShowSettings()
        {
            _settingsForm.RefreshStatus(_settings.IsEnabled);
            _settingsForm.ApplyLanguageUI();
            _settingsForm.Show();
            _settingsForm.BringToFront();
            _settingsForm.WindowState = FormWindowState.Normal;
        }

        public void ExitApplication()
        {
            if (_toggleSound != null)
            {
                try { _toggleSound.Dispose(); } catch { }
                _toggleSound = null;
            }
            _hookManager.Stop();
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _candidateBar.Dispose();
            _settingsForm.Dispose();
            Application.Exit();
        }
    }
}
