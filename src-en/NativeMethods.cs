using System;
using System.Runtime.InteropServices;

namespace UniPinyin
{
    public static class NativeMethods
    {
        public const int WH_KEYBOARD_LL = 13;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WM_SYSKEYUP = 0x0105;

        public const uint INPUT_KEYBOARD = 1;
        public const uint KEYEVENTF_KEYUP = 0x0002;
        public const uint KEYEVENTF_UNICODE = 0x0004;

        public static readonly IntPtr EXTRA_INFO_SIGNATURE = (IntPtr)0x554E4950; // "UNIP"

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetGUIThreadInfo(uint idThread, ref GUITHREADINFO lpgui);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("oleacc.dll")]
        public static extern int AccessibleObjectFromWindow(IntPtr hwnd, uint dwId, ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppvObject);

        [ComImport, Guid("618736e0-3c3d-11cf-810c-00aa00389b71"), InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface IAccessible
        {
            void D1(); void D2(); void D3(); void D4();
            [PreserveSig] int get_accParent(out object ppdispParent);
            [PreserveSig] int get_accChildCount(out int pcountChildren);
            [PreserveSig] int get_accChild(object varChild, out object ppdispChild);
            [PreserveSig] int get_accName(object varChild, out string pszName);
            [PreserveSig] int get_accValue(object varChild, out string pszValue);
            [PreserveSig] int get_accDescription(object varChild, out string pszDescription);
            [PreserveSig] int get_accRole(object varChild, out object pvarRole);
            [PreserveSig] int get_accState(object varChild, out object pvarState);
            [PreserveSig] int get_accHelp(object varChild, out string pszHelp);
            [PreserveSig] int get_accHelpTopic(out string pszHelpFile, object varChild, out int pidTopic);
            [PreserveSig] int get_accKeyboardShortcut(object varChild, out string pszKeyboardShortcut);
            [PreserveSig] int get_accFocus(out object pvarChild);
            [PreserveSig] int get_accSelection(out object pvarChildren);
            [PreserveSig] int get_accDefaultAction(object varChild, out string pszDefaultAction);
            [PreserveSig] int accSelect(int flagsSelect, object varChild);
            [PreserveSig] int accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, object varChild);
            [PreserveSig] int accNavigate(int navDir, object varStart, out object pvarEndUpAt);
            [PreserveSig] int accHitTest(int xLeft, int yTop, out object pvarChild);
            [PreserveSig] int accDoDefaultAction(object varChild);
            [PreserveSig] int put_accName(object varChild, string szName);
            [PreserveSig] int put_accValue(object varChild, string szValue);
        }

        public const uint OBJID_CARET = 0xFFFFFFF8;
        public static readonly Guid IID_IAccessible = new Guid("618736e0-3c3d-11cf-810c-00aa00389b71");

        public const int GWL_STYLE = -16;
        public const int ES_READONLY = 0x0800;

        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public const uint SWP_NOACTIVATE = 0x0010;
        public const uint SWP_SHOWWINDOW = 0x0040;

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GUITHREADINFO
        {
            public uint cbSize;
            public uint flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rcCaret;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public uint type;
            public MOUSEKEYBDHARDWAREINPUT u;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        public static bool SendUnicodeString(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            INPUT[] inputs = new INPUT[text.Length * 2];
            int idx = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];

                // KeyDown
                inputs[idx].type = INPUT_KEYBOARD;
                inputs[idx].u.ki.wVk = 0;
                inputs[idx].u.ki.wScan = (ushort)ch;
                inputs[idx].u.ki.dwFlags = KEYEVENTF_UNICODE;
                inputs[idx].u.ki.dwExtraInfo = EXTRA_INFO_SIGNATURE;
                idx++;

                // KeyUp
                inputs[idx].type = INPUT_KEYBOARD;
                inputs[idx].u.ki.wVk = 0;
                inputs[idx].u.ki.wScan = (ushort)ch;
                inputs[idx].u.ki.dwFlags = KEYEVENTF_UNICODE | KEYEVENTF_KEYUP;
                inputs[idx].u.ki.dwExtraInfo = EXTRA_INFO_SIGNATURE;
                idx++;
            }

            int structSize = Marshal.SizeOf(typeof(INPUT));
            uint sent = SendInput((uint)inputs.Length, inputs, structSize);
            return sent > 0;
        }

        public static bool TryGetMsaaCaretRect(IntPtr hWnd, out RECT rect)
        {
            rect = new RECT();
            if (hWnd == IntPtr.Zero) return false;
            try
            {
                Guid iid = IID_IAccessible;
                object obj;
                int hr = AccessibleObjectFromWindow(hWnd, OBJID_CARET, ref iid, out obj);
                if (hr >= 0 && obj is IAccessible)
                {
                    int l, t, w, h;
                    if (((IAccessible)obj).accLocation(out l, out t, out w, out h, 0) == 0 && (h > 0 || w > 0))
                    {
                        rect.Left = l;
                        rect.Top = t;
                        rect.Right = l + w;
                        rect.Bottom = t + h;
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        public static bool TryGetCaretScreenPoint(out POINT pt)
        {
            pt = new POINT();
            IntPtr hFore = GetForegroundWindow();
            if (hFore == IntPtr.Zero) return false;

            uint threadId = GetWindowThreadProcessId(hFore, IntPtr.Zero);
            GUITHREADINFO gui = new GUITHREADINFO();
            gui.cbSize = (uint)Marshal.SizeOf(gui);

            // 1. Try standard Win32 Caret
            if (GetGUIThreadInfo(threadId, ref gui) && gui.hwndCaret != IntPtr.Zero)
            {
                pt.X = gui.rcCaret.Left;
                pt.Y = gui.rcCaret.Bottom + 6;
                ClientToScreen(gui.hwndCaret, ref pt);
                return true;
            }

            // 2. Try MSAA Caret (Chrome, Edge, Word, etc.)
            RECT msaaRect;
            if (gui.hwndFocus != IntPtr.Zero && TryGetMsaaCaretRect(gui.hwndFocus, out msaaRect))
            {
                pt.X = msaaRect.Left;
                pt.Y = msaaRect.Bottom + 6;
                return true;
            }

            if (TryGetMsaaCaretRect(hFore, out msaaRect))
            {
                pt.X = msaaRect.Left;
                pt.Y = msaaRect.Bottom + 6;
                return true;
            }

            return false;
        }

        public static bool IsTypingContextActive()
        {
            IntPtr hFore = GetForegroundWindow();
            if (hFore == IntPtr.Zero) return false;

            System.Text.StringBuilder sbCls = new System.Text.StringBuilder(256);
            GetClassName(hFore, sbCls, 256);
            string foreCls = sbCls.ToString();

            // 1. Exclude system non-typing containers
            if (foreCls == "Progman" || foreCls == "WorkerW" || 
                foreCls == "Shell_TrayWnd" || foreCls == "Shell_SecondaryTrayWnd" ||
                foreCls == "NotifyIconOverflowWindow")
            {
                return false;
            }

            uint threadId = GetWindowThreadProcessId(hFore, IntPtr.Zero);
            GUITHREADINFO gui = new GUITHREADINFO();
            gui.cbSize = (uint)Marshal.SizeOf(gui);

            if (GetGUIThreadInfo(threadId, ref gui))
            {
                // A. Active Win32 Caret
                if (gui.hwndCaret != IntPtr.Zero) return true;
                if ((gui.flags & 1) != 0) return true; // GUI_CARETBLINKING
                if (gui.rcCaret.Right > gui.rcCaret.Left || gui.rcCaret.Bottom > gui.rcCaret.Top) return true;

                // B. Focus control check
                if (gui.hwndFocus != IntPtr.Zero)
                {
                    System.Text.StringBuilder sbFocus = new System.Text.StringBuilder(256);
                    GetClassName(gui.hwndFocus, sbFocus, 256);
                    string focusCls = sbFocus.ToString();

                    // Standard text edit controls
                    if (focusCls.IndexOf("Edit", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        focusCls.IndexOf("RichEdit", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        focusCls.IndexOf("Scintilla", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        focusCls.IndexOf("TextBox", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        focusCls == "ConsoleWindowClass" ||
                        focusCls == "CASCADIA_HOSTING_WINDOW_CLASS")
                    {
                        int style = GetWindowLong(gui.hwndFocus, GWL_STYLE);
                        if ((style & ES_READONLY) == 0)
                        {
                            return true;
                        }
                    }

                    // Explorer list view (when not renaming)
                    if (focusCls == "UIItemsView" || focusCls == "DirectUIHWND" || focusCls == "SysListView32" || focusCls == "SysTreeView32")
                    {
                        if (foreCls == "CabinetWClass" || foreCls == "ExploreWClass")
                        {
                            return false;
                        }
                    }

                    // Try MSAA Caret on hwndFocus
                    RECT rectFocus;
                    if (TryGetMsaaCaretRect(gui.hwndFocus, out rectFocus))
                    {
                        return true;
                    }
                }
            }

            // C. Try MSAA Caret on Foreground window
            RECT rectFore;
            if (TryGetMsaaCaretRect(hFore, out rectFore))
            {
                return true;
            }

            return false;
        }
    }
}
