using BUS;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GUI.UserControls
{
    /// <summary>
    /// Màn quản lý Độc Giả theo mẫu chuẩn — hỗ trợ Thêm đầy đủ (kèm tài khoản đăng nhập).
    /// </summary>
    public class ucDocGiaNew : ucCrudBase
    {
        private List<LOAIDOCGIA> _loai;
        private List<NHOMNGUOIDUNG> _nhomDG;

        protected override string ScreenTitle => "Quản Lý Độc Giả";
        protected override string SearchTitle => "Tìm kiếm Độc Giả";
        protected override string[] SearchByOptions => new[] { "Mã Độc Giả", "Tên Độc Giả", "Email" };
        protected override int InfoHeight => 300;

        protected override void BuildFields()
        {
            _loai = BUSLoaiDocGia.Instance.GetAllLoaiDocGia();
            _nhomDG = BUSNhomNguoiDung.Instance.GetAllNhomNguoiDung()
                .Where(n => n.CHUCNANGs.Any(c => c.TenChucNang == "DG")).ToList();

            AddField(new CrudField { Key = "ma", Label = "Mã ĐG :", ReadOnly = true });
            AddField(new CrudField { Key = "loai", Label = "Loại ĐG :", IsCombo = true,
                ComboSource = () => _loai.Select(l => (object)l.TenLoaiDocGia).ToList() });
            AddField(new CrudField { Key = "ten", Label = "Tên ĐG :" });
            AddField(new CrudField { Key = "email", Label = "Email :" });
            AddField(new CrudField { Key = "ns", Label = "Ngày sinh :", IsDate = true });
            AddField(new CrudField { Key = "diachi", Label = "Địa chỉ :" });
            AddField(new CrudField { Key = "lapthe", Label = "Lập thẻ :", IsDate = true });
            AddField(new CrudField { Key = "hethan", Label = "Hết hạn :", ReadOnly = true });
            // Các trường chỉ dùng khi Thêm (cấp tài khoản đăng nhập)
            AddField(new CrudField { Key = "dangnhap", Label = "Đăng nhập :" });
            AddField(new CrudField { Key = "matkhau", Label = "Mật khẩu :" });

            AddField(new CrudField { Key = "dangmuon", Label = "Đang mượn :", ReadOnly = true });
            AddField(new CrudField { Key = "vipham", Label = "Vi phạm :", ReadOnly = true });

            AddField(new CrudField { Key = "nhom", Label = "Nhóm :", IsCombo = true,
                ComboSource = () => _nhomDG.Select(n => (object)n.TenNhomNguoiDung).ToList() });

            // Tự cập nhật ngày hết hạn khi đổi ngày lập thẻ
            var lapThe = Fields.Find(f => f.Key == "lapthe");
            if (lapThe?.Input is DateTimePicker dp)
                dp.ValueChanged += (s, e) => RecalcHetHan();
        }

        private void RecalcHetHan()
        {
            try
            {
                var ts = BUSThamSo.Instance.GetAllThamSo();
                var hh = GetDate("lapthe").AddMonths((int)ts.ThoiHanThe);
                SetText("hethan", hh.ToShortDateString());
            }
            catch { }
        }

        protected override void BuildGridColumns()
        {
            grid.Columns.Clear();
            grid.Columns.Add("id", "id"); grid.Columns["id"].Visible = false;
            grid.Columns.Add("ma", "Mã ĐG");
            grid.Columns.Add("ten", "Tên Độc Giả");
            grid.Columns.Add("loai", "Loại");
            grid.Columns.Add("email", "Email");
            grid.Columns.Add("diachi", "Địa Chỉ");
            grid.Columns.Add("ns", "Ngày Sinh");
            grid.Columns.Add("lapthe", "Lập Thẻ");
            grid.Columns.Add("hethan", "Hết Hạn");
            grid.Columns.Add("no", "Tổng Nợ");
            grid.Columns.Add("muon", "Đang Mượn");
            grid.Columns.Add("vipham", "Vi Phạm");
        }

        protected override void LoadData() => Bind(BUSDocGia.Instance.GetAllDocGia());

        private void Bind(List<DOCGIA> list)
        {
            grid.Rows.Clear();
            foreach (var d in list)
                grid.Rows.Add(d.ID, d.MaDocGia, d.TenDocGia,
                    d.LOAIDOCGIA?.TenLoaiDocGia, d.Email, d.DiaChi,
                    d.NgaySinh.ToShortDateString(), d.NgayLapThe.ToShortDateString(),
                    d.NgayHetHan.ToShortDateString(), d.TongNoHienTai,
                    BUSDocGia.Instance.GetBorrowedCount(d.MaDocGia),
                    BUSDocGia.Instance.GetViolationCount(d.MaDocGia));
        }

        protected override void OnRowSelected(DataGridViewRow row)
        {
            if (row.Cells["id"].Value == null) return;
            SelectedId = Convert.ToInt32(row.Cells["id"].Value);
            SetText("ma", row.Cells["ma"].Value?.ToString());
            SetText("ten", row.Cells["ten"].Value?.ToString());
            SetText("loai", row.Cells["loai"].Value?.ToString());
            SetText("email", row.Cells["email"].Value?.ToString());
            SetText("diachi", row.Cells["diachi"].Value?.ToString());
            SetText("ns", row.Cells["ns"].Value?.ToString());
            SetText("lapthe", row.Cells["lapthe"].Value?.ToString());
            SetText("hethan", row.Cells["hethan"].Value?.ToString());
            SetText("dangmuon", row.Cells["muon"].Value?.ToString());
            SetText("vipham", row.Cells["vipham"].Value?.ToString());
        }

        protected override void AfterSetMode(Mode m)
        {
            // Các trường cấp tài khoản chỉ mở khi Thêm mới.
            bool adding = m == Mode.Add;
            SetFieldEnabled("dangnhap", adding);
            SetFieldEnabled("matkhau", adding);
            SetFieldEnabled("nhom", adding);
            SetFieldEnabled("lapthe", adding);   // lập thẻ chỉ đặt khi tạo mới
            if (adding) RecalcHetHan();
        }

        protected override void DoSearch(int byIndex, string keyword)
        {
            var all = BUSDocGia.Instance.GetAllDocGia();
            keyword = (keyword ?? "").ToLower();
            var res = all.Where(d =>
            {
                string field = byIndex == 0 ? d.MaDocGia : byIndex == 1 ? d.TenDocGia : d.Email;
                return string.IsNullOrEmpty(keyword) || (field ?? "").ToLower().Contains(keyword);
            }).ToList();
            Bind(res);
        }

        protected override void DoAdd()
        {
            string ten = GetText("ten");
            string dn = GetText("dangnhap");
            string mk = GetText("matkhau");
            int li = GetComboIndex("loai");
            int ni = GetComboIndex("nhom");

            if (string.IsNullOrWhiteSpace(ten) || string.IsNullOrWhiteSpace(dn) || string.IsNullOrWhiteSpace(mk))
            { Warn("Nhập đủ Tên, Đăng nhập, Mật khẩu."); return; }
            if (li < 0) { Warn("Chọn loại độc giả."); return; }
            if (ni < 0) { Warn("Chọn nhóm người dùng (Độc Giả)."); return; }

            DateTime lapThe = GetDate("lapthe");
            var ts = BUSThamSo.Instance.GetAllThamSo();
            DateTime hetHan = lapThe.AddMonths((int)ts.ThoiHanThe);

            string err = BUSDocGia.Instance.AddDocGia(ten, _loai[li].id, lapThe,
                GetText("email"), GetText("diachi"), GetDate("ns"), hetHan,
                dn, mk, _nhomDG[ni].id);
            if (string.IsNullOrEmpty(err)) Info("Thêm độc giả thành công."); else Warn(err);
        }

        protected override void DoEdit()
        {
            if (SelectedId == -1) return;
            int li = GetComboIndex("loai");
            int? idLDG = (li >= 0 && li < _loai.Count) ? _loai[li].id : (int?)null;
            string err = BUSDocGia.Instance.UpdDocGia(SelectedId, GetText("ten"), idLDG,
                GetText("email"), GetText("diachi"), GetDate("ns"));
            if (string.IsNullOrEmpty(err)) Info("Cập nhật thành công."); else Warn(err);
        }

        protected override void DoDelete()
        {
            if (SelectedId == -1) { Warn("Chọn một dòng để xóa."); return; }
            if (MessageBox.Show("Bạn có chắc muốn xóa độc giả này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string err = BUSDocGia.Instance.DelDocGia(SelectedId);
            if (string.IsNullOrEmpty(err)) Info("Đã xóa."); else Warn(err);
        }

        private void Warn(string m) => MessageBox.Show(m, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void Info(string m) => MessageBox.Show(m, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ucDocGiaNew
            // 
            this.Name = "ucDocGiaNew";
            this.Size = new System.Drawing.Size(1258, 842);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}

