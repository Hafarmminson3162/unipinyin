using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace UniPinyin
{
    public class Settings
    {
        public bool IsEnabled = true;           // Bật/tắt gõ tiếng Trung (true: Trung, false: Anh)
        public bool IsSimplified = true;        // Giản thể (true) / Phồn thể (false)
        public int HotkeyMode = 2;              // Mặc định: 2 = Alt+Z (0: Ctrl+Shift, 1: Ctrl+Space, 2: Alt+Z)
        public int CandidateCount = 5;          // 5 hoặc 9 gợi ý
        public bool UseGoogleOnline = true;     // Ưu tiên Google Cloud Suggestion
        public bool SpaceSelectFirst = true;    // Phím Space chọn từ số 1
        public bool AutoFollowCaret = true;     // Tự động bám theo con trỏ chuột/caret
        public bool StartWithWindows = false;   // Khởi động cùng Windows
        public bool ShowNotifications = true;   // Hiện thông báo chuyển chế độ
        public bool PlaySoundOnToggle = true;   // Âm báo bíp khi đổi ngôn ngữ
        public string UiLanguage = "vi";        // Ngôn ngữ giao diện ("vi" hoặc "en")

        private static string ConfigPath
        {
            get
            {
                string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UniPinyin");
                if (!Directory.Exists(dir))
                {
                    try { Directory.CreateDirectory(dir); } catch { }
                }
                return Path.Combine(dir, "config.ini");
            }
        }

        public static Settings Load()
        {
            Settings s = new Settings();
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string[] lines = File.ReadAllLines(ConfigPath);
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
                        string[] parts = line.Split(new char[] { '=' }, 2);
                        if (parts.Length != 2) continue;
                        string key = parts[0].Trim();
                        string val = parts[1].Trim();

                        switch (key)
                        {
                            case "IsEnabled": s.IsEnabled = bool.Parse(val); break;
                            case "IsSimplified": s.IsSimplified = bool.Parse(val); break;
                            case "HotkeyMode": s.HotkeyMode = int.Parse(val); break;
                            case "CandidateCount": s.CandidateCount = int.Parse(val); break;
                            case "UseGoogleOnline": s.UseGoogleOnline = bool.Parse(val); break;
                            case "SpaceSelectFirst": s.SpaceSelectFirst = bool.Parse(val); break;
                            case "AutoFollowCaret": s.AutoFollowCaret = bool.Parse(val); break;
                            case "StartWithWindows": s.StartWithWindows = bool.Parse(val); break;
                            case "ShowNotifications": s.ShowNotifications = bool.Parse(val); break;
                            case "PlaySoundOnToggle": s.PlaySoundOnToggle = bool.Parse(val); break;
                            case "UiLanguage": s.UiLanguage = val.ToLowerInvariant(); break;
                        }
                    }
                }
            }
            catch { }
            return s;
        }

        public void Save()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(ConfigPath, false, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine("# Cấu hình UniPinyin");
                    sw.WriteLine("IsEnabled=" + IsEnabled);
                    sw.WriteLine("IsSimplified=" + IsSimplified);
                    sw.WriteLine("HotkeyMode=" + HotkeyMode);
                    sw.WriteLine("CandidateCount=" + CandidateCount);
                    sw.WriteLine("UseGoogleOnline=" + UseGoogleOnline);
                    sw.WriteLine("SpaceSelectFirst=" + SpaceSelectFirst);
                    sw.WriteLine("AutoFollowCaret=" + AutoFollowCaret);
                    sw.WriteLine("StartWithWindows=" + StartWithWindows);
                    sw.WriteLine("ShowNotifications=" + ShowNotifications);
                    sw.WriteLine("PlaySoundOnToggle=" + PlaySoundOnToggle);
                    sw.WriteLine("UiLanguage=" + UiLanguage);
                }

                // Cập nhật Registry AutoStart
                SetAutoStart(StartWithWindows);
            }
            catch { }
        }

        public static void SetAutoStart(bool enable)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (key != null)
                    {
                        if (enable)
                        {
                            key.SetValue("UniPinyin", Application.ExecutablePath);
                        }
                        else
                        {
                            key.DeleteValue("UniPinyin", false);
                        }
                    }
                }
            }
            catch { }
        }
    }
}
