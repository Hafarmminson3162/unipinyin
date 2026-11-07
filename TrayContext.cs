using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
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
                    string msg = enabled ? "Chinese IME Enabled [中]" : "English Mode Enabled [EN]";
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

            _hookManager.Start();
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
                        int freq = enabled ? 1000 : 600;
                        Console.Beep(freq, 50);
                    }
                    catch
                    {
                        System.Media.SystemSounds.Asterisk.Play();
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
            if (_settings.IsEnabled)
            {
                _notifyIcon.Icon = _iconChinese;
                _notifyIcon.Text = "UniPinyin: Chinese [中] (Click or Ctrl+Shift to toggle)";
            }
            else
            {
                _notifyIcon.Icon = _iconEnglish;
                _notifyIcon.Text = "UniPinyin: English [EN] (Click or Ctrl+Shift to toggle)";
            }
            BuildContextMenu();
        }

        private void CreateDynamicIcons()
        {
            _iconChinese = GenerateSquareIcon("中", Color.FromArgb(24, 103, 192), Color.White);
            _iconEnglish = GenerateSquareIcon("EN", Color.FromArgb(90, 100, 110), Color.White);
        }

        private Icon GenerateSquareIcon(string text, Color bg, Color fg)
        {
            int size = 32;
            using (Bitmap bmp = new Bitmap(size, size))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                using (SolidBrush brush = new SolidBrush(bg))
                {
                    g.FillRectangle(brush, 0, 0, size, size);
                }

                using (Pen pen = new Pen(Color.FromArgb(200, 255, 255, 255), 1.5f))
                {
                    g.DrawRectangle(pen, 1, 1, size - 2, size - 2);
                }

                float fontSize = (text.Length > 1) ? 12.5f : 15.5f;
                using (Font font = new Font("Segoe UI", fontSize, FontStyle.Bold))
                using (SolidBrush textBrush = new SolidBrush(fg))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(text, font, textBrush, new RectangleF(0, 0, size, size + 1), sf);
                }

                IntPtr hIcon = bmp.GetHicon();
                return Icon.FromHandle(hIcon);
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
            _hookManager.Stop();
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _candidateBar.Dispose();
            _settingsForm.Dispose();
            Application.Exit();
        }
    }
}
