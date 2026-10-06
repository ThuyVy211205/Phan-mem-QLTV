using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BUS
{
    public class BUSNotification
    {
        private static BUSNotification instance;
        public static BUSNotification Instance
        {
            get { if (instance == null) instance = new BUSNotification(); return instance; }
            set => instance = value;
        }

        private string GetConnStr()
        {
            return DTO.QLTVDb.Instance.Database.Connection.ConnectionString;
        }

        public List<OverdueBook> GetOverdueBooks()
        {
            var list = new List<OverdueBook>();
            try
            {
                using (var cn = new SqlConnection(GetConnStr()))
                {
                    cn.Open();
                    var sql = @"SELECT pmt.SoPhieuMuonTra, dg.MaDocGia, dg.TenDocGia, cs.MaCuonSach, ts.TenTuaSach,
                               pmt.NgayMuon, pmt.HanTra
                        FROM PHIEUMUONTRA pmt
                        JOIN CUONSACH cs ON pmt.idCuonSach = cs.id
                        JOIN SACH s ON cs.idSach = s.id
                        JOIN TUASACH ts ON s.idTuaSach = ts.id
                        JOIN DOCGIA dg ON pmt.idDocGia = dg.ID
                        WHERE pmt.NgayTra IS NULL AND pmt.DaMat = 0 AND pmt.HanTra < GETDATE()
                        ORDER BY pmt.HanTra";
                    using (var cmd = new SqlCommand(sql, cn))
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new OverdueBook
                            {
                                SoPhieu = r.GetInt32(0), MaDocGia = r.GetString(1), TenDocGia = r.GetString(2),
                                MaCuonSach = r.GetString(3), TenSach = r.GetString(4),
                                NgayMuon = r.GetDateTime(5), HanTra = r.GetDateTime(6),
                                SoNgayTre = (DateTime.Now - r.GetDateTime(6)).Days
                            });
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        public List<OverdueBook> GetOverdueByUser(int userId)
        {
            var list = new List<OverdueBook>();
            try
            {
                using (var cn = new SqlConnection(GetConnStr()))
                {
                    cn.Open();
                    var sql = @"SELECT pmt.SoPhieuMuonTra, dg.MaDocGia, dg.TenDocGia, cs.MaCuonSach, ts.TenTuaSach,
                               pmt.NgayMuon, pmt.HanTra
                        FROM PHIEUMUONTRA pmt
                        JOIN CUONSACH cs ON pmt.idCuonSach = cs.id
                        JOIN SACH s ON cs.idSach = s.id
                        JOIN TUASACH ts ON s.idTuaSach = ts.id
                        JOIN DOCGIA dg ON pmt.idDocGia = dg.ID
                        WHERE pmt.NgayTra IS NULL AND pmt.DaMat = 0 AND pmt.HanTra < GETDATE() AND dg.idNguoiDung = @uid
                        ORDER BY pmt.HanTra";
                    using (var cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@uid", userId);
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                list.Add(new OverdueBook
                                {
                                    SoPhieu = r.GetInt32(0), MaDocGia = r.GetString(1), TenDocGia = r.GetString(2),
                                    MaCuonSach = r.GetString(3), TenSach = r.GetString(4),
                                    NgayMuon = r.GetDateTime(5), HanTra = r.GetDateTime(6),
                                    SoNgayTre = (DateTime.Now - r.GetDateTime(6)).Days
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return list;
        }
    }

    public class OverdueBook
    {
        public int SoPhieu { get; set; }
        public string MaDocGia { get; set; }
        public string TenDocGia { get; set; }
        public string TenSach { get; set; }
        public string MaCuonSach { get; set; }
        public DateTime NgayMuon { get; set; }
        public DateTime HanTra { get; set; }
        public int SoNgayTre { get; set; }
    }

    public class DatTruocNotify
    {
        public string MaDocGia { get; set; }
        public string TenDocGia { get; set; }
        public string TenSach { get; set; }
        public string MaSach { get; set; }
        public DateTime NgayDat { get; set; }
    }
}
