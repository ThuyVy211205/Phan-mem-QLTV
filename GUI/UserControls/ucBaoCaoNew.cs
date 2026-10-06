using BUS;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GUI.UserControls
{
    /// <summary>
    /// Màn Báo Cáo Thống Kê (lượt mượn theo thể loại) theo mẫu chuẩn.
    /// Tạo báo cáo theo tháng/năm; chọn báo cáo xem chi tiết theo thể loại.
    /// </summary>
    public class ucBaoCaoNew : ucCrudBase
    {
        protected override string ScreenTitle => "Báo Cáo - Thống Kê";
        protected override string SearchTitle => "Tìm kiếm Báo Cáo";
        protected override string[] SearchByOptions => new[] { "Mã Báo Cáo", "Năm" };
        protected override int InfoHeight => 170;
        protected override bool ImmediateActions => true;

        protected override string LabelThem => "Tạo Báo Cáo";
        protected override string LabelSua => "Xem Chi Tiết";
        protected override string LabelLuu => null;
        protected override string LabelHuy => null;

        protected override void BuildFields()
        {
            AddField(new CrudField { Key = "thang", Label = "Tháng :", IsCombo = true,
                ComboSource = () => Enumerable.Range(1, 12).Select(i => (object)i.ToString()).ToList() });
            AddField(new CrudField { Key = "nam", Label = "Năm :" });
        }

        protected override void BuildGridColumns()
        {
            grid.Columns.Clear();
            grid.Columns.Add("id", "id"); grid.Columns["id"].Visible = false;
            grid.Columns.Add("ma", "Mã Báo Cáo");
            grid.Columns.Add("thang", "Tháng");
            grid.Columns.Add("nam", "Năm");
            grid.Columns.Add("tong", "Tổng Lượt Mượn");
        }

        protected override void LoadData() => Bind(BUSBCTheoTheLoai.Instance.GetAllBC());

        private void Bind(List<BCLUOTMUONTHEOTHELOAI> list)
        {
            grid.Rows.Clear();
            foreach (var b in list)
                grid.Rows.Add(b.id, b.MaBaoCao, b.Thang, b.Nam, b.TongSoLuotMuon);
        }

        protected override void OnRowSelected(DataGridViewRow row)
        {
            if (row.Cells["id"].Value == null) return;
            SelectedId = Convert.ToInt32(row.Cells["id"].Value);
            SetText("thang", row.Cells["thang"].Value?.ToString());
            SetText("nam", row.Cells["nam"].Value?.ToString());
        }

        protected override void DoSearch(int byIndex, string keyword)
        {
            var all = BUSBCTheoTheLoai.Instance.GetAllBC();
            keyword = (keyword ?? "").ToLower();
            var res = all.Where(b =>
            {
                if (byIndex == 0) return (b.MaBaoCao ?? "").ToLower().Contains(keyword);
                return b.Nam.ToString().Contains(keyword);
            }).ToList();
            Bind(res);
        }

        protected override void DoAdd()
        {
            if (!int.TryParse(GetText("thang"), out int thang) || thang < 1 || thang > 12)
            { Warn("Chọn tháng hợp lệ."); return; }
            if (!int.TryParse(GetText("nam"), out int nam) || nam < 1)
            { Warn("Nhập năm hợp lệ."); return; }
            string err = BUSBCTheoTheLoai.Instance.AddBC(thang, nam);
            if (string.IsNullOrEmpty(err)) Info("Tạo báo cáo thành công."); else Warn(err);
        }

        // "Xem chi tiết" — hiển thị chi tiết lượt mượn theo thể loại của báo cáo đang chọn
        protected override void DoEdit()
        {
            if (SelectedId == -1) { Warn("Chọn một báo cáo."); return; }
            var bc = BUSBCTheoTheLoai.Instance.GetBCById(SelectedId);
            if (bc == null) { Warn("Không tìm thấy báo cáo."); return; }
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Báo cáo {bc.MaBaoCao} - Tháng {bc.Thang}/{bc.Nam}");
            sb.AppendLine($"Tổng lượt mượn: {bc.TongSoLuotMuon}");
            sb.AppendLine("-----------------------------------");
            foreach (var ct in bc.CT_BCLUOTMUONTHEOTHELOAI)
                sb.AppendLine($"{ct.THELOAI?.TenTheLoai}: {ct.SoLuotMuon} lượt ({ct.TiLe}%)");
            MessageBox.Show(sb.ToString(), "Chi tiết báo cáo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void DoDelete()
        {
            if (SelectedId == -1) { Warn("Chọn một báo cáo để xóa."); return; }
            var bc = BUSBCTheoTheLoai.Instance.GetBCById(SelectedId);
            if (bc == null) return;
            if (MessageBox.Show("Xóa báo cáo này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string err = BUSBCTheoTheLoai.Instance.DelBC(bc.MaBaoCao);
            if (string.IsNullOrEmpty(err)) Info("Đã xóa."); else Warn(err);
        }

        private void Warn(string m) => MessageBox.Show(m, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void Info(string m) => MessageBox.Show(m, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ucBaoCaoNew
            // 
            this.Name = "ucBaoCaoNew";
            this.Size = new System.Drawing.Size(1258, 842);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}

