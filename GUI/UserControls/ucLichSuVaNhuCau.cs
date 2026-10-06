using BUS;
using DTO;
using GUI.Theme;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GUI.UserControls
{
    public partial class ucLichSuVaNhuCau : UserControl
    {
        private DataGridView dgvLichSu, dgvNhuCau;
        private TextBox txtMaDG, txtMaSach, txtGhiChu;
        private Button butAdd, butDel;
        private Label lblDG, lblSach, lblGhiChu;
        private readonly bool _showLichSu;
        private readonly bool _showNhuCau;

        private SplitContainer splitVertical;
        private Panel panelPhai;
        private GroupBox grpLichSu;

        public ucLichSuVaNhuCau(bool showLichSu = true, bool showNhuCau = true)
        {
            this._showLichSu = showLichSu;
            this._showNhuCau = showNhuCau;
            this.Dock = DockStyle.Fill;
            this.BackColor = AppTheme.Background;
            this.Font = AppTheme.Base;
            this.AutoScaleMode = AutoScaleMode.Inherit;
            this.Load += (s, e) => { BuildUI(); ThemeManager.Apply(this); };
        }

        private void BuildUI()
        {
            if (Controls.Count > 0) return;

            splitVertical = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 500,
                Panel1Collapsed = !_showLichSu,
                Panel2Collapsed = !_showNhuCau
            };

            // ===== Panel Trái: Lịch sử mượn =====
            grpLichSu = new GroupBox
            {
                Text = "Lịch Sử Mượn - Trả",
                Dock = DockStyle.Fill,
                Font = new Font(AppTheme.FontFamily, 10.5F, FontStyle.Bold),
                ForeColor = AppTheme.Primary,
                Padding = new Padding(6)
            };

            dgvLichSu = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };
            dgvLichSu.Columns.Add("sp", "Số Phiếu");
            dgvLichSu.Columns.Add("madg", "Mã ĐG");
            dgvLichSu.Columns.Add("tendg", "Tên Độc Giả");
            dgvLichSu.Columns.Add("macs", "Mã Cuốn");
            dgvLichSu.Columns.Add("tensach", "Tên Sách");
            dgvLichSu.Columns.Add("ngaymuon", "Ngày Mượn");
            dgvLichSu.Columns.Add("hantra", "Hạn Trả");
            dgvLichSu.Columns.Add("ngaytra", "Ngày Trả");
            dgvLichSu.Columns.Add("trangthai", "Trạng Thái");
            dgvLichSu.Columns.Add("phat", "Phạt");
            grpLichSu.Controls.Add(dgvLichSu);

            // ===== Panel Phải: Nhu cầu đọc + Form nhập =====
            panelPhai = new Panel { Dock = DockStyle.Fill };

            // Form nhập liệu (Top)
            var pnlInput = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                Padding = new Padding(6),
                BackColor = AppTheme.Surface
            };
            pnlInput.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            pnlInput.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            lblDG = new Label { Text = "Mã Độc Giả:", TextAlign = ContentAlignment.MiddleLeft, AutoSize = false, Width = 130, Height = 30, Margin = new Padding(4, 8, 4, 3) };
            txtMaDG = new TextBox { ReadOnly = true, Width = 200, Margin = new Padding(4) };
            lblSach = new Label { Text = "Mã Sách:", TextAlign = ContentAlignment.MiddleLeft, AutoSize = false, Width = 130, Height = 30, Margin = new Padding(4, 8, 4, 3) };
            txtMaSach = new TextBox { ReadOnly = true, Width = 200, Margin = new Padding(4) };
            lblGhiChu = new Label { Text = "Ghi Chú:", TextAlign = ContentAlignment.MiddleLeft, AutoSize = false, Width = 130, Height = 30, Margin = new Padding(4, 8, 4, 3) };
            txtGhiChu = new TextBox { Width = 200, Margin = new Padding(4) };

            pnlInput.Controls.Add(lblDG, 0, 0);
            pnlInput.Controls.Add(txtMaDG, 1, 0);
            pnlInput.Controls.Add(lblSach, 0, 1);
            pnlInput.Controls.Add(txtMaSach, 1, 1);
            pnlInput.Controls.Add(lblGhiChu, 0, 2);
            pnlInput.Controls.Add(txtGhiChu, 1, 2);

            // Buttons
            var pnlBtn = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(6),
                FlowDirection = FlowDirection.LeftToRight
            };
            butAdd = new Button
            {
                Text = "\u2795  Thêm vào danh sách",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0x0D, 0x6E, 0xFD),
                ForeColor = Color.White,
                Font = new Font(AppTheme.FontFamily, 11F, FontStyle.Bold),
                AutoSize = true,
                MinimumSize = new Size(160, 36),
                Cursor = Cursors.Hand,
                Margin = new Padding(3)
            };
            butAdd.FlatAppearance.BorderSize = 0;
            RoundedCorner.Apply(butAdd, 6);
            butAdd.Click += butAdd_Click;

            butDel = new Button
            {
                Text = "\U0001F5D1\uFE0F  Xóa khỏi danh sách",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0xDC, 0x35, 0x45),
                ForeColor = Color.White,
                Font = new Font(AppTheme.FontFamily, 11F, FontStyle.Bold),
                AutoSize = true,
                MinimumSize = new Size(170, 36),
                Cursor = Cursors.Hand,
                Margin = new Padding(3)
            };
            butDel.FlatAppearance.BorderSize = 0;
            RoundedCorner.Apply(butDel, 6);
            butDel.Click += butDel_Click;

            pnlBtn.Controls.Add(butAdd);
            pnlBtn.Controls.Add(butDel);

            // Grid Nhu cầu (Fill)
            var grpNhuCau = new GroupBox
            {
                Text = "Danh Sách Nhu Cầu Đọc",
                Dock = DockStyle.Fill,
                Font = new Font(AppTheme.FontFamily, 10.5F, FontStyle.Bold),
                ForeColor = AppTheme.Primary,
                Padding = new Padding(6)
            };
            dgvNhuCau = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };
            dgvNhuCau.Columns.Add("id", "ID"); dgvNhuCau.Columns["id"].Visible = false;
            dgvNhuCau.Columns.Add("manc", "Mã NC");
            dgvNhuCau.Columns.Add("madg", "Mã ĐG");
            dgvNhuCau.Columns.Add("masach", "Mã Sách");
            dgvNhuCau.Columns.Add("tensach", "Tên Sách");
            dgvNhuCau.Columns.Add("ghichu", "Ghi Chú");
            dgvNhuCau.Columns.Add("ngaythem", "Ngày Thêm");
            dgvNhuCau.SelectionChanged += (s, e) =>
            {
                if (dgvNhuCau.CurrentRow == null) return;
                var row = dgvNhuCau.CurrentRow;
                txtMaDG.Text   = row.Cells["madg"].Value?.ToString();
                txtMaSach.Text = row.Cells["masach"].Value?.ToString();
                txtGhiChu.Text = row.Cells["ghichu"].Value?.ToString();
            };
            grpNhuCau.Controls.Add(dgvNhuCau);

            panelPhai.Controls.Add(grpNhuCau);
            panelPhai.Controls.Add(pnlBtn);
            panelPhai.Controls.Add(pnlInput);

            splitVertical.Panel1.Controls.Add(grpLichSu);
            splitVertical.Panel2.Controls.Add(panelPhai);

            this.Controls.Add(splitVertical);
        }

        public void LoadData(string maDocGia)
        {
            BuildUI();
            txtMaDG.Text = maDocGia;

            // Lịch sử mượn
            try
            {
                var ls = string.IsNullOrWhiteSpace(maDocGia)
                    ? BUSNhuCauDoc.Instance.GetAllLichSu()
                    : BUSNhuCauDoc.Instance.GetLichSuByDocGia(maDocGia);
                dgvLichSu.Rows.Clear();
                foreach (var l in ls)
                    dgvLichSu.Rows.Add(l.SoPhieuMuonTra, l.MaDocGia, l.TenDocGia, l.MaCuonSach,
                        l.TenSach, l.NgayMuon.ToShortDateString(), l.HanTra.ToShortDateString(),
                        l.NgayTra?.ToShortDateString() ?? "Chưa trả", l.TrangThai, l.SoTienPhat);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            // Nhu cầu đọc
            try
            {
                var nc = BUSNhuCauDoc.Instance.GetNhuCauByDocGia(maDocGia);
                dgvNhuCau.Rows.Clear();
                foreach (var n in nc)
                    dgvNhuCau.Rows.Add(n.id, n.MaNhuCau, n.MaDocGia, n.MaSach, n.TenSach,
                        n.GhiChu ?? "", n.NgayThem.ToShortDateString());
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void butAdd_Click(object sender, EventArgs e)
        {
            string maDG   = txtMaDG.Text.Trim();
            string maSach = txtMaSach.Text.Trim();
            if (string.IsNullOrWhiteSpace(maDG) || string.IsNullOrWhiteSpace(maSach))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã Độc Giả và Mã Sách.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string err = BUSNhuCauDoc.Instance.AddNhuCau(maDG, maSach, txtGhiChu.Text.Trim());
            if (string.IsNullOrEmpty(err))
            {
                LoadData(maDG);
                txtMaSach.Clear();
                txtGhiChu.Clear();
                MessageBox.Show("Đã thêm vào danh sách nhu cầu đọc.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(err, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void butDel_Click(object sender, EventArgs e)
        {
            if (dgvNhuCau.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một dòng để xóa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int id = Convert.ToInt32(dgvNhuCau.CurrentRow.Cells["id"].Value);
            string err = BUSNhuCauDoc.Instance.DelNhuCau(id);
            if (string.IsNullOrEmpty(err))
            {
                LoadData(txtMaDG.Text.Trim());
                MessageBox.Show("Đã xóa khỏi danh sách.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(err, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
