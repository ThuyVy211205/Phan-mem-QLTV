using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DALThamSo
    {
        private static DALThamSo instance;

        public static DALThamSo Instance
        {
            get
            {
                if (instance == null) instance = new DALThamSo();
                return instance;
            }
            set => instance = value;
        }

        public DALThamSo()
        {
            if (!QLTVDb.Instance.THAMSOes.Any())
            {
                QLTVDb.Instance.THAMSOes.Add(new THAMSO
                {
                    TuoiToiThieu = 18,
                    TuoiToiDa = 55,
                    ThoiHanThe = 6,
                    KhoangCachXuatBan = 8,
                    SoSachMuonToiDa = 5,
                    SoNgayMuonToiDa = 4,
                    DonGiaPhat = 1000,
                    AD_QDKTTienThu = 1
                });
                QLTVDb.Instance.SaveChanges();
            }
        }
        public THAMSO GetAllThamSo()
        {

            var ts = QLTVDb.Instance.THAMSOes.First();
            try
            {
                using (var cn = new System.Data.SqlClient.SqlConnection(QLTVDb.Instance.Database.Connection.ConnectionString))
                {
                    cn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT HeSoPhatMatSach FROM THAMSO WHERE id = @id", cn))
                    {
                        cmd.Parameters.AddWithValue("@id", ts.id);
                        var result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                            ts.HeSoPhatMatSach = Convert.ToInt32(result);
                    }
                }
            }
            catch { }
            return ts;
        }

        public bool UpdTuoiToiThieu (int tuoi)
        {
            try
            {
                THAMSO ts = GetAllThamSo();
                ts.TuoiToiThieu = tuoi;
                QLTVDb.Instance.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
                return false;
            }
            
        }
        public bool UpdTuoiToiDa(int tuoi)
        {
            try
            {
                THAMSO ts = GetAllThamSo();
                ts.TuoiToiDa = tuoi;
                QLTVDb.Instance.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
                return false;
            }
        }

        public bool UpdThoiHanThe(int thoiHan)
        {
            try
            {
                THAMSO ts = GetAllThamSo();
                ts.ThoiHanThe = thoiHan;
                QLTVDb.Instance.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
                return false;
            }
        }

        public bool UpdKhoangCachXB(int kc)
        {
            try
            {
                THAMSO ts = GetAllThamSo();
                ts.KhoangCachXuatBan = kc;
                QLTVDb.Instance.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
                return false;
            }

        }

        public bool UpdSoSachMuonToiDa(int soSach)
        {
            try
            {
                THAMSO ts = GetAllThamSo();
                ts.SoSachMuonToiDa = soSach;
                QLTVDb.Instance.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
                return false;
            }

        }

        public bool UpdSoNgayMuonToiDa(int soNgay)
        {
            try
            {
                THAMSO ts = GetAllThamSo();
                ts.SoNgayMuonToiDa = soNgay;
                QLTVDb.Instance.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
                return false;
            }

        }

        public bool UpdDonGiaPhat(int donGia)
        {
            try
            {
                THAMSO ts = GetAllThamSo();
                ts.DonGiaPhat = donGia;
                QLTVDb.Instance.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
                return false;
            }
        }

        public bool UpdQDKTTienPhat(int apDung)
        {
            try
            {
                THAMSO ts = GetAllThamSo();
                ts.AD_QDKTTienThu = apDung;
                QLTVDb.Instance.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
                return false;
            }

        }

        public bool UpdHeSoPhatMatSach(int heSo)
        {
            try
            {
                THAMSO ts = QLTVDb.Instance.THAMSOes.First();
                using (var cn = new System.Data.SqlClient.SqlConnection(QLTVDb.Instance.Database.Connection.ConnectionString))
                {
                    cn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand("UPDATE THAMSO SET HeSoPhatMatSach = @val WHERE id = @id", cn))
                    {
                        cmd.Parameters.AddWithValue("@val", heSo);
                        cmd.Parameters.AddWithValue("@id", ts.id);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
                return false;
            }
        }
    }
}
