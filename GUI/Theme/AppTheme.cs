using System.Drawing;

namespace GUI.Theme
{
    /// <summary>
    /// Bảng màu tập trung cho toàn ứng dụng (tông xanh dương hiện đại).
    /// Thay đổi màu ở đây sẽ áp dụng cho mọi màn hình qua ThemeManager.
    /// </summary>
    public static class AppTheme
    {
        // Màu thương hiệu
        public static readonly Color Primary = Color.FromArgb(37, 99, 235);      // blue-600
        public static readonly Color PrimaryDark = Color.FromArgb(30, 58, 138);  // blue-900
        public static readonly Color PrimaryLight = Color.FromArgb(59, 130, 246);// blue-500
        public static readonly Color Accent = Color.FromArgb(14, 165, 233);      // sky-500

        // Nền & bề mặt
        public static readonly Color Background = Color.FromArgb(241, 245, 249); // slate-100
        public static readonly Color Surface = Color.White;
        public static readonly Color SidebarTop = Color.FromArgb(30, 58, 138);   // blue-900
        public static readonly Color SidebarBottom = Color.FromArgb(37, 99, 235);// blue-600

        // Chữ
        public static readonly Color TextPrimary = Color.FromArgb(15, 23, 42);   // slate-900
        public static readonly Color TextMuted = Color.FromArgb(100, 116, 139);  // slate-500
        public static readonly Color TextOnPrimary = Color.White;

        // Đường viền & trạng thái
        public static readonly Color Border = Color.FromArgb(203, 213, 225);     // slate-300
        public static readonly Color Hover = Color.FromArgb(219, 234, 254);      // blue-100
        public static readonly Color Selection = Color.FromArgb(191, 219, 254);  // blue-200

        // Lưới dữ liệu
        public static readonly Color GridHeader = Color.FromArgb(30, 58, 138);
        public static readonly Color GridHeaderText = Color.White;
        public static readonly Color GridRowAlt = Color.FromArgb(239, 246, 255); // blue-50
        public static readonly Color GridLine = Color.FromArgb(226, 232, 240);

        // Font
        public const string FontFamily = "Segoe UI";
        public static Font Base => new Font(FontFamily, 10F, FontStyle.Regular);
        public static Font Heading => new Font(FontFamily, 20F, FontStyle.Bold);
        public static Font Button => new Font(FontFamily, 10.5F, FontStyle.Bold);
    }
}
