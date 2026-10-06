using BUS;
using DAL;
using DTO;
using GUI.Theme;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GUI.UserControls
{
    public class ucPhieuNhapSachNew : UserControl
    {
        private DataGridView gridPhieu, gridCT;
        private Button butThemPhieu, butXoaPhieu, butThemCT, butXoaCT;
        private DateTimePicker dtpNgayNhap;
        private TextBox txtMaSach, txtSoLuongNhap;
        private Label lblTongTien, lblSoPhieu;

        private List<PHIEUNHAPSACH> dsPhieu;
        private int selectedSoPhieu = -1;

        public ucPhieuNhapSachNew()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = AppTheme.Background;
            this.Font = new Font(AppTheme.FontFamily, 12F);
            this.AutoScaleMode = AutoScaleMode.Inherit;
            this.Load += (s, e) => { if (Controls.Count == 0) { BuildUI(); LoadData(); ThemeManager.Apply(this); } };
        }

        private void BuildUI()
        {
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill, Orientation = Orientation.Horizontal,
                SplitterDistance = 260, BackColor = AppTheme.Background,
                Panel1MinSize = 200, Panel2MinSize = 150
            };

            // ===== Panel 1: Danh sách Phiếu Nhập =====
            var grpPhieu = new GroupBox
            {
                Text = "  Danh sách Phiếu Nhập  ",
                Font = new Font(AppTheme.FontFamily, 13F, FontStyle.Bold),
                ForeColor = AppTheme.Primary, Dock = DockStyle.Fill,
                Padding = new Padding(12, 28, 12, 12)
            };

            var pnlPhieuTop = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(0, 4, 0, 8) };

            var lblNgay = new Label { Text = "Ngày Nhập:", AutoSize = true, Margin = new Padding(0, 6, 8, 0), Font = new Font(AppTheme.FontFamily, 12F, FontStyle.Bold) };
            dtpNgayNhap = new DateTimePicker { Format = DateTimePickerFormat.Short, Font = new Font(AppTheme.FontFamily, 12F), Width = 140, Margin = new Padding(0, 4, 12, 0) };
            dtpNgayNhap.Value = DateTime.Now;

            butThemPhieu = MkBtn("\u2795  Thêm Phiếu Nhập", Color.FromArgb(0x0D, 0x6E, 0xFD), 180);
            butThemPhieu.Click += butThemPhieu_Click;

            butXoaPhieu = MkBtn("\U0001F5D1  Xóa", Color.FromArgb(0xDC, 0x35, 0x45), 110);
            butXoaPhieu.Click += butXoaPhieu_Click;

            lblSoPhieu = new Label { Text = "", AutoSize = true, Margin = new Padding(20, 6, 0, 0), Font = new Font(AppTheme.FontFamily, 11F, FontStyle.Italic), ForeColor = AppTheme.TextMuted };

            pnlPhieuTop.Controls.AddRange(new Control[] { lblNgay, dtpNgayNhap, butThemPhieu, butXoaPhieu, lblSoPhieu });

            gridPhieu = MkGrid();
            gridPhieu.SelectionChanged += GridPhieu_SelectionChanged;
            gridPhieu.Columns.Add("sp", "Số Phiếu");
            gridPhieu.Columns.Add("ngay", "Ngày Nhập");
            gridPhieu.Columns.Add("tong", "Tổng Tiền");
            gridPhieu.Columns.Add("soCT", "Số Đầu Sách");

            grpPhieu.Controls.Add(gridPhieu);
            grpPhieu.Controls.Add(pnlPhieuTop);
            split.Panel1.Controls.Add(grpPhieu);

            // ===== Panel 2: Chi Tiết Phiếu Nhập =====
            var grpCT = new GroupBox
            {
                Text = "  Chi Tiết Phiếu Nhập  ",
                Font = new Font(AppTheme.FontFamily, 13F, FontStyle.Bold),
                ForeColor = AppTheme.Primary, Dock = DockStyle.Fill,
                Padding = new Padding(12, 28, 12, 12)
            };

            var pnlCTTop = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(0, 4, 0, 8) };

            var lblMaSach = new Label { Text = "Mã Sách:", AutoSize = true, Margin = new Padding(0, 6, 4, 0), Font = new Font(AppTheme.FontFamily, 12F, FontStyle.Bold) };
            txtMaSach = new TextBox { Width = 120, Font = new Font(AppTheme.FontFamily, 12F), Margin = new Padding(0, 4, 12, 0) };
            var lblSL = new Label { Text = "SL Nhập:", AutoSize = true, Margin = new Padding(0, 6, 4, 0), Font = new Font(AppTheme.FontFamily, 12F, FontStyle.Bold) };
            txtSoLuongNhap = new TextBox { Width = 70, Font = new Font(AppTheme.FontFamily, 12F), Margin = new Padding(0, 4, 12, 0) };

            butThemCT = MkBtn("\u2795  Thêm vào phiếu", Color.FromArgb(0x19, 0x87, 0x54), 170);
            butThemCT.Click += butThemCT_Click;

            butXoaCT = MkBtn("\U0001F5D1  Xóa dòng", Color.FromArgb(0xDC, 0x35, 0x45), 130);
            butXoaCT.Click += butXoaCT_Click;

            lblTongTien = new Label { Text = "", AutoSize = true, Margin = new Padding(20, 6, 0, 0), Font = new Font(AppTheme.FontFamily, 12F, FontStyle.Bold), ForeColor = AppTheme.Primary };

            pnlCTTop.Controls.AddRange(new Control[] { lblMaSach, txtMaSach, lblSL, txtSoLuongNhap, butThemCT, butXoaCT, lblTongTien });

            gridCT = MkGrid();
            gridCT.Columns.Add("masach", "Mã Sách");
            gridCT.Columns.Add("tensach", "Tên Sách");
            gridCT.Columns.Add("dongia", "Đơn Giá");
            gridCT.Columns.Add("slnhap", "SL Nhập");
            gridCT.Columns.Add("thanhtien", "Thành Tiền");

            grpCT.Controls.Add(gridCT);
            grpCT.Controls.Add(pnlCTTop);
            split.Panel2.Controls.Add(grpCT);

            this.Controls.Add(split);
        }

        private DataGridView MkGrid()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false, BackgroundColor = AppTheme.Surface,
                Font = new Font(AppTheme.FontFamily, 11F)
            };
        }

        private Button MkBtn(string text, Color back, int width)
        {
            var b = new Button
            {
                Text = text, FlatStyle = FlatStyle.Flat, BackColor = back, ForeColor = Color.White,
                Font = new Font(AppTheme.FontFamily, 11F, FontStyle.Bold),
                Size = new Size(width, 36), Cursor = Cursors.Hand, Margin = new Padding(0, 4, 8, 0)
            };
            b.FlatAppearance.BorderSize = 0;
            RoundedCorner.Apply(b, 6);
            return b;
        }

        private List<CT_PHIEUNHAP> allCT;

        private void LoadData()
        {
            dsPhieu = BUSPhieuNhap.Instance.GetAllPhieuNhap();
            allCT = BUSCT_PhieuNhap.Instance.GetAllCT_PHIEUNHAP();
            selectedSoPhieu = -1;
            BindPhieu();

            if (dsPhieu.Count > 0)
            {
                selectedSoPhieu = dsPhieu[0].SoPhieuNhap;
                dtpNgayNhap.Value = dsPhieu[0].NgayNhap;
            }
            BindCT();
        }

        private void BindPhieu()
        {
            gridPhieu.Rows.Clear();
            foreach (var p in dsPhieu)
            {
                int soCT = allCT.Count(c => c.SoPhieuNhap == p.SoPhieuNhap);
                gridPhieu.Rows.Add(p.SoPhieuNhap, p.NgayNhap.ToShortDateString(), p.TongTien.ToString("N0"), soCT);
            }
            if (gridPhieu.Rows.Count > 0)
                gridPhieu.Rows[0].Selected = true;
        }

        private void BindCT()
        {
            gridCT.Rows.Clear();
            lblTongTien.Text = "";
            lblSoPhieu.Text = "";

            if (selectedSoPhieu <= 0) return;

            var phieu = dsPhieu.FirstOrDefault(p => p.SoPhieuNhap == selectedSoPhieu);
            if (phieu == null) return;

            lblSoPhieu.Text = $"Phiếu #{selectedSoPhieu}";
            lblTongTien.Text = $"Tổng tiền: {phieu.TongTien:N0} VNĐ";

            var ctList = allCT.Where(c => c.SoPhieuNhap == selectedSoPhieu).ToList();

            foreach (var ct in ctList)
            {
                var sach = DALSach.Instance.GetSachById(ct.idSach);
                gridCT.Rows.Add(sach?.MaSach ?? "", sach?.TUASACH?.TenTuaSach ?? "",
                    ct.DonGia.ToString("N0"), ct.SoLuongNhap, ct.ThanhTien.ToString("N0"));
            }
        }

        private void GridPhieu_SelectionChanged(object sender, EventArgs e)
        {
            if (gridPhieu.CurrentRow == null || gridPhieu.CurrentRow.Cells["sp"].Value == null) return;
            selectedSoPhieu = Convert.ToInt32(gridPhieu.CurrentRow.Cells["sp"].Value);
            try { dtpNgayNhap.Value = DateTime.Parse(gridPhieu.CurrentRow.Cells["ngay"].Value.ToString()); } catch { }
            BindCT();
        }

        private void butThemPhieu_Click(object sender, EventArgs e)
        {
            DateTime ngay = dtpNgayNhap.Value;
            if (ngay > DateTime.Now) { Warn("Ngày nhập không thể trong tương lai."); return; }

            int soPhieu = BUSPhieuNhap.Instance.AddPhieuNhap(ngay);
            if (soPhieu < 0) { Warn("Không thể tạo phiếu nhập."); return; }

            Info($"Đã tạo phiếu nhập #{soPhieu}");
            LoadData();
        }

        private void butXoaPhieu_Click(object sender, EventArgs e)
        {
            if (selectedSoPhieu <= 0) { Warn("Chọn một phiếu nhập để xóa."); return; }
            if (MessageBox.Show($"Xóa phiếu nhập #{selectedSoPhieu}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            string err = BUSPhieuNhap.Instance.DelPhieuNhap(selectedSoPhieu);
            if (string.IsNullOrEmpty(err)) { Info("Đã xóa."); LoadData(); }
            else Warn(err);
        }

        private void butThemCT_Click(object sender, EventArgs e)
        {
            if (selectedSoPhieu <= 0) { Warn("Chọn một phiếu nhập trước."); return; }
            string maSach = txtMaSach.Text.Trim();
            if (string.IsNullOrWhiteSpace(maSach)) { Warn("Nhập Mã Sách."); return; }
            if (!int.TryParse(txtSoLuongNhap.Text.Trim(), out int sl) || sl <= 0) { Warn("Số lượng nhập không hợp lệ."); return; }

            var sach = DALSach.Instance.GetSachByMa(maSach);
            if (sach == null) { Warn("Mã sách không tồn tại."); return; }

            string err = BUSCT_PhieuNhap.Instance.AddCtPhieuNhap(selectedSoPhieu, sach.id, sach.DonGia, sl);
            if (string.IsNullOrEmpty(err))
            {
                Info($"Đã thêm {sl} cuốn sách {maSach} vào phiếu.");
                LoadData();
            }
            else Warn(err);
        }

        private void butXoaCT_Click(object sender, EventArgs e)
        {
            if (gridCT.CurrentRow == null) { Warn("Chọn một dòng chi tiết để xóa."); return; }
            // Not implemented in DAL - limited support
            Warn("Chức năng xóa chi tiết phiếu nhập chưa được hỗ trợ.");
        }

        private void Warn(string m) => MessageBox.Show(m, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void Info(string m) => MessageBox.Show(m, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
