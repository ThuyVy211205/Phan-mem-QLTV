using DAL;
using DTO;
using System;
using System.Collections.Generic;

namespace BUS
{
    public class BUSQuanLyMuonTra
    {
        private static BUSQuanLyMuonTra instance;
        public static BUSQuanLyMuonTra Instance
        {
            get { if (instance == null) instance = new BUSQuanLyMuonTra(); return instance; }
            set => instance = value;
        }

        // ===== LỊCH SỬ MƯỢN - TRẢ =====

        public List<LichSuMuon> GetAllLichSu() => DALQuanLyMuonTra.Instance.GetAllLichSu();

        public List<LichSuMuon> FilterLichSu(string maDocGia, string trangThai, DateTime? tuNgay, DateTime? denNgay)
            => DALQuanLyMuonTra.Instance.FilterLichSu(maDocGia, trangThai, tuNgay, denNgay);

        public string EditPhieuMuon(int idNguoiDung, int soPhieu, DateTime ngayMuon, DateTime hanTra, DateTime? ngayTra, int tienPhat)
        {
            if (ngayMuon > DateTime.Now) return "Ngày mượn không thể trong tương lai.";
            if (!DALQuanLyMuonTra.Instance.EditPhieuMuon(soPhieu, ngayMuon, hanTra, ngayTra, tienPhat))
                return "Không thể cập nhật phiếu mượn.";
            DALQuanLyMuonTra.Instance.LogAudit(idNguoiDung, "SỬA", "PHIEUMUONTRA", soPhieu,
                $"Ngày mượn={ngayMuon:dd/MM/yyyy}, Hạn={hanTra:dd/MM/yyyy}, Trả={ngayTra?.ToString("dd/MM/yyyy") ?? "null"}, Phạt={tienPhat}");
            return "";
        }

        public string DelPhieuMuon(int idNguoiDung, int soPhieu)
        {
            var pm = DALPhieuMuonTra.Instance.GetPhieuMuonTraById(soPhieu);
            if (pm != null && pm.NgayTra == null)
                return "Không thể xóa phiếu đang mượn (chưa trả).";
            if (!DALQuanLyMuonTra.Instance.DelPhieuMuon(soPhieu))
                return "Không thể xóa phiếu mượn.";
            DALQuanLyMuonTra.Instance.LogAudit(idNguoiDung, "XÓA", "PHIEUMUONTRA", soPhieu, null);
            return "";
        }

        // ===== ĐÁNH DẤU SÁCH BỊ MẤT =====

        public string MarkBookAsLost(int soPhieu, string maCuonSach, int idNguoiDung)
        {
            var pm = DALPhieuMuonTra.Instance.GetPhieuMuonTraById(soPhieu);
            if (pm == null)
                return "Số phiếu mượn không hợp lệ.";

            if (pm.NgayTra != null)
                return "Sách đã được trả, không thể đánh dấu mất.";

            if (pm.CUONSACH == null || pm.CUONSACH.MaCuonSach != maCuonSach)
                return "Mã cuốn sách không khớp với phiếu mượn.";

            THAMSO ts = DALThamSo.Instance.GetAllThamSo();
            int donGia = pm.CUONSACH.SACH?.DonGia ?? 0;
            int heSo = ts.HeSoPhatMatSach;
            if (heSo <= 0) heSo = 3;

            int tienPhat = donGia * heSo;

            if (!DALQuanLyMuonTra.Instance.MarkBookAsLost(soPhieu, tienPhat))
                return "Sách đã bị đánh dấu mất hoặc không thể cập nhật.";

            DALDocGia.Instance.AddTongNoDocGia(pm.idDocGia, tienPhat);

            var cs = DALCuonSach.Instance.GetCuonSachById(pm.idCuonSach);
            if (cs != null && cs.SACH != null)
            {
                var sach = DALSach.Instance.GetSachById(cs.SACH.id);
                if (sach != null)
                {
                    sach.SoLuong--;
                    if (sach.SoLuongConLai > 0) sach.SoLuongConLai--;
                    QLTVDb.Instance.SaveChanges();
                }
            }

            DALQuanLyMuonTra.Instance.LogAudit(idNguoiDung, "MẤT SÁCH", "PHIEUMUONTRA", soPhieu,
                $"Cuốn sách {maCuonSach} bị mất. Phạt = {donGia} x {heSo} = {tienPhat}");

            return "";
        }

        // ===== NHU CẦU ĐỌC (Admin) =====

        public List<NhuCauDoc> GetAllNhuCau() => DALQuanLyMuonTra.Instance.GetAllNhuCau();

        public string MarkFulfilled(int idNguoiDung, int id)
        {
            if (!DALQuanLyMuonTra.Instance.UpdateTrangThai(id, "Đã đáp ứng"))
                return "Không thể cập nhật trạng thái.";
            DALQuanLyMuonTra.Instance.LogAudit(idNguoiDung, "ĐÁP ỨNG", "NHUCAUDOC", id, null);
            return "";
        }

        public string EditNhuCau(int idNguoiDung, int id, string maSach, string ghiChu)
        {
            if (string.IsNullOrWhiteSpace(maSach)) return "Mã sách không được để trống.";
            if (!DALQuanLyMuonTra.Instance.EditNhuCau(id, maSach, ghiChu))
                return "Không thể cập nhật nhu cầu đọc.";
            DALQuanLyMuonTra.Instance.LogAudit(idNguoiDung, "SỬA", "NHUCAUDOC", id, $"Mã sách={maSach}, Ghi chú={ghiChu}");
            return "";
        }

        public string DelNhuCau(int idNguoiDung, int id)
        {
            if (!DALNhuCauDoc.Instance.DelNhuCau(id))
                return "Không thể xóa nhu cầu đọc.";
            DALQuanLyMuonTra.Instance.LogAudit(idNguoiDung, "XÓA", "NHUCAUDOC", id, null);
            return "";
        }
    }
}
