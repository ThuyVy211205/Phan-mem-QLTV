using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GUI.Theme
{
    public static class RoundedCorner
    {
        public static void Apply(Control ctrl, int radius = 8)
        {
            void SetRegion()
            {
                if (ctrl.Width <= 0 || ctrl.Height <= 0) return;
                var path = new GraphicsPath();
                int r = Math.Min(radius, Math.Min(ctrl.Width / 2, ctrl.Height / 2));
                path.AddArc(0, 0, r, r, 180, 90);
                path.AddArc(ctrl.Width - r - 1, 0, r, r, 270, 90);
                path.AddArc(ctrl.Width - r - 1, ctrl.Height - r - 1, r, r, 0, 90);
                path.AddArc(0, ctrl.Height - r - 1, r, r, 90, 90);
                path.CloseFigure();
                ctrl.Region = new Region(path);
            }

            ctrl.SizeChanged += (s, e) => SetRegion();
            if (ctrl.IsHandleCreated)
                SetRegion();
        }
    }
}
