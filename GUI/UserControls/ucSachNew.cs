using BUS;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GUI.UserControls
{
    /// <summary>
    /// Màn quản lý Sách theo mẫu chuẩn (đặc tả "Quản Lý Sách").
    /// </summary>
    public class ucSachNew : ucCrudBase
    {
        protected override string ScreenTitle => "Quản Lý Sách";
        protected override string SearchTitle => "Tìm kiếm Sách";
        protected override string[] SearchByOptions => new[] { "Mã Sách", "Tựa Sách", "Tên TG", "Tên TL" };

        protected override void BuildFields()
        {
            AddField(new CrudField { Key = "ma", Label = "Mã Sách :", ReadOnly = true });
            AddField(new CrudField { Key = "chude", Label = "Thể Loại :", IsCombo = true,
                ComboSource = () => BUSTheLoai.Instance.GetAllTheLoai().Select(tl => (object)tl.TenTheLoai).ToList() });
            AddField(new CrudField { Key = "ten", Label = "Tựa Sách :" });                 // cho phep sua
            AddField(new CrudField { Key = "tacgia", Label = "Tác Giả :" });               // cho phep sua
            AddField(new CrudField { Key = "dongia", Label = "Đơn Giá :" });
            AddField(new CrudField { Key = "nxb", Label = "NXB :" });
            AddField(new CrudField { Key = "slnhap", Label = "SL Nhập :" });               // cho phep sua
            AddField(new CrudField { Key = "tinhtrang", Label = "Tình Trạng :", IsCombo = true,
                ComboSource = () => new List<object> { "Còn", "Hết", "Chưa ẩn", "Đã ẩn" } });
            AddField(new CrudField { Key = "namxb", Label = "Năm XB :" });
            AddField(new CrudField { Key = "ghichu", Label = "Ghi Chú :" });
        }

        protected override void BuildGridColumns()
        {
            grid.Columns.Clear();
            grid.Columns.Add("id", "id"); grid.Columns["id"].Visible = false;
            grid.Columns.Add("ma", "Mã Sách");
            grid.Columns.Add("ten", "Tựa Sách");
            grid.Columns.Add("chude", "Thể Loại");
            grid.Columns.Add("tacgia", "Tác Giả");
            grid.Columns.Add("nxb", "NXB");
            grid.Columns.Add("namxb", "Năm XB");
            grid.Columns.Add("slnhap", "SL Nhập");
            grid.Columns.Add("conlai", "Còn Lại");
            grid.Columns.Add("daan", "Đã Ẩn");
        }

        protected override void LoadData() => Bind(BUSSach.Instance.GetAllSach());

        private void Bind(List<SACH> list)
        {
            grid.Rows.Clear();
            foreach (var s in list)
            {
                string tacgia = "";
                try { tacgia = string.Join(", ", s.TUASACH.TACGIAs
                    .Select(tg => tg.TenTacGia)); } catch { }
                grid.Rows.Add(s.id, s.MaSach, s.TUASACH?.TenTuaSach,
                    s.TUASACH?.THELOAI?.TenTheLoai, tacgia, s.NhaXB, s.NamXB,
                    s.SoLuong, s.SoLuongConLai, s.DaAn == 1 ? "Đã ẩn" : "");
            }
        }

        protected override void OnRowSelected(DataGridViewRow row)
        {
            if (row.Cells["id"].Value == null) return;
            SelectedId = Convert.ToInt32(row.Cells["id"].Value);
            SetText("ma", row.Cells["ma"].Value?.ToString());
            SetText("ten", row.Cells["ten"].Value?.ToString());
            SetText("chude", row.Cells["chude"].Value?.ToString());
            SetText("tacgia", row.Cells["tacgia"].Value?.ToString());
            SetText("nxb", row.Cells["nxb"].Value?.ToString());
            SetText("namxb", row.Cells["namxb"].Value?.ToString());
            SetText("slnhap", row.Cells["slnhap"].Value?.ToString());
        }

        protected override void DoSearch(int byIndex, string keyword)
        {
            var all = BUSSach.Instance.GetAllSach();
            keyword = (keyword ?? "").ToLower();
            if (string.IsNullOrEmpty(keyword)) { Bind(all); return; }
            var res = all.Where(s =>
            {
                switch (byIndex)
                {
                    case 0: return (s.MaSach ?? "").ToLower().Contains(keyword);
                    case 1: return (s.TUASACH?.TenTuaSach ?? "").ToLower().Contains(keyword);
                    case 2:
                        try { return s.TUASACH.TACGIAs.Any(tg =>
                            (tg.TenTacGia ?? "").ToLower().Contains(keyword)); }
                        catch { return false; }
                    case 3: return (s.TUASACH?.THELOAI?.TenTheLoai ?? "").ToLower().Contains(keyword);
                    default: return false;
                }
            }).ToList();
            Bind(res);
        }

        protected override void DoAdd()
        {
            string tenSach = GetText("ten");
            string chude = GetText("chude");
            string tacgiaText = GetText("tacgia");
            string nxb = GetText("nxb");

            if (string.IsNullOrWhiteSpace(tenSach) || string.IsNullOrWhiteSpace(chude))
            { Warn("Nhập Tựa Sách và Thể Loại."); return; }
            if (!int.TryParse(GetText("dongia"), out int dg) || dg <= 0) dg = 0;
            if (!int.TryParse(GetText("namxb"), out int nam) || nam <= 0) nam = DateTime.Now.Year;
            if (!int.TryParse(GetText("slnhap"), out int sl) || sl <= 0) sl = 1;

            // Tìm hoặc tạo Thể Loại
            var tlList = BUSTheLoai.Instance.GetAllTheLoai();
            THELOAI tl = tlList.FirstOrDefault(t => t.TenTheLoai == chude);
            if (tl == null) { Warn("Thể loại không tồn tại."); return; }

            // Tìm hoặc tạo Tựa Sách
            var tsList = BUSTuaSach.Instance.GetAllTuaSach();
            TUASACH ts = tsList.FirstOrDefault(t => t.TenTuaSach == tenSach && t.idTheLoai == tl.id);

            // Xử lý Tác Giả
            var tgList = new List<TACGIA>();
            if (!string.IsNullOrWhiteSpace(tacgiaText))
            {
                var tgNames = tacgiaText.Split(',').Select(x => x.Trim()).Where(x => x != "").ToList();
                foreach (var name in tgNames)
                {
                    var existing = BUSTacGia.Instance.FindTacGia(name);
                    if (existing != null && existing.Count > 0) tgList.Add(existing[0]);
                    else
                    {
                        int newId = BUSTacGia.Instance.AddTacGia(name);
                        if (newId != -1) tgList.Add(BUSTacGia.Instance.GetTacGia(newId));
                    }
                }
            }

            if (ts == null)
            {
                string err = BUSTuaSach.Instance.AddTuaSach(tenSach, tl, tgList);
                if (!string.IsNullOrEmpty(err)) { Warn(err); return; }
                ts = BUSTuaSach.Instance.GetAllTuaSach().FirstOrDefault(t => t.TenTuaSach == tenSach);
            }
            else if (tgList.Count > 0)
            {
                BUSTuaSach.Instance.UpdTuaSach(ts.id, null, null, tgList);
            }

            // Tạo Sách mới
            var sach = new SACH
            {
                idTuaSach = ts.id, TUASACH = ts,
                SoLuong = sl, SoLuongConLai = sl,
                DonGia = dg, NamXB = nam,
                NhaXB = string.IsNullOrWhiteSpace(nxb) ? "" : nxb, DaAn = 0
            };
            QLTVDb.Instance.SACHes.Add(sach);
            QLTVDb.Instance.SaveChanges();

            // Tạo Cuốn Sách
            for (int i = 0; i < sl; i++)
                DAL.DALCuonSach.Instance.AddCuonSach(sach, 1);

            Info("Thêm sách thành công.");
        }

        protected override void DoEdit()
        {
            if (SelectedId == -1) { Warn("Chọn một dòng."); return; }
            var sach = BUSSach.Instance.GetSach(SelectedId);
            if (sach == null) { Warn("Không tìm thấy sách."); return; }

            // 1) Cap nhat TuaSach (Ten Sach, Chu De) neu co thay doi
            string tenMoi = GetText("ten");
            string chudeMoi = GetText("chude");
            var tlList = BUSTheLoai.Instance.GetAllTheLoai();
            THELOAI tlMoi = tlList.FirstOrDefault(t => t.TenTheLoai == chudeMoi);

            if (!string.IsNullOrWhiteSpace(tenMoi) || tlMoi != null)
            {
                THELOAI tl = tlMoi ?? sach.TUASACH.THELOAI;
                BUSTuaSach.Instance.UpdTuaSach(sach.TUASACH.id, tenMoi, tl, null);
            }

            // 2) Cap nhat Tac Gia
            string tgText = GetText("tacgia");
            if (!string.IsNullOrWhiteSpace(tgText))
            {
                var tgNames = tgText.Split(',').Select(x => x.Trim()).Where(x => x != "").ToList();
                var tgList = new List<TACGIA>();
                foreach (var name in tgNames)
                {
                    var existing = BUSTacGia.Instance.FindTacGia(name);
                    if (existing != null && existing.Count > 0)
                        tgList.Add(existing[0]);
                    else
                    {
                        int newId = BUSTacGia.Instance.AddTacGia(name);
                        if (newId != -1) tgList.Add(BUSTacGia.Instance.GetTacGia(newId));
                    }
                }
                if (tgList.Count > 0)
                    BUSTuaSach.Instance.UpdTuaSach(sach.TUASACH.id, null, null, tgList);
            }

            // 3) Cap nhat SACH (Don gia, NXB, Nam XB, SL Nhap)
            if (!int.TryParse(GetText("dongia"), out int dg)) dg = -1;
            if (!int.TryParse(GetText("namxb"), out int nam)) nam = -1;
            if (!int.TryParse(GetText("slnhap"), out int sl)) sl = -1;
            string nxb = GetText("nxb");

            if (dg > 0 || nam > 0 || !string.IsNullOrWhiteSpace(nxb))
                BUSSach.Instance.UpdSach(SelectedId, nam > 0 ? (int?)nam : null,
                    string.IsNullOrWhiteSpace(nxb) ? null : nxb, dg > 0 ? (int?)dg : null);

            if (sl > 0)
            {
                sach.SoLuong = sl;
                if (sl > sach.SoLuongConLai) sach.SoLuongConLai = sl;
                QLTVDb.Instance.SaveChanges();
            }

            // 4) Xu ly an/hien
            string tt = GetText("tinhtrang");
            if (tt == "Đã ẩn") BUSSach.Instance.UpdAnSach(SelectedId, 1);
            else if (tt == "Chưa ẩn") BUSSach.Instance.UpdAnSach(SelectedId, 0);

            Info("Đã cập nhật sách.");
        }

        protected override void DoDelete()
        {
            if (SelectedId == -1) { Warn("Chọn một dòng để ẩn."); return; }
            if (MessageBox.Show("Ẩn sách này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string err = BUSSach.Instance.UpdAnSach(SelectedId, 1);
            if (string.IsNullOrEmpty(err)) Info("Đã ẩn."); else Warn(err);
        }

        private void Warn(string m) => MessageBox.Show(m, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void Info(string m) => MessageBox.Show(m, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ucSachNew
            // 
            this.Name = "ucSachNew";
            this.Size = new System.Drawing.Size(1258, 842);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
