using System.Windows.Forms;

namespace GUI.Theme
{
    /// <summary>
    /// Giúp form luôn vừa đẹp với màn hình hiện tại, có khoảng đệm quanh cửa sổ
    /// (tránh dính mép, che taskbar, cắt chữ trên màn nhỏ hoặc DPI cao).
    /// </summary>
    public static class FormFitter
    {
        /// <summary>
        /// Áp kích thước hợp lý:
        ///   - Normal: 85% vùng làm việc, căn giữa.
        ///   - Maximized + borderless: thu về vừa vùng làm việc.
        /// Gọi trong Load của form.
        /// </summary>
        public static void Fit(Form form, double ratio = 0.85)
        {
            var wa = Screen.FromControl(form).WorkingArea;

            if (form.WindowState == FormWindowState.Maximized)
            {
                if (form.FormBorderStyle == FormBorderStyle.None)
                {
                    form.WindowState = FormWindowState.Normal;
                    form.Bounds = wa;
                }
                return;
            }

            // Chỉ áp khi cửa sổ lớn hơn 85% hoặc vượt màn — giữ nguyên nếu đã nhỏ hơn.
            int targetW = (int)(wa.Width * ratio);
            int targetH = (int)(wa.Height * ratio);
            bool needResize = form.Width > wa.Width || form.Height > wa.Height
                           || form.Width > targetW || form.Height > targetH;

            if (needResize)
            {
                int w = targetW;
                int h = targetH;
                // Tôn trọng kích thước tối thiểu của form (nếu có).
                if (form.MinimumSize.Width > w) w = form.MinimumSize.Width;
                if (form.MinimumSize.Height > h) h = form.MinimumSize.Height;
                form.Size = new System.Drawing.Size(w, h);
            }

            // Căn giữa trong vùng làm việc (đảm bảo có khoảng đệm 4 cạnh).
            form.Location = new System.Drawing.Point(
                wa.Left + (wa.Width - form.Width) / 2,
                wa.Top + (wa.Height - form.Height) / 2);
        }
    }
}
