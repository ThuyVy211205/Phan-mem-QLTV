using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DTO;

namespace DAL
{
    public class DALNhuCauDoc
    {
        private static DALNhuCauDoc instance;
        public static DALNhuCauDoc Instance
        {
            get
            {
                if (instance == null) instance = new DALNhuCauDoc();
                return instance;
            }
            set => instance = value;
        }

        private string connectionString
        {
            get
            {
                return QLTVDb.Instance.Database.Connection.ConnectionString;
            }
        }

        // ===== LỊCH SỬ MƯỢN (from View) =====

        public List<LichSuMuon> GetLichSuByDocGia(string maDocGia)
        {
            var list = new List<LichSuMuon>();
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(
                "SELECT SoPhieuMuonTra, MaDocGia, TenDocGia, MaCuonSach, TenSach, " +
                "NgayMuon, HanTra, NgayTra, TrangThai, SoTienPhat " +
                "FROM v_LichSuMuonTra WHERE MaDocGia = @MaDG ORDER BY NgayMuon DESC", cn))
            {
                cmd.Parameters.Add(new SqlParameter("@MaDG", maDocGia));
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new LichSuMuon
                        {
                            SoPhieuMuonTra = r.GetInt32(0),
                            MaDocGia       = r.GetString(1),
                            TenDocGia      = r.GetString(2),
                            MaCuonSach     = r.GetString(3),
                            TenSach        = r.GetString(4),
                            NgayMuon       = r.GetDateTime(5),
                            HanTra         = r.GetDateTime(6),
                            NgayTra        = r.IsDBNull(7) ? (DateTime?)null : r.GetDateTime(7),
                            TrangThai      = r.GetString(8),
                            SoTienPhat     = r.GetInt32(9)
                        });
                    }
                }
            }
            return list;
        }

        public List<LichSuMuon> GetAllLichSu()
        {
            var list = new List<LichSuMuon>();
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(
                "SELECT SoPhieuMuonTra, MaDocGia, TenDocGia, MaCuonSach, TenSach, " +
                "NgayMuon, HanTra, NgayTra, TrangThai, SoTienPhat " +
                "FROM v_LichSuMuonTra ORDER BY NgayMuon DESC", cn))
            {
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new LichSuMuon
                        {
                            SoPhieuMuonTra = r.GetInt32(0),
                            MaDocGia       = r.GetString(1),
                            TenDocGia      = r.GetString(2),
                            MaCuonSach     = r.GetString(3),
                            TenSach        = r.GetString(4),
                            NgayMuon       = r.GetDateTime(5),
                            HanTra         = r.GetDateTime(6),
                            NgayTra        = r.IsDBNull(7) ? (DateTime?)null : r.GetDateTime(7),
                            TrangThai      = r.GetString(8),
                            SoTienPhat     = r.GetInt32(9)
                        });
                    }
                }
            }
            return list;
        }

        // ===== NHU CẦU ĐỌC (Wishlist) =====

        public List<NhuCauDoc> GetNhuCauByDocGia(string maDocGia)
        {
            var list = new List<NhuCauDoc>();
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(
                "SELECT n.id, n.MaNhuCau, n.idDocGia, n.idSach, n.NgayThem, n.GhiChu, " +
                "dg.MaDocGia, dg.TenDocGia, s.MaSach, ts.TenTuaSach " +
                "FROM NHUCAUDOC n " +
                "JOIN DOCGIA dg  ON n.idDocGia = dg.ID " +
                "JOIN SACH s      ON n.idSach = s.id " +
                "JOIN TUASACH ts   ON s.idTuaSach = ts.id " +
                "WHERE dg.MaDocGia = @MaDG ORDER BY n.NgayThem DESC", cn))
            {
                cmd.Parameters.Add(new SqlParameter("@MaDG", maDocGia));
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new NhuCauDoc
                        {
                            id        = r.GetInt32(0),
                            MaNhuCau  = r.GetString(1),
                            idDocGia  = r.GetInt32(2),
                            idSach    = r.GetInt32(3),
                            NgayThem  = r.GetDateTime(4),
                            GhiChu    = r.IsDBNull(5) ? null : r.GetString(5),
                            MaDocGia  = r.GetString(6),
                            TenDocGia = r.GetString(7),
                            MaSach    = r.GetString(8),
                            TenSach   = r.GetString(9)
                        });
                    }
                }
            }
            return list;
        }

        public int AddNhuCau(string maDocGia, string maSach, string ghiChu)
        {
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(
                "INSERT INTO NHUCAUDOC (idDocGia, idSach, GhiChu) " +
                "SELECT dg.ID, s.id, @GhiChu FROM DOCGIA dg, SACH s " +
                "WHERE dg.MaDocGia = @MaDG AND s.MaSach = @MaSach; " +
                "SELECT SCOPE_IDENTITY();", cn))
            {
                cmd.Parameters.Add(new SqlParameter("@MaDG",   maDocGia));
                cmd.Parameters.Add(new SqlParameter("@MaSach", maSach));
                cmd.Parameters.Add(new SqlParameter("@GhiChu", (object)ghiChu ?? DBNull.Value));
                cn.Open();
                object result = cmd.ExecuteScalar();
                return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : -1;
            }
        }

        public bool DelNhuCau(int id)
        {
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("DELETE FROM NHUCAUDOC WHERE id = @Id", cn))
            {
                cmd.Parameters.Add(new SqlParameter("@Id", id));
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
