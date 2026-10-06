using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace GUI.Theme
{
    /// <summary>
    /// Áp theme (tông xanh hiện đại) lên toàn bộ cây control của một Form/UserControl.
    /// Dùng reflection để set các thuộc tính màu của control Siticone mà không phụ thuộc
    /// vào kiểu cụ thể, nhờ vậy phủ được mọi màn hình mà không cần sửa từng Designer.
    /// </summary>
    public static class ThemeManager
    {
        /// <summary>
        /// Điểm vào chính: gọi trong Load của mỗi Form/UserControl.
        /// </summary>
        public static void Apply(Control root)
        {
            if (root == null) return;
            if (root is Form form)
            {
                form.BackColor = AppTheme.Background;
                form.Font = AppTheme.Base;
            }
            StyleTree(root);
        }

        private static void StyleTree(Control ctrl)
        {
            foreach (Control c in ctrl.Controls)
            {
                StyleControl(c);
                if (c.HasChildren) StyleTree(c);
            }
        }

        private static void StyleControl(Control c)
        {
            string typeName = c.GetType().Name;

            if (c is DataGridView grid) { StyleGrid(grid); return; }
            if (c is Label lbl) { StyleLabel(lbl); return; }
            if (c is Button) { StyleButton(c); return; }

            switch (typeName)
            {
                case "SiticoneButton":
                    StyleButton(c);
                    break;
                case "SiticoneTextBox":
                    StyleTextBox(c);
                    break;
                case "SiticoneComboBox":
                    StyleComboBox(c);
                    break;
                case "SiticonePanel":
                case "SiticoneGradientPanel":
                    StylePanel(c);
                    break;
                case "SiticoneDataGridView":
                    if (c is DataGridView dgv) StyleGrid(dgv);
                    break;
                case "SiticoneCheckBox":
                case "SiticoneToggleSwitch":
                case "SiticoneImageCheckBox":
                    SetProp(c, "CheckedState.FillColor", AppTheme.Primary);
                    break;
                case "SiticoneSeparator":
                    SetProp(c, "FillColor", AppTheme.Primary);
                    SetProp(c, "FillThickness", 2);
                    break;
                case "SiticoneNumericUpDown":
                    SetProp(c, "UpDownButtonFillColor", AppTheme.Primary);
                    break;
                case "SiticoneTabControl":
                    StyleTabControl(c);
                    break;
                case "SiticoneHtmlLabel":
                case "SiticoneVScrollBar":
                case "SiticoneControlBox":
                    // giữ nguyên
                    break;
            }
        }

        private static void StyleButton(Control c)
        {
            object mode = GetProp(c, "ButtonMode");
            bool isRadio = mode != null && mode.ToString().Contains("Radio");
            if (isRadio) return;

            string text = (c.Text ?? "").ToLower();
            Color backColor, foreColor;

            if (text.Contains("thêm") || text.Contains("tạo") || text.Contains("mượn")
                || text.Contains("đăng") || text.Contains("đổi") || text.Contains("tìm")
                || text.Contains("lọc") || text.Contains("tải") || text.Contains("home") || text.Contains("load"))
            {
                backColor = Color.FromArgb(0x0D, 0x6E, 0xFD);
                foreColor = Color.White;
            }
            else if (text.Contains("lưu") || text.Contains("thu"))
            {
                backColor = Color.FromArgb(0x19, 0x87, 0x54);
                foreColor = Color.White;
            }
            else if (text.Contains("sửa") || text.Contains("xem") || text.Contains("trả"))
            {
                backColor = Color.FromArgb(0xFF, 0xC1, 0x07);
                foreColor = Color.Black;
            }
            else if (text.Contains("xóa") || text.Contains("hủy"))
            {
                backColor = Color.FromArgb(0xDC, 0x35, 0x45);
                foreColor = Color.White;
            }
            else
            {
                backColor = Color.FromArgb(0x6C, 0x75, 0x7D);
                foreColor = Color.White;
            }

            Font buttonFont = new Font("Segoe UI", 9F, FontStyle.Bold);

            if (c is Button btn)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Font = buttonFont;
                btn.BackColor = backColor;
                btn.ForeColor = foreColor;

                btn.FlatAppearance.MouseOverBackColor = SubtractSafe(backColor, 30);
                btn.FlatAppearance.MouseDownBackColor = SubtractSafe(backColor, 50);

                btn.Tag = (backColor, foreColor);
                btn.EnabledChanged -= Button_EnabledChanged;
                btn.EnabledChanged += Button_EnabledChanged;

                Size sz = TextRenderer.MeasureText(btn.Text, buttonFont);
                btn.Width = Math.Max(btn.Width, sz.Width + btn.Padding.Horizontal + 20);
            }
            else
            {
                SetProp(c, "FillColor", backColor);
                SetProp(c, "ForeColor", foreColor);
                SetProp(c, "BorderRadius", 8);
                SetProp(c, "HoverState.FillColor", HoverHighlight(backColor));
                SetProp(c, "PressedColor", PressedShade(backColor));
                SetProp(c, "AutoSize", true);
                c.Font = buttonFont;
            }
        }

        private static void StyleTextBox(Control c)
        {
            SetProp(c, "BorderColor", AppTheme.Border);
            SetProp(c, "BorderRadius", 8);
            SetProp(c, "FillColor", AppTheme.Surface);
            SetProp(c, "ForeColor", AppTheme.TextPrimary);
            SetProp(c, "FocusedState.BorderColor", AppTheme.Primary);
            SetProp(c, "HoverState.BorderColor", AppTheme.PrimaryLight);
            SetProp(c, "PlaceholderForeColor", AppTheme.TextMuted);
        }

        private static void StyleComboBox(Control c)
        {
            SetProp(c, "BorderColor", AppTheme.Border);
            SetProp(c, "BorderRadius", 8);
            SetProp(c, "FillColor", AppTheme.Surface);
            SetProp(c, "ForeColor", AppTheme.TextPrimary);
            SetProp(c, "FocusedState.BorderColor", AppTheme.Primary);
            SetProp(c, "ItemHeight", 30);
        }

        private static void StylePanel(Control c)
        {
            object fill = GetProp(c, "FillColor");
            // Panel tiêu đề (đang tô đậm) -> chuyển sang xanh thương hiệu.
            if (fill is Color fc && IsDark(fc))
            {
                SetProp(c, "FillColor", AppTheme.Primary);
                c.ForeColor = AppTheme.TextOnPrimary;
            }
        }

        private static void StyleTabControl(Control c)
        {
            // Menu ngang trên cùng: nền menu xanh đậm, nút chọn nền trắng chữ xanh.
            SetProp(c, "TabMenuBackColor", AppTheme.SidebarTop);
            Font tabFont = new Font(AppTheme.FontFamily, 10.5F, FontStyle.Bold);

            SetProp(c, "TabButtonIdleState.InnerColor", AppTheme.PrimaryDark);
            SetProp(c, "TabButtonIdleState.FillColor", AppTheme.PrimaryDark);
            SetProp(c, "TabButtonIdleState.ForeColor", AppTheme.TextOnPrimary);
            SetProp(c, "TabButtonIdleState.BorderColor", AppTheme.PrimaryDark);
            SetProp(c, "TabButtonIdleState.Font", tabFont);

            SetProp(c, "TabButtonHoverState.InnerColor", AppTheme.PrimaryLight);
            SetProp(c, "TabButtonHoverState.FillColor", AppTheme.PrimaryLight);
            SetProp(c, "TabButtonHoverState.ForeColor", AppTheme.TextOnPrimary);
            SetProp(c, "TabButtonHoverState.BorderColor", AppTheme.PrimaryLight);
            SetProp(c, "TabButtonHoverState.Font", tabFont);

            SetProp(c, "TabButtonSelectedState.InnerColor", AppTheme.Surface);
            SetProp(c, "TabButtonSelectedState.FillColor", AppTheme.Surface);
            SetProp(c, "TabButtonSelectedState.ForeColor", AppTheme.Primary);
            SetProp(c, "TabButtonSelectedState.BorderColor", AppTheme.Surface);
            SetProp(c, "TabButtonSelectedState.Font", tabFont);

            if (c is TabControl tc)
            {
                foreach (TabPage page in tc.TabPages)
                    page.BackColor = AppTheme.Background;
            }
        }

        private static void StyleLabel(Label lbl)
        {
            // Nhãn có nền thương hiệu (SlateBlue...) -> đồng bộ nền xanh + chữ trắng.
            if (lbl.BackColor != Color.Transparent && IsDark(lbl.BackColor))
            {
                lbl.BackColor = AppTheme.Primary;
                lbl.ForeColor = AppTheme.TextOnPrimary;
                return;
            }
            // Tiêu đề lớn trên nền đậm -> chữ trắng; còn lại -> chữ tối.
            if (lbl.Parent != null && IsDark(lbl.Parent.BackColor))
                lbl.ForeColor = AppTheme.TextOnPrimary;
            else if (lbl.ForeColor == Color.SlateBlue || lbl.ForeColor == Color.DarkSlateBlue)
                lbl.ForeColor = AppTheme.Primary;
        }

        private static void StyleGrid(DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.BackgroundColor = AppTheme.Surface;
            grid.BorderStyle = BorderStyle.None;
            grid.GridColor = AppTheme.GridLine;
            grid.Font = AppTheme.Base;
            grid.RowHeadersVisible = false;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            grid.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.GridHeader;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.GridHeaderText;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font(AppTheme.FontFamily, 10F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = AppTheme.GridHeader;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            grid.ColumnHeadersHeight = 40;

            grid.DefaultCellStyle.BackColor = AppTheme.Surface;
            grid.DefaultCellStyle.ForeColor = AppTheme.TextPrimary;
            grid.DefaultCellStyle.SelectionBackColor = AppTheme.Selection;
            grid.DefaultCellStyle.SelectionForeColor = AppTheme.TextPrimary;
            grid.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
            grid.AlternatingRowsDefaultCellStyle.BackColor = AppTheme.GridRowAlt;
            grid.RowTemplate.Height = 34;
        }

        private static bool IsDark(Color c)
        {
            // Độ sáng cảm nhận (perceived luminance)
            double lum = (0.299 * c.R + 0.587 * c.G + 0.114 * c.B);
            return lum < 140 && c.A > 0;
        }

        private static Color SubtractSafe(Color c, int amount)
        {
            return Color.FromArgb(
                Math.Max(0, c.R - amount),
                Math.Max(0, c.G - amount),
                Math.Max(0, c.B - amount));
        }

        private static void Button_EnabledChanged(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || !(btn.Tag is ValueTuple<Color, Color> colors)) return;
            var (backColor, foreColor) = colors;
            if (btn.Enabled)
            {
                btn.BackColor = backColor;
                btn.ForeColor = foreColor;
            }
            else
            {
                btn.BackColor = Color.FromArgb(0xE9, 0xEC, 0xEF);
                btn.ForeColor = Color.FromArgb(0x6C, 0x75, 0x7D);
            }
        }

        private static Color HoverHighlight(Color c)
        {
            return Color.FromArgb(
                Math.Min(255, c.R + (int)((255 - c.R) * 0.2)),
                Math.Min(255, c.G + (int)((255 - c.G) * 0.2)),
                Math.Min(255, c.B + (int)((255 - c.B) * 0.2)));
        }

        private static Color PressedShade(Color c)
        {
            return Color.FromArgb(
                Math.Max(0, c.R - 20),
                Math.Max(0, c.G - 20),
                Math.Max(0, c.B - 20));
        }

        #region Reflection helpers

        private static object GetProp(object obj, string path)
        {
            try
            {
                foreach (string part in path.Split('.'))
                {
                    if (obj == null) return null;
                    PropertyInfo p = obj.GetType().GetProperty(part,
                        BindingFlags.Public | BindingFlags.Instance);
                    if (p == null) return null;
                    obj = p.GetValue(obj, null);
                }
                return obj;
            }
            catch { return null; }
        }

        private static void SetProp(object obj, string path, object value)
        {
            try
            {
                string[] parts = path.Split('.');
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    PropertyInfo p = obj.GetType().GetProperty(parts[i],
                        BindingFlags.Public | BindingFlags.Instance);
                    if (p == null) return;
                    obj = p.GetValue(obj, null);
                    if (obj == null) return;
                }
                PropertyInfo target = obj.GetType().GetProperty(parts[parts.Length - 1],
                    BindingFlags.Public | BindingFlags.Instance);
                if (target == null || !target.CanWrite) return;

                if (target.PropertyType == typeof(int) && value is int)
                    target.SetValue(obj, value, null);
                else if (target.PropertyType == typeof(Color) && value is Color)
                    target.SetValue(obj, value, null);
                else if (target.PropertyType.IsInstanceOfType(value))
                    target.SetValue(obj, value, null);
            }
            catch { /* bỏ qua control không hỗ trợ thuộc tính này */ }
        }

        #endregion
    }
}
