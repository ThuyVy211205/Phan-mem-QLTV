using DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL
{
    public class DALNhaXuatBan
    {
        private static DALNhaXuatBan instance;
        public static DALNhaXuatBan Instance
        {
            get { if (instance == null) instance = new DALNhaXuatBan(); return instance; }
            set => instance = value;
        }

        private string GetConnectionString()
        {
            return QLTVDb.Instance.Database.Connection.ConnectionString;
        }

        public List<NHAXUATBAN> GetAll()
        {
            var list = new List<NHAXUATBAN>();
            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataReader r = null;
            try
            {
                cn = new SqlConnection(GetConnectionString());
                cn.Open();
                cmd = new SqlCommand("SELECT id, MaNXB, TenNXB, DiaChi, Email, DienThoai FROM NHAXUATBAN ORDER BY MaNXB", cn);
                r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(new NHAXUATBAN
                    {
                        id = r.GetInt32(0),
                        MaNXB = r.GetString(1),
                        TenNXB = r.GetString(2),
                        DiaChi = r.IsDBNull(3) ? null : r.GetString(3),
                        Email = r.IsDBNull(4) ? null : r.GetString(4),
                        DienThoai = r.IsDBNull(5) ? null : r.GetString(5)
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

        public NHAXUATBAN GetById(int id)
        {
            foreach (var nxb in GetAll())
                if (nxb.id == id) return nxb;
            return null;
        }

        public NHAXUATBAN GetByMa(string maNXB)
        {
            foreach (var nxb in GetAll())
                if (nxb.MaNXB == maNXB) return nxb;
            return null;
        }

        public int Add(string tenNXB, string diaChi, string email, string dienThoai)
        {
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(GetConnectionString());
                cn.Open();
                cmd = new SqlCommand(
                    "INSERT INTO NHAXUATBAN(TenNXB, DiaChi, Email, DienThoai) VALUES (@Ten, @DC, @EM, @DT); SELECT SCOPE_IDENTITY();", cn);
                cmd.Parameters.AddWithValue("@Ten", tenNXB);
                cmd.Parameters.AddWithValue("@DC", (object)diaChi ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EM", (object)email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DT", (object)dienThoai ?? DBNull.Value);
                return System.Convert.ToInt32(cmd.ExecuteScalar());
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (cn != null) { cn.Close(); cn.Dispose(); }
            }
        }

        public bool Update(int id, string tenNXB, string diaChi, string email, string dienThoai)
        {
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(GetConnectionString());
                cn.Open();
                cmd = new SqlCommand(
                    "UPDATE NHAXUATBAN SET TenNXB=@Ten, DiaChi=@DC, Email=@EM, DienThoai=@DT WHERE id=@Id", cn);
                cmd.Parameters.AddWithValue("@Ten", tenNXB);
                cmd.Parameters.AddWithValue("@DC", (object)diaChi ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EM", (object)email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DT", (object)dienThoai ?? DBNull.Value);
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
                cmd = new SqlCommand("DELETE FROM NHAXUATBAN WHERE id=@Id", cn);
                cmd.Parameters.AddWithValue("@Id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (cn != null) { cn.Close(); cn.Dispose(); }
            }
        }
    }
}
