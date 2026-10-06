using GUI.Theme;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GUI.UserControls
{
    /// <summary>
    /// Mô tả một trường nhập liệu trên form CRUD.
    /// </summary>
    public class CrudField
    {
        public string Key;                 // khoá nội bộ
        public string Label;               // nhãn hiển thị "Tên Sách :"
        public bool IsCombo;               // true -> ComboBox
        public bool IsDate;                // true -> DateTimePicker
        public bool ReadOnly;              // ô chỉ đọc (vd: Mã)
        public Func<List<object>> ComboSource; // nguồn cho combo (nếu IsCombo)
        public Control Input;              // control thực (gán khi dựng)
    }

    /// <summary>
    /// Lớp cơ sở cho các màn hình quản lý theo mẫu "Quản Lý Sách":
    /// [Tìm kiếm + radio] -> [Home / Load] -> [Thông tin: 2 cột] -> [Thêm/Sửa/Xóa/Lưu/Hủy] -> [DataGridView].
    /// Lớp con chỉ cần override BuildFields / BuildGridColumns / LoadData và các thao tác CRUD.
    /// </summary>
    public class ucCrudBase : UserControl
    {
        // --- Các control layout ---
        protected FlowLayoutPanel radioPanel;
        protected TextBox txtSearch;
        protected Button butHome, butLoad;
        protected Button butThem, butSua, butXoa, butLuu, butHuy;
        protected DataGridView grid;
        protected TableLayoutPanel infoTable;

        protected readonly List<CrudField> Fields = new List<CrudField>();
        protected readonly List<RadioButton> SearchRadios = new List<RadioButton>();

        // Trạng thái: đang xem / thêm mới / đang sửa
        protected enum Mode { View, Add, Edit }
        protected Mode CurrentMode = Mode.View;
        protected int SelectedId = -1;

        public ucCrudBase()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = AppTheme.Background;
            this.Font = AppTheme.Base;
            // Kế thừa tỉ lệ scale từ Form cha -> tránh scale 2 lần gây cắt control ở DPI cao.
            this.AutoScaleMode = AutoScaleMode.Inherit;
            this.Load += (s, e) => InitScreen();
        }

        // ===== Các điểm mở rộng cho lớp con =====
        protected virtual string ScreenTitle => "Quản Lý";
        protected virtual string SearchTitle => "Tìm kiếm";
        protected virtual string[] SearchByOptions => new[] { "Mã", "Tên" };
        protected virtual int InfoHeight => 230;
        // Nếu true: các trường luôn cho nhập, nút Thêm/Sửa thực thi ngay (không cần Lưu).
        protected virtual bool ImmediateActions => false;
        // Nhãn nút hành động (lớp con có thể override; null = ẩn nút)
        protected virtual string LabelThem => "Thêm";
        protected virtual string LabelSua => "Sửa";
        protected virtual string LabelXoa => "Xóa";
        protected virtual string LabelLuu => "Lưu";
        protected virtual string LabelHuy => "Hủy";
        protected virtual void BuildFields() { }
        protected virtual void BuildGridColumns() { }
        protected virtual void LoadData() { }
        protected virtual void DoAdd() { }
        protected virtual void DoEdit() { }
        protected virtual void DoDelete() { }
        protected virtual void DoSearch(int byIndex, string keyword) { }
        protected virtual void OnRowSelected(DataGridViewRow row) { }

        private bool _built;
        protected void InitScreen()
        {
            if (_built) return;
            _built = true;
            BuildLayout();
            BuildFields();
            BuildGridColumns();
            LoadData();
            SetMode(Mode.View);
            ThemeManager.Apply(this);
        }

        #region Dựng layout theo đặc tả
        private void BuildLayout()
        {
            // ---- Section 5: Grid (Fill, thêm trước để nằm dưới cùng theo Dock) ----
            var grpGrid = MakeGroup("Danh sách");
            grpGrid.Dock = DockStyle.Fill;
            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ScrollBars = ScrollBars.Both,
                RowHeadersVisible = false
            };
            grid.SelectionChanged += Grid_SelectionChanged;
            grpGrid.Controls.Add(grid);

            // ---- Section 1: Tìm kiếm (dùng TableLayoutPanel tự co giãn, không đặt vị trí cứng) ----
            var grpSearch = MakeGroup(SearchTitle);
            grpSearch.Dock = DockStyle.Top;
            grpSearch.AutoSize = true;
            grpSearch.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            grpSearch.MinimumSize = new Size(0, 96);

            var searchTable = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                Padding = new Padding(10, 6, 10, 6)
            };
            searchTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            searchTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var grpBy = new GroupBox
            {
                Text = "Tìm Theo",
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(3, 3, 12, 3),
                ForeColor = AppTheme.TextPrimary
            };
            radioPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(6, 6, 6, 6)
            };
            for (int i = 0; i < SearchByOptions.Length; i++)
            {
                var rb = new RadioButton
                {
                    Text = SearchByOptions[i],
                    AutoSize = true,
                    Margin = new Padding(6, 6, 12, 6),
                    Checked = i == 0
                };
                SearchRadios.Add(rb);
                radioPanel.Controls.Add(rb);
            }
            grpBy.Controls.Add(radioPanel);

            var rightBox = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Dock = DockStyle.Fill,
                Padding = new Padding(6, 6, 0, 0)
            };
            var lblSearch = new Label
            {
                Text = "Nhập thông tin cần Tìm Kiếm",
                AutoSize = true,
                Margin = new Padding(3, 3, 3, 2),
                ForeColor = AppTheme.TextMuted
            };
            txtSearch = new TextBox { Width = 320, Font = AppTheme.Base, Margin = new Padding(3) };
            txtSearch.TextChanged += (s, e) =>
            {
                int idx = SearchRadios.FindIndex(r => r.Checked);
                DoSearch(idx < 0 ? 0 : idx, txtSearch.Text.Trim());
            };
            rightBox.Controls.Add(lblSearch);
            rightBox.Controls.Add(txtSearch);

            searchTable.Controls.Add(grpBy, 0, 0);
            searchTable.Controls.Add(rightBox, 1, 0);
            grpSearch.Controls.Add(searchTable);

            // ---- Section 2: Home / Load ----
            butHome = MakeButton("Home", "home");
            butLoad = MakeButton("Load Danh Sách", "load");
            butHome.Click += (s, e) => OnHome();
            butLoad.Click += (s, e) => { txtSearch.Clear(); LoadData(); SetMode(Mode.View); };
            var navHost = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(0, 6, 0, 6) };
            navHost.Controls.Add(butHome);
            navHost.Controls.Add(butLoad);
            // căn giữa 2 nút
            navHost.Layout += (s, e) => CenterFlow(navHost);

            // ---- Section 3: Thông tin (2 cột) ----
            var grpInfo = MakeGroup("Thông tin");
            grpInfo.Dock = DockStyle.Top;
            grpInfo.AutoSize = true;
            grpInfo.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            grpInfo.MinimumSize = new Size(0, 120);
            infoTable = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 4,
                Padding = new Padding(10, 8, 10, 8)
            };
            infoTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            infoTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            infoTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            infoTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            grpInfo.Controls.Add(infoTable);

            // ---- Section 4: 5 nút hành động ----
            var actionHost = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(0, 4, 0, 8) };
            butThem = MakeButton("Thêm", "them");
            butSua = MakeButton("Sửa", "sua");
            butXoa = MakeButton("Xóa", "xoa");
            butLuu = MakeButton("Lưu", "luu");
            butHuy = MakeButton("Hủy", "huy");
            butThem.Click += (s, e) => BeginAdd();
            butSua.Click += (s, e) => BeginEdit();
            butXoa.Click += (s, e) => { DoDelete(); LoadData(); SetMode(Mode.View); };
            butLuu.Click += (s, e) => Save();
            butHuy.Click += (s, e) => { SetMode(Mode.View); ReselectGrid(); };
            actionHost.Controls.AddRange(new Control[] { butThem, butSua, butXoa, butLuu, butHuy });
            actionHost.Layout += (s, e) => CenterFlow(actionHost);

            // Cho phép lớp con đổi nhãn nút / ẩn nút
            RelabelButton(butThem, LabelThem);
            RelabelButton(butSua, LabelSua);
            RelabelButton(butXoa, LabelXoa);
            RelabelButton(butLuu, LabelLuu);
            RelabelButton(butHuy, LabelHuy);

            // ---- Ghép theo thứ tự (Dock Top xếp ngược, add từ dưới lên) ----
            this.Controls.Add(grpGrid);     // Fill - dưới cùng
            this.Controls.Add(actionHost);  // Top
            this.Controls.Add(grpInfo);     // Top
            this.Controls.Add(navHost);     // Top
            this.Controls.Add(grpSearch);   // Top - trên cùng
        }

        private void RelabelButton(Button b, string label)
        {
            if (label == null) { b.Visible = false; return; }
            // Gi? icon Unicode (BMP: 1 ky t?, surrogate: 2 ky t?)
            string iconPrefix = "";
            if (b.Text.Length > 0 && (b.Text[0] > 127 || char.IsSurrogate(b.Text[0])))
            {
                iconPrefix = char.IsSurrogate(b.Text[0]) && b.Text.Length >= 2
                    ? b.Text.Substring(0, 2)
                    : b.Text[0].ToString();
            }
            b.Text = iconPrefix + "  " + label;
            using (Graphics g = b.CreateGraphics())
            {
                SizeF sz = g.MeasureString(b.Text, b.Font);
                int newWidth = (int)Math.Ceiling(sz.Width) + b.Padding.Horizontal + 20;
                b.Width = Math.Max(b.Width, newWidth);
            }
        }

        private GroupBox MakeGroup(string text)
        {
            return new GroupBox
            {
                Text = text,
                Font = new Font(AppTheme.FontFamily, 10.5F, FontStyle.Bold),
                ForeColor = AppTheme.Primary,
                Padding = new Padding(6)
            };
        }

        // Icon Unicode chuyen nghiep (dung escape sequence de bao toan qua moi encoding file)
        private static string IconFor(string key)
        {
            // THU TU QUAN TRONG: "Home"/"Ho" truoc "H" (Huy), "Thu"/"Tu" truoc "Th" (Them)
            if (key.Contains("Ho") || key.Contains("home"))  return "\U0001F3E0"; // ngoi nha
            if (key.Contains("Lo") || key.Contains("load") || key.Contains("refresh")) return "\U0001F504"; // lam moi
            if (key.Contains("Tu") || key.Contains("thu"))  return "\U0001F4B0"; // tui tien
            if (key.Contains("Th") || key.Contains("them") || key.Contains("add")  || key.Contains("Mu")) return "\u2795";   // +
            if (key.Contains("S")  || key.Contains("sua") || key.Contains("edit") || key.Contains("Tr")) return "\u270F\uFE0F"; // but chi
            if (key.Contains("X")  || key.Contains("xoa") || key.Contains("del")) return "\U0001F5D1\uFE0F"; // thung rac
            if (key.Contains("Lu") || key.Contains("luu") || key.Contains("save")) return "\U0001F4BE"; // dia mem
            if (key.Contains("H")  || key.Contains("huy") || key.Contains("cancel")) return "\u274C";   // X
            if (key.Contains("Ti") || key.Contains("tim") || key.Contains("search")) return "\U0001F50D"; // kinh lup
            return "";
        }

        private Button MakeButton(string text, string iconKey)
        {
            string icon = IconFor(iconKey);
            var b = new Button
            {
                Text = icon + "  " + text,
                AutoSize = false,
                Width = (text.Length > 6) ? 170 : 140,
                Height = 38,
                Margin = new Padding(8, 6, 8, 6),
                Padding = new Padding(4, 0, 6, 0),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = AppTheme.Primary,
                BackColor = AppTheme.Surface,
                Font = new Font(AppTheme.FontFamily, 11F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.BorderColor = AppTheme.Border;
            b.FlatAppearance.MouseOverBackColor = AppTheme.Hover;
            b.FlatAppearance.MouseDownBackColor = AppTheme.Selection;
            RoundedCorner.Apply(b, 6);
            return b;
        }

        private void CenterFlow(FlowLayoutPanel host)
        {
            int total = 0;
            foreach (Control c in host.Controls) total += c.Width + c.Margin.Left + c.Margin.Right;
            int left = Math.Max(0, (host.ClientSize.Width - total) / 2);
            host.Padding = new Padding(left, host.Padding.Top, 0, host.Padding.Bottom);
        }
        #endregion

        #region Trường nhập liệu
        /// <summary>
        /// Lớp con gọi để thêm trường; tự động phân bổ vào 2 cột (trái/phải).
        /// </summary>
        protected void AddField(CrudField f)
        {
            int index = Fields.Count;
            int col = index % 2 == 0 ? 0 : 2; // chẵn -> cột trái, lẻ -> cột phải
            int row = index / 2;

            // Đảm bảo có RowStyle đủ cao cho hàng này (tránh cắt ở DPI cao).
            while (infoTable.RowStyles.Count <= row)
                infoTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            if (infoTable.RowCount <= row) infoTable.RowCount = row + 1;

            var lbl = new Label
            {
                Text = f.Label,
                AutoSize = false,
                Dock = DockStyle.Fill,
                Height = 34,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = AppTheme.TextPrimary
            };

            Control input;
            if (f.IsCombo)
            {
                var cb = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Font = AppTheme.Base, Margin = new Padding(3, 6, 3, 6) };
                if (f.ComboSource != null)
                    foreach (var it in f.ComboSource()) cb.Items.Add(it);
                input = cb;
            }
            else if (f.IsDate)
            {
                input = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short, Font = AppTheme.Base, Margin = new Padding(3, 6, 3, 6) };
            }
            else
            {
                input = new TextBox { Dock = DockStyle.Fill, ReadOnly = f.ReadOnly, Font = AppTheme.Base, Margin = new Padding(3, 6, 3, 6) };
            }
            f.Input = input;
            Fields.Add(f);

            infoTable.Controls.Add(lbl, col, row);
            infoTable.Controls.Add(input, col + 1, row);
        }

        protected string GetText(string key)
        {
            var f = Fields.Find(x => x.Key == key);
            if (f == null) return "";
            if (f.Input is ComboBox cb) return cb.SelectedItem?.ToString() ?? "";
            if (f.Input is DateTimePicker dp) return dp.Value.ToShortDateString();
            return f.Input.Text.Trim();
        }

        protected DateTime GetDate(string key)
        {
            var f = Fields.Find(x => x.Key == key);
            if (f?.Input is DateTimePicker dp) return dp.Value;
            return DateTime.Now;
        }

        protected int GetComboIndex(string key)
        {
            var f = Fields.Find(x => x.Key == key);
            if (f?.Input is ComboBox cb) return cb.SelectedIndex;
            return -1;
        }

        protected void SetText(string key, string val)
        {
            var f = Fields.Find(x => x.Key == key);
            if (f == null) return;
            if (f.Input is ComboBox cb) cb.Text = val;
            else if (f.Input is DateTimePicker dp)
            {
                if (DateTime.TryParse(val, out var d)) dp.Value = d;
            }
            else f.Input.Text = val;
        }

        protected void SetComboIndex(string key, int idx)
        {
            var f = Fields.Find(x => x.Key == key);
            if (f?.Input is ComboBox cb && idx >= 0 && idx < cb.Items.Count) cb.SelectedIndex = idx;
        }

        protected void ClearInputs()
        {
            foreach (var f in Fields)
            {
                if (f.Input is ComboBox cb) cb.SelectedIndex = -1;
                else if (f.Input is DateTimePicker dp) dp.Value = DateTime.Now;
                else f.Input.Text = "";
            }
        }
        #endregion

        #region Điều khiển trạng thái
        protected void SetMode(Mode m)
        {
            CurrentMode = m;
            if (ImmediateActions)
            {
                // Chế độ thao tác tức thì: luôn cho nhập, chỉ bật Thêm/Sửa/Xóa.
                foreach (var f in Fields) f.Input.Enabled = !f.ReadOnly;
                butThem.Enabled = butSua.Enabled = butXoa.Enabled = true;
                butLuu.Enabled = butHuy.Enabled = false;
                AfterSetMode(m);
                return;
            }
            bool editing = m != Mode.View;
            foreach (var f in Fields)
            {
                bool enable = editing && !(f.ReadOnly);
                f.Input.Enabled = enable;
            }
            butThem.Enabled = butSua.Enabled = butXoa.Enabled = (m == Mode.View);
            butLuu.Enabled = butHuy.Enabled = editing;
            AfterSetMode(m);
        }

        /// <summary>Cho lớp con tinh chỉnh trạng thái control theo mode (vd: bật ô chỉ khi Thêm).</summary>
        protected virtual void AfterSetMode(Mode m) { }

        /// <summary>Bật/tắt một trường theo key (bỏ qua thuộc tính ReadOnly mặc định).</summary>
        protected void SetFieldEnabled(string key, bool enabled)
        {
            var f = Fields.Find(x => x.Key == key);
            if (f != null) f.Input.Enabled = enabled;
        }

        private void BeginAdd()
        {
            if (ImmediateActions) { DoAdd(); LoadData(); SetMode(Mode.View); return; }
            ClearInputs();
            SelectedId = -1;
            SetMode(Mode.Add);
        }

        private void BeginEdit()
        {
            if (ImmediateActions) { DoEdit(); LoadData(); SetMode(Mode.View); return; }
            if (SelectedId == -1)
            {
                MessageBox.Show("Vui lòng chọn một dòng để sửa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SetMode(Mode.Edit);
        }

        private void Save()
        {
            if (CurrentMode == Mode.Add) DoAdd();
            else if (CurrentMode == Mode.Edit) DoEdit();
            LoadData();
            SetMode(Mode.View);
        }

        protected virtual void OnHome()
        {
            // Nút "Home" trong module -> yêu cầu dashboard quay về trang chủ.
            var form = this.FindForm();
            if (form is fDashboard dash) dash.GoHome();
            else if (Parent != null) Parent.Controls.Clear();
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null) return;
            OnRowSelected(grid.CurrentRow);
        }

        private void ReselectGrid()
        {
            if (grid.CurrentRow != null) OnRowSelected(grid.CurrentRow);
        }
        #endregion
    }
}
