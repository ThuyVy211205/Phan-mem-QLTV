using BUS;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GUI.UserControls
{
    /// <summary>
    /// Màn quản lý Phiếu Mượn - Trả theo mẫu chuẩn.
    /// Mượn = nhập Mã Cuốn Sách + Mã Độc Giả + Ngày Mượn.
    /// Trả  = chọn phiếu chưa trả rồi bấm "Trả".
    /// </summary>
    public class ucPhieuMuonTraNew : ucCrudBase
    {
        protected override string ScreenTitle => "Quản Lý Phiếu Mượn - Trả";
        protected override string SearchTitle => "Tìm kiếm Phiếu Mượn Trả";
        protected override string[] SearchByOptions => new[] { "Số Phiếu", "Mã Độc Giả", "Mã Cuốn Sách", "Chưa Trả" };
        protected override int InfoHeight => 190;
        protected override bool ImmediateActions => true;

        // Đổi nhãn nút cho phù hợp nghiệp vụ
        protected override string LabelThem => "Mượn";
        protected override string LabelSua => "Trả";
        protected override string LabelLuu => null;   // ẩn
        protected override string LabelHuy => null;   // ẩn

        protected override void BuildFields()
        {
            AddField(new CrudField { Key = "macs", Label = "Mã Sách :" });
            AddField(new CrudField { Key = "madg", Label = "Mã Độc Giả :" });
            AddField(new CrudField { Key = "ngaymuon", Label = "Ngày Mượn :", IsDate = true });
            AddField(new CrudField { Key = "ngaytra", Label = "Ngày Trả :", IsDate = true });
        }

        protected override void BuildGridColumns()
        {
            grid.Columns.Clear();
            grid.Columns.Add("sp", "Số Phiếu"); grid.Columns["sp"].Visible = false;
            grid.Columns.Add("macs", "Mã Cuốn");
            grid.Columns.Add("tensach", "Tên Sách");
            grid.Columns.Add("madg", "Mã Độc Giả");
            grid.Columns.Add("tendg", "Tên Độc Giả");
            grid.Columns.Add("ngaymuon", "Ngày Mượn");
            grid.Columns.Add("hantra", "Hạn Trả");
            grid.Columns.Add("ngaytra", "Ngày Trả");
            grid.Columns.Add("phat", "Tiền Phạt");
        }

        protected override void LoadData() => Bind(BUSPhieuMuonTra.Instance.GetAllPhieuMuon());

        private void Bind(List<PHIEUMUONTRA> list)
        {
            grid.Rows.Clear();
            foreach (var p in list)
                grid.Rows.Add(p.SoPhieuMuonTra,
                    p.CUONSACH?.MaCuonSach,
                    p.CUONSACH?.SACH?.TUASACH?.TenTuaSach,
                    p.DOCGIA?.MaDocGia,
                    p.DOCGIA?.TenDocGia,
                    p.NgayMuon.ToShortDateString(),
                    p.HanTra.ToShortDateString(),
                    p.NgayTra == null ? "Chưa trả" : ((DateTime)p.NgayTra).ToShortDateString(),
                    p.SoTienPhat);
        }

        protected override void OnRowSelected(DataGridViewRow row)
        {
            if (row.Cells["sp"].Value == null) return;
            SelectedId = Convert.ToInt32(row.Cells["sp"].Value);
            SetText("macs", row.Cells["macs"].Value?.ToString());
            SetText("madg", row.Cells["madg"].Value?.ToString());
            SetText("ngaymuon", row.Cells["ngaymuon"].Value?.ToString());
            // Dong bo ngay tra tu grid -> form thong tin
            string nt = row.Cells["ngaytra"].Value?.ToString();
            var dp = Fields.Find(f => f.Key == "ngaytra")?.Input as DateTimePicker;
            if (string.IsNullOrEmpty(nt) || nt == "Chưa trả")
            {
                if (dp != null)
                {
                    dp.Format = DateTimePickerFormat.Custom;
                    dp.CustomFormat = " ";               // hien trong
                }
                SetFieldEnabled("ngaytra", false);
            }
            else
            {
                if (dp != null)
                {
                    dp.Format = DateTimePickerFormat.Short;
                }
                SetText("ngaytra", nt);
                SetFieldEnabled("ngaytra", false);
            }
        }

        protected override void DoSearch(int byIndex, string keyword)
        {
            var all = BUSPhieuMuonTra.Instance.GetAllPhieuMuon();
            keyword = (keyword ?? "").ToLower();
            var res = all.Where(p =>
            {
                switch (byIndex)
                {
                    case 0: return p.SoPhieuMuonTra.ToString().Contains(keyword);
                    case 1: return (p.DOCGIA?.MaDocGia ?? "").ToLower().Contains(keyword);
                    case 2: return (p.CUONSACH?.MaCuonSach ?? "").ToLower().Contains(keyword);
                    case 3: return p.NgayTra == null;
                    default: return true;
                }
            }).ToList();
            Bind(res);
        }

        // "Mượn" — lập phiếu mượn mới
        protected override void DoAdd()
        {
            string macs = GetText("macs");
            string madg = GetText("madg");
            if (string.IsNullOrWhiteSpace(macs) || string.IsNullOrWhiteSpace(madg))
            { Warn("Nhập Mã Sách và Mã Độc Giả."); return; }
            string err = BUSPhieuMuonTra.Instance.AddPhieuMuonTra(macs.Trim(), madg.Trim(), GetDate("ngaymuon"));
            if (string.IsNullOrEmpty(err)) Info("Lập phiếu mượn thành công."); else Warn(err);
        }

        // "Trả" — cập nhật ngày trả cho phiếu đang chọn
        protected override void DoEdit()
        {
            if (SelectedId == -1) { Warn("Chọn một phiếu để trả."); return; }
            string err = BUSPhieuMuonTra.Instance.UpdPhieuMuonTra(SelectedId, GetDate("ngaytra"));
            if (string.IsNullOrEmpty(err))
            {
                var pm = BUSPhieuMuonTra.Instance.GetPhieuMuonTra(SelectedId);
                if (pm != null)
                {
                    var sachId = pm.CUONSACH?.idSach;
                    if (sachId != null)
                    {
                        var pending = BUSDatTruoc.Instance.GetPendingForSach(sachId.Value);
                        if (pending.Count > 0)
                        {
                            var first = pending[0];
                            Info(string.Format("Trả sách thành công.\n\nCẢNH BÁO: Có {0} phiếu đặt trước đang chờ cho sách này!\nĐộc giả đầu tiên: {1} ({2})\n\nVào mục 'Quản lý đặt trước' để xử lý.",
                                pending.Count, first.TenDocGia, first.MaDocGia));
                            LoadData();
                            return;
                        }
                    }
                }
                Info("Trả sách thành công.");
            }
            else Warn(err);
        }

        protected override void DoDelete()
        {
            if (SelectedId == -1) { Warn("Chọn một phiếu để xóa."); return; }
            if (MessageBox.Show("Xóa phiếu mượn này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string err = BUSPhieuMuonTra.Instance.DelPhieuMuon(SelectedId);
            if (string.IsNullOrEmpty(err)) Info("Đã xóa."); else Warn(err);
        }

        private void Warn(string m) => MessageBox.Show(m, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void Info(string m) => MessageBox.Show(m, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ucPhieuMuonTraNew
            // 
            this.Name = "ucPhieuMuonTraNew";
            this.Size = new System.Drawing.Size(1258, 797);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}

