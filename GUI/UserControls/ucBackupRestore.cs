using BUS;
using GUI.Theme;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GUI.UserControls
{
    public class ucBackupRestore : UserControl
    {
        private Button butBackup, butRestore;
        private Label lblStatus;
        private TextBox txtPath;

        public ucBackupRestore()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = AppTheme.Background;
            this.Font = new Font(AppTheme.FontFamily, 12F);
            this.AutoScaleMode = AutoScaleMode.Inherit;
            this.Load += (s, e) => { if (Controls.Count == 0) { BuildUI(); ThemeManager.Apply(this); } };
        }

        private void BuildUI()
        {
            var fontNormal = new Font(AppTheme.FontFamily, 13F);
            var fontBold = new Font(AppTheme.FontFamily, 13F, FontStyle.Bold);
            var fontTitle = new Font(AppTheme.FontFamily, 18F, FontStyle.Bold);
            var fontSmall = new Font(AppTheme.FontFamily, 11F, FontStyle.Italic);

            // ---- Title ----
            var pnlTitle = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(16, 10, 0, 0) };
            var lblTitle = new Label
            {
                Text = "Sao lưu & Khôi phục Cơ sở dữ liệu",
                Font = fontTitle,
                ForeColor = AppTheme.Primary,
                AutoSize = true
            };
            pnlTitle.Controls.Add(lblTitle);

            // ---- Content ----
            var pnlContent = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20, 10, 20, 10) };

            // --- Backup section ---
            var grpBackup = new GroupBox
            {
                Text = "  Sao lưu (Backup)  ",
                Font = new Font(AppTheme.FontFamily, 14F, FontStyle.Bold),
                ForeColor = AppTheme.Primary,
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new Size(0, 200),
                Padding = new Padding(16, 30, 16, 14)
            };

            var tblBackup = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(0, 6, 0, 6)
            };
            tblBackup.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            tblBackup.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            tblBackup.Controls.Add(new Label { Text = "Thư mục lưu:", Font = fontBold, AutoSize = true, Margin = new Padding(0, 8, 0, 0) }, 0, 0);

            var pnlPath = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, WrapContents = false };
            txtPath = new TextBox { Width = 400, Font = fontNormal, ReadOnly = true, BackColor = Color.White, Margin = new Padding(0, 6, 8, 0) };
            var butBrowse = MakeButton("\U0001F4C2  Chọn thư mục", Color.FromArgb(0x6C, 0x75, 0x7D), 160);
            butBrowse.Click += (s2, e2) =>
            {
                using (var fbd = new FolderBrowserDialog { Description = "Chọn thư mục lưu file sao lưu", ShowNewFolderButton = true })
                    if (fbd.ShowDialog() == DialogResult.OK) txtPath.Text = fbd.SelectedPath;
            };
            pnlPath.Controls.Add(txtPath);
            pnlPath.Controls.Add(butBrowse);
            tblBackup.Controls.Add(pnlPath, 1, 0);

            tblBackup.Controls.Add(new Label { Text = "", AutoSize = true }, 0, 1);

            var pnlBackupBtn = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = new Padding(0, 4, 0, 0) };
            butBackup = MakeButton("\U0001F4BE  Sao lưu ngay", Color.FromArgb(0x0D, 0x6E, 0xFD), 200);
            butBackup.Click += butBackup_Click;
            pnlBackupBtn.Controls.Add(butBackup);
            var lblHint = new Label { Text = "  File .bak sẽ được tạo với tên database + thời gian", Font = fontSmall, ForeColor = AppTheme.TextMuted, AutoSize = true, Margin = new Padding(12, 6, 0, 0) };
            pnlBackupBtn.Controls.Add(lblHint);
            tblBackup.Controls.Add(pnlBackupBtn, 1, 2);

            grpBackup.Controls.Add(tblBackup);

            // --- Restore section ---
            var grpRestore = new GroupBox
            {
                Text = "  Khôi phục (Restore)  ",
                Font = new Font(AppTheme.FontFamily, 14F, FontStyle.Bold),
                ForeColor = AppTheme.Primary,
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new Size(0, 150),
                Padding = new Padding(16, 30, 16, 14)
            };

            var tblRestore = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(0, 6, 0, 6)
            };
            tblRestore.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var lblWarn = new Label
            {
                Text = "\u26A0\uFE0F  Cảnh báo: Dữ liệu hiện tại sẽ bị ghi đè. Hãy sao lưu trước khi khôi phục.",
                Font = new Font(AppTheme.FontFamily, 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0xDC, 0x35, 0x45),
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 8)
            };
            tblRestore.Controls.Add(lblWarn, 0, 0);

            butRestore = MakeButton("\U0001F504  Khôi phục từ file .bak", Color.FromArgb(0xE6, 0x4A, 0x19), 280);
            butRestore.Click += butRestore_Click;
            tblRestore.Controls.Add(butRestore, 0, 1);

            grpRestore.Controls.Add(tblRestore);

            // --- Status ---
            lblStatus = new Label
            {
                Text = "",
                Font = new Font(AppTheme.FontFamily, 12F),
                ForeColor = AppTheme.TextPrimary,
                Dock = DockStyle.Top,
                AutoSize = true,
                Padding = new Padding(0, 14, 0, 0)
            };

            pnlContent.Controls.Add(lblStatus);
            pnlContent.Controls.Add(grpRestore);
            pnlContent.Controls.Add(grpBackup);

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlTitle);
        }

        private Button MakeButton(string text, Color back, int width)
        {
            var b = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = back,
                ForeColor = Color.White,
                Font = new Font(AppTheme.FontFamily, 12F, FontStyle.Bold),
                Size = new Size(width, 40),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 4, 8, 4)
            };
            b.FlatAppearance.BorderSize = 0;
            RoundedCorner.Apply(b, 6);
            return b;
        }

        private void butBackup_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPath.Text))
            {
                lblStatus.Text = "Vui lòng chọn thư mục lưu file.";
                lblStatus.ForeColor = AppTheme.TextMuted;
                return;
            }

            SetBusy(true);
            string result = BUSBackupRestore.Instance.BackupDatabase(txtPath.Text);
            SetBusy(false);

            lblStatus.Text = result;
            lblStatus.ForeColor = result.StartsWith("Sao lưu thành công")
                ? Color.FromArgb(0x19, 0x87, 0x54)
                : Color.FromArgb(0xDC, 0x35, 0x45);
        }

        private void butRestore_Click(object sender, EventArgs e)
        {
            string filePath;
            using (var ofd = new OpenFileDialog
            {
                Title = "Chọn file sao lưu (.bak)",
                Filter = "Backup Files (*.bak)|*.bak|All Files (*.*)|*.*",
                FilterIndex = 1
            })
            {
                if (ofd.ShowDialog() != DialogResult.OK) return;
                filePath = ofd.FileName;
            }

            var result = MessageBox.Show(
                "XÁC NHẬN KHÔI PHỤC DATABASE\n\n" +
                "Toàn bộ dữ liệu hiện tại sẽ bị xoá và thay thế\n" +
                "bằng dữ liệu từ file sao lưu.\n\n" +
                "Ứng dụng sẽ thoát sau khi khôi phục.\n\n" +
                "Bạn có chắc chắn muốn tiếp tục?",
                "Xác nhận khôi phục",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            SetBusy(true);
            string restoreResult = BUSBackupRestore.Instance.RestoreDatabase(filePath);
            SetBusy(false);

            if (restoreResult.StartsWith("Khôi phục thành công"))
            {
                MessageBox.Show(
                    "Khôi phục thành công!\n\nỨng dụng sẽ tự động thoát.\nVui lòng khởi động lại chương trình.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                Application.Exit();
            }
            else
            {
                lblStatus.Text = restoreResult;
                lblStatus.ForeColor = Color.FromArgb(0xDC, 0x35, 0x45);
            }
        }

        private void SetBusy(bool busy)
        {
            butBackup.Enabled = !busy;
            butRestore.Enabled = !busy;
            if (busy)
            {
                lblStatus.Text = "Đang xử lý, vui lòng đợi...";
                lblStatus.ForeColor = AppTheme.TextMuted;
            }
            Application.DoEvents();
        }
    }
}
