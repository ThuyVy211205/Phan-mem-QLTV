using BUS;
using DTO;
using GUI.Theme;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GUI.UserControls
{
    public partial class ucQuanLyLichSuMuon : UserControl
    {
        private readonly int userId;
        private DataGridView grid;
        private TextBox txtMaDG, txtNgayTu, txtNgayDen;
        private ComboBox cboTrangThai;
        private Button butFilter, butEdit, butDel, butLost;
        private Label lblDG, lblTT, lblTu, lblDen;

        public ucQuanLyLichSuMuon(int userId)
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

            // Filter bar (Top)
            var pnlFilter = new TableLayoutPanel
            {
                Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 8, Padding = new Padding(6),
                BackColor = AppTheme.Surface
            };
            pnlFilter.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlFilter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            pnlFilter.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlFilter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            pnlFilter.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlFilter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            pnlFilter.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlFilter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

            lblDG = new Label { Text = "Mã ĐG:", AutoSize = true, Margin = new Padding(3, 6, 3, 3) };
            txtMaDG = new TextBox { Width = 90, Margin = new Padding(3) };
            lblTT = new Label { Text = "Trạng thái:", AutoSize = true, Margin = new Padding(8, 6, 3, 3) };
            cboTrangThai = new ComboBox { Width = 100, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(3) };
            cboTrangThai.Items.AddRange(new[] { "(Tất cả)", "Đang mượn", "Đã trả", "Quá hạn", "Đã mất" });
            cboTrangThai.SelectedIndex = 0;
            lblTu = new Label { Text = "Từ ngày:", AutoSize = true, Margin = new Padding(8, 6, 3, 3) };
            txtNgayTu = new TextBox { Width = 100, Margin = new Padding(3) };
            lblDen = new Label { Text = "Đến:", AutoSize = true, Margin = new Padding(8, 6, 3, 3) };
            txtNgayDen = new TextBox { Width = 100, Margin = new Padding(3) };

            butFilter = new Button { Text = "\U0001F50D  Lọc", FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(0x0D, 0x6E, 0xFD),
                ForeColor = Color.White, Font = new Font(AppTheme.FontFamily, 10, FontStyle.Bold), AutoSize = true,
                MinimumSize = new Size(80, 30), Cursor = Cursors.Hand, Margin = new Padding(6, 3, 3, 3) };
            butFilter.FlatAppearance.BorderSize = 0;
            RoundedCorner.Apply(butFilter, 6);
            butFilter.Click += butFilter_Click;

            butEdit = new Button { Text = "\u270F\uFE0F  Sửa", FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(0xFF, 0xC1, 0x07),
                ForeColor = Color.Black, Font = new Font(AppTheme.FontFamily, 10, FontStyle.Bold), AutoSize = true,
                MinimumSize = new Size(80, 30), Cursor = Cursors.Hand, Margin = new Padding(3) };
            butEdit.FlatAppearance.BorderSize = 0;
            RoundedCorner.Apply(butEdit, 6);
            butEdit.Click += butEdit_Click;

            butDel = new Button { Text = "\U0001F5D1\uFE0F  Xóa", FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(0xDC, 0x35, 0x45),
                ForeColor = Color.White, Font = new Font(AppTheme.FontFamily, 10, FontStyle.Bold), AutoSize = true,
                MinimumSize = new Size(80, 30), Cursor = Cursors.Hand, Margin = new Padding(3) };
            butDel.FlatAppearance.BorderSize = 0;
            RoundedCorner.Apply(butDel, 6);
            butDel.Click += butDel_Click;

            butLost = new Button { Text = "\u26A0\uFE0F  Mất Sách", FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(0xE6, 0x4A, 0x19),
                ForeColor = Color.White, Font = new Font(AppTheme.FontFamily, 10, FontStyle.Bold), AutoSize = true,
                MinimumSize = new Size(100, 30), Cursor = Cursors.Hand, Margin = new Padding(3) };
            butLost.FlatAppearance.BorderSize = 0;
            RoundedCorner.Apply(butLost, 6);
            butLost.Click += butLost_Click;

            pnlFilter.Controls.Add(lblDG, 0, 0); pnlFilter.Controls.Add(txtMaDG, 1, 0);
            pnlFilter.Controls.Add(lblTT, 2, 0);  pnlFilter.Controls.Add(cboTrangThai, 3, 0);
            pnlFilter.Controls.Add(lblTu, 4, 0);  pnlFilter.Controls.Add(txtNgayTu, 5, 0);
            pnlFilter.Controls.Add(lblDen, 6, 0); pnlFilter.Controls.Add(txtNgayDen, 7, 0);

            var pnlBtn = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(6, 2, 6, 2), FlowDirection = FlowDirection.LeftToRight };
            pnlBtn.Controls.Add(butFilter);
            pnlBtn.Controls.Add(butEdit);
            pnlBtn.Controls.Add(butDel);
            pnlBtn.Controls.Add(butLost);

            // Grid
            var grp = new GroupBox { Text = "Lịch Sử Mượn - Trả", Dock = DockStyle.Fill,
                Font = new Font(AppTheme.FontFamily, 10.5F, FontStyle.Bold), ForeColor = AppTheme.Primary, Padding = new Padding(6) };
            grid = new DataGridView { Dock = DockStyle.Fill, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false };
            grid.Columns.Add("sp", "Số Phiếu");
            grid.Columns.Add("madg", "Mã ĐG");
            grid.Columns.Add("tendg", "Tên Độc Giả");
            grid.Columns.Add("macs", "Mã Cuốn");
            grid.Columns.Add("tensach", "Tên Sách");
            grid.Columns.Add("ngaymuon", "Ngày Mượn");
            grid.Columns.Add("hantra", "Hạn Trả");
            grid.Columns.Add("ngaytra", "Ngày Trả");
            grid.Columns.Add("trangthai", "Trạng Thái");
            grid.Columns.Add("phat", "Tiền Phạt");
            grid.Columns.Add("dongia", "Đơn Giá");
            grp.Controls.Add(grid);

            this.Controls.Add(grp);
            this.Controls.Add(pnlBtn);
            this.Controls.Add(pnlFilter);
        }

        private void LoadData()
        {
            try
            {
                grid.Rows.Clear();
                foreach (var l in BUSQuanLyMuonTra.Instance.GetAllLichSu())
                    AddRow(l);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải dữ liệu:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void AddRow(LichSuMuon l) {
            int idx = grid.Rows.Add(l.SoPhieuMuonTra, l.MaDocGia, l.TenDocGia, l.MaCuonSach,
                l.TenSach, l.NgayMuon.ToShortDateString(), l.HanTra.ToShortDateString(),
                l.NgayTra?.ToShortDateString() ?? "", l.TrangThai, l.SoTienPhat, l.DonGia);

            if (l.DaMat == 1)
            {
                grid.Rows[idx].DefaultCellStyle.BackColor = Color.FromArgb(0xFD, 0xE0, 0xE0);
                grid.Rows[idx].DefaultCellStyle.ForeColor = Color.FromArgb(0xB7, 0x1C, 0x1C);
            }
        }

        private void butFilter_Click(object sender, EventArgs e)
        {
            string madg = txtMaDG.Text.Trim();
            string tt = cboTrangThai.SelectedIndex <= 0 ? null : cboTrangThai.SelectedItem.ToString();
            DateTime? tu = TryParse(txtNgayTu.Text);
            DateTime? den = TryParse(txtNgayDen.Text);
            grid.Rows.Clear();
            foreach (var l in BUSQuanLyMuonTra.Instance.FilterLichSu(madg, tt, tu, den))
                AddRow(l);
        }

        private DateTime? TryParse(string s)
        {
            if (DateTime.TryParseExact(s, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var d))
                return d;
            if (DateTime.TryParse(s, out d)) return d;
            return null;
        }

        private void butEdit_Click(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null) { Warn("Chọn một phiếu để sửa."); return; }
            var row = grid.CurrentRow;
            int sp = Convert.ToInt32(row.Cells["sp"].Value);
            string tt = row.Cells["trangthai"].Value?.ToString() ?? "";
            if (tt == "Đã mất") { Warn("Không thể sửa phiếu đã đánh dấu mất sách."); return; }
            using (var f = new Form { Text = "Sửa Phiếu Mượn", Size = new Size(380, 310), StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false, BackColor = AppTheme.Background })
            {
                var dtpMuon = new DateTimePicker { Value = DateTime.Parse(row.Cells["ngaymuon"].Value.ToString()), Format = DateTimePickerFormat.Short, Location = new Point(120, 20), Width = 200 };
                var dtpHan  = new DateTimePicker { Value = DateTime.Parse(row.Cells["hantra"].Value.ToString()), Format = DateTimePickerFormat.Short, Location = new Point(120, 60), Width = 200 };
                var dtpTra  = new DateTimePicker { Location = new Point(120, 100), Width = 200, Format = DateTimePickerFormat.Short };
                var chkTra  = new CheckBox { Text = "Đã trả", Location = new Point(30, 103), AutoSize = true, Checked = row.Cells["ngaytra"].Value?.ToString() != "" };
                if (chkTra.Checked && DateTime.TryParse(row.Cells["ngaytra"].Value?.ToString(), out var d)) dtpTra.Value = d;
                chkTra.CheckedChanged += (s, ev) => dtpTra.Enabled = chkTra.Checked;
                dtpTra.Enabled = chkTra.Checked;
                var lblPhat = new Label { Text = "Tiền phạt:", Location = new Point(30, 140), AutoSize = true };
                var txtPhat = new TextBox { Text = row.Cells["phat"].Value.ToString(), Location = new Point(120, 138), Width = 200 };

                var butOk = new Button { Text = "Lưu", FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(0x19, 0x87, 0x54),
                    ForeColor = Color.White, Location = new Point(120, 190), Size = new Size(90, 35) };
                butOk.FlatAppearance.BorderSize = 0;
                RoundedCorner.Apply(butOk, 6);
                butOk.Click += (s, ev) =>
                {
                    if (!int.TryParse(txtPhat.Text, out int phat)) { Warn("Tiền phạt không hợp lệ."); return; }
                    string err = BUSQuanLyMuonTra.Instance.EditPhieuMuon(userId, sp,
                        dtpMuon.Value, dtpHan.Value, chkTra.Checked ? dtpTra.Value : (DateTime?)null, phat);
                    if (string.IsNullOrEmpty(err)) { f.DialogResult = DialogResult.OK; f.Close(); }
                    else Warn(err);
                };
                f.Controls.AddRange(new Control[] {
                    new Label { Text = "Ngày mượn:", Location = new Point(30, 23), AutoSize = true }, dtpMuon,
                    new Label { Text = "Hạn trả:", Location = new Point(30, 63), AutoSize = true }, dtpHan,
                    chkTra, dtpTra, lblPhat, txtPhat, butOk
                });
                if (f.ShowDialog() == DialogResult.OK) LoadData();
            }
        }

        private void butDel_Click(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null) { Warn("Chọn một phiếu để xóa."); return; }
            int sp = Convert.ToInt32(grid.CurrentRow.Cells["sp"].Value);
            if (MessageBox.Show($"Xóa phiếu mượn #{sp}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string err = BUSQuanLyMuonTra.Instance.DelPhieuMuon(userId, sp);
            if (string.IsNullOrEmpty(err)) { Info("Đã xóa."); LoadData(); }
            else Warn(err);
        }

        private void butLost_Click(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null) { Warn("Chọn một phiếu để đánh dấu mất sách."); return; }
            var row = grid.CurrentRow;
            int sp = Convert.ToInt32(row.Cells["sp"].Value);
            string macs = row.Cells["macs"].Value?.ToString() ?? "";
            string tt = row.Cells["trangthai"].Value?.ToString() ?? "";
            int donGia = row.Cells["dongia"].Value != null ? Convert.ToInt32(row.Cells["dongia"].Value) : 0;

            if (tt == "Đã mất") { Warn("Phiếu này đã được đánh dấu mất sách."); return; }
            if (tt == "Đã trả") { Warn("Sách đã được trả, không thể đánh dấu mất."); return; }

            var ts = BUSThamSo.Instance.GetAllThamSo();
            int heSo = ts.HeSoPhatMatSach;
            if (heSo <= 0) heSo = 3;
            int tienUocTinh = donGia * heSo;

            string msg = string.Format("Xác nhận đánh dấu MẤT SÁCH?\n\nCuốn sách: {0}\nĐơn giá: {1:N0} VNĐ\nHệ số phạt: x{2}\nTiền phạt dự kiến: {3:N0} VNĐ\n\nHành động này không thể hoàn tác.",
                macs, donGia, heSo, tienUocTinh);

            if (MessageBox.Show(msg, "Xác nhận mất sách", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            string err = BUSQuanLyMuonTra.Instance.MarkBookAsLost(sp, macs, userId);
            if (string.IsNullOrEmpty(err)) { Info(string.Format("Đã đánh dấu mất sách.\nTiền phạt: {0:N0} VNĐ", tienUocTinh)); LoadData(); }
            else Warn(err);
        }

        private void Warn(string m) => MessageBox.Show(m, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void Info(string m) => MessageBox.Show(m, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
