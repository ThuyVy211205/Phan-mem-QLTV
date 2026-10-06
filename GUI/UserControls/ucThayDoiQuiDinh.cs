using BUS;
using DTO;
using GUI.Theme;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GUI.UserControls
{
    public partial class ucThayDoiQuiDinh : UserControl
    {
        private Siticone.Desktop.UI.WinForms.SiticoneNumericUpDown numHeSoMat;
        private Label lblHeSoMat, lblHeSoMatUnit;

        public ucThayDoiQuiDinh()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.AutoScaleMode = AutoScaleMode.Inherit;
            butSave.BorderRadius = 8;
            StyleControls();
            AddHeSoPhatMatSach();
            Bind();
        }

        private void StyleControls()
        {
            this.BackColor = AppTheme.Background;
            infoPanel.BackColor = Color.White;
            infoPanel.BorderColor = AppTheme.Border;
            infoPanel.Padding = new Padding(20, 16, 20, 16);

            // Panel 1 - Độc giả section
            splitContainer1.Panel1.BackColor = Color.White;
            splitContainer1.Panel1.Padding = new Padding(24, 16, 24, 8);
            // Panel 2 - Sách section
            splitContainer1.Panel2.BackColor = Color.White;
            splitContainer1.Panel2.Padding = new Padding(24, 16, 24, 8);

            splitContainer1.SplitterDistance = 320;

            // Labels - convert old dark colors to AppTheme
            foreach (var row in splitContainer1.Panel1.Controls)
                if (row is Control c) StylePanelRow(c);
            foreach (var row in splitContainer1.Panel2.Controls)
                if (row is Control c) StylePanelRow(c);

            // Section headers
            label1.ForeColor = AppTheme.Primary;
            label1.Font = new Font(AppTheme.FontFamily, 18F, FontStyle.Bold);
            label16.ForeColor = AppTheme.Primary;
            label16.Font = new Font(AppTheme.FontFamily, 18F, FontStyle.Bold);

            // Separators
            siticoneSeparator1.FillColor = AppTheme.Border;
            siticoneSeparator2.FillColor = AppTheme.Border;

            // Save button
            butSave.FillColor = AppTheme.Primary;
            butSave.ForeColor = Color.White;
        }

        private void StylePanelRow(Control panel)
        {
            panel.BackColor = Color.White;
            foreach (Control c in panel.Controls)
            {
                if (c is Label lbl)
                {
                    if (lbl.ForeColor == Color.DarkSlateBlue || lbl.ForeColor == Color.SlateBlue)
                    {
                        lbl.ForeColor = AppTheme.TextPrimary;
                        if (lbl.Font.Size >= 13) lbl.Font = new Font(AppTheme.FontFamily, 13F);
                    }
                    else if (lbl.ForeColor == Color.MediumSlateBlue)
                        lbl.ForeColor = AppTheme.Border;
                }
                if (c is Siticone.Desktop.UI.WinForms.SiticoneNumericUpDown num)
                    num.UpDownButtonFillColor = AppTheme.Primary;
            }
        }

        private void AddHeSoPhatMatSach()
        {
            var panel2 = splitContainer1.Panel2;
            var siticonePanel11 = panel2.Controls.Find("siticonePanel11", true);
            var insertAfter = siticonePanel11.Length > 0 ? siticonePanel11[0] : null;

            var row = new Siticone.Desktop.UI.WinForms.SiticonePanel
            {
                Dock = DockStyle.Top,
                Height = 42,
                Padding = new Padding(0, 8, 0, 4),
                BackColor = Color.White,
                CustomBorderColor = AppTheme.Border,
                CustomBorderThickness = new Padding(0, 0, 0, 1)
            };

            var lbl = new Label
            {
                Text = "Hệ số phạt mất sách:",
                Font = new Font(AppTheme.FontFamily, 13F),
                ForeColor = AppTheme.TextPrimary,
                Dock = DockStyle.Left,
                AutoSize = true,
                Location = new Point(0, 10),
                Width = 311
            };

            numHeSoMat = new Siticone.Desktop.UI.WinForms.SiticoneNumericUpDown
            {
                Minimum = 1, Maximum = 10, Value = 3,
                Font = new Font(AppTheme.FontFamily, 10F),
                UpDownButtonFillColor = AppTheme.Primary,
                BackColor = Color.Transparent,
                Dock = DockStyle.Left,
                Width = 70
            };

            lblHeSoMatUnit = new Label
            {
                Text = "(lần đơn giá)",
                Font = new Font(AppTheme.FontFamily, 11F, FontStyle.Italic),
                ForeColor = AppTheme.TextMuted,
                Dock = DockStyle.Left,
                AutoSize = true,
                Padding = new Padding(4, 6, 0, 0)
            };

            row.Controls.Add(lblHeSoMatUnit);
            row.Controls.Add(numHeSoMat);
            row.Controls.Add(lbl);

            if (insertAfter != null)
            {
                int idx = panel2.Controls.IndexOf(insertAfter);
                panel2.Controls.Add(row);
                if (idx >= 0 && idx + 1 < panel2.Controls.Count)
                    panel2.Controls.SetChildIndex(row, idx + 1);
            }
            else
            {
                panel2.Controls.Add(row);
            }
        }

        private void Bind()
        {
            var ts = BUSThamSo.Instance.GetAllThamSo();
            numTuoiMin.Value = ts.TuoiToiThieu;
            numTuoiMax.Value = ts.TuoiToiDa;
            numThoiHan.Value = ts.ThoiHanThe;
            numNgayMuon.Value = ts.SoNgayMuonToiDa;
            numKcNam.Value = ts.KhoangCachXuatBan;
            numSoSach.Value = ts.SoSachMuonToiDa;
            txtDonGia.PlaceholderText = ts.DonGiaPhat.ToString();
            txtDonGia.Text = ts.DonGiaPhat.ToString();
            checkQDThu.Checked = ts.AD_QDKTTienThu == 1;
            if (numHeSoMat != null)
                numHeSoMat.Value = ts.HeSoPhatMatSach > 0 ? ts.HeSoPhatMatSach : 3;
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            string err = BUSThamSo.Instance.UpdTuoiToiThieu((int)numTuoiMin.Value);
            if (err != "") { Err(err, "Tuổi tối thiểu"); return; }

            err = BUSThamSo.Instance.UpTuoiToiDa((int)numTuoiMax.Value);
            if (err != "") { Err(err, "Tuổi tối đa"); return; }

            err = BUSThamSo.Instance.UpdThoiHanThe((int)numThoiHan.Value);
            if (err != "") { Err(err, "Thời hạn thẻ"); return; }

            err = BUSThamSo.Instance.UpdSoSachToiDa((int)numSoSach.Value);
            if (err != "") { Err(err, "Số sách mượn"); return; }

            err = BUSThamSo.Instance.UpdSoNgayMuon((int)numNgayMuon.Value);
            if (err != "") { Err(err, "Số ngày mượn"); return; }

            err = BUSThamSo.Instance.UpdKCXB((int)numKcNam.Value);
            if (err != "") { Err(err, "Khoảng cách XB"); return; }

            if (!int.TryParse(txtDonGia.Text, out int dg) || dg < 0)
            { Err("Đơn giá không hợp lệ.", "Đơn giá"); return; }
            err = BUSThamSo.Instance.UpdDonGiaPhat(dg);
            if (err != "") { Err(err, "Đơn giá"); return; }

            err = BUSThamSo.Instance.UpdHeSoPhatMatSach((int)numHeSoMat.Value);
            if (err != "") { Err(err, "Hệ số phạt mất sách"); return; }

            BUSThamSo.Instance.UpdADQDTienPhat(checkQDThu.Checked ? 1 : 0);

            MessageBox.Show("Đã lưu thay đổi.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Bind();
        }

        private void Err(string msg, string field)
        {
            MessageBox.Show($"{field}: {msg}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Bind();
        }

        private void txtDonGia_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDonGia.Text)) return;
            if (!int.TryParse(txtDonGia.Text, out _))
            {
                MessageBox.Show("Vui lòng chỉ nhập số nguyên dương.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDonGia.Text = "";
            }
        }
    }
}
