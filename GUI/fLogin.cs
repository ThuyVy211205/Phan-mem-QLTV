using BUS;
using DTO;
using GUI;
using GUI.Theme;
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
    public partial class fLogin : Form
    {
        public fLogin()
        {
            InitializeComponent();
            ThemeManager.Apply(this);
            butLogin.BorderRadius = 8;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += (s, e) => { FormFitter.Fit(this); CenterLoginPanel(); };
            this.Resize += (s, e) => CenterLoginPanel();
        }

        /// <summary>
        /// Căn giữa card đăng nhập (siticonePanel1) theo cả chiều ngang lẫn dọc.
        /// Gọi trong Load và Resize để luôn đúng dù form bị kéo.
        /// </summary>
        private void CenterLoginPanel()
        {
            // Căn card login giữa form, bỏ qua top bar (siticonePanel2 dock Top).
            int topBarH = siticonePanel2.Height;
            int availableH = this.ClientSize.Height - topBarH;
            int x = (this.ClientSize.Width - siticonePanel1.Width) / 2;
            int y = topBarH + (availableH - siticonePanel1.Height) / 2;
            siticonePanel1.Location = new Point(Math.Max(0, x), Math.Max(0, y));
        }

        private void resetTextboxs()
        {
            txtUsername.Clear();
            txtUserpwd.Clear();
            txtUsername.Focus();
        }
        private void butLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string userpwd = txtUserpwd.Text;
            BUSLogin bLogin = new BUSLogin();
            int id = bLogin.checkValidLogin(username, userpwd);
            if (id != -1)
            {
                MessageBox.Show("Đăng nhập thành công!\nChào mừng " + username + "!",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                var user = BUSNguoiDung.Instance.GetNguoiDungById(id);
                this.Hide();
                var f = new fDashboard(id);
                f.ShowDialog();
                this.resetTextboxs();
                this.Show();
            }
            else
            {
                MessageBox.Show("Tài khoản hoặc mật khẩu không đúng!", "Thông báo", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                this.resetTextboxs();
            }
        }

    }
}
