using BUS;
using DTO;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GUI.UserControls
{
    /// <summary>
    /// Màn quản lý Loại Độc Giả theo mẫu chuẩn.
    /// </summary>
    public class ucLoaiDGNew : ucCrudBase
    {
        protected override string ScreenTitle => "Quản Lý Loại Độc Giả";
        protected override string SearchTitle => "Tìm kiếm Loại Độc Giả";
        protected override string[] SearchByOptions => new[] { "Mã Loại", "Tên Loại" };

        protected override void BuildFields()
        {
            AddField(new CrudField { Key = "ma", Label = "Mã LĐG :", ReadOnly = true });
            AddField(new CrudField { Key = "ten", Label = "Tên LĐG :" });
        }

        protected override void BuildGridColumns()
        {
            grid.Columns.Clear();
            grid.Columns.Add("id", "id");
            grid.Columns["id"].Visible = false;
            grid.Columns.Add("ma", "Mã Loại Độc Giả");
            grid.Columns.Add("ten", "Tên Loại Độc Giả");
        }

        protected override void LoadData()
        {
            Bind(BUSLoaiDocGia.Instance.GetAllLoaiDocGia());
        }

        private void Bind(List<LOAIDOCGIA> list)
        {
            grid.Rows.Clear();
            foreach (var l in list)
                grid.Rows.Add(l.id, l.MaLoaiDocGia, l.TenLoaiDocGia);
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
            var all = BUSLoaiDocGia.Instance.GetAllLoaiDocGia();
            var res = new List<LOAIDOCGIA>();
            foreach (var l in all)
            {
                string field = byIndex == 0 ? l.MaLoaiDocGia : l.TenLoaiDocGia;
                if (string.IsNullOrEmpty(keyword) || (field ?? "").ToLower().Contains(keyword.ToLower()))
                    res.Add(l);
            }
            Bind(res);
        }

        protected override void DoAdd()
        {
            string ten = GetText("ten");
            if (string.IsNullOrWhiteSpace(ten)) { Warn("Tên loại độc giả không được rỗng."); return; }
            string err = BUSLoaiDocGia.Instance.AddLoaiDocGia(ten);
            if (string.IsNullOrEmpty(err)) Info("Thêm thành công."); else Warn(err);
        }

        protected override void DoEdit()
        {
            if (SelectedId == -1) return;
            string ten = GetText("ten");
            if (string.IsNullOrWhiteSpace(ten)) { Warn("Tên loại độc giả không được rỗng."); return; }
            string err = BUSLoaiDocGia.Instance.UpdLoaiDocGia(SelectedId, ten);
            if (string.IsNullOrEmpty(err)) Info("Cập nhật thành công."); else Warn(err);
        }

        protected override void DoDelete()
        {
            if (SelectedId == -1) { Warn("Chọn một dòng để xóa."); return; }
            if (MessageBox.Show("Bạn có chắc muốn xóa loại độc giả này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string err = BUSLoaiDocGia.Instance.DelLoaiDocGia(SelectedId);
            if (string.IsNullOrEmpty(err)) Info("Đã xóa."); else Warn(err);
        }

        private void Warn(string m) => MessageBox.Show(m, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void Info(string m) => MessageBox.Show(m, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ucLoaiDGNew
            // 
            this.Name = "ucLoaiDGNew";
            this.Size = new System.Drawing.Size(1258, 842);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}

