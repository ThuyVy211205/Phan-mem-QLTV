using BUS;
using DTO;
using GUI.Theme;
using GUI.UserControls;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GUI
{
    public partial class fDashboard : Form
    {
        private readonly NGUOIDUNG user;

        public fDashboard(int id)
        {
            InitializeComponent();
            user = BUSNguoiDung.Instance.GetNguoiDungById(id);
            LoadBackgroundImage();
            LoadMenuIcons();
            BuildMenuByPermission();
            ThemeManager.Apply(this);
            this.Load += (s, e) => { FormFitter.Fit(this); CheckOverdueNotification(); };
        }

        private void LoadMenuIcons()
        {
            string imgDir = Path.Combine(Application.StartupPath, "images");
            if (!Directory.Exists(imgDir)) return;
            var tint = Color.SlateBlue;
            SetMenuImage(menuTaiKhoan, Path.Combine(imgDir, "taikhoan.png"), tint);
            SetMenuImage(menuQuanLyDanhMuc, Path.Combine(imgDir, "book_icon.png"), tint);
            SetMenuImage(menuQuanLyMuonTra, Path.Combine(imgDir, "phieumuontra.png"), tint);
            SetMenuImage(menuBaoCaoThongKe, Path.Combine(imgDir, "report_icon.png"), tint);
            SetMenuImage(menuThoat, Path.Combine(imgDir, "close_icon.png"), tint);
        }

        private static void SetMenuImage(ToolStripMenuItem item, string path, Color? tint = null)
        {
            if (!File.Exists(path)) return;
            item.Image = LoadMenuIcon(path, 24, tint);
        }

        private static Image LoadMenuIcon(string path, int size = 24, Color? tint = null)
        {
            using (var src = new Bitmap(path))
            {
                var result = new Bitmap(size, size);
                using (var g = Graphics.FromImage(result))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(src, 0, 0, size, size);
                }
                if (tint.HasValue)
                {
                    for (int y = 0; y < size; y++)
                        for (int x = 0; x < size; x++)
                        {
                            var p = result.GetPixel(x, y);
                            if (p.A > 0)
                                result.SetPixel(x, y, Color.FromArgb(p.A, tint.Value.R, tint.Value.G, tint.Value.B));
                        }
                }
                return result;
            }
        }

        private void LoadBackgroundImage()
        {
            // Ảnh nền sách thư viện nằm trong thư mục images (TV.png), copy ra thư mục build.
            string path = Path.Combine(Application.StartupPath, "images", "Thu_vien.jpg");
            if (File.Exists(path))
                picBackground.Image = System.Drawing.Image.FromFile(path);
        }

        /// <summary>
        /// Hiển thị một UserControl chiếm toàn vùng nội dung dưới menu.
        /// Ẩn tiêu đề lớn + ảnh nền để module chiếm trọn không gian.
        /// </summary>
        private void ShowContent(UserControl uc)
        {
            contentPanel.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(uc);

            // Ẩn phần trang chủ (banner tiêu đề + ảnh nền)
            headerPanel.Visible = false;
            picBackground.Visible = false;

            contentPanel.Visible = true;
            contentPanel.BringToFront();
            ThemeManager.Apply(contentPanel);
        }

        /// <summary>
        /// Quay về Trang chủ: hiện lại tiêu đề + ảnh nền, ẩn module.
        /// </summary>
        public void GoHome()
        {
            contentPanel.Visible = false;
            contentPanel.Controls.Clear();

            headerPanel.Visible = true;
            picBackground.Visible = true;
            picBackground.BringToFront();
        }

        /// <summary>
        /// Dựng các mục menu con dựa theo quyền (CHUCNANG) của nhóm người dùng.
        /// </summary>
        private void BuildMenuByPermission()
        {
            // Tài Khoản: luôn có
            AddSub(menuTaiKhoan, "Thông tin tài khoản",
                () => ShowContent(new ucTaiKhoanNew(user.id)), "taikhoan.png", Color.Black);
            AddSub(menuTaiKhoan, "Đổi mật khẩu",
                () => new fChangePassword(user.id).ShowDialog(), "key_icon.png", Color.Black);

            // Gom quyền để biết có những quyền gì
            var perms = new System.Collections.Generic.HashSet<string>();
            foreach (var cn in user.NHOMNGUOIDUNG.CHUCNANGs)
                perms.Add(cn.TenChucNang);

            // ---- Quản Lý Danh Mục ----
            bool hasDM = perms.Contains("QLDG") || perms.Contains("QLS") || perms.Contains("QLND") || perms.Contains("TDQD") || perms.Contains("DG");

            if (hasDM)
            {
                bool hasSach = perms.Contains("QLS") || perms.Contains("QLDG");
                if (hasSach)
                {
                    AddLabel(menuQuanLyDanhMuc, "▸ Nhóm Sách");
                    if (perms.Contains("QLS"))
                    {
                        AddSub(menuQuanLyDanhMuc, "Quản lý sách",
                            () => ShowContent(new ucSachNew()), "book_icon.png", Color.Black);
                        AddSub(menuQuanLyDanhMuc, "Thể loại",
                            () => ShowContent(new ucTheLoaiNew()), "category.png");
                        AddSub(menuQuanLyDanhMuc, "Tác giả",
                            () => ShowContent(new ucTacGiaNew()), "author.jpg");
                        AddSub(menuQuanLyDanhMuc, "Nhà xuất bản",
                            () => ShowContent(new ucNhaXuatBanNew()), "publishing_house.jpg");
                        AddSub(menuQuanLyDanhMuc, "Phiếu nhập sách",
                            () => ShowContent(new ucPhieuNhapSachNew()), "Book_receipt_slip.jpg");
                    }
                    if (perms.Contains("QLDG"))
                        AddSub(menuQuanLyDanhMuc, "Loại độc giả",
                            () => ShowContent(new ucLoaiDGNew()), "type_of_reader.jpg");
                }

                bool hasHeThong = perms.Contains("QLDG") || perms.Contains("QLND") || perms.Contains("TDQD");
                if (hasHeThong)
                {
                    AddSep(menuQuanLyDanhMuc);
                    AddLabel(menuQuanLyDanhMuc, "▸ Nhóm Hệ Thống");
                    if (perms.Contains("QLDG"))
                        AddSub(menuQuanLyDanhMuc, "Quản lý độc giả",
                            () => ShowContent(new ucDocGiaNew()), "docgia.png", Color.Black);
                    if (perms.Contains("QLND"))
                        AddSub(menuQuanLyDanhMuc, "Quản lý người dùng",
                            () => ShowContent(new ucNguoiDungNew()), "nguoidung.png", Color.Black);
                    if (perms.Contains("TDQD"))
                        AddSub(menuQuanLyDanhMuc, "Thay đổi quy định",
                            () => ShowContent(new ucThayDoiQuiDinh()), "thaydoiquydinh.png", Color.Black);
                }
            }

            // ---- Quản Lý Mượn Trả ----
            bool hasMT = perms.Contains("QLPM") || perms.Contains("QLPT");
            if (hasMT)
            {
                if (perms.Contains("QLPM"))
                {
                    AddSub(menuQuanLyMuonTra, "Phiếu mượn trả",
                        () => ShowContent(new ucPhieuMuonTraNew()), "phieumuontra.png", Color.Black);
                    AddSub(menuQuanLyMuonTra, "Quản lý đặt trước",
                        () => ShowContent(new ucDatTruocNew(user.id)), "Reservation_management.jpg");
                }
                if (perms.Contains("QLPT"))
                    AddSub(menuQuanLyMuonTra, "Quản lý thu phạt",
                        () => ShowContent(new ucPhieuThuNew()), "phieuthu.png", Color.Black);
                if (perms.Contains("QLPM"))
                {
                    AddSub(menuQuanLyMuonTra, "Quản lý nhu cầu đọc",
                        () => ShowContent(new ucQuanLyNhuCauDoc(user.id)), "reading_needs.jpg");
                    AddSub(menuQuanLyMuonTra, "Lịch sử & chỉnh sửa",
                        () => ShowContent(new ucQuanLyLichSuMuon(user.id)), "edit_history.jpg");
                }
            }

            // ---- Báo Cáo & Hệ Thống (chỉ admin/thủ thư) ----
            bool isDocGia = perms.Count == 1 && perms.Contains("DG");
            if (!isDocGia)
            {
                if (perms.Contains("DG"))
                    AddSub(menuBaoCaoThongKe, "Tra cứu sách",
                        () => ShowContent(new ucTraCuuSachNew()), "search_icon.png", Color.Black);
                if (perms.Contains("BCTK"))
                    AddSub(menuBaoCaoThongKe, "Xem báo cáo thống kê",
                        () => ShowContent(new ucBaoCaoNew()), "report_icon.png", Color.Black);
                if (perms.Contains("TDQD"))
                    AddSub(menuBaoCaoThongKe, "Sao lưu & khôi phục",
                        () => ShowContent(new ucBackupRestore()), "Backup_Restore.jpg");
            }

            // ---- Độc Giả: mục riêng ----
            if (isDocGia)
            {
                AddSub(menuQuanLyDanhMuc, "Tra cứu sách",
                    () => ShowContent(new ucTraCuuSachNew()), "search_icon.png", Color.Black);
                AddSub(menuQuanLyDanhMuc, "Lịch sử Mượn - Trả", () =>
                {
                    var ucl = new ucLichSuVaNhuCau(showLichSu: true, showNhuCau: false);
                    try { var dg = QLTVDb.Instance.DOCGIAs.FirstOrDefault(d => d.idNguoiDung == user.id); if (dg != null) ucl.LoadData(dg.MaDocGia); } catch { }
                    ShowContent(ucl);
                }, "edit_history.jpg");
                AddSub(menuQuanLyDanhMuc, "Danh sách Nhu cầu đọc", () =>
                {
                    var ucl = new ucLichSuVaNhuCau(showLichSu: false, showNhuCau: true);
                    try { var dg = QLTVDb.Instance.DOCGIAs.FirstOrDefault(d => d.idNguoiDung == user.id); if (dg != null) ucl.LoadData(dg.MaDocGia); } catch { }
                    ShowContent(ucl);
                }, "reading_needs.jpg");
                AddSub(menuQuanLyDanhMuc, "Đặt trước sách", () =>
                {
                    try
                    {
                        var dg = QLTVDb.Instance.DOCGIAs.FirstOrDefault(d => d.idNguoiDung == user.id);
                        if (dg == null) { MessageBox.Show("Tài khoản chưa liên kết với độc giả.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                        using (var f = new Form
                        {
                            Text = "Đặt trước sách",
                            Size = new System.Drawing.Size(480, 290),
                            StartPosition = FormStartPosition.CenterParent,
                            FormBorderStyle = FormBorderStyle.FixedDialog,
                            MaximizeBox = false, MinimizeBox = false,
                            BackColor = System.Drawing.Color.FromArgb(241, 245, 249),
                            Font = new System.Drawing.Font("Segoe UI", 12F)
                        })
                        {
                            var fontBold = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
                            var pnlContent = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 20, 24, 16), BackColor = System.Drawing.Color.FromArgb(241, 245, 249) };
                            var lblDG = new Label { Text = "Độc giả:", Font = fontBold, ForeColor = System.Drawing.Color.FromArgb(30, 58, 138), Location = new System.Drawing.Point(24, 10), AutoSize = true };
                            var txtDG = new TextBox { Text = $"{dg.MaDocGia} - {dg.TenDocGia}", Font = new System.Drawing.Font("Segoe UI", 12F), Location = new System.Drawing.Point(140, 7), Width = 280, ReadOnly = true, BackColor = System.Drawing.Color.FromArgb(226, 232, 240), BorderStyle = BorderStyle.FixedSingle };
                            var lblMa = new Label { Text = "Mã Sách:", Font = fontBold, ForeColor = System.Drawing.Color.FromArgb(30, 58, 138), Location = new System.Drawing.Point(24, 55), AutoSize = true };
                            var txtMaSach = new TextBox { Font = new System.Drawing.Font("Segoe UI", 12F), Location = new System.Drawing.Point(140, 52), Width = 280, BorderStyle = BorderStyle.FixedSingle };
                            var lblGc = new Label { Text = "Ghi Chú:", Font = fontBold, ForeColor = System.Drawing.Color.FromArgb(30, 58, 138), Location = new System.Drawing.Point(24, 100), AutoSize = true };
                            var txtGhiChu = new TextBox { Font = new System.Drawing.Font("Segoe UI", 12F), Location = new System.Drawing.Point(140, 97), Width = 280, Height = 50, Multiline = true, BorderStyle = BorderStyle.FixedSingle };
                            var lblInfo = new Label { Text = "Chỉ đặt trước khi sách đã hết. Admin/Thủ thư sẽ xác nhận khi có sách trả.", Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic), ForeColor = System.Drawing.Color.FromArgb(100, 116, 139), Location = new System.Drawing.Point(24, 158), AutoSize = true, MaximumSize = new System.Drawing.Size(420, 0) };
                            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 56, BackColor = System.Drawing.Color.White, Padding = new Padding(0, 10, 16, 0) };
                            var butOk = new Button { Text = "\u2705  Xác nhận đặt trước", FlatStyle = FlatStyle.Flat, BackColor = System.Drawing.Color.FromArgb(25, 135, 84), ForeColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold), Size = new System.Drawing.Size(200, 38), Location = new System.Drawing.Point(128, 10), Cursor = Cursors.Hand };
                             butOk.FlatAppearance.BorderSize = 0;
                             RoundedCorner.Apply(butOk, 6);
                            butOk.Click += (s2, ev2) =>
                            {
                                string masach = txtMaSach.Text.Trim();
                                if (string.IsNullOrWhiteSpace(masach)) { MessageBox.Show("Vui lòng nhập Mã Sách."); return; }
                                string err = BUSDatTruoc.Instance.AddDatTruoc(dg.MaDocGia, masach, txtGhiChu.Text, user.id);
                                if (string.IsNullOrEmpty(err)) { MessageBox.Show("Đặt trước thành công!"); f.DialogResult = DialogResult.OK; f.Close(); }
                                else MessageBox.Show(err, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            };
                            pnlContent.Controls.AddRange(new Control[] { lblDG, txtDG, lblMa, txtMaSach, lblGc, txtGhiChu, lblInfo });
                            pnlBottom.Controls.Add(butOk);
                            f.Controls.Add(pnlContent); f.Controls.Add(pnlBottom);
                            f.ShowDialog();
                        }
                    }
                    catch { }
                }, "Reservation_management.jpg");

                menuQuanLyMuonTra.DropDownItems.Clear();
                menuQuanLyMuonTra.Click += (s, e) =>
                    MessageBox.Show("Bạn không có quyền truy cập vào chức năng này.\n\nVui lòng liên hệ Quản trị viên để được hỗ trợ.",
                        "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                menuBaoCaoThongKe.DropDownItems.Clear();
                menuBaoCaoThongKe.Click += (s, e) =>
                    MessageBox.Show("Bạn không có quyền truy cập vào chức năng này.\n\nVui lòng liên hệ Quản trị viên để được hỗ trợ.",
                        "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void AddSub(ToolStripMenuItem parent, string text, Action action, string iconFile = null, Color? tint = null)
        {
            var item = new ToolStripMenuItem(text);
            item.Click += (s, e) => action();
            if (!string.IsNullOrEmpty(iconFile))
            {
                string path = Path.Combine(Application.StartupPath, "images", iconFile);
                if (File.Exists(path))
                {
                    try { item.Image = LoadMenuIcon(path, 24, tint); } catch { }
                }
            }
            parent.DropDownItems.Add(item);
        }

        private void AddSep(ToolStripMenuItem parent)
        {
            parent.DropDownItems.Add(new ToolStripSeparator());
        }

        private void AddLabel(ToolStripMenuItem parent, string text)
        {
            var item = new ToolStripMenuItem(text) { Enabled = false };
            parent.DropDownItems.Add(item);
        }

        private void menuThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có muốn đóng giao diện này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            {
                this.Close();
            }
        }

        private void contentPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void CheckOverdueNotification()
        {
            try
            {
                bool isReader = user.NHOMNGUOIDUNG.CHUCNANGs.All(c => c.TenChucNang == "DG");
                if (isReader)
                {
                    var overdue = BUSNotification.Instance.GetOverdueByUser(user.id);
                    if (overdue.Count > 0)
                        MessageBox.Show(
                            string.Format("Bạn đang có {0} cuốn sách quá hạn.\nVui lòng trả sách sớm để tránh bị phạt.", overdue.Count),
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var allOverdue = BUSNotification.Instance.GetOverdueBooks();
                if (allOverdue.Count == 0) return;

                string msg = string.Format("Có {0} sách quá hạn chưa trả:\n\n", allOverdue.Count);
                foreach (var o in allOverdue)
                    msg += string.Format("  Phiếu #{0} | {1} ({2})\n  Mượn: {3:dd/MM} - Hạn: {4:dd/MM} (Trễ {5} ngày)\n\n",
                        o.SoPhieu, o.TenDocGia, o.MaDocGia,
                        o.NgayMuon, o.HanTra, o.SoNgayTre);

                MessageBox.Show(msg, "Cảnh báo sách quá hạn",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch { }
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {

        }
    }
}
