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
                    string title = "UniPinyin";
                    string msg = enabled ? "Chinese IME Enabled [中]" : "English Mode Enabled [E]";
                    _notifyIcon.ShowBalloonTip(1000, title, msg, ToolTipIcon.Info);
                }
            };

            _settingsForm = new SettingsForm(_settings, () =>
            {
                UpdateTrayIcon();
            });

            CreateDynamicIcons();

            _notifyIcon = new NotifyIcon
            {
                Visible = true,
                Text = "UniPinyin - Chinese IME"
            };

            _notifyIcon.MouseClick += NotifyIcon_MouseClick;
            _notifyIcon.DoubleClick += (s, e) => ShowSettings();

            BuildContextMenu();
            UpdateTrayIcon();

            // Initialize audio chime (Samsung Galaxy sound)
            InitializeAudio();

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
                    string msg = _settings.IsEnabled ? "Chinese IME Enabled [中]" : "English Mode Enabled [EN]";
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
            ContextMenuStrip menu = new ContextMenuStrip();

            ToolStripMenuItem itemToggle = new ToolStripMenuItem("Enable Chinese Input (Pinyin)");
            itemToggle.Checked = _settings.IsEnabled;
            itemToggle.Click += (s, e) =>
            {
                _settings.IsEnabled = !_settings.IsEnabled;
                _settings.Save();
                UpdateTrayIcon();
            };

            ToolStripMenuItem itemSimp = new ToolStripMenuItem("Simplified Chinese (简体)");
            itemSimp.Checked = _settings.IsSimplified;
            itemSimp.Click += (s, e) =>
            {
                _settings.IsSimplified = true;
                _settings.Save();
                BuildContextMenu();
            };

            ToolStripMenuItem itemTrad = new ToolStripMenuItem("Traditional Chinese (繁體)");
            itemTrad.Checked = !_settings.IsSimplified;
            itemTrad.Click += (s, e) =>
            {
                _settings.IsSimplified = false;
                _settings.Save();
                BuildContextMenu();
            };

            ToolStripMenuItem itemSettings = new ToolStripMenuItem("Control Panel... [Settings]");
            itemSettings.Font = new Font(itemSettings.Font, FontStyle.Bold);
            itemSettings.Click += (s, e) => ShowSettings();

            ToolStripMenuItem itemWeb = new ToolStripMenuItem("Open Web IME (Browser Demo)");
            itemWeb.Click += (s, e) =>
            {
                string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UniPinyin_Web_EN.html");
                if (!File.Exists(htmlPath))
                {
                    htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UniPinyin_Web.html");
                }
                if (File.Exists(htmlPath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(htmlPath) { UseShellExecute = true });
                }
            };

            ToolStripMenuItem itemHelp = new ToolStripMenuItem("Pinyin Typing Guide");
            itemHelp.Click += (s, e) =>
            {
                string msg = "Quick Toggle Hotkey: Ctrl + Shift or Ctrl + Space\n" +
                             "- Type 'nihao' -> Press Spacebar or 1 to select 你好\n" +
                             "- Press '.' or PageDown to next page\n" +
                             "- Press Enter to commit raw English pinyin text\n" +
                             "- Press Esc to cancel composition";
                MessageBox.Show(msg, "UniPinyin - Guide", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            ToolStripMenuItem itemExit = new ToolStripMenuItem("Exit UniPinyin");
            itemExit.Click += (s, e) => ExitApplication();

            menu.Items.Add(itemToggle);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(itemSimp);
            menu.Items.Add(itemTrad);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(itemSettings);
            menu.Items.Add(itemWeb);
            menu.Items.Add(itemHelp);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(itemExit);

            _notifyIcon.ContextMenuStrip = menu;
        }

        private void UpdateTrayIcon()
        {
            string hotkeyText = (_settings.HotkeyMode == 0) ? "Ctrl+Shift" : (_settings.HotkeyMode == 1 ? "Ctrl+Space" : "Alt+Z");

            if (_settings.IsEnabled)
            {
                _notifyIcon.Icon = _iconChinese;
                _notifyIcon.Text = "UniPinyin: Chinese [中] (Click or " + hotkeyText + " to toggle)";
            }
            else
            {
                _notifyIcon.Icon = _iconEnglish;
                _notifyIcon.Text = "UniPinyin: English [E] (Click or " + hotkeyText + " to toggle)";
            }
            BuildContextMenu();
        }

        private void CreateDynamicIcons()
        {
            // EVKey / UniKey style: enlarged full-height glyphs '中' and 'E' with clean crisp stroke outline
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
                    // Target height in 32x32 canvas so when scaled to taskbar it matches EVKey (21-22px)
                    float targetH = (text == "中") ? 24.5f : 25.5f;
                    float testSize = 30f;
                    path.AddString(text, ff, (int)FontStyle.Bold, testSize, new PointF(16f, 16f), sf);
                    RectangleF b = path.GetBounds();
                    float scale = targetH / (b.Height > 0 ? b.Height : 1f);

                    path.Reset();
                    float finalSize = testSize * scale;
                    path.AddString(text, ff, (int)FontStyle.Bold, finalSize, new PointF(16f, 16f), sf);
                    b = path.GetBounds();

                    // Center precisely in 32x32 canvas
                    float dx = 16f - (b.X + b.Width / 2f);
                    float dy = 16f - (b.Y + b.Height / 2f);
                    using (Matrix m = new Matrix())
                    {
                        m.Translate(dx, dy);
                        path.Transform(m);
                    }

                    // 1. Crisp stroke outline (like EVKey) for distinct visibility on both dark and light taskbars
                    float penWidth = (text == "中") ? 2.0f : 2.2f;
                    using (Pen pen = new Pen(strokeColor, penWidth))
                    {
                        pen.LineJoin = LineJoin.Round;
                        g.DrawPath(pen, path);
                    }

                    // 2. Main fill
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
