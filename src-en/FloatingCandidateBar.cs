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

        private readonly Font _fontPinyin = new Font("Segoe UI", 10.5f, FontStyle.Bold);
        private readonly Font _fontCandidate = new Font("Microsoft YaHei UI", 12.5f, FontStyle.Regular);
        private readonly Font _fontCandidateBold = new Font("Microsoft YaHei UI", 12.5f, FontStyle.Bold);
        private readonly Font _fontNumber = new Font("Segoe UI", 9f, FontStyle.Bold);
        private readonly Font _fontBadge = new Font("Segoe UI", 7.5f, FontStyle.Bold);
        private readonly Font _fontPage = new Font("Segoe UI", 8.5f, FontStyle.Regular);

        public FloatingCandidateBar()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = Color.FromArgb(28, 28, 30); // Apple Dark Mode Charcoal
            this.DoubleBuffered = true;
            this.Height = 68;
            this.Width = 320;
            ApplyRoundedCorners();
        }

        private void ApplyRoundedCorners()
        {
            try
            {
                IntPtr hRgn = NativeMethods.CreateRoundRectRgn(0, 0, this.Width, this.Height, 14, 14);
                this.Region = Region.FromHrgn(hRgn);
            }
            catch { }
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

            using (Graphics g = this.CreateGraphics())
            {
                float totalWidth = 32f;
                for (int i = 0; i < _candidates.Count; i++)
                {
                    string numStr = string.Format("{0}.", i + 1);
                    SizeF numSize = g.MeasureString(numStr, _fontNumber);
                    Font cFont = (i == 0) ? _fontCandidateBold : _fontCandidate;
                    SizeF candSize = g.MeasureString(_candidates[i], cFont);
                    totalWidth += (numSize.Width + candSize.Width + 22f);
                }

                SizeF pinyinSize = g.MeasureString(_pinyin, _fontPinyin);
                float requiredWidth = Math.Max(260f, Math.Max(pinyinSize.Width + 110f, totalWidth + 30f));
                this.Width = (int)Math.Min(requiredWidth, 850f);
            }

            ApplyRoundedCorners();
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
                NativeMethods.POINT caretPt;
                if (NativeMethods.TryGetCaretScreenPoint(out caretPt))
                {
                    target = new Point(caretPt.X, caretPt.Y);
                }
                else
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
                Rectangle workArea = Screen.PrimaryScreen.WorkingArea;
                target = new Point(workArea.Right - this.Width - 30, workArea.Bottom - this.Height - 40);
            }

            Screen screen = Screen.FromPoint(target);
            Rectangle bounds = screen.WorkingArea;

            if (target.X + this.Width > bounds.Right) target.X = bounds.Right - this.Width - 10;
            if (target.X < bounds.Left) target.X = bounds.Left + 10;
            if (target.Y + this.Height > bounds.Bottom) target.Y = target.Y - this.Height - 30;
            if (target.Y < bounds.Top) target.Y = bounds.Top + 10;

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

            // Apple Dark Mode translucent glass panel
            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(28, 28, 30)))
            using (Pen borderPen = new Pen(Color.FromArgb(68, 68, 72), 1.2f))
            {
                g.FillRectangle(bgBrush, rect);
                g.DrawRectangle(borderPen, rect);
            }

            // Top Row (Pinyin & Page Badge)
            float currentX = 12f;

            // Apple Badge "PINYIN"
            RectangleF badgeRect = new RectangleF(currentX, 6f, 46f, 16f);
            using (SolidBrush badgeBg = new SolidBrush(Color.FromArgb(48, 48, 52)))
            using (SolidBrush badgeText = new SolidBrush(Color.FromArgb(160, 160, 165)))
            {
                using (GraphicsPath bPath = GetRoundedPath(badgeRect, 4f))
                {
                    g.FillPath(badgeBg, bPath);
                }
                StringFormat sfBadge = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString("PINYIN", _fontBadge, badgeText, badgeRect, sfBadge);
            }
            currentX += 52f;

            // Pinyin string (Apple Blue #0A84FF)
            using (SolidBrush pinyinBrush = new SolidBrush(Color.FromArgb(10, 132, 255)))
            {
                g.DrawString(_pinyin, _fontPinyin, pinyinBrush, currentX, 4f);
            }

            // Page count badge at top-right
            int totalPages = Math.Max(1, (int)Math.Ceiling((double)_totalCandidates / _pageSize));
            string pageText = string.Format("{0} / {1}", _pageIndex + 1, totalPages);
            SizeF pageSizeVal = g.MeasureString(pageText, _fontPage);
            float pageX = this.Width - pageSizeVal.Width - 20f;
            RectangleF pageBadgeRect = new RectangleF(pageX, 6f, pageSizeVal.Width + 10f, 16f);
            using (SolidBrush pageBg = new SolidBrush(Color.FromArgb(44, 44, 48)))
            using (SolidBrush pageBrush = new SolidBrush(Color.FromArgb(170, 175, 185)))
            {
                using (GraphicsPath pPath = GetRoundedPath(pageBadgeRect, 4f))
                {
                    g.FillPath(pageBg, pPath);
                }
                StringFormat sfPage = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(pageText, _fontPage, pageBrush, pageBadgeRect, sfPage);
            }

            // Hairline divider
            using (Pen sepPen = new Pen(Color.FromArgb(48, 48, 52), 1f))
            {
                g.DrawLine(sepPen, 10, 26, this.Width - 10, 26);
            }

            // Bottom Row (Candidates)
            float candidateX = 12f;
            float candidateY = 32f;

            using (SolidBrush numBrushActive = new SolidBrush(Color.FromArgb(10, 132, 255)))
            using (SolidBrush numBrushOther = new SolidBrush(Color.FromArgb(255, 159, 10)))
            using (SolidBrush textBrushActive = new SolidBrush(Color.White))
            using (SolidBrush textBrushOther = new SolidBrush(Color.FromArgb(240, 240, 245)))
            using (SolidBrush hintBrush = new SolidBrush(Color.FromArgb(142, 142, 147)))
            {
                if (_candidates.Count == 0)
                {
                    g.DrawString("(Searching for candidates...)", _fontPage, hintBrush, candidateX, candidateY + 5f);
                }
                else
                {
                    for (int i = 0; i < _candidates.Count; i++)
                    {
                        string num = string.Format("{0}.", i + 1);
                        string cand = _candidates[i];
                        Font cFont = (i == 0) ? _fontCandidateBold : _fontCandidate;
                        SizeF candSize = g.MeasureString(cand, cFont);
                        SizeF numSize = g.MeasureString(num, _fontNumber);
                        float itemWidth = numSize.Width + candSize.Width + 6f;

                        // Pill highlight behind candidate #1 (Spacebar default)
                        if (i == 0)
                        {
                            RectangleF pillRect = new RectangleF(candidateX - 3f, candidateY + 1f, itemWidth + 8f, 28f);
                            using (SolidBrush pillBg = new SolidBrush(Color.FromArgb(35, 10, 132, 255)))
                            using (Pen pillBorder = new Pen(Color.FromArgb(80, 10, 132, 255), 1f))
                            {
                                using (GraphicsPath pillPath = GetRoundedPath(pillRect, 6f))
                                {
                                    g.FillPath(pillBg, pillPath);
                                    g.DrawPath(pillBorder, pillPath);
                                }
                            }
                        }

                        // Index number
                        SolidBrush numB = (i == 0) ? numBrushActive : numBrushOther;
                        g.DrawString(num, _fontNumber, numB, candidateX, candidateY + 5f);
                        candidateX += numSize.Width + 1f;

                        // Character
                        SolidBrush txtB = (i == 0) ? textBrushActive : textBrushOther;
                        g.DrawString(cand, cFont, txtB, candidateX, candidateY + 1f);
                        candidateX += candSize.Width + 18f;
                    }
                }
            }
        }

        private static GraphicsPath GetRoundedPath(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float d = radius * 2f;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
