using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace UniPinyin
{
    public class FloatingCandidateBar : Form
    {
        private string _pinyin = "";
        private List<string> _candidates = new List<string>();
        private int _pageIndex = 0;
        private int _pageSize = 5;
        private int _totalCandidates = 0;

        private readonly Font _fontPinyin = new Font("Segoe UI", 10f, FontStyle.Bold);
        private readonly Font _fontCandidate = new Font("Microsoft YaHei", 12f, FontStyle.Regular);
        private readonly Font _fontNumber = new Font("Segoe UI", 9f, FontStyle.Bold);
        private readonly Font _fontPage = new Font("Segoe UI", 8.5f, FontStyle.Regular);

        public FloatingCandidateBar()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = Color.FromArgb(32, 33, 36); // Google Dark Theme
            this.DoubleBuffered = true;
            this.Height = 65;
            this.Width = 320;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
                cp.ExStyle |= 0x00000080; // WS_EX_TOOLWINDOW
                cp.ExStyle |= 0x00000008; // WS_EX_TOPMOST
                return cp;
            }
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        public void UpdateDisplay(string pinyin, List<string> allCandidates, int pageIndex, int pageSize)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => UpdateDisplay(pinyin, allCandidates, pageIndex, pageSize)));
                return;
            }

            _pinyin = pinyin;
            _pageIndex = pageIndex;
            _pageSize = pageSize;
            _totalCandidates = allCandidates != null ? allCandidates.Count : 0;

            _candidates.Clear();
            if (allCandidates != null && allCandidates.Count > 0)
            {
                int start = _pageIndex * _pageSize;
                for (int i = 0; i < _pageSize && (start + i) < allCandidates.Count; i++)
                {
                    _candidates.Add(allCandidates[start + i]);
                }
            }

            // Tính toán kích thước thanh hiển thị linh hoạt
            using (Graphics g = this.CreateGraphics())
            {
                float totalWidth = 30f;
                for (int i = 0; i < _candidates.Count; i++)
                {
                    string numStr = string.Format("{0}.", i + 1);
                    SizeF numSize = g.MeasureString(numStr, _fontNumber);
                    SizeF candSize = g.MeasureString(_candidates[i], _fontCandidate);
                    totalWidth += (numSize.Width + candSize.Width + 18f);
                }

                SizeF pinyinSize = g.MeasureString(_pinyin, _fontPinyin);
                float requiredWidth = Math.Max(240f, Math.Max(pinyinSize.Width + 80f, totalWidth + 50f));
                this.Width = (int)Math.Min(requiredWidth, 800f);
            }

            this.Invalidate();
        }

        private const int WM_MOUSEACTIVATE = 0x0021;
        private const int MA_NOACTIVATE = 3;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_MOUSEACTIVATE)
            {
                m.Result = (IntPtr)MA_NOACTIVATE;
                return;
            }
            base.WndProc(ref m);
        }

        public void PositionNearCaretOrCursor(bool autoFollow)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => PositionNearCaretOrCursor(autoFollow)));
                return;
            }

            Point target = new Point(0, 0);

            if (autoFollow)
            {
                // Thử tìm tọa độ Caret soạn thảo
                IntPtr hFore = NativeMethods.GetForegroundWindow();
                uint threadId = NativeMethods.GetWindowThreadProcessId(hFore, IntPtr.Zero);
                NativeMethods.GUITHREADINFO gui = new NativeMethods.GUITHREADINFO();
                gui.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(gui);

                if (NativeMethods.GetGUIThreadInfo(threadId, ref gui) && gui.hwndCaret != IntPtr.Zero)
                {
                    NativeMethods.POINT pt = new NativeMethods.POINT
                    {
                        X = gui.rcCaret.Left,
                        Y = gui.rcCaret.Bottom + 6
                    };
                    NativeMethods.ClientToScreen(gui.hwndCaret, ref pt);
                    target = new Point(pt.X, pt.Y);
                }

                // Nếu không lấy được Caret, dùng con trỏ chuột
                if (target.X == 0 && target.Y == 0)
                {
                    NativeMethods.POINT mousePt;
                    if (NativeMethods.GetCursorPos(out mousePt))
                    {
                        target = new Point(mousePt.X + 15, mousePt.Y + 22);
                    }
                }
            }
            else
            {
                // Cố định góc dưới bên phải màn hình
                Rectangle workArea = Screen.PrimaryScreen.WorkingArea;
                target = new Point(workArea.Right - this.Width - 30, workArea.Bottom - this.Height - 40);
            }

            // Đảm bảo không tràn màn hình
            Screen screen = Screen.FromPoint(target);
            Rectangle bounds = screen.WorkingArea;

            if (target.X + this.Width > bounds.Right) target.X = bounds.Right - this.Width - 10;
            if (target.X < bounds.Left) target.X = bounds.Left + 10;
            if (target.Y + this.Height > bounds.Bottom) target.Y = target.Y - this.Height - 30; // Hiện phía trên nếu sát mép dưới
            if (target.Y < bounds.Top) target.Y = bounds.Top + 10;

            // Dùng SetWindowPos với SWP_NOACTIVATE để tuyệt đối không cướp focus ứng dụng đang gõ
            NativeMethods.SetWindowPos(this.Handle, NativeMethods.HWND_TOPMOST, target.X, target.Y, this.Width, this.Height, NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_SHOWWINDOW);
        }

        public void HideBar()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(HideBar));
                return;
            }
            this.Hide();
            _pinyin = "";
            _candidates.Clear();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Khung viền và nền
            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(32, 33, 36)))
            using (Pen borderPen = new Pen(Color.FromArgb(70, 75, 85), 1.5f))
            {
                g.FillRectangle(bgBrush, rect);
                g.DrawRectangle(borderPen, rect);
            }

            // Dòng 1: Pinyin
            float currentX = 12f;
            using (SolidBrush pinyinBrush = new SolidBrush(Color.FromArgb(128, 216, 255))) // Light Cyan
            using (SolidBrush labelBrush = new SolidBrush(Color.FromArgb(160, 160, 160)))
            {
                g.DrawString("Pinyin:", _fontNumber, labelBrush, currentX, 6f);
                SizeF lblSize = g.MeasureString("Pinyin:", _fontNumber);
                currentX += lblSize.Width + 4f;

                g.DrawString(_pinyin, _fontPinyin, pinyinBrush, currentX, 4f);
            }

            // Hiển thị số trang ở góc phải dòng 1
            int totalPages = Math.Max(1, (int)Math.Ceiling((double)_totalCandidates / _pageSize));
            string pageText = string.Format("[{0}/{1}]", _pageIndex + 1, totalPages);
            SizeF pageSizeVal = g.MeasureString(pageText, _fontPage);
            using (SolidBrush pageBrush = new SolidBrush(Color.FromArgb(170, 175, 185)))
            {
                g.DrawString(pageText, _fontPage, pageBrush, this.Width - pageSizeVal.Width - 10f, 6f);
            }

            // Đường kẻ mờ phân cách
            using (Pen sepPen = new Pen(Color.FromArgb(50, 54, 62), 1f))
            {
                g.DrawLine(sepPen, 8, 28, this.Width - 8, 28);
            }

            // Dòng 2: Danh sách gợi ý từ
            float candidateX = 12f;
            float candidateY = 32f;

            using (SolidBrush numBrush = new SolidBrush(Color.FromArgb(255, 183, 77)))      // Cam/Vàng
            using (SolidBrush textBrush = new SolidBrush(Color.White))                       // Trắng
            using (SolidBrush hotkeyHintBrush = new SolidBrush(Color.FromArgb(140, 140, 140)))
            {
                if (_candidates.Count == 0)
                {
                    g.DrawString("(Đang tìm kiếm gợi ý...)", _fontPage, hotkeyHintBrush, candidateX, candidateY + 4f);
                }
                else
                {
                    for (int i = 0; i < _candidates.Count; i++)
                    {
                        string num = string.Format("{0}.", i + 1);
                        string cand = _candidates[i];

                        // Vẽ số thứ tự
                        g.DrawString(num, _fontNumber, numBrush, candidateX, candidateY + 3f);
                        SizeF numSize = g.MeasureString(num, _fontNumber);
                        candidateX += numSize.Width + 1f;

                        // Vẽ chữ tiếng Trung
                        g.DrawString(cand, _fontCandidate, textBrush, candidateX, candidateY);
                        SizeF candSize = g.MeasureString(cand, _fontCandidate);
                        candidateX += candSize.Width + 14f;
                    }
                }
            }
        }
    }
}
