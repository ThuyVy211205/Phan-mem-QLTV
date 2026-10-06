using BUS;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GUI.UserControls
{
    /// <summary>
    /// Màn quản lý Phiếu Thu (thu tiền phạt) theo mẫu chuẩn.
    /// </summary>
    public class ucPhieuThuNew : ucCrudBase
    {
        protected override string ScreenTitle => "Quản Lý Phiếu Thu";
        protected override string SearchTitle => "Tìm kiếm Phiếu Thu";
        protected override string[] SearchByOptions => new[] { "Số Phiếu", "Mã Độc Giả" };
        protected override int InfoHeight => 170;
        protected override bool ImmediateActions => true;

        protected override string LabelThem => "Thu Tiền";
        protected override string LabelSua => null;    // ẩn
        protected override string LabelLuu => null;
        protected override string LabelHuy => null;

        protected override void BuildFields()
        {
            AddField(new CrudField { Key = "madg", Label = "Mã Độc Giả :" });
            AddField(new CrudField { Key = "sotien", Label = "Số Tiền :" });
            AddField(new CrudField { Key = "ngaylap", Label = "Ngày Lập :", IsDate = true });
        }

        protected override void BuildGridColumns()
        {
            grid.Columns.Clear();
            grid.Columns.Add("sp", "Số Phiếu"); grid.Columns["sp"].Visible = false;
            grid.Columns.Add("madg", "Mã Độc Giả");
            grid.Columns.Add("tendg", "Tên Độc Giả");
            grid.Columns.Add("sotien", "Số Tiền Thu");
            grid.Columns.Add("ngaylap", "Ngày Lập");
            grid.Columns.Add("no", "Tổng Nợ Hiện Tại");
        }

        protected override void LoadData() => Bind(BUSPhieuThu.Instance.GetAllPhieuThu());

        private void Bind(List<PHIEUTHU> list)
        {
            grid.Rows.Clear();
            foreach (var p in list)
                grid.Rows.Add(p.SoPhieuThu, p.DOCGIA?.MaDocGia, p.DOCGIA?.TenDocGia,
                    p.SoTienThu, p.NgayLap.ToShortDateString(), p.DOCGIA?.TongNoHienTai);
        }

        protected override void OnRowSelected(DataGridViewRow row)
        {
            if (row.Cells["sp"].Value == null) return;
            SelectedId = Convert.ToInt32(row.Cells["sp"].Value);
            SetText("madg", row.Cells["madg"].Value?.ToString());
            SetText("sotien", row.Cells["sotien"].Value?.ToString());
        }

        protected override void DoSearch(int byIndex, string keyword)
        {
            var all = BUSPhieuThu.Instance.GetAllPhieuThu();
            keyword = (keyword ?? "").ToLower();
            var res = all.Where(p =>
            {
                if (byIndex == 0) return p.SoPhieuThu.ToString().Contains(keyword);
                return (p.DOCGIA?.MaDocGia ?? "").ToLower().Contains(keyword);
            }).ToList();
            Bind(res);
        }

        protected override void DoAdd()
        {
            string madg = GetText("madg");
            if (string.IsNullOrWhiteSpace(madg)) { Warn("Nhập Mã Độc Giả."); return; }
            if (!int.TryParse(GetText("sotien"), out int sotien) || sotien <= 0)
            { Warn("Số tiền không hợp lệ."); return; }

            var dg = BUSDocGia.Instance.GetAllDocGia()
                .FirstOrDefault(d => d.MaDocGia.Trim() == madg.Trim());
            if (dg == null) { Warn("Không tìm thấy độc giả."); return; }

            string err = BUSPhieuThu.Instance.AddPhieuThu(dg.ID, sotien, GetDate("ngaylap"));
            if (string.IsNullOrEmpty(err))
            {
                // trừ nợ hiện tại của độc giả
                BUSDocGia.Instance.UpdTongNo(dg.ID, Math.Max(0, dg.TongNoHienTai - sotien));
                Info("Thu tiền thành công.");
            }
            else Warn(err);
        }

        protected override void DoDelete()
        {
            if (SelectedId == -1) { Warn("Chọn một phiếu để xóa."); return; }
            if (MessageBox.Show("Xóa phiếu thu này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string err = BUSPhieuThu.Instance.DelPhieuThu(SelectedId);
            if (string.IsNullOrEmpty(err)) Info("Đã xóa."); else Warn(err);
        }

        private void Warn(string m) => MessageBox.Show(m, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void Info(string m) => MessageBox.Show(m, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ucPhieuThuNew
            // 
            this.Name = "ucPhieuThuNew";
            this.Size = new System.Drawing.Size(1258, 842);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}

