using BUS;
using DTO;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GUI.UserControls
{
    /// <summary>
    /// Màn quản lý Thể Loại theo mẫu chuẩn (search + info + CRUD + grid).
    /// </summary>
    public class ucTheLoaiNew : ucCrudBase
    {
        protected override string ScreenTitle => "Quản Lý Thể Loại";
        protected override string SearchTitle => "Tìm kiếm Thể Loại";
        protected override string[] SearchByOptions => new[] { "Mã Thể Loại", "Tên Thể Loại" };

        protected override void BuildFields()
        {
            AddField(new CrudField { Key = "ma", Label = "Mã TL :", ReadOnly = true });
            AddField(new CrudField { Key = "ten", Label = "Tên TL :" });
        }

        protected override void BuildGridColumns()
        {
            grid.Columns.Clear();
            grid.Columns.Add("id", "id");
            grid.Columns["id"].Visible = false;
            grid.Columns.Add("ma", "Mã Thể Loại");
            grid.Columns.Add("ten", "Tên Thể Loại");
        }

        protected override void LoadData()
        {
            Bind(BUSTheLoai.Instance.GetAllTheLoai());
        }

        private void Bind(List<THELOAI> list)
        {
            grid.Rows.Clear();
            foreach (var tl in list)
                grid.Rows.Add(tl.id, tl.MaTheLoai, tl.TenTheLoai);
        }

        protected override void OnRowSelected(DataGridViewRow row)
        {
            if (row.Cells["id"].Value == null) return;
            SelectedId = Convert.ToInt32(row.Cells["id"].Value);
            SetText("ma", row.Cells["ma"].Value?.ToString());
            SetText("ten", row.Cells["ten"].Value?.ToString());
        }

        protected override void DoSearch(int byIndex, string keyword)
        {
            var all = BUSTheLoai.Instance.GetAllTheLoai();
            var res = new List<THELOAI>();
            foreach (var tl in all)
            {
                string field = byIndex == 0 ? tl.MaTheLoai : tl.TenTheLoai;
                if (string.IsNullOrEmpty(keyword) || (field ?? "").ToLower().Contains(keyword.ToLower()))
                    res.Add(tl);
            }
            Bind(res);
        }

        protected override void DoAdd()
        {
            string ten = GetText("ten");
            if (string.IsNullOrWhiteSpace(ten)) { Warn("Tên thể loại không được rỗng."); return; }
            if (BUSTheLoai.Instance.AddTheLoai(ten)) Info("Thêm thể loại thành công.");
            else Warn("Không thể thêm thể loại.");
        }

        protected override void DoEdit()
        {
            if (SelectedId == -1) return;
            string ten = GetText("ten");
            if (string.IsNullOrWhiteSpace(ten)) { Warn("Tên thể loại không được rỗng."); return; }
            if (BUSTheLoai.Instance.UpdTheLoai(SelectedId, ten)) Info("Cập nhật thành công.");
            else Warn("Không thể cập nhật.");
        }

        protected override void DoDelete()
        {
            if (SelectedId == -1) { Warn("Chọn một dòng để xóa."); return; }
            if (MessageBox.Show("Bạn có chắc muốn xóa thể loại này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            if (BUSTheLoai.Instance.DelTheLoai(SelectedId)) Info("Đã xóa.");
            else Warn("Không thể xóa (có thể đang được dùng).");
        }

        private void Warn(string m) => MessageBox.Show(m, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void Info(string m) => MessageBox.Show(m, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ucTheLoaiNew
            // 
            this.Name = "ucTheLoaiNew";
            this.Size = new System.Drawing.Size(1258, 842);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}

