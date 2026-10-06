using BUS;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GUI.UserControls
{
    /// <summary>
    /// Màn quản lý Người Dùng theo mẫu chuẩn.
    /// </summary>
    public class ucNguoiDungNew : ucCrudBase
    {
        private List<NHOMNGUOIDUNG> _nhom;

        protected override string ScreenTitle => "Quản Lý Người Dùng";
        protected override string SearchTitle => "Tìm kiếm Người Dùng";
        protected override string[] SearchByOptions => new[] { "Mã Người Dùng", "Tên", "Đăng Nhập" };

        protected override void BuildFields()
        {
            _nhom = BUSNhomNguoiDung.Instance.GetAllNhomNguoiDung();

            AddField(new CrudField { Key = "ma", Label = "Mã ND :", ReadOnly = true });
            AddField(new CrudField { Key = "nhom", Label = "Nhóm :", IsCombo = true,
                ComboSource = () => _nhom.Select(n => (object)n.TenNhomNguoiDung).ToList() });
            AddField(new CrudField { Key = "ten", Label = "Tên ND :" });
            AddField(new CrudField { Key = "dangnhap", Label = "Đăng nhập :" });
            AddField(new CrudField { Key = "ns", Label = "Ngày sinh :", IsDate = true });
            AddField(new CrudField { Key = "matkhau", Label = "Mật khẩu :" });
        }

        protected override void BuildGridColumns()
        {
            grid.Columns.Clear();
            grid.Columns.Add("id", "id"); grid.Columns["id"].Visible = false;
            grid.Columns.Add("ma", "Mã ND");
            grid.Columns.Add("ten", "Tên Người Dùng");
            grid.Columns.Add("nhom", "Nhóm");
            grid.Columns.Add("dangnhap", "Đăng Nhập");
            grid.Columns.Add("ns", "Ngày Sinh");
        }

        protected override void LoadData() => Bind(BUSNguoiDung.Instance.GetAllNguoiDung());

        private void Bind(List<NGUOIDUNG> list)
        {
            grid.Rows.Clear();
            foreach (var n in list)
                grid.Rows.Add(n.id, n.MaNguoiDung, n.TenNguoiDung,
                    n.NHOMNGUOIDUNG?.TenNhomNguoiDung, n.TenDangNhap,
                    n.NgaySinh?.ToShortDateString());
        }

        protected override void OnRowSelected(DataGridViewRow row)
        {
            if (row.Cells["id"].Value == null) return;
            SelectedId = Convert.ToInt32(row.Cells["id"].Value);
            SetText("ma", row.Cells["ma"].Value?.ToString());
            SetText("ten", row.Cells["ten"].Value?.ToString());
            SetText("nhom", row.Cells["nhom"].Value?.ToString());
            SetText("dangnhap", row.Cells["dangnhap"].Value?.ToString());
            SetText("ns", row.Cells["ns"].Value?.ToString());
        }

        protected override void DoSearch(int byIndex, string keyword)
        {
            var all = BUSNguoiDung.Instance.GetAllNguoiDung();
            keyword = (keyword ?? "").ToLower();
            var res = all.Where(n =>
            {
                string field = byIndex == 0 ? n.MaNguoiDung : byIndex == 1 ? n.TenNguoiDung : n.TenDangNhap;
                return string.IsNullOrEmpty(keyword) || (field ?? "").ToLower().Contains(keyword);
            }).ToList();
            Bind(res);
        }

        protected override void DoAdd()
        {
            string ten = GetText("ten");
            string dn = GetText("dangnhap");
            string mk = GetText("matkhau");
            int ni = GetComboIndex("nhom");
            if (string.IsNullOrWhiteSpace(ten) || string.IsNullOrWhiteSpace(dn) || string.IsNullOrWhiteSpace(mk))
            { Warn("Nhập đủ Tên, Đăng nhập, Mật khẩu."); return; }
            if (ni < 0) { Warn("Chọn nhóm người dùng."); return; }
            int id = BUSNguoiDung.Instance.AddNguoiDung(ten, GetDate("ns"), dn, mk, _nhom[ni].id);
            if (id != -1) Info("Thêm người dùng thành công.");
            else Warn("Không thể thêm (tên đăng nhập có thể đã tồn tại).");
        }

        protected override void DoEdit()
        {
            if (SelectedId == -1) return;
            int ni = GetComboIndex("nhom");
            if (ni < 0) { Warn("Chọn nhóm người dùng."); return; }
            string err = BUSNguoiDung.Instance.UpdNguoiDung(SelectedId, GetText("ten"),
                GetDate("ns"), _nhom[ni].id);
            if (string.IsNullOrEmpty(err)) Info("Cập nhật thành công."); else Warn(err);
        }

        protected override void DoDelete()
        {
            if (SelectedId == -1) { Warn("Chọn một dòng để xóa."); return; }
            var ma = grid.CurrentRow?.Cells["ma"].Value?.ToString();
            if (string.IsNullOrEmpty(ma)) return;
            if (MessageBox.Show("Bạn có chắc muốn xóa người dùng này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string err = BUSNguoiDung.Instance.DelNguoidung(ma.Trim());
            if (string.IsNullOrEmpty(err)) Info("Đã xóa."); else Warn(err);
        }

        private void Warn(string m) => MessageBox.Show(m, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void Info(string m) => MessageBox.Show(m, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ucNguoiDungNew
            // 
            this.Name = "ucNguoiDungNew";
            this.Size = new System.Drawing.Size(1258, 842);
            this.Load += new System.EventHandler(this.ucNguoiDungNew_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ucNguoiDungNew_Load(object sender, EventArgs e)
        {

        }
    }
}

