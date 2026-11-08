using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace UniPinyin
{
    public class Settings
    {
        public bool IsEnabled = true;           // Chinese input ON (true) / Direct English (false)
        public bool IsSimplified = true;        // Simplified (true) / Traditional (false)
        public int HotkeyMode = 2;              // Default: 2 = Alt+Z (0: Ctrl+Shift, 1: Ctrl+Space, 2: Alt+Z)
        public int CandidateCount = 5;          // 5 or 9 candidates
        public bool UseGoogleOnline = true;     // Google Cloud Suggestions enabled
        public bool SpaceSelectFirst = true;    // Space key commits 1st candidate
        public bool AutoFollowCaret = true;     // Candidate bar follows caret/cursor
        public bool StartWithWindows = false;   // Launch at Windows startup
        public bool ShowNotifications = true;   // Show balloon notifications
        public bool PlaySoundOnToggle = true;   // Play sound beep on language toggle

        private static string ConfigPath
        {
            get
            {
                string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UniPinyin");
                if (!Directory.Exists(dir))
                {
                    try { Directory.CreateDirectory(dir); } catch { }
                }
                return Path.Combine(dir, "config-en.ini");
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
                    sw.WriteLine("# UniPinyin Configuration (English Edition)");
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
                }

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
                            key.SetValue("UniPinyin-EN", Application.ExecutablePath);
                        }
                        else
                        {
                            key.DeleteValue("UniPinyin-EN", false);
                        }
                    }
                }
            }
            catch { }
        }
    }
}
