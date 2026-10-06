using DAL;
using DTO;
using System;
using System.Collections.Generic;

namespace BUS
{
    public class BUSDatTruoc
    {
        private static BUSDatTruoc instance;
        public static BUSDatTruoc Instance
        {
            get { if (instance == null) instance = new BUSDatTruoc(); return instance; }
            set => instance = value;
        }

        public List<DatTruocDisplay> GetAll()
        {
            return DALDatTruoc.Instance.GetAll();
        }

        public string AddDatTruoc(string maDocGia, string maSach, string ghiChu, int idNguoiDung)
        {
            var dg = DALDocGia.Instance.GetDocGiaByMa(maDocGia);
            if (dg == null) return "Mã độc giả không tồn tại.";

            if (dg.NgayHetHan < DateTime.Now)
                return "Thẻ độc giả đã hết hạn.";

            var sach = DALSach.Instance.GetSachByMa(maSach);
            if (sach == null) return "Mã sách không tồn tại.";

            if (sach.SoLuongConLai > 0)
                return "Sách vẫn còn trong kho, không cần đặt trước. Vui lòng tạo phiếu mượn.";

            var existing = DALDatTruoc.Instance.GetAll()
                .FindAll(d => d.MaDocGia == maDocGia && d.MaSach == maSach && d.TrangThai == "Đang chờ");
            if (existing.Count > 0)
                return "Độc giả này đã đặt trước cuốn sách này rồi.";

            if (!DALDatTruoc.Instance.Add(dg.ID, sach.id, ghiChu))
                return "Không thể tạo phiếu đặt trước.";

            DALQuanLyMuonTra.Instance.LogAudit(idNguoiDung, "ĐẶT TRƯỚC", "DATTRUOC", 0,
                $"Độc giả {maDocGia} đặt trước sách {maSach}");

            return "";
        }

        public string MarkFulfilled(int id, int idNguoiDung)
        {
            if (!DALDatTruoc.Instance.UpdateTrangThai(id, "Đã đáp ứng"))
                return "Không thể cập nhật trạng thái.";

            DALQuanLyMuonTra.Instance.LogAudit(idNguoiDung, "ĐÁP ỨNG", "DATTRUOC", id, null);
            return "";
        }

        public string HuyDatTruoc(int id, int idNguoiDung)
        {
            if (!DALDatTruoc.Instance.UpdateTrangThai(id, "Đã huỷ"))
                return "Không thể huỷ đặt trước.";

            DALQuanLyMuonTra.Instance.LogAudit(idNguoiDung, "HUỶ", "DATTRUOC", id, null);
            return "";
        }

        public string Delete(int id, int idNguoiDung)
        {
            if (!DALDatTruoc.Instance.Delete(id))
                return "Không thể xoá.";

            DALQuanLyMuonTra.Instance.LogAudit(idNguoiDung, "XOÁ", "DATTRUOC", id, null);
            return "";
        }

        public string FulfillAndBorrow(int idDatTruoc, string maCuonSach, DateTime ngayMuon, int idNguoiDung)
        {
            var dt = DALDatTruoc.Instance.GetById(idDatTruoc);
            if (dt == null) return "Phiếu đặt trước không tồn tại.";
            if (dt.TrangThai != "Đang chờ") return "Phiếu đặt trước không ở trạng thái chờ.";

            var err = BUSPhieuMuonTra.Instance.AddPhieuMuonTra(maCuonSach, dt.MaDocGia, ngayMuon);
            if (!string.IsNullOrEmpty(err)) return err;

            DALDatTruoc.Instance.UpdateTrangThai(idDatTruoc, "Đã đáp ứng");
            DALQuanLyMuonTra.Instance.LogAudit(idNguoiDung, "ĐÁP ỨNG+MƯỢN", "DATTRUOC", idDatTruoc,
                $"Cuốn {maCuonSach} cho độc giả {dt.MaDocGia}");
            return "";
        }

        public List<DatTruocDisplay> GetPendingForSach(int idSach)
        {
            return DALDatTruoc.Instance.GetPendingBySachId(idSach);
        }

        public int GetPendingCount()
        {
            return DALDatTruoc.Instance.GetAll().FindAll(d => d.TrangThai == "Đang chờ").Count;
        }
    }
}
