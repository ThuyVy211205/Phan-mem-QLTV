using DAL;
using DTO;
using System.Collections.Generic;

namespace BUS
{
    public class BUSNhaXuatBan
    {
        private static BUSNhaXuatBan instance;
        public static BUSNhaXuatBan Instance
        {
            get { if (instance == null) instance = new BUSNhaXuatBan(); return instance; }
            set => instance = value;
        }

        public List<NHAXUATBAN> GetAll()
        {
            return DALNhaXuatBan.Instance.GetAll();
        }

        public NHAXUATBAN GetById(int id)
        {
            return DALNhaXuatBan.Instance.GetById(id);
        }

        public string Add(string tenNXB, string diaChi, string email, string dienThoai)
        {
            if (string.IsNullOrWhiteSpace(tenNXB))
                return "Tên NXB không được để trống.";

            int id = DALNhaXuatBan.Instance.Add(tenNXB.Trim(), diaChi?.Trim(), email?.Trim(), dienThoai?.Trim());
            if (id > 0) return "";
            return "Không thể thêm NXB.";
        }

        public string Update(int id, string tenNXB, string diaChi, string email, string dienThoai)
        {
            if (string.IsNullOrWhiteSpace(tenNXB))
                return "Tên NXB không được để trống.";

            if (DALNhaXuatBan.Instance.Update(id, tenNXB.Trim(), diaChi?.Trim(), email?.Trim(), dienThoai?.Trim()))
                return "";
            return "Không thể cập nhật NXB.";
        }

        public string Delete(int id)
        {
            var nxb = DALNhaXuatBan.Instance.GetById(id);
            if (nxb == null) return "NXB không tồn tại.";

            if (DALNhaXuatBan.Instance.Delete(id))
                return "";
            return "Không thể xoá NXB.";
        }
    }
}
