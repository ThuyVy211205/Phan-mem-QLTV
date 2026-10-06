using BUS;
using DTO;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GUI.UserControls
{
    /// <summary>
    /// Màn quản lý Tác Giả theo mẫu chuẩn.
    /// </summary>
    public class ucTacGiaNew : ucCrudBase
    {
        protected override string ScreenTitle => "Quản Lý Tác Giả";
        protected override string SearchTitle => "Tìm kiếm Tác Giả";
        protected override string[] SearchByOptions => new[] { "Mã Tác Giả", "Tên Tác Giả" };

        protected override void BuildFields()
        {
            AddField(new CrudField { Key = "ma", Label = "Mã TG :", ReadOnly = true });
            AddField(new CrudField { Key = "ten", Label = "Tên TG :" });
            AddField(new CrudField { Key = "email", Label = "Email :" });
            AddField(new CrudField { Key = "diachi", Label = "Địa chỉ :" });
        }

        protected override void BuildGridColumns()
        {
            grid.Columns.Clear();
            grid.Columns.Add("id", "id");
            grid.Columns["id"].Visible = false;
            grid.Columns.Add("ma", "Mã Tác Giả");
            grid.Columns.Add("ten", "Tên Tác Giả");
            grid.Columns.Add("email", "Email");
            grid.Columns.Add("diachi", "Địa Chỉ");
        }

        protected override void LoadData()
        {
            var list = BUSTacGia.Instance.GetAllTacGia();
            // Doc email/diachi truc tiep tu DB (EF model cu khong co 2 cot nay)
            var extras = new Dictionary<int, (string email, string diachi)>();
            try
            {
                var rows = QLTVDb.Instance.Database.SqlQuery<EmailDiaChiRow>(
                    "SELECT id, ISNULL(Email,'') Email, ISNULL(DiaChi,'') DiaChi FROM TACGIA");
                foreach (var r in rows) extras[r.id] = (r.Email, r.DiaChi);
            }
            catch { }
            Bind(list, extras);
        }

        private void Bind(List<TACGIA> list, Dictionary<int, (string email, string diachi)> extras)
        {
            grid.Rows.Clear();
            foreach (var tg in list)
            {
                extras.TryGetValue(tg.id, out var ex);
                grid.Rows.Add(tg.id, tg.MATACGIA, tg.TenTacGia, ex.email, ex.diachi);
            }
        }

        protected override void OnRowSelected(DataGridViewRow row)
        {
            if (row.Cells["id"].Value == null) return;
            SelectedId = Convert.ToInt32(row.Cells["id"].Value);
            SetText("ma", row.Cells["ma"].Value?.ToString());
            SetText("ten", row.Cells["ten"].Value?.ToString());
            SetText("email", row.Cells["email"].Value?.ToString());
            SetText("diachi", row.Cells["diachi"].Value?.ToString());
        }

        protected override void DoSearch(int byIndex, string keyword)
        {
            var all = BUSTacGia.Instance.GetAllTacGia();
            var res = new List<TACGIA>();
            foreach (var tg in all)
            {
                string field = byIndex == 0 ? tg.MATACGIA : tg.TenTacGia;
                if (string.IsNullOrEmpty(keyword) || (field ?? "").ToLower().Contains(keyword.ToLower()))
                    res.Add(tg);
            }
            Bind(res, new Dictionary<int, (string email, string diachi)>());
        }

        protected override void DoAdd()
        {
            string ten = GetText("ten");
            if (string.IsNullOrWhiteSpace(ten)) { Warn("Tên tác giả không được rỗng."); return; }
            if (BUSTacGia.Instance.AddTacGia(ten) != -1) Info("Thêm tác giả thành công.");
            else Warn("Không thể thêm tác giả.");
        }

        protected override void DoEdit()
        {
            if (SelectedId == -1) return;
            string ten = GetText("ten");
            string email = GetText("email");
            string diachi = GetText("diachi");
            if (string.IsNullOrWhiteSpace(ten)) { Warn("Tên tác giả không được rỗng."); return; }
            if (!BUSTacGia.Instance.UpdTacGia(SelectedId, ten)) { Warn("Không thể cập nhật."); return; }
            // Luu email + diachi qua raw SQL (EF model cu chua map 2 cot nay)
            try { QLTVDb.Instance.Database.ExecuteSqlCommand(
                "UPDATE TACGIA SET Email={0}, DiaChi=N'{1}' WHERE id={2}", email, diachi, SelectedId); }
            catch { }
            Info("Cập nhật thành công.");
        }

        protected override void DoDelete()
        {
            if (SelectedId == -1) { Warn("Chọn một dòng để xóa."); return; }
            if (MessageBox.Show("Bạn có chắc muốn xóa tác giả này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string err = BUSTacGia.Instance.DelTacGia(SelectedId);
            if (string.IsNullOrEmpty(err)) Info("Đã xóa.");
            else Warn(err);
        }

        private void Warn(string m) => MessageBox.Show(m, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void Info(string m) => MessageBox.Show(m, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ucTacGiaNew
            // 
            this.Name = "ucTacGiaNew";
            this.Size = new System.Drawing.Size(1258, 843);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }

    // Helper cho SqlQuery raw SQL
    internal class EmailDiaChiRow
    {
        public int id { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
    }
}

