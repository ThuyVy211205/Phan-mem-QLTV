using DAL;
using DTO;
using System;
using System.Collections.Generic;

namespace BUS
{
    public class BUSNhuCauDoc
    {
        private static BUSNhuCauDoc instance;
        public static BUSNhuCauDoc Instance
        {
            get
            {
                if (instance == null) instance = new BUSNhuCauDoc();
                return instance;
            }
            set => instance = value;
        }

        // ===== LỊCH SỬ MƯỢN =====

        public List<LichSuMuon> GetLichSuByDocGia(string maDocGia)
        {
            if (string.IsNullOrWhiteSpace(maDocGia))
                throw new ArgumentException("Mã độc giả không được để trống.");
            return DALNhuCauDoc.Instance.GetLichSuByDocGia(maDocGia.Trim());
        }

        public List<LichSuMuon> GetAllLichSu()
        {
            return DALNhuCauDoc.Instance.GetAllLichSu();
        }

        // ===== NHU CẦU ĐỌC =====

        public List<NhuCauDoc> GetNhuCauByDocGia(string maDocGia)
        {
            if (string.IsNullOrWhiteSpace(maDocGia))
                throw new ArgumentException("Mã độc giả không được để trống.");
            return DALNhuCauDoc.Instance.GetNhuCauByDocGia(maDocGia.Trim());
        }

        public string AddNhuCau(string maDocGia, string maSach, string ghiChu)
        {
            if (string.IsNullOrWhiteSpace(maDocGia))
                return "Mã độc giả không được để trống.";
            if (string.IsNullOrWhiteSpace(maSach))
                return "Mã sách không được để trống.";

            int id = DALNhuCauDoc.Instance.AddNhuCau(maDocGia.Trim(), maSach.Trim(), ghiChu);
            if (id > 0) return "";
            return "Không thể thêm nhu cầu đọc. Kiểm tra lại Mã Độc Giả và Mã Sách.";
        }

        public string DelNhuCau(int id)
        {
            if (id <= 0) return "ID không hợp lệ.";
            if (DALNhuCauDoc.Instance.DelNhuCau(id)) return "";
            return "Không thể xóa nhu cầu đọc.";
        }
    }
}
