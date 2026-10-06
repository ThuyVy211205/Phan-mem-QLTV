using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DALDocGia
    {
        private static DALDocGia instance;

        public static DALDocGia Instance 
        { 
            get
            {
                if (instance == null) instance = new DALDocGia();
                return instance;
            }
            set { instance = value; }
        }

        public bool AddDocGia(string tenDocGia, DateTime ngaySinh, string diaChi, string email,
            DateTime ngayLapThe, DateTime ngayHetHan, int idLoaiDocGia, int tongNoHienTai, int idND)
        {
            try
            {
                var obj = new DOCGIA();
                obj.TenDocGia = tenDocGia;
                obj.NgaySinh = ngaySinh;
                obj.DiaChi = diaChi;
                obj.Email = email;
                obj.NgayLapThe = ngayLapThe;
                obj.NgayHetHan = ngayHetHan;
                obj.idLoaiDocGia = idLoaiDocGia;
                obj.TongNoHienTai = tongNoHienTai;
                obj.idNguoiDung = idND;
                obj.NGUOIDUNG = DALNguoiDung.Instance.GetNguoiDungById(idND);
                QLTVDb.Instance.DOCGIAs.Add(obj);
                QLTVDb.Instance.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
                return false;
            }
        }

        public DOCGIA GetDocGiaById(int idDocGia)
        {
            return QLTVDb.Instance.DOCGIAs.Find(idDocGia);
        }

        public DOCGIA GetDocGiaByMa(string maDocGia)
        {
            var res = QLTVDb.Instance.DOCGIAs.AsNoTracking().Where(d => d.MaDocGia == maDocGia);
            if (res.Any())
                return res.FirstOrDefault();
            return null;
        }

        public List<DOCGIA> GetAllDocGia()
        {
            return QLTVDb.Instance.DOCGIAs.AsNoTracking().ToList();
        }

        public List<DOCGIA> FindDocGia(string tenDocGia, string email, int? idLoaiDocGia)
        {
            var res = QLTVDb.Instance.DOCGIAs.ToList();
            if (tenDocGia != null) res = res.Where(d => d.TenDocGia == tenDocGia).Select(d => d).ToList();
            if (email != null) res = res.Where(d => d.Email == email).Select(d => d).ToList();
            if (idLoaiDocGia != null) res = res.Where(d => d.idLoaiDocGia == idLoaiDocGia).Select(d => d).ToList();
            return res;
        }

        public DOCGIA FindDocGiaByIdND(int idNguoiDung)
        {
            return QLTVDb.Instance.DOCGIAs.AsNoTracking().Where(d => d.idNguoiDung == idNguoiDung).First();
        }

        public bool UpdDocGia(int idDocGia, string tenDocGia, DateTime? ngaySinh, string diaChi, string email,
            DateTime? ngayHetHan, int? idLoaiDocGia)
        {
            try
            {
                DOCGIA dg = GetDocGiaById(idDocGia);
                if (dg == null) return false;
                if (tenDocGia != null) dg.TenDocGia = tenDocGia;
                if (ngaySinh != null) dg.NgaySinh = (DateTime)ngaySinh;
                if (diaChi != null) dg.DiaChi = diaChi;
                if (email != null) dg.Email = email;
                if (ngayHetHan != null) dg.NgayHetHan = (DateTime)ngayHetHan;
                if (idLoaiDocGia != null) dg.idLoaiDocGia = (int)idLoaiDocGia;
                QLTVDb.Instance.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        public bool UpdTongNoDocGia(int idDocGia, int tongNoMoi)
        {
            try
            {
                DOCGIA dg = GetDocGiaById(idDocGia);
                if (dg == null) return false;
                dg.TongNoHienTai = tongNoMoi;
                QLTVDb.Instance.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        public bool AddTongNoDocGia(int idDocGia, int soTienThem)
        {
            try
            {
                QLTVDb.Instance.Database.ExecuteSqlCommand(
                    "UPDATE DOCGIA SET TongNoHienTai = TongNoHienTai + @p0 WHERE ID = @p1",
                    soTienThem, idDocGia);
                return true;
            }
            catch { return false; }
        }

        public bool DelDocGia(int idDocGia)
        {
            try
            {
                DOCGIA dg = GetDocGiaById(idDocGia);
                if (dg == null) return false;
                QLTVDb.Instance.DOCGIAs.Remove(dg);
                QLTVDb.Instance.SaveChanges();
                return true;
            }
            catch { return false; }
        }

        public int GetBorrowedCount(string maDocGia)
        {
            try
            {
                var dg = GetDocGiaByMa(maDocGia);
                if (dg == null) return 0;
                return QLTVDb.Instance.PHIEUMUONTRAs
                    .Count(p => p.idDocGia == dg.ID && p.NgayTra == null);
            }
            catch { return 0; }
        }

        public int GetViolationCount(string maDocGia)
        {
            try
            {
                var dg = GetDocGiaByMa(maDocGia);
                if (dg == null) return 0;
                int viPham = QLTVDb.Instance.PHIEUMUONTRAs
                    .Count(p => p.idDocGia == dg.ID && ((p.NgayTra != null && p.NgayTra > p.HanTra) || p.SoTienPhat > 0));
                return viPham;
            }
            catch { return 0; }
        }

        public void RecalcTongNo(string maDocGia)
        {
            try
            {
                var dg = GetDocGiaByMa(maDocGia);
                if (dg == null) return;
                int tongPhat = QLTVDb.Instance.PHIEUMUONTRAs
                    .Where(p => p.idDocGia == dg.ID).Sum(p => (int?)p.SoTienPhat) ?? 0;
                int tongThu = QLTVDb.Instance.PHIEUTHUs
                    .Where(p => p.idDocGia == dg.ID).Sum(p => (int?)p.SoTienThu) ?? 0;
                dg = GetDocGiaById(dg.ID);
                dg.TongNoHienTai = tongPhat - tongThu;
                QLTVDb.Instance.SaveChanges();
            }
            catch { }
        }

        public void RecalcAllTongNo()
        {
            try
            {
                var all = GetAllDocGia();
                foreach (var dg in all) RecalcTongNo(dg.MaDocGia);
            }
            catch { }
        }
    }
}
