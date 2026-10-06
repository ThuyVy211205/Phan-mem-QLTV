using BUS;
using DTO;
using GUI.Theme;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GUI.UserControls
{
    /// <summary>
    /// Màn Tra cứu sách (chỉ đọc) đồng bộ theme xanh: khu tìm kiếm + lọc + bảng kết quả.
    /// </summary>
    public class ucTraCuuSachNew : UserControl
    {
        private TextBox txtSearch;
        private ComboBox comboTheLoai;
        private ComboBox comboTinhTrang;
        private DataGridView grid;

        public ucTraCuuSachNew()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = AppTheme.Background;
            this.Font = AppTheme.Base;
            this.AutoScaleMode = AutoScaleMode.Inherit;
            this.Load += (s, e) => { BuildUI(); LoadAll(); ThemeManager.Apply(this); };
        }

        private void BuildUI()
        {
            if (Controls.Count > 0) return;

            // ---- Bảng kết quả (Fill) ----
            var grpGrid = MakeGroup("Danh sách sách");
            grpGrid.Dock = DockStyle.Fill;
            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ScrollBars = ScrollBars.Both,
                RowHeadersVisible = false
            };
            grid.Columns.Add("ma", "Mã Sách");
            grid.Columns.Add("ten", "Tựa Sách");
            grid.Columns.Add("chude", "Thể Loại");
            grid.Columns.Add("tacgia", "Tác Giả");
            grid.Columns.Add("nxb", "NXB");
            grid.Columns.Add("namxb", "Năm XB");
            grid.Columns.Add("tinhtrang", "Tình Trạng");
            grpGrid.Controls.Add(grid);

            // ---- Khu tìm kiếm (Top) - dùng TableLayoutPanel tự co giãn ----
            var grpSearch = MakeGroup("Tra cứu sách");
            grpSearch.Dock = DockStyle.Top;
            grpSearch.AutoSize = true;
            grpSearch.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 3,
                Padding = new Padding(10, 6, 10, 6)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Hàng 1: từ khóa + nút tìm
            var lblKey = new Label { Text = "Nhập Mã / Tên sách / Tác giả / NXB", AutoSize = true, Margin = new Padding(3, 8, 8, 3), ForeColor = AppTheme.TextMuted };
            txtSearch = new TextBox { Width = 300, Font = AppTheme.Base, Margin = new Padding(3, 5, 8, 5) };
            var butSearch = MakeButton("Tìm kiếm", "tim");
            butSearch.Click += (s, e) => DoSearchByKeyword();
            table.Controls.Add(lblKey, 0, 0);
            table.Controls.Add(txtSearch, 1, 0);
            table.Controls.Add(butSearch, 2, 0);

            // Hàng 2: lọc thể loại
            var lblTL = new Label { Text = "Thể loại:", AutoSize = true, Margin = new Padding(3, 8, 8, 3), ForeColor = AppTheme.TextMuted };
            comboTheLoai = new ComboBox { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.Base, Margin = new Padding(3, 5, 8, 5) };
            var butFilTL = MakeButton("Lọc thể loại", "loc");
            butFilTL.Click += (s, e) => DoFilterTheLoai();
            table.Controls.Add(lblTL, 0, 1);
            table.Controls.Add(comboTheLoai, 1, 1);
            table.Controls.Add(butFilTL, 2, 1);

            // Hàng 3: lọc tình trạng + tải lại
            var lblTT = new Label { Text = "Tình trạng:", AutoSize = true, Margin = new Padding(3, 8, 8, 3), ForeColor = AppTheme.TextMuted };
            comboTinhTrang = new ComboBox { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.Base, Margin = new Padding(3, 5, 8, 5) };
            comboTinhTrang.Items.AddRange(new object[] { "Còn", "Hết" });
            var filTTHost = new FlowLayoutPanel { AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Margin = new Padding(0) };
            var butFilTT = MakeButton("Lọc tình trạng", "loc");
            butFilTT.Click += (s, e) => DoFilterTinhTrang();
            var butRefresh = MakeButton("Tải lại", "refresh");
            butRefresh.Click += (s, e) => { txtSearch.Clear(); LoadAll(); };
            filTTHost.Controls.Add(butFilTT);
            filTTHost.Controls.Add(butRefresh);
            table.Controls.Add(lblTT, 0, 2);
            table.Controls.Add(comboTinhTrang, 1, 2);
            table.Controls.Add(filTTHost, 2, 2);

            grpSearch.Controls.Add(table);

            this.Controls.Add(grpGrid);
            this.Controls.Add(grpSearch);

            // nạp combo thể loại
            comboTheLoai.DisplayMember = "TenTheLoai";
            comboTheLoai.ValueMember = "id";
            comboTheLoai.DataSource = BUSTheLoai.Instance.GetAllTheLoai();
            comboTheLoai.SelectedIndex = -1;
        }

        private void LoadAll() => Bind(BUSSach.Instance.GetAllSach());

        private void Bind(List<SACH> list)
        {
            grid.Rows.Clear();
            foreach (SACH sach in list)
            {
                if (sach.DaAn != 0) continue;
                string tacgia = "";
                try { tacgia = string.Join(", ", sach.TUASACH.TACGIAs.Select(t => t.TenTacGia)); } catch { }
                string con = sach.SoLuongConLai > 0 ? "Còn" : "Hết";
                grid.Rows.Add(sach.MaSach, sach.TUASACH?.TenTuaSach,
                    sach.TUASACH?.THELOAI?.TenTheLoai, tacgia, sach.NhaXB, sach.NamXB, con);
            }
        }

        private void DoSearchByKeyword()
        {
            string pat = (txtSearch.Text ?? "").ToLower();
            var res = BUSSach.Instance.GetAllSach().Where(sach =>
                (sach.MaSach ?? "").ToLower().Contains(pat) ||
                (sach.TUASACH?.TenTuaSach ?? "").ToLower().Contains(pat) ||
                (sach.NhaXB ?? "").ToLower().Contains(pat) ||
                sach.NamXB.ToString().Contains(pat) ||
                SafeTacGiaContains(sach, pat)).ToList();
            Bind(res);
        }

        private bool SafeTacGiaContains(SACH sach, string pat)
        {
            try { return sach.TUASACH.TACGIAs.Any(t => (t.TenTacGia ?? "").ToLower().Contains(pat)); }
            catch { return false; }
        }

        private void DoFilterTheLoai()
        {
            if (comboTheLoai.SelectedValue == null) return;
            int id = (int)comboTheLoai.SelectedValue;
            var res = BUSSach.Instance.GetAllSach()
                .Where(s => s.TUASACH?.THELOAI?.id == id).ToList();
            Bind(res);
        }

        private void DoFilterTinhTrang()
        {
            int tt = comboTinhTrang.SelectedIndex;
            if (tt < 0) return;
            var res = BUSSach.Instance.GetAllSach().Where(s =>
                (tt == 0 && s.SoLuongConLai > 0) || (tt == 1 && s.SoLuongConLai <= 0)).ToList();
            Bind(res);
        }

        private GroupBox MakeGroup(string text)
        {
            return new GroupBox
            {
                Text = text,
                Font = new Font(AppTheme.FontFamily, 10.5F, FontStyle.Bold),
                ForeColor = AppTheme.Primary,
                Padding = new Padding(6)
            };
        }

        private Button MakeButton(string text, string iconKey)
        {
            string icon = IconFor(iconKey);
            var b = new Button
            {
                Text = icon + "  " + text,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new Size(130, 34),
                Margin = new Padding(6, 4, 6, 4),
                Padding = new Padding(8, 4, 10, 4),
                FlatStyle = FlatStyle.Flat,
                BackColor = AppTheme.Primary,
                ForeColor = AppTheme.TextOnPrimary,
                Font = new Font(AppTheme.FontFamily, 11F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft
            };
            b.FlatAppearance.BorderSize = 0;
            RoundedCorner.Apply(b, 6);
            return b;
        }

        private static string IconFor(string key)
        {
            if (key.Contains("tim") || key.Contains("search")) return "\U0001F50D";  // 🔍
            if (key.Contains("loc") || key.Contains("filter")) return "\U0001F4CB";  // 📋
            if (key.Contains("tai") || key.Contains("refresh") || key.Contains("reload")) return "\U0001F504"; // 🔄
            return "";
        }
    }
}
