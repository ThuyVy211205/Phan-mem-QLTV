using System;

namespace DTO
{
    public class DATTRUOC
    {
        public int id { get; set; }
        public int idDocGia { get; set; }
        public int idSach { get; set; }
        public DateTime NgayDat { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }

        public virtual DOCGIA DOCGIA { get; set; }
        public virtual SACH SACH { get; set; }
    }

    public class DatTruocDisplay
    {
        public int id { get; set; }
        public string MaDocGia { get; set; }
        public string TenDocGia { get; set; }
        public string MaSach { get; set; }
        public string TenSach { get; set; }
        public string TenTuaSach { get; set; }
        public int SoLuongConLai { get; set; }
        public DateTime NgayDat { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }
    }
}
