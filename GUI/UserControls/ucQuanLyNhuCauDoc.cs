using BUS;
using DTO;
using GUI.Theme;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GUI.UserControls
{
    public partial class ucQuanLyNhuCauDoc : UserControl
    {
        private readonly int userId;
        private DataGridView grid;
        private Button butFulfill, butEdit, butDel;
        private TextBox txtMaSach, txtGhiChu;
        private Label lblSach, lblGhiChu;

        public ucQuanLyNhuCauDoc(int userId)
        {
            this.userId = userId;
            this.Dock = DockStyle.Fill;
            this.BackColor = AppTheme.Background;
            this.Font = AppTheme.Base;
            this.AutoScaleMode = AutoScaleMode.Inherit;
            this.Load += (s, e) => { BuildUI(); LoadData(); ThemeManager.Apply(this); };
        }

        private void BuildUI()
        {
            if (Controls.Count > 0) return;

            // Edit bar (Top)
            var pnlEdit = new TableLayoutPanel
            {
                Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 4, Padding = new Padding(6),
                BackColor = AppTheme.Surface
            };
            pnlEdit.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlEdit.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
            pnlEdit.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlEdit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            lblSach = new Label { Text = "Mã Sách:", AutoSize = true, Margin = new Padding(3, 8, 3, 3) };
            txtMaSach = new TextBox { Width = 170, Margin = new Padding(3) };
            lblGhiChu = new Label { Text = "Ghi Chú:", AutoSize = true, Margin = new Padding(8, 8, 3, 3) };
            txtGhiChu = new TextBox { Width = 250, Margin = new Padding(3) };
            pnlEdit.Controls.Add(lblSach, 0, 0); pnlEdit.Controls.Add(txtMaSach, 1, 0);
            pnlEdit.Controls.Add(lblGhiChu, 2, 0); pnlEdit.Controls.Add(txtGhiChu, 3, 0);

            // Buttons
            var pnlBtn = new FlowLayoutPanel
            {
                Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(6, 2, 6, 2), FlowDirection = FlowDirection.LeftToRight
            };

            butFulfill = new Button { Text = "\u2705  Đánh dấu đã đáp ứng", FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0x19, 0x87, 0x54), ForeColor = Color.White,
                Font = new Font(AppTheme.FontFamily, 10, FontStyle.Bold), AutoSize = true,
                MinimumSize = new Size(200, 32), Cursor = Cursors.Hand, Margin = new Padding(3) };
            butFulfill.FlatAppearance.BorderSize = 0;
            RoundedCorner.Apply(butFulfill, 6);
            butFulfill.Click += butFulfill_Click;

            butEdit = new Button { Text = "\u270F\uFE0F  Sửa", FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0xFF, 0xC1, 0x07), ForeColor = Color.Black,
                Font = new Font(AppTheme.FontFamily, 10, FontStyle.Bold), AutoSize = true,
                MinimumSize = new Size(90, 32), Cursor = Cursors.Hand, Margin = new Padding(3) };
            butEdit.FlatAppearance.BorderSize = 0;
            RoundedCorner.Apply(butEdit, 6);
            butEdit.Click += butEdit_Click;

            butDel = new Button { Text = "\U0001F5D1\uFE0F  Xóa", FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0xDC, 0x35, 0x45), ForeColor = Color.White,
                Font = new Font(AppTheme.FontFamily, 10, FontStyle.Bold), AutoSize = true,
                MinimumSize = new Size(90, 32), Cursor = Cursors.Hand, Margin = new Padding(3) };
            butDel.FlatAppearance.BorderSize = 0;
            RoundedCorner.Apply(butDel, 6);
            butDel.Click += butDel_Click;

            pnlBtn.Controls.Add(butFulfill);
            pnlBtn.Controls.Add(butEdit);
            pnlBtn.Controls.Add(butDel);

            // Grid
            var grp = new GroupBox { Text = "Quản Lý Nhu Cầu Đọc", Dock = DockStyle.Fill,
                Font = new Font(AppTheme.FontFamily, 10.5F, FontStyle.Bold), ForeColor = AppTheme.Primary, Padding = new Padding(6) };
            grid = new DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false };
            grid.Columns.Add("id", "ID"); grid.Columns["id"].Visible = false;
            grid.Columns.Add("manc", "Mã NC");
            grid.Columns.Add("madg", "Mã ĐG");
            grid.Columns.Add("tendg", "Tên Độc Giả");
            grid.Columns.Add("masach", "Mã Sách");
            grid.Columns.Add("tensach", "Tên Sách");
            grid.Columns.Add("tacgia", "Tác Giả");
            grid.Columns.Add("trangthai", "Trạng Thái");
            grid.Columns.Add("ghichu", "Ghi Chú");
            grid.Columns.Add("ngaythem", "Ngày Thêm");
            grid.SelectionChanged += (s, e) =>
            {
                if (grid.CurrentRow == null) return;
                txtMaSach.Text = grid.CurrentRow.Cells["masach"].Value?.ToString();
                txtGhiChu.Text = grid.CurrentRow.Cells["ghichu"].Value?.ToString();
            };
            grp.Controls.Add(grid);

            this.Controls.Add(grp);
            this.Controls.Add(pnlBtn);
            this.Controls.Add(pnlEdit);
        }

        private void LoadData()
        {
            try
            {
                grid.Rows.Clear();
                foreach (var n in BUSQuanLyMuonTra.Instance.GetAllNhuCau())
                    grid.Rows.Add(n.id, n.MaNhuCau, n.MaDocGia, n.TenDocGia, n.MaSach, n.TenSach,
                        n.TenTacGia, n.TrangThai, n.GhiChu ?? "", n.NgayThem.ToShortDateString());
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải dữ liệu:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void butFulfill_Click(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null) { Warn("Chọn một yêu cầu."); return; }
            int id = Convert.ToInt32(grid.CurrentRow.Cells["id"].Value);
            string err = BUSQuanLyMuonTra.Instance.MarkFulfilled(userId, id);
            if (string.IsNullOrEmpty(err)) { Info("Đã đánh dấu 'Đã đáp ứng'."); LoadData(); }
            else Warn(err);
        }

        private void butEdit_Click(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null) { Warn("Chọn một yêu cầu để sửa."); return; }
            int id = Convert.ToInt32(grid.CurrentRow.Cells["id"].Value);
            string mas = txtMaSach.Text.Trim();
            string err = BUSQuanLyMuonTra.Instance.EditNhuCau(userId, id, mas, txtGhiChu.Text.Trim());
            if (string.IsNullOrEmpty(err)) { Info("Đã cập nhật."); LoadData(); }
            else Warn(err);
        }

        private void butDel_Click(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null) { Warn("Chọn một yêu cầu để xóa."); return; }
            int id = Convert.ToInt32(grid.CurrentRow.Cells["id"].Value);
            if (MessageBox.Show("Xóa yêu cầu này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string err = BUSQuanLyMuonTra.Instance.DelNhuCau(userId, id);
            if (string.IsNullOrEmpty(err)) { Info("Đã xóa."); LoadData(); }
            else Warn(err);
        }

        private void Warn(string m) => MessageBox.Show(m, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void Info(string m) => MessageBox.Show(m, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
