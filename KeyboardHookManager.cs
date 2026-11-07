using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UniPinyin
{
    public class KeyboardHookManager
    {
        private IntPtr _hookId = IntPtr.Zero;
        private NativeMethods.HookProc _proc;

        private readonly Settings _settings;
        private readonly CandidateManager _candidateManager;
        private readonly FloatingCandidateBar _candidateBar;

        public event Action<bool> OnLanguageToggled;

        public KeyboardHookManager(Settings settings, CandidateManager candidateManager, FloatingCandidateBar candidateBar)
        {
            _settings = settings;
            _candidateManager = candidateManager;
            _candidateBar = candidateBar;
            _proc = HookCallback;
        }

        public void Start()
        {
            if (_hookId == IntPtr.Zero)
            {
                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    _hookId = NativeMethods.SetWindowsHookEx(
                        NativeMethods.WH_KEYBOARD_LL,
                        _proc,
                        NativeMethods.GetModuleHandle(curModule.ModuleName),
                        0);
                }
            }
        }

        public void Stop()
        {
            if (_hookId != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(_hookId);
                _hookId = IntPtr.Zero;
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                NativeMethods.KBDLLHOOKSTRUCT kb = (NativeMethods.KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(NativeMethods.KBDLLHOOKSTRUCT));

                // Ignore simulated inputs sent by UniPinyin
                if (kb.dwExtraInfo == NativeMethods.EXTRA_INFO_SIGNATURE)
                {
                    return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
                }

                int msg = wParam.ToInt32();

                if (msg == NativeMethods.WM_KEYDOWN || msg == NativeMethods.WM_SYSKEYDOWN)
                {
                    uint vk = kb.vkCode;

                    // 1. Check Toggle Hotkey
                    if (CheckHotkeyToggle(vk))
                    {
                        _settings.IsEnabled = !_settings.IsEnabled;
                        _settings.Save();
                        _candidateManager.Clear();
                        _candidateBar.HideBar();

                        if (OnLanguageToggled != null)
                        {
                            OnLanguageToggled(_settings.IsEnabled);
                        }
                        return (IntPtr)1;
                    }

                    // 2. If disabled (English mode), pass all keys through
                    if (!_settings.IsEnabled)
                    {
                        return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
                    }

                    // 3. Chinese mode:
                    bool isCtrl = (NativeMethods.GetAsyncKeyState(0x11) & 0x8000) != 0;
                    bool isAlt = (NativeMethods.GetAsyncKeyState(0x12) & 0x8000) != 0;
                    bool isShift = (NativeMethods.GetAsyncKeyState(0x10) & 0x8000) != 0;
                    bool isCapsLock = (Control.IsKeyLocked(Keys.CapsLock));

                    // If system shortcut (Ctrl+C, Ctrl+V, Alt+Tab...), pass through
                    if (isCtrl || isAlt)
                    {
                        if (_candidateManager.HasBuffer)
                        {
                            _candidateManager.CommitRaw();
                            _candidateBar.HideBar();
                        }
                        return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
                    }

                    // A. When buffer is empty
                    if (!_candidateManager.HasBuffer)
                    {
                        if (vk >= 0x41 && vk <= 0x5A)
                        {
                            if (isShift || isCapsLock)
                            {
                                return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
                            }

                            char c = (char)('a' + (vk - 0x41));
                            _candidateManager.AppendChar(c, _settings);
                            _candidateBar.PositionNearCaretOrCursor(_settings.AutoFollowCaret);
                            return (IntPtr)1;
                        }

                        return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
                    }

                    // B. When composing Pinyin
                    // B1: Append characters a-z
                    if (vk >= 0x41 && vk <= 0x5A)
                    {
                        char c = (char)('a' + (vk - 0x41));
                        _candidateManager.AppendChar(c, _settings);
                        _candidateBar.PositionNearCaretOrCursor(_settings.AutoFollowCaret);
                        return (IntPtr)1;
                    }

                    // B2: Numbers 1-9 to select candidates
                    int digit = -1;
                    if (vk >= 0x31 && vk <= 0x39)
                    {
                        digit = (int)(vk - 0x30);
                    }
                    else if (vk >= 0x61 && vk <= 0x69)
                    {
                        digit = (int)(vk - 0x60);
                    }

                    if (digit >= 1 && digit <= _settings.CandidateCount)
                    {
                        if (_candidateManager.CommitCandidateByIndex(digit - 1, _settings.CandidateCount))
                        {
                            _candidateBar.HideBar();
                            return (IntPtr)1;
                        }
                    }

                    // B3: Space key (commit 1st candidate or fallback raw + space)
                    if (vk == 0x20)
                    {
                        if (_settings.SpaceSelectFirst)
                        {
                            bool committed = _candidateManager.CommitCandidateByIndex(0, _settings.CandidateCount);
                            if (!committed)
                            {
                                _candidateManager.CommitRaw();
                                NativeMethods.SendUnicodeString(" ");
                            }
                        }
                        else
                        {
                            _candidateManager.CommitRaw();
                            NativeMethods.SendUnicodeString(" ");
                        }
                        _candidateBar.HideBar();
                        return (IntPtr)1;
                    }

                    // B4: Enter key (commit raw pinyin)
                    if (vk == 0x0D)
                    {
                        _candidateManager.CommitRaw();
                        _candidateBar.HideBar();
                        return (IntPtr)1;
                    }

                    // B5: Backspace (remove last char)
                    if (vk == 0x08)
                    {
                        _candidateManager.RemoveLastChar(_settings);
                        if (!_candidateManager.HasBuffer)
                        {
                            _candidateBar.HideBar();
                        }
                        return (IntPtr)1;
                    }

                    // B6: Escape (cancel)
                    if (vk == 0x1B)
                    {
                        _candidateManager.Clear();
                        _candidateBar.HideBar();
                        return (IntPtr)1;
                    }

                    // B7: PageDown / '.' / '>'
                    if (vk == 0x22 || vk == 0xBE)
                    {
                        _candidateManager.NextPage(_settings.CandidateCount);
                        return (IntPtr)1;
                    }

                    // B8: PageUp / ',' / '<'
                    if (vk == 0x21 || vk == 0xBC)
                    {
                        _candidateManager.PrevPage(_settings.CandidateCount);
                        return (IntPtr)1;
                    }

                    // B9: Other keys: commit 1st candidate and let key through
                    _candidateManager.CommitCandidateByIndex(0, _settings.CandidateCount);
                    _candidateBar.HideBar();
                    return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
                }
            }

            return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private bool CheckHotkeyToggle(uint vk)
        {
            if (_settings.HotkeyMode == 0) // Ctrl + Shift
            {
                if (vk == 0x10)
                {
                    return (NativeMethods.GetAsyncKeyState(0x11) & 0x8000) != 0;
                }
                if (vk == 0x11)
                {
                    return (NativeMethods.GetAsyncKeyState(0x10) & 0x8000) != 0;
                }
            }
            else if (_settings.HotkeyMode == 1) // Ctrl + Space
            {
                if (vk == 0x20)
                {
                    return (NativeMethods.GetAsyncKeyState(0x11) & 0x8000) != 0;
                }
            }
            else if (_settings.HotkeyMode == 2) // Alt + Z
            {
                if (vk == 0x5A)
                {
                    return (NativeMethods.GetAsyncKeyState(0x12) & 0x8000) != 0;
                }
            }

            return false;
        }
    }
}
