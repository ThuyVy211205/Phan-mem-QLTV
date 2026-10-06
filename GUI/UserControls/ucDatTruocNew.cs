using BUS;
using DTO;
using GUI.Theme;
using System;
using System.Linq;
using System.Windows.Forms;

namespace GUI.UserControls
{
    public class ucDatTruocNew : ucCrudBase
    {
        private Button butDapUng;
        private readonly int userId;

        public ucDatTruocNew(int userId)
        {
            this.userId = userId;
            this.Load += (s, e) =>
            {
                butDapUng = new Button
                {
                    Text = "\u2705  Đáp Ứng + Mượn",
                    FlatStyle = FlatStyle.Flat,
                    BackColor = System.Drawing.Color.FromArgb(0x19, 0x87, 0x54),
                    ForeColor = System.Drawing.Color.White,
                    Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                    AutoSize = true,
                    MinimumSize = new System.Drawing.Size(160, 38),
                    Cursor = Cursors.Hand,
                    Margin = new Padding(8, 6, 8, 6)
                };
                butDapUng.FlatAppearance.BorderSize = 0;
                RoundedCorner.Apply(butDapUng, 6);
                butDapUng.Click += butDapUng_Click;

                var actionHost = this.Controls.OfType<FlowLayoutPanel>()
                    .FirstOrDefault(p => p.Controls.Contains(butThem));
                if (actionHost != null)
                    actionHost.Controls.Add(butDapUng);
            };
        }

        protected override string ScreenTitle => "Quản Lý Đặt Trước";
        protected override string SearchTitle => "Tìm kiếm Đặt Trước";
        protected override string[] SearchByOptions => new[] { "Mã ĐG", "Mã Sách", "Đang chờ" };
        protected override int InfoHeight => 190;
        protected override bool ImmediateActions => true;

        protected override string LabelThem => "Đặt Trước";
        protected override string LabelSua => null;
        protected override string LabelLuu => null;
        protected override string LabelHuy => null;

        protected override void BuildFields()
        {
            AddField(new CrudField { Key = "madg", Label = "Mã Độc Giả :" });
            AddField(new CrudField { Key = "masach", Label = "Mã Sách :" });
            AddField(new CrudField { Key = "ghichu", Label = "Ghi Chú :" });
            AddField(new CrudField { Key = "trangthai", Label = "Trạng Thái :", ReadOnly = true });
        }

        protected override void BuildGridColumns()
        {
            grid.Columns.Clear();
            grid.Columns.Add("id", "ID"); grid.Columns["id"].Visible = false;
            grid.Columns.Add("madg", "Mã ĐG");
            grid.Columns.Add("tendg", "Tên Độc Giả");
            grid.Columns.Add("masach", "Mã Sách");
            grid.Columns.Add("tensach", "Tựa Sách");
            grid.Columns.Add("conlai", "Còn Lại");
            grid.Columns.Add("ngaydat", "Ngày Đặt");
            grid.Columns.Add("trangthai", "Trạng Thái");
            grid.Columns.Add("ghichu", "Ghi Chú");
        }

        protected override void LoadData() => Bind(BUSDatTruoc.Instance.GetAll());

        private void Bind(System.Collections.Generic.List<DatTruocDisplay> list)
        {
            grid.Rows.Clear();
            foreach (var d in list)
                grid.Rows.Add(d.id, d.MaDocGia, d.TenDocGia, d.MaSach, d.TenTuaSach,
                    d.SoLuongConLai, d.NgayDat.ToShortDateString(), d.TrangThai, d.GhiChu);
        }

        protected override void OnRowSelected(DataGridViewRow row)
        {
            if (row.Cells["id"].Value == null) return;
            SelectedId = Convert.ToInt32(row.Cells["id"].Value);
            SetText("madg", row.Cells["madg"].Value?.ToString());
            SetText("masach", row.Cells["masach"].Value?.ToString());
            SetText("ghichu", row.Cells["ghichu"].Value?.ToString());
            SetText("trangthai", row.Cells["trangthai"].Value?.ToString());
        }

        protected override void DoSearch(int byIndex, string keyword)
        {
            var all = BUSDatTruoc.Instance.GetAll();
            keyword = (keyword ?? "").ToLower();
            if (string.IsNullOrEmpty(keyword)) { Bind(all); return; }
            var res = all.Where(d =>
            {
                switch (byIndex)
                {
                    case 0: return (d.MaDocGia ?? "").ToLower().Contains(keyword);
                    case 1: return (d.MaSach ?? "").ToLower().Contains(keyword);
                    case 2: return d.TrangThai == "Đang chờ";
                    default: return true;
                }
            }).ToList();
            Bind(res);
        }

        protected override void DoAdd()
        {
            string madg = GetText("madg");
            string masach = GetText("masach");
            if (string.IsNullOrWhiteSpace(madg) || string.IsNullOrWhiteSpace(masach))
            { Warn("Nhập Mã Độc Giả và Mã Sách."); return; }

            string err = BUSDatTruoc.Instance.AddDatTruoc(madg.Trim(), masach.Trim(), GetText("ghichu"), userId);
            if (string.IsNullOrEmpty(err)) Info("Đặt trước thành công.");
            else Warn(err);
        }

        protected override void DoDelete()
        {
            if (SelectedId == -1) { Warn("Chọn một phiếu để xoá."); return; }
            string tt = GetText("trangthai");
            string err;
            if (tt == "Đang chờ")
                err = BUSDatTruoc.Instance.HuyDatTruoc(SelectedId, userId);
            else
                err = BUSDatTruoc.Instance.Delete(SelectedId, userId);

            if (string.IsNullOrEmpty(err)) Info("Đã xoá/huỷ.");
            else Warn(err);
        }

        private void butDapUng_Click(object sender, EventArgs e)
        {
            if (SelectedId == -1) { Warn("Chọn một phiếu đặt trước đang chờ."); return; }
            if (GetText("trangthai") != "Đang chờ")
            { Warn("Chỉ có thể đáp ứng phiếu đang chờ."); return; }

            using (var f = new Form
            {
                Text = "Đáp ứng đặt trước & Mượn sách",
                Size = new System.Drawing.Size(380, 230),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            })
            {
                var lblMa = new Label { Text = "Mã Cuốn Sách:", Location = new System.Drawing.Point(30, 23), AutoSize = true };
                var txtMaCS = new TextBox { Location = new System.Drawing.Point(160, 20), Width = 180 };
                var lblNgay = new Label { Text = "Ngày Mượn:", Location = new System.Drawing.Point(30, 63), AutoSize = true };
                var dtp = new DateTimePicker { Location = new System.Drawing.Point(160, 60), Width = 180, Format = DateTimePickerFormat.Short };

                var info = new Label
                {
                    Text = $"Độc giả: {GetText("madg")}\nSách: {GetText("masach")}",
                    Location = new System.Drawing.Point(30, 100),
                    AutoSize = true,
                    MaximumSize = new System.Drawing.Size(320, 0)
                };

                var butOk = new Button
                {
                    Text = "Xác nhận",
                    FlatStyle = FlatStyle.Flat,
                    BackColor = System.Drawing.Color.FromArgb(0x19, 0x87, 0x54),
                    ForeColor = System.Drawing.Color.White,
                    Location = new System.Drawing.Point(160, 140),
                    Size = new System.Drawing.Size(100, 35)
                };
                butOk.FlatAppearance.BorderSize = 0;
                RoundedCorner.Apply(butOk, 6);
                butOk.Click += (s, ev) =>
                {
                    string maCS = txtMaCS.Text.Trim();
                    if (string.IsNullOrWhiteSpace(maCS)) { Warn("Nhập mã cuốn sách."); return; }
                    string err = BUSDatTruoc.Instance.FulfillAndBorrow(SelectedId, maCS, dtp.Value, userId);
                    if (string.IsNullOrEmpty(err)) { Info("Đáp ứng và mượn thành công."); f.DialogResult = DialogResult.OK; f.Close(); }
                    else Warn(err);
                };

                f.Controls.AddRange(new Control[] { lblMa, txtMaCS, lblNgay, dtp, info, butOk });
                if (f.ShowDialog() == DialogResult.OK) LoadData();
            }
        }

        private void Warn(string m) => MessageBox.Show(m, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void Info(string m) => MessageBox.Show(m, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ucDatTruocNew
            // 
            this.Name = "ucDatTruocNew";
            this.Size = new System.Drawing.Size(1258, 842);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
