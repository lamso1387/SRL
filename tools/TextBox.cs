using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SRL
{
    public class TextBoxBorderColor : TextBox
    {
        public Color border_color;
        public Color border_focus_color;
        public string border_or_focus_or_both;

        public TextBoxBorderColor(Color border_color_, Color border_focus_color_, string border_or_focus_or_both_)
        {
            border_color = border_color_;
            border_focus_color = border_focus_color_;
            border_or_focus_or_both = border_or_focus_or_both_;
        }
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern IntPtr GetWindowDC(IntPtr hwnd);
        private const int WM_NCPAINT = 0x85;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            Pen pen;
            switch (border_or_focus_or_both)
            {
                case "border":
                    if (m.Msg == WM_NCPAINT && !this.Focused)
                    {
                        pen = new Pen(border_color);
                        var dc = GetWindowDC(Handle);
                        using (Graphics g = Graphics.FromHdc(dc))
                        {
                            g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                        }
                    }
                    break;
                case "focus":
                    if (m.Msg == WM_NCPAINT && this.Focused)
                    {
                        pen = new Pen(border_focus_color);
                        var dc = GetWindowDC(Handle);
                        using (Graphics g = Graphics.FromHdc(dc))
                        {
                            g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                        }
                    }
                    break;
                case "both":
                    if (m.Msg == WM_NCPAINT && this.Focused)
                    {
                        pen = new Pen(border_focus_color);
                        var dc = GetWindowDC(Handle);
                        using (Graphics g = Graphics.FromHdc(dc))
                        {
                            g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                        }
                    }
                    else if (m.Msg == WM_NCPAINT && !this.Focused)
                    {
                        pen = new Pen(border_color);
                        var dc = GetWindowDC(Handle);
                        using (Graphics g = Graphics.FromHdc(dc))
                        {
                            g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                        }
                    }
                    break;

            }

        }
    }
}
