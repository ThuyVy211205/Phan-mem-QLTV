using System;

namespace DTO
{
    public class NhuCauDoc
    {
        public int id { get; set; }
        public string MaNhuCau { get; set; }
        public int idDocGia { get; set; }
        public int idSach { get; set; }
        public DateTime NgayThem { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; }

        // Display-only (join)
        public string MaDocGia { get; set; }
        public string TenDocGia { get; set; }
        public string MaSach { get; set; }
        public string TenSach { get; set; }
        public string TenTacGia { get; set; }
    }

    public class LichSuMuon
    {
        public int SoPhieuMuonTra { get; set; }
        public string MaDocGia { get; set; }
        public string TenDocGia { get; set; }
        public string MaCuonSach { get; set; }
        public string TenSach { get; set; }
        public DateTime NgayMuon { get; set; }
        public DateTime HanTra { get; set; }
        public DateTime? NgayTra { get; set; }
        public string TrangThai { get; set; }
        public int SoTienPhat { get; set; }
        public int DonGia { get; set; }
        public int DaMat { get; set; }
    }

    public class AuditLog
    {
        public int id { get; set; }
        public int idNguoiDung { get; set; }
        public string TenNguoiDung { get; set; }
        public string HanhDong { get; set; }
        public string Bang { get; set; }
        public int idBanGhi { get; set; }
        public string ChiTiet { get; set; }
        public DateTime ThoiGian { get; set; }
    }
}
