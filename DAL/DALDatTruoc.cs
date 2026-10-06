using DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DAL
{
    public class DALDatTruoc
    {
        private static DALDatTruoc instance;
        public static DALDatTruoc Instance
        {
            get { if (instance == null) instance = new DALDatTruoc(); return instance; }
            set => instance = value;
        }

        private string GetConnectionString()
        {
            return QLTVDb.Instance.Database.Connection.ConnectionString;
        }

        public List<DatTruocDisplay> GetAll()
        {
            var list = new List<DatTruocDisplay>();
            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;
            try
            {
                cn = new SqlConnection(GetConnectionString());
                cn.Open();
                cmd = new SqlCommand(
                    "SELECT dt.id, dg.MaDocGia, dg.TenDocGia, s.MaSach, ts.TenTuaSach, s.SoLuongConLai, " +
                    "dt.NgayDat, dt.TrangThai, dt.GhiChu " +
                    "FROM DATTRUOC dt " +
                    "JOIN DOCGIA dg ON dt.idDocGia = dg.ID " +
                    "JOIN SACH s ON dt.idSach = s.id " +
                    "JOIN TUASACH ts ON s.idTuaSach = ts.id " +
                    "ORDER BY dt.TrangThai ASC, dt.NgayDat ASC", cn);
                r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(new DatTruocDisplay
                    {
                        id = r.GetInt32(0),
                        MaDocGia = r.GetString(1),
                        TenDocGia = r.GetString(2),
                        MaSach = r.GetString(3),
                        TenTuaSach = r.GetString(4),
                        SoLuongConLai = r.GetInt32(5),
                        NgayDat = r.GetDateTime(6),
                        TrangThai = r.GetString(7),
                        GhiChu = r.IsDBNull(8) ? "" : r.GetString(8)
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

        public bool Add(int idDocGia, int idSach, string ghiChu)
        {
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(GetConnectionString());
                cn.Open();
                cmd = new SqlCommand(
                    "INSERT INTO DATTRUOC(idDocGia, idSach, GhiChu) VALUES (@DG, @S, @GC)", cn);
                cmd.Parameters.AddWithValue("@DG", idDocGia);
                cmd.Parameters.AddWithValue("@S", idSach);
                cmd.Parameters.AddWithValue("@GC", (object)ghiChu ?? DBNull.Value);
                return cmd.ExecuteNonQuery() > 0;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (cn != null) { cn.Close(); cn.Dispose(); }
            }
        }

        public bool UpdateTrangThai(int id, string trangThai)
        {
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(GetConnectionString());
                cn.Open();
                cmd = new SqlCommand("UPDATE DATTRUOC SET TrangThai=@TT WHERE id=@Id", cn);
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

        public bool Delete(int id)
        {
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(GetConnectionString());
                cn.Open();
                cmd = new SqlCommand("DELETE FROM DATTRUOC WHERE id=@Id", cn);
                cmd.Parameters.AddWithValue("@Id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (cn != null) { cn.Close(); cn.Dispose(); }
            }
        }

        public DatTruocDisplay GetById(int id)
        {
            var all = GetAll();
            return all.FirstOrDefault(d => d.id == id);
        }

        public List<DatTruocDisplay> GetPendingBySachId(int idSach)
        {
            var all = GetAll();
            return all.Where(d => d.MaSach == DALSach.Instance.GetSachById(idSach)?.MaSach
                              && d.TrangThai == "Đang chờ")
                      .OrderBy(d => d.NgayDat).ToList();
        }
    }
}
