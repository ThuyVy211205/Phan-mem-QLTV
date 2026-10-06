using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DTO;

namespace DAL
{
    public class DALQuanLyMuonTra
    {
        private static DALQuanLyMuonTra instance;
        public static DALQuanLyMuonTra Instance
        {
            get { if (instance == null) instance = new DALQuanLyMuonTra(); return instance; }
            set { instance = value; }
        }

        private string GetConnectionString()
        {
            return QLTVDb.Instance.Database.Connection.ConnectionString;
        }

        public List<LichSuMuon> GetAllLichSu()
        {
            List<LichSuMuon> list = new List<LichSuMuon>();
            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;
            try
            {
                cn = new SqlConnection(GetConnectionString());
                cn.Open();
                cmd = new SqlCommand("SELECT SoPhieuMuonTra, MaDocGia, TenDocGia, MaCuonSach, TenSach, NgayMuon, HanTra, NgayTra, TrangThai, SoTienPhat, DonGia, DaMat FROM v_LichSuMuonTra ORDER BY NgayMuon DESC", cn);
                r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(new LichSuMuon
                    {
                        SoPhieuMuonTra = r.IsDBNull(0) ? 0 : r.GetInt32(0),
                        MaDocGia = r.IsDBNull(1) ? "" : r.GetString(1),
                        TenDocGia = r.IsDBNull(2) ? "" : r.GetString(2),
                        MaCuonSach = r.IsDBNull(3) ? "" : r.GetString(3),
                        TenSach = r.IsDBNull(4) ? "" : r.GetString(4),
                        NgayMuon = r.IsDBNull(5) ? DateTime.MinValue : r.GetDateTime(5),
                        HanTra = r.IsDBNull(6) ? DateTime.MinValue : r.GetDateTime(6),
                        NgayTra = r.IsDBNull(7) ? (DateTime?)null : r.GetDateTime(7),
                        TrangThai = r.GetString(8),
                        SoTienPhat = r.GetInt32(9),
                        DonGia = r.IsDBNull(10) ? 0 : r.GetInt32(10),
                        DaMat = r.IsDBNull(11) ? 0 : r.GetInt32(11)
                    });
                }
            }
            finally
            {
                if (r != null) { r.Close(); r.Dispose(); }
                if (cmd != null) cmd.Dispose();
                if (cn != null) { cn.Close(); cn.Dispose(); }
            }
            return list;
        }

        public List<LichSuMuon> FilterLichSu(string maDocGia, string trangThai, DateTime? tuNgay, DateTime? denNgay)
        {
            List<LichSuMuon> list = new List<LichSuMuon>();
            string sql = "SELECT SoPhieuMuonTra, MaDocGia, TenDocGia, MaCuonSach, TenSach, NgayMuon, HanTra, NgayTra, TrangThai, SoTienPhat, DonGia, DaMat FROM v_LichSuMuonTra WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(maDocGia)) sql += " AND MaDocGia = @MaDG";
            if (!string.IsNullOrWhiteSpace(trangThai)) sql += " AND TrangThai = @TT";
            if (tuNgay.HasValue) sql += " AND NgayMuon >= @Tu";
            if (denNgay.HasValue) sql += " AND NgayMuon <= @Den";
            sql += " ORDER BY NgayMuon DESC";

            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;
            try
            {
                cn = new SqlConnection(GetConnectionString());
                cn.Open();
                cmd = new SqlCommand(sql, cn);
                if (!string.IsNullOrWhiteSpace(maDocGia)) cmd.Parameters.AddWithValue("@MaDG", maDocGia.Trim());
                if (!string.IsNullOrWhiteSpace(trangThai)) cmd.Parameters.AddWithValue("@TT", trangThai);
                if (tuNgay.HasValue) cmd.Parameters.AddWithValue("@Tu", tuNgay.Value);
                if (denNgay.HasValue) cmd.Parameters.AddWithValue("@Den", denNgay.Value);
                r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(new LichSuMuon
                    {
                        SoPhieuMuonTra = r.IsDBNull(0) ? 0 : r.GetInt32(0), MaDocGia = r.IsDBNull(1) ? "" : r.GetString(1), TenDocGia = r.IsDBNull(2) ? "" : r.GetString(2),
                        MaCuonSach = r.IsDBNull(3) ? "" : r.GetString(3), TenSach = r.IsDBNull(4) ? "" : r.GetString(4), NgayMuon = r.IsDBNull(5) ? DateTime.MinValue : r.GetDateTime(5),
                        HanTra = r.IsDBNull(6) ? DateTime.MinValue : r.GetDateTime(6), NgayTra = r.IsDBNull(7) ? (DateTime?)null : r.GetDateTime(7),
                        TrangThai = r.IsDBNull(8) ? "" : r.GetString(8), SoTienPhat = r.IsDBNull(9) ? 0 : r.GetInt32(9),
                        DonGia = r.IsDBNull(10) ? 0 : r.GetInt32(10), DaMat = r.IsDBNull(11) ? 0 : r.GetInt32(11)
                    });
                }
            }
            finally
            {
                if (r != null) { r.Close(); r.Dispose(); }
                if (cmd != null) cmd.Dispose();
                if (cn != null) { cn.Close(); cn.Dispose(); }
            }
            return list;
        }

        public bool EditPhieuMuon(int soPhieu, DateTime ngayMuon, DateTime hanTra, DateTime? ngayTra, int tienPhat)
        {
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(GetConnectionString());
                cn.Open();
                cmd = new SqlCommand(
                    "UPDATE PHIEUMUONTRA SET NgayMuon=@NM, HanTra=@HT, NgayTra=@NT, SoTienPhat=@SP WHERE SoPhieuMuonTra=@SPT", cn);
                cmd.Parameters.AddWithValue("@NM", ngayMuon);
                cmd.Parameters.AddWithValue("@HT", hanTra);
                cmd.Parameters.AddWithValue("@NT", (object)ngayTra ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SP", tienPhat);
                cmd.Parameters.AddWithValue("@SPT", soPhieu);
                int rows = cmd.ExecuteNonQuery();
                if (rows > 0 && ngayTra != null)
                {
                    cmd.Dispose();
                    cmd = new SqlCommand("SELECT idCuonSach FROM PHIEUMUONTRA WHERE SoPhieuMuonTra=@SPT2", cn);
                    cmd.Parameters.AddWithValue("@SPT2", soPhieu);
                    var idCS = cmd.ExecuteScalar();
                    if (idCS != null && idCS != DBNull.Value)
                    {
                        cmd.Dispose();
                        cmd = new SqlCommand("UPDATE CUONSACH SET TinhTrang=1 WHERE id=@IdCS", cn);
                        cmd.Parameters.AddWithValue("@IdCS", (int)idCS);
                        cmd.ExecuteNonQuery();
                    }
                }
                return rows > 0;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (cn != null) { cn.Close(); cn.Dispose(); }
            }
        }

        public bool DelPhieuMuon(int soPhieu)
        {
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(GetConnectionString());
                cn.Open();
                cmd = new SqlCommand("DELETE FROM PHIEUMUONTRA WHERE SoPhieuMuonTra=@SPT", cn);
                cmd.Parameters.AddWithValue("@SPT", soPhieu);
                return cmd.ExecuteNonQuery() > 0;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (cn != null) { cn.Close(); cn.Dispose(); }
            }
        }

        public List<NhuCauDoc> GetAllNhuCau()
        {
            List<NhuCauDoc> list = new List<NhuCauDoc>();
            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;
            try
            {
                cn = new SqlConnection(GetConnectionString());
                cn.Open();
                cmd = new SqlCommand(
                    "SELECT n.id, n.MaNhuCau, n.idDocGia, n.idSach, n.NgayThem, n.GhiChu, n.TrangThai, " +
                    "dg.MaDocGia, dg.TenDocGia, s.MaSach, ts.TenTuaSach, " +
                    "STUFF((SELECT ', '+t.TenTacGia FROM CT_TACGIA ct JOIN TACGIA t ON ct.idTacGia=t.id WHERE ct.idTuaSach=ts.id FOR XML PATH('')),1,2,'') AS TacGia " +
                    "FROM NHUCAUDOC n JOIN DOCGIA dg ON n.idDocGia=dg.ID JOIN SACH s ON n.idSach=s.id JOIN TUASACH ts ON s.idTuaSach=ts.id ORDER BY n.NgayThem DESC", cn);
                r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(new NhuCauDoc
                    {
                        id = r.GetInt32(0), MaNhuCau = r.GetString(1), idDocGia = r.GetInt32(2),
                        idSach = r.GetInt32(3), NgayThem = r.GetDateTime(4),
                        GhiChu = r.IsDBNull(5) ? null : r.GetString(5),
                        TrangThai = r.IsDBNull(6) ? "" : r.GetString(6),
                        MaDocGia = r.GetString(7), TenDocGia = r.GetString(8),
                        MaSach = r.GetString(9), TenSach = r.GetString(10),
                        TenTacGia = r.IsDBNull(11) ? "" : r.GetString(11)
                    });
                }
            }
            finally
            {
                if (r != null) { r.Close(); r.Dispose(); }
                if (cmd != null) cmd.Dispose();
                if (cn != null) { cn.Close(); cn.Dispose(); }
            }
            return list;
        }

        public bool UpdateTrangThai(int id, string trangThai)
        {
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(GetConnectionString());
                cn.Open();
                cmd = new SqlCommand("UPDATE NHUCAUDOC SET TrangThai=@TT WHERE id=@Id", cn);
                cmd.Parameters.AddWithValue("@TT", trangThai);
                cmd.Parameters.AddWithValue("@Id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (cn != null) { cn.Close(); cn.Dispose(); }
            }
        }

        public bool EditNhuCau(int id, string maSach, string ghiChu)
        {
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(GetConnectionString());
                cn.Open();
                cmd = new SqlCommand("UPDATE n SET n.idSach=s.id, n.GhiChu=@GC FROM NHUCAUDOC n, SACH s WHERE n.id=@Id AND s.MaSach=@MaSach", cn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@MaSach", maSach.Trim());
                cmd.Parameters.AddWithValue("@GC", (object)ghiChu ?? DBNull.Value);
                return cmd.ExecuteNonQuery() > 0;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (cn != null) { cn.Close(); cn.Dispose(); }
            }
        }

        public bool MarkBookAsLost(int soPhieu, int tienPhat)
        {
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(GetConnectionString());
                cn.Open();

                cmd = new SqlCommand("SELECT DaMat FROM PHIEUMUONTRA WHERE SoPhieuMuonTra=@SPT", cn);
                cmd.Parameters.AddWithValue("@SPT", soPhieu);
                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value || Convert.ToInt32(result) == 1)
                    return false;

                cmd.Dispose();
                cmd = new SqlCommand("UPDATE PHIEUMUONTRA SET DaMat=1, SoTienPhat=@SP WHERE SoPhieuMuonTra=@SPT2", cn);
                cmd.Parameters.AddWithValue("@SP", tienPhat);
                cmd.Parameters.AddWithValue("@SPT2", soPhieu);
                int affected = cmd.ExecuteNonQuery();

                if (affected > 0)
                {
                    cmd.Dispose();
                    cmd = new SqlCommand(@"
                        DECLARE @idCuonSach INT;
                        SELECT @idCuonSach = idCuonSach FROM PHIEUMUONTRA WHERE SoPhieuMuonTra = @SPT3;
                        UPDATE CUONSACH SET TinhTrang = 2, DaAn = 1 WHERE id = @idCuonSach;
                        UPDATE SACH SET SoLuongConLai = SoLuongConLai - 1 WHERE id = (SELECT idSach FROM CUONSACH WHERE id = @idCuonSach)
                            AND SoLuongConLai > 0;", cn);
                    cmd.Parameters.AddWithValue("@SPT3", soPhieu);
                    cmd.ExecuteNonQuery();
                }

                return affected > 0;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (cn != null) { cn.Close(); cn.Dispose(); }
            }
        }

        public void LogAudit(int idNguoiDung, string hanhDong, string bang, int idBanGhi, string chiTiet)
        {
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(GetConnectionString());
                cn.Open();
                cmd = new SqlCommand("sp_AuditLog", cn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idNguoiDung", idNguoiDung);
                cmd.Parameters.AddWithValue("@HanhDong", hanhDong);
                cmd.Parameters.AddWithValue("@Bang", bang);
                cmd.Parameters.AddWithValue("@idBanGhi", idBanGhi);
                cmd.Parameters.AddWithValue("@ChiTiet", (object)chiTiet ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (cn != null) { cn.Close(); cn.Dispose(); }
            }
        }
    }
}
