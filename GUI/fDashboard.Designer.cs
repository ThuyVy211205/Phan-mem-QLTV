namespace GUI
{
    partial class fDashboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fDashboard));
            this.headerPanel = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuTaiKhoan = new System.Windows.Forms.ToolStripMenuItem();
            this.menuQuanLyDanhMuc = new System.Windows.Forms.ToolStripMenuItem();
            this.menuQuanLyMuonTra = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBaoCaoThongKe = new System.Windows.Forms.ToolStripMenuItem();
            this.menuThoat = new System.Windows.Forms.ToolStripMenuItem();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.picBackground = new System.Windows.Forms.PictureBox();
            this.headerPanel.SuspendLayout();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).BeginInit();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.White;
            this.headerPanel.Controls.Add(this.lblTitle);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1549, 138);
            this.headerPanel.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Beige;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Times New Roman", 34F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1549, 138);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Quản Lý Thư Viện";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.Click += new System.EventHandler(this.lblTitle_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.Color.WhiteSmoke;
            this.menuStrip.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuTaiKhoan,
            this.menuQuanLyDanhMuc,
            this.menuQuanLyMuonTra,
            this.menuBaoCaoThongKe,
            this.menuThoat});
            this.menuStrip.Location = new System.Drawing.Point(0, 138);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(7, 4, 0, 4);
            this.menuStrip.Size = new System.Drawing.Size(1549, 48);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip";
            // 
            // menuTaiKhoan
            // 
            this.menuTaiKhoan.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuTaiKhoan.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.menuTaiKhoan.Margin = new System.Windows.Forms.Padding(40, 0, 40, 0);
            this.menuTaiKhoan.Name = "menuTaiKhoan";
            this.menuTaiKhoan.Size = new System.Drawing.Size(151, 40);
            this.menuTaiKhoan.Text = "Tài Khoản";
            // 
            // menuQuanLyDanhMuc
            // 
            this.menuQuanLyDanhMuc.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuQuanLyDanhMuc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.menuQuanLyDanhMuc.Margin = new System.Windows.Forms.Padding(40, 0, 40, 0);
            this.menuQuanLyDanhMuc.Name = "menuQuanLyDanhMuc";
            this.menuQuanLyDanhMuc.Size = new System.Drawing.Size(262, 40);
            this.menuQuanLyDanhMuc.Text = "Quản Lý Danh Mục";
            // 
            // menuQuanLyMuonTra
            // 
            this.menuQuanLyMuonTra.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuQuanLyMuonTra.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.menuQuanLyMuonTra.Margin = new System.Windows.Forms.Padding(40, 0, 40, 0);
            this.menuQuanLyMuonTra.Name = "menuQuanLyMuonTra";
            this.menuQuanLyMuonTra.Size = new System.Drawing.Size(273, 40);
            this.menuQuanLyMuonTra.Text = "Quản Lý Mượn - Trả";
            // 
            // menuBaoCaoThongKe
            // 
            this.menuBaoCaoThongKe.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuBaoCaoThongKe.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.menuBaoCaoThongKe.Margin = new System.Windows.Forms.Padding(40, 0, 40, 0);
            this.menuBaoCaoThongKe.Name = "menuBaoCaoThongKe";
            this.menuBaoCaoThongKe.Size = new System.Drawing.Size(273, 40);
            this.menuBaoCaoThongKe.Text = "Báo Cáo - Thống Kê";
            // 
            // menuThoat
            // 
            this.menuThoat.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuThoat.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.menuThoat.Margin = new System.Windows.Forms.Padding(40, 0, 40, 0);
            this.menuThoat.Name = "menuThoat";
            this.menuThoat.Size = new System.Drawing.Size(102, 40);
            this.menuThoat.Text = "Thoát";
            this.menuThoat.Click += new System.EventHandler(this.menuThoat_Click);
            // 
            // contentPanel
            // 
            this.contentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.contentPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("contentPanel.BackgroundImage")));
            this.contentPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.contentPanel.Location = new System.Drawing.Point(0, 186);
            this.contentPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.contentPanel.Size = new System.Drawing.Size(1549, 665);
            this.contentPanel.TabIndex = 3;
            this.contentPanel.Visible = false;
            this.contentPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.contentPanel_Paint);
            // 
            // picBackground
            // 
            this.picBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picBackground.Location = new System.Drawing.Point(0, 186);
            this.picBackground.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.picBackground.Name = "picBackground";
            this.picBackground.Size = new System.Drawing.Size(1549, 665);
            this.picBackground.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBackground.TabIndex = 2;
            this.picBackground.TabStop = false;
            // 
            // fDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1549, 851);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.picBackground);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.headerPanel);
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(1010, 736);
            this.Name = "fDashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Phần mềm Quản Lý Thư Viện";
            this.headerPanel.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuTaiKhoan;
        private System.Windows.Forms.ToolStripMenuItem menuQuanLyDanhMuc;
        private System.Windows.Forms.ToolStripMenuItem menuQuanLyMuonTra;
        private System.Windows.Forms.ToolStripMenuItem menuBaoCaoThongKe;
        private System.Windows.Forms.ToolStripMenuItem menuThoat;
        private System.Windows.Forms.PictureBox picBackground;
        private System.Windows.Forms.Panel contentPanel;
    }
}