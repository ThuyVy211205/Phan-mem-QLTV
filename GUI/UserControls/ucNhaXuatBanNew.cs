using BUS;
using DTO;
using System;
using System.Linq;
using System.Windows.Forms;

namespace GUI.UserControls
{
    public class ucNhaXuatBanNew : ucCrudBase
    {
        protected override string ScreenTitle => "Quản Lý NXB";
        protected override string SearchTitle => "Tìm kiếm NXB";
        protected override string[] SearchByOptions => new[] { "Mã NXB", "Tên NXB" };
        protected override int InfoHeight => 260;
        protected override bool ImmediateActions => true;

        protected override string LabelThem => "Thêm";
        protected override string LabelSua => "Sửa";
        protected override string LabelXoa => "Xóa";
        protected override string LabelLuu => null;
        protected override string LabelHuy => null;

        protected override void BuildFields()
        {
            AddField(new CrudField { Key = "ma", Label = "Mã NXB :", ReadOnly = true });
            AddField(new CrudField { Key = "ten", Label = "Tên NXB :" });
            AddField(new CrudField { Key = "diachi", Label = "Địa Chỉ :" });
            AddField(new CrudField { Key = "email", Label = "Email :" });
            AddField(new CrudField { Key = "dienthoai", Label = "Điện Thoại :" });
        }

        protected override void BuildGridColumns()
        {
            grid.Columns.Clear();
            grid.Columns.Add("id", "ID"); grid.Columns["id"].Visible = false;
            grid.Columns.Add("ma", "Mã NXB");
            grid.Columns.Add("ten", "Tên NXB");
            grid.Columns.Add("diachi", "Địa Chỉ");
            grid.Columns.Add("email", "Email");
            grid.Columns.Add("dienthoai", "Điện Thoại");
        }

        protected override void LoadData() => Bind(BUSNhaXuatBan.Instance.GetAll());

        private void Bind(System.Collections.Generic.List<NHAXUATBAN> list)
        {
            grid.Rows.Clear();
            foreach (var nxb in list)
                grid.Rows.Add(nxb.id, nxb.MaNXB, nxb.TenNXB, nxb.DiaChi, nxb.Email ?? "", nxb.DienThoai ?? "");
        }

        protected override void OnRowSelected(DataGridViewRow row)
        {
            if (row.Cells["id"].Value == null) return;
            SelectedId = Convert.ToInt32(row.Cells["id"].Value);
            SetText("ma", row.Cells["ma"].Value?.ToString());
            SetText("ten", row.Cells["ten"].Value?.ToString());
            SetText("diachi", row.Cells["diachi"].Value?.ToString());
            SetText("email", row.Cells["email"].Value?.ToString());
            SetText("dienthoai", row.Cells["dienthoai"].Value?.ToString());
        }

        protected override void DoSearch(int byIndex, string keyword)
        {
            var all = BUSNhaXuatBan.Instance.GetAll();
            keyword = (keyword ?? "").ToLower();
            if (string.IsNullOrEmpty(keyword)) { Bind(all); return; }
            var res = all.Where(n =>
            {
                if (byIndex == 0) return (n.MaNXB ?? "").ToLower().Contains(keyword);
                return (n.TenNXB ?? "").ToLower().Contains(keyword);
            }).ToList();
            Bind(res);
        }

        protected override void DoAdd()
        {
            string ten = GetText("ten");
            if (string.IsNullOrWhiteSpace(ten)) { Warn("Nhập Tên NXB."); return; }
            string err = BUSNhaXuatBan.Instance.Add(ten, GetText("diachi"), GetText("email"), GetText("dienthoai"));
            if (string.IsNullOrEmpty(err)) Info("Thêm NXB thành công.");
            else Warn(err);
        }

        protected override void DoEdit()
        {
            if (SelectedId == -1) { Warn("Chọn một NXB để sửa."); return; }
            string ten = GetText("ten");
            if (string.IsNullOrWhiteSpace(ten)) { Warn("Nhập Tên NXB."); return; }
            string err = BUSNhaXuatBan.Instance.Update(SelectedId, ten, GetText("diachi"), GetText("email"), GetText("dienthoai"));
            if (string.IsNullOrEmpty(err)) Info("Cập nhật NXB thành công.");
            else Warn(err);
        }

        protected override void DoDelete()
        {
            if (SelectedId == -1) { Warn("Chọn một NXB để xoá."); return; }
            if (MessageBox.Show("Xoá NXB này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string err = BUSNhaXuatBan.Instance.Delete(SelectedId);
            if (string.IsNullOrEmpty(err)) Info("Đã xoá.");
            else Warn(err);
        }

        private void Warn(string m) => MessageBox.Show(m, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void Info(string m) => MessageBox.Show(m, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ucNhaXuatBanNew
            // 
            this.Name = "ucNhaXuatBanNew";
            this.Size = new System.Drawing.Size(1258, 842);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
