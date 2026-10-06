using BUS;
using DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class fChangePassword : Form
    {
        private NGUOIDUNG usr;
        public fChangePassword(int _id)
        {
            InitializeComponent();
            GUI.Theme.ThemeManager.Apply(this);
            butSave.BorderRadius = 8;
            usr = BUSNguoiDung.Instance.GetNguoiDungById(_id);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.StartPosition = FormStartPosition.CenterScreen;
            // Nút close/minimize luôn sát mép phải
            this.Resize += (s, e) =>
            {
                siticoneControlBox1.Left = this.ClientSize.Width - siticoneControlBox1.Width;
                siticoneControlBox2.Left = siticoneControlBox1.Left - siticoneControlBox2.Width - 4;
            };
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            if(txtMKHT.Text == "" || txtRMKM.Text == "" || txtMKM.Text == "")
            {
                MessageBox.Show("Chưa điền đủ thông tin", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(txtMKHT.Text != usr.MatKhau)
            {
                MessageBox.Show("Mật khẩu hiện tại không đúng", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; 
            }
            if(txtMKM.Text != txtRMKM.Text)
            {
                MessageBox.Show("Mật khẩu mới không khớp", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            BUSNguoiDung.Instance.UpdMK(usr.id, txtMKM.Text);
            MessageBox.Show("Cập nhật mật khẩu thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void txtRMKM_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
