using BUS;
using DTO;
using GUI.Theme;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GUI.UserControls
{
    /// <summary>
    /// Màn Thông tin Tài khoản đồng bộ theme xanh: thẻ hồ sơ + chi tiết + danh sách chức năng.
    /// </summary>
    public class ucTaiKhoanNew : UserControl
    {
        private readonly NGUOIDUNG usr;

        public ucTaiKhoanNew(int id)
        {
            usr = BUSNguoiDung.Instance.GetNguoiDungById(id);
            this.Dock = DockStyle.Fill;
            this.BackColor = AppTheme.Background;
            this.Font = AppTheme.Base;
            this.AutoScaleMode = AutoScaleMode.Inherit;
            this.Load += (s, e) => { BuildUI(); ThemeManager.Apply(this); };
        }

        private void BuildUI()
        {
            if (Controls.Count > 0) return;

            // ===== Header: banner hồ sơ =====
            var header = new Panel { Dock = DockStyle.Top, Height = 140, BackColor = AppTheme.Primary };
            var lblName = new Label
            {
                Text = usr.TenNguoiDung,
                Font = new Font(AppTheme.FontFamily, 20F, FontStyle.Bold),
                ForeColor = AppTheme.TextOnPrimary,
                AutoSize = true,
                Location = new Point(140, 30),
                BackColor = Color.Transparent
            };
            var lblRole = new Label
            {
                Text = usr.NHOMNGUOIDUNG?.TenNhomNguoiDung ?? "",
                Font = new Font(AppTheme.FontFamily, 12F, FontStyle.Regular),
                ForeColor = AppTheme.TextOnPrimary,
                AutoSize = true,
                Location = new Point(143, 78),
                BackColor = Color.Transparent
            };
            var avatar = new Label
            {
                Text = "\U0001F464", // icon nguoi dung Unicode
                Font = new Font("Segoe UI Symbol", 52F, FontStyle.Regular),
                ForeColor = AppTheme.TextOnPrimary,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(90, 90),
                Location = new Point(30, 20),
                BackColor = Color.Transparent
            };
            header.Controls.Add(avatar);
            header.Controls.Add(lblName);
            header.Controls.Add(lblRole);

            // ===== Thân: 2 panel có chiều rộng tối đa, không giãn lan ra cả form =====
            int maxColWidth = 560;
            var body = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = AppTheme.Background
            };

            var leftBox = BuildInfoBox();
            leftBox.Width = Math.Min(body.ClientSize.Width / 2, maxColWidth);
            leftBox.Top = 16;
            leftBox.Left = 16;
            leftBox.Height = body.ClientSize.Height - 32;

            var rightBox = BuildFeatureBox();
            rightBox.Width = Math.Min(body.ClientSize.Width / 2, maxColWidth);
            rightBox.Top = 16;
            rightBox.Left = leftBox.Right + 12;
            rightBox.Height = body.ClientSize.Height - 32;

            body.Controls.Add(leftBox);
            body.Controls.Add(rightBox);
            body.Resize += (s, e) =>
            {
                leftBox.Width = Math.Min(body.ClientSize.Width / 2, maxColWidth);
                leftBox.Height = body.ClientSize.Height - 32;
                rightBox.Width = Math.Min(body.ClientSize.Width / 2, maxColWidth);
                rightBox.Left = leftBox.Right + 12;
                rightBox.Height = body.ClientSize.Height - 32;
            };

            this.Controls.Add(body);
            this.Controls.Add(header);
        }

        private GroupBox BuildInfoBox()
        {
            var grp = MakeGroup("Chi tiết người dùng");
            grp.Dock = DockStyle.Fill;
            grp.Margin = new Padding(0, 0, 8, 0);

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                Padding = new Padding(12, 10, 12, 10)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));

            AddRow(table, "Mã người dùng:", usr.MaNguoiDung);
            AddRow(table, "Tên đăng nhập:", usr.TenDangNhap);
            AddRow(table, "Email:", usr.Email ?? "(chưa cập nhật)");
            AddRow(table, "Địa chỉ:", usr.DiaChi ?? "(chưa cập nhật)");
            AddRow(table, "Ngày sinh:", usr.NgaySinh != null ? ((DateTime)usr.NgaySinh).ToShortDateString() : "(chưa cập nhật)");
            AddRow(table, "Vai trò:", usr.NHOMNGUOIDUNG?.TenNhomNguoiDung ?? "");

            var butPass = new Button
            {
                Text = "  Đổi mật khẩu",
                Height = 40,
                Width = 170,
                FlatStyle = FlatStyle.Flat,
                BackColor = AppTheme.Primary,
                ForeColor = AppTheme.TextOnPrimary,
                Font = AppTheme.Button,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
                Margin = new Padding(4, 16, 4, 4),
                Cursor = Cursors.Hand
            };
            butPass.FlatAppearance.BorderSize = 0;
            butPass.Click += (s, e) => new fChangePassword(usr.id).ShowDialog();
            RoundedCorner.Apply(butPass, 6);
            table.Controls.Add(new Label(), 0, table.RowCount); // spacer
            table.Controls.Add(butPass, 1, table.RowCount);

            grp.Controls.Add(table);
            return grp;
        }

        private GroupBox BuildFeatureBox()
        {
            var grp = MakeGroup("Các chức năng được phép");
            grp.Dock = DockStyle.Fill;
            grp.Margin = new Padding(8, 0, 0, 0);

            var list = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                BackColor = AppTheme.Surface,
                ForeColor = AppTheme.TextPrimary,
                Font = AppTheme.Base,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                BorderStyle = BorderStyle.None
            };
            list.Columns.Add("STT", 50);
            list.Columns.Add("Chức năng", 300);
            int i = 1;
            if (usr.NHOMNGUOIDUNG != null)
                foreach (CHUCNANG cn in usr.NHOMNGUOIDUNG.CHUCNANGs)
                    list.Items.Add(new ListViewItem(new[] { (i++).ToString(), cn.TenManHinh }));

            grp.Controls.Add(list);
            return grp;
        }

        private void AddRow(TableLayoutPanel table, string label, string value)
        {
            int row = table.RowCount;
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            table.Controls.Add(new Label
            {
                Text = label,
                Font = new Font(AppTheme.FontFamily, 11F, FontStyle.Bold),
                ForeColor = AppTheme.TextMuted,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            }, 0, row);
            table.Controls.Add(new TextBox
            {
                Text = value,
                ReadOnly = true,
                Font = new Font(AppTheme.FontFamily, 12F, FontStyle.Regular),
                ForeColor = AppTheme.TextPrimary,
                BackColor = AppTheme.Surface,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 8, 4, 8)
            }, 1, row);
            table.RowCount++;
        }

        private GroupBox MakeGroup(string text)
        {
            return new GroupBox
            {
                Text = text,
                Font = new Font(AppTheme.FontFamily, 11F, FontStyle.Bold),
                ForeColor = AppTheme.Primary,
                BackColor = AppTheme.Surface,
                Padding = new Padding(8)
            };
        }
    }
}
