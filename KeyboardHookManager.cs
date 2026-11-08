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

                // Bỏ qua nếu là sự kiện do chính UniPinyin phát ra
                if (kb.dwExtraInfo == NativeMethods.EXTRA_INFO_SIGNATURE)
                {
                    return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
                }

                int msg = wParam.ToInt32();

                // Chỉ xử lý khi nhấn phím (KeyDown)
                if (msg == NativeMethods.WM_KEYDOWN || msg == NativeMethods.WM_SYSKEYDOWN)
                {
                    uint vk = kb.vkCode;

                    // 1. Kiểm tra phím tắt chuyển đổi chế độ (Hotkey Toggle)
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
                        return (IntPtr)1; // Chặn phím tắt lan truyền nếu cần
                    }

                    // 2. Nếu đang ở chế độ Tiếng Anh (tắt), cho phím đi qua bình thường
                    if (!_settings.IsEnabled)
                    {
                        return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
                    }

                    // 3. Đang ở chế độ Tiếng Trung:
                    bool isCtrl = (NativeMethods.GetAsyncKeyState(0x11) & 0x8000) != 0;
                    bool isAlt = (NativeMethods.GetAsyncKeyState(0x12) & 0x8000) != 0;
                    bool isShift = (NativeMethods.GetAsyncKeyState(0x10) & 0x8000) != 0;
                    bool isCapsLock = (Control.IsKeyLocked(Keys.CapsLock));

                    // Nếu đang nhấn kèm Ctrl hoặc Alt (phím tắt hệ thống như Ctrl+C, Ctrl+V, Alt+Tab...), cho đi qua
                    if (isCtrl || isAlt)
                    {
                        if (_candidateManager.HasBuffer)
                        {
                            _candidateManager.CommitRaw();
                            _candidateBar.HideBar();
                        }
                        return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
                    }

                    // A. Khi chưa có buffer pinyin
                    if (!_candidateManager.HasBuffer)
                    {
                        // Nếu nhấn chữ cái A-Z và không giữ Shift, không CapsLock
                        if (vk >= 0x41 && vk <= 0x5A)
                        {
                            if (isShift || isCapsLock)
                            {
                                // Người dùng muốn gõ chữ hoa tiếng Anh
                                return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
                            }

                            char c = (char)('a' + (vk - 0x41));
                            _candidateManager.AppendChar(c, _settings);
                            _candidateBar.PositionNearCaretOrCursor(_settings.AutoFollowCaret);
                            return (IntPtr)1; // Chặn ký tự gốc
                        }

                        return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
                    }

                    // B. Khi ĐÃ CÓ buffer pinyin (đang soạn thảo)
                    // B1: Tiếp tục gõ chữ cái A-Z
                    if (vk >= 0x41 && vk <= 0x5A)
                    {
                        char c = (char)('a' + (vk - 0x41));
                        _candidateManager.AppendChar(c, _settings);
                        _candidateBar.PositionNearCaretOrCursor(_settings.AutoFollowCaret);
                        return (IntPtr)1;
                    }

                    // B1.1: Gõ dấu nháy đơn ' (VK_OEM_7 = 0xDE) để ngăn cách âm tiết / từ
                    if (vk == 0xDE && !isShift)
                    {
                        if (!_candidateManager.CurrentBuffer.EndsWith("'"))
                        {
                            _candidateManager.AppendChar('\'', _settings);
                            _candidateBar.PositionNearCaretOrCursor(_settings.AutoFollowCaret);
                        }
                        return (IntPtr)1;
                    }

                    // B2: Nhấn số 1-9 để chọn từ
                    int digit = -1;
                    if (vk >= 0x31 && vk <= 0x39) // Top row '1' - '9'
                    {
                        digit = (int)(vk - 0x30);
                    }
                    else if (vk >= 0x61 && vk <= 0x69) // NumPad '1' - '9'
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

                    // B3: Phím Space (chọn từ đầu tiên nếu bật SpaceSelectFirst)
                    if (vk == 0x20) // VK_SPACE
                    {
                        if (_settings.SpaceSelectFirst)
                        {
                            bool committed = _candidateManager.CommitCandidateByIndex(0, _settings.CandidateCount);
                            if (!committed)
                            {
                                // Fallback: Nếu chưa có gợi ý hoặc từ điển không khớp, xuất chuỗi pinyin gốc + dấu cách
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

                    // B4: Phím Enter (chấp nhận chữ thô Pinyin tiếng Anh)
                    if (vk == 0x0D) // VK_RETURN
                    {
                        _candidateManager.CommitRaw();
                        _candidateBar.HideBar();
                        return (IntPtr)1;
                    }

                    // B5: Phím Backspace (xóa 1 ký tự pinyin)
                    if (vk == 0x08) // VK_BACK
                    {
                        _candidateManager.RemoveLastChar(_settings);
                        if (!_candidateManager.HasBuffer)
                        {
                            _candidateBar.HideBar();
                        }
                        return (IntPtr)1;
                    }

                    // B6: Phím Escape (hủy bỏ)
                    if (vk == 0x1B) // VK_ESCAPE
                    {
                        _candidateManager.Clear();
                        _candidateBar.HideBar();
                        return (IntPtr)1;
                    }

                    // B7: Chuyển trang: PageDown hoặc dấu chấm '.' hoặc '>'
                    if (vk == 0x22 || vk == 0xBE) // VK_NEXT (PageDown) hoặc VK_OEM_PERIOD
                    {
                        _candidateManager.NextPage(_settings.CandidateCount);
                        return (IntPtr)1;
                    }

                    // B8: Lùi trang: PageUp hoặc dấu phẩy ',' hoặc '<'
                    if (vk == 0x21 || vk == 0xBC) // VK_PRIOR (PageUp) hoặc VK_OEM_COMMA
                    {
                        _candidateManager.PrevPage(_settings.CandidateCount);
                        return (IntPtr)1;
                    }

                    // B9: Các phím khác (dấu câu, mũi tên...): Cam kết từ đầu tiên và để phím đi qua
                    _candidateManager.CommitCandidateByIndex(0, _settings.CandidateCount);
                    _candidateBar.HideBar();
                    return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
                }
            }

            return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private bool CheckHotkeyToggle(uint vk)
        {
            // 0: Ctrl + Shift
            if (_settings.HotkeyMode == 0)
            {
                if (vk == 0x10) // Nhấn Shift
                {
                    return (NativeMethods.GetAsyncKeyState(0x11) & 0x8000) != 0; // Khi đang giữ Ctrl
                }
                if (vk == 0x11) // Nhấn Ctrl
                {
                    return (NativeMethods.GetAsyncKeyState(0x10) & 0x8000) != 0; // Khi đang giữ Shift
                }
            }
            // 1: Ctrl + Space
            else if (_settings.HotkeyMode == 1)
            {
                if (vk == 0x20) // Nhấn Space
                {
                    return (NativeMethods.GetAsyncKeyState(0x11) & 0x8000) != 0; // Khi đang giữ Ctrl
                }
            }
            // 2: Alt + Z
            else if (_settings.HotkeyMode == 2)
            {
                if (vk == 0x5A) // Nhấn 'Z'
                {
                    return (NativeMethods.GetAsyncKeyState(0x12) & 0x8000) != 0; // Khi đang giữ Alt
                }
            }

            return false;
        }
    }
}
