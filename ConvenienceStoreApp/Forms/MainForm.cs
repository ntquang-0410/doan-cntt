using System;
using System.Drawing;
using System.Windows.Forms;

namespace ConvenienceStoreApp.Forms
{
    public class MainForm : Form
    {
        private Panel sidebarPanel;
        private Panel headerPanel;
        private Panel contentPanel;
        
        private Label lblLogo;
        private Label lblUserStatus;
        private Button btnLogout;
        
        // Navigation Buttons
        private Button btnPOS;
        private Button btnShift;
        private Button btnCustomers;
        private Button btnProducts;
        private Button btnInventory;
        private Button btnPromotions;
        private Button btnEmployees;
        private Button btnReports;

        private Button activeNavButton = null;

        public MainForm()
        {
            InitializeComponent();
            LoadDefaultScreen();
        }

        private void InitializeComponent()
        {
            this.Text = "Hệ Thống Quản Lý Cửa Hàng Tiện Lợi";
            this.Size = new Size(1280, 800);
            this.MinimumSize = new Size(1024, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 246, 250);

            // 1. Sidebar Panel
            sidebarPanel = new Panel();
            sidebarPanel.Width = 230;
            sidebarPanel.Dock = DockStyle.Left;
            sidebarPanel.BackColor = Color.FromArgb(44, 62, 80); // Slate blue
            
            lblLogo = new Label();
            lblLogo.Text = "🏪 CỬA HÀNG ABC";
            lblLogo.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblLogo.ForeColor = Color.White;
            lblLogo.Size = new Size(230, 60);
            lblLogo.Location = new Point(0, 0);
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            lblLogo.BackColor = Color.FromArgb(52, 73, 94); // Darker tone
            sidebarPanel.Controls.Add(lblLogo);

            // Create Sidebar Navigation buttons
            int startY = 80;
            int btnHeight = 45;
            int spacing = 5;

            btnPOS = CreateNavButton("🛒 Bán Hàng (POS)", startY, () => ShowChildForm(new POSForm()));
            startY += btnHeight + spacing;

            btnShift = CreateNavButton("⏰ Ca Làm Việc", startY, () => ShowChildForm(new DailyShiftForm()));
            startY += btnHeight + spacing;

            btnCustomers = CreateNavButton("👥 Khách Hàng", startY, () => ShowChildForm(new CustomerForm()));
            startY += btnHeight + spacing;

            // Restrict management modules to Managers and Admins
            if (SessionManager.IsManager)
            {
                btnProducts = CreateNavButton("📦 Sản Phẩm", startY, () => ShowChildForm(new ProductManagementForm()));
                startY += btnHeight + spacing;

                btnInventory = CreateNavButton("🗄️ Tồn Kho & Nhập Hàng", startY, () => ShowChildForm(new InventoryForm()));
                startY += btnHeight + spacing;

                btnPromotions = CreateNavButton("🎁 Khuyến Mãi", startY, () => ShowChildForm(new PromotionForm()));
                startY += btnHeight + spacing;
            }

            // Restrict Employee Management to Admins only
            if (SessionManager.IsAdmin)
            {
                btnEmployees = CreateNavButton("👤 Nhân Viên", startY, () => ShowChildForm(new EmployeeForm()));
                startY += btnHeight + spacing;
            }

            // Restrict Reports/Dashboards to Managers and Admins
            if (SessionManager.IsManager)
            {
                btnReports = CreateNavButton("📈 Báo Cáo Thống Kê", startY, () => ShowChildForm(new ReportForm()));
                startY += btnHeight + spacing;
            }

            Panel logoutPanel = new Panel();
            logoutPanel.Dock = DockStyle.Bottom;
            logoutPanel.Height = 70;
            logoutPanel.BackColor = Color.FromArgb(44, 62, 80);

            btnLogout = new Button();
            btnLogout.Text = "↩ Đăng Xuất";
            btnLogout.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            btnLogout.ForeColor = Color.White;
            btnLogout.BackColor = Color.FromArgb(231, 76, 60);
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatAppearance.MouseDownBackColor = Color.FromArgb(192, 57, 43);
            btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(203, 67, 53);
            btnLogout.Size = new Size(210, 45);
            btnLogout.Location = new Point(10, 12);
            btnLogout.TextAlign = ContentAlignment.MiddleLeft;
            btnLogout.Padding = new Padding(15, 0, 0, 0);
            btnLogout.Cursor = Cursors.Hand;
            btnLogout.Click += BtnLogout_Click;

            logoutPanel.Controls.Add(btnLogout);
            sidebarPanel.Controls.Add(logoutPanel);
            logoutPanel.BringToFront();

            // 2. Header Panel
            headerPanel = new Panel();
            headerPanel.Height = 60;
            headerPanel.Dock = DockStyle.Top;
            headerPanel.BackColor = Color.White;
            headerPanel.Paint += HeaderPanel_Paint;

            lblUserStatus = new Label();
            lblUserStatus.Text = string.Format(
                "Xin chào, {0} ({1}){2}",
                SessionManager.FullName,
                GetRoleDisplayName(SessionManager.Role),
                SessionManager.IsDemoMode ? " - Chế độ demo giao diện" : "");
            lblUserStatus.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblUserStatus.ForeColor = Color.FromArgb(44, 62, 80);
            lblUserStatus.AutoSize = true;
            lblUserStatus.Location = new Point(20, 20);

            headerPanel.Controls.Add(lblUserStatus);

            // 3. Content Panel
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = Color.FromArgb(245, 246, 250);

            // Add Panels to Main Form
            this.Controls.Add(contentPanel);
            this.Controls.Add(headerPanel);
            this.Controls.Add(sidebarPanel);
        }

        private Button CreateNavButton(string text, int top, Action onClickAction)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Size = new Size(210, 45);
            btn.Location = new Point(10, top);
            btn.Font = new Font("Segoe UI", 10.5f, FontStyle.Regular);
            btn.ForeColor = Color.FromArgb(220, 224, 230);
            btn.BackColor = Color.Transparent;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(52, 73, 94);
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(48, 57, 82);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(15, 0, 0, 0);
            btn.Cursor = Cursors.Hand;
            btn.Click += (s, e) =>
            {
                try
                {
                    if (!CanLeaveCurrentChildForm())
                    {
                        return;
                    }

                    onClickAction();
                    HighlightNavButton(btn);
                }
                catch (Exception ex)
                {
                    ShowModuleError(text, ex);
                }
            };

            sidebarPanel.Controls.Add(btn);
            return btn;
        }

        private bool CanLeaveCurrentChildForm()
        {
            foreach (Control ctrl in contentPanel.Controls)
            {
                POSForm posForm = ctrl as POSForm;
                if (posForm != null && posForm.HasPendingCart)
                {
                    DialogResult dr = MessageBox.Show(
                        "POS đang có giỏ hàng chưa thanh toán. Nếu chuyển màn hình, đơn hiện tại sẽ bị hủy khỏi giao diện.\n\nBạn có muốn chuyển tiếp không?",
                        "Giỏ hàng chưa thanh toán",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    return dr == DialogResult.Yes;
                }
            }

            return true;
        }

        private void HighlightNavButton(Button selectedBtn)
        {
            if (activeNavButton != null)
            {
                activeNavButton.BackColor = Color.Transparent;
                activeNavButton.ForeColor = Color.FromArgb(220, 224, 230);
                activeNavButton.Font = new Font("Segoe UI", 10.5f, FontStyle.Regular);
            }

            activeNavButton = selectedBtn;
            activeNavButton.BackColor = Color.FromArgb(26, 188, 156); // Teal highlight
            activeNavButton.ForeColor = Color.White;
            activeNavButton.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
        }

        private void HeaderPanel_Paint(object sender, PaintEventArgs e)
        {
            // Bottom gray border line
            using (Pen pen = new Pen(Color.FromArgb(220, 224, 230), 1))
            {
                e.Graphics.DrawLine(pen, 0, headerPanel.Height - 1, headerPanel.Width, headerPanel.Height - 1);
            }
        }

        public void ShowChildForm(Form childForm)
        {
            try
            {
                // Clear content panel
                foreach (Control ctrl in contentPanel.Controls)
                {
                    Form form = ctrl as Form;
                    if (form != null)
                    {
                        form.Close();
                    }
                }
                contentPanel.Controls.Clear();

                // Set up child form properties
                childForm.TopLevel = false;
                childForm.FormBorderStyle = FormBorderStyle.None;
                childForm.Dock = DockStyle.Fill;
                
                contentPanel.Controls.Add(childForm);
                childForm.Show();
            }
            catch
            {
                if (childForm != null)
                {
                    childForm.Dispose();
                }
                throw;
            }
        }

        private void LoadDefaultScreen()
        {
            if (SessionManager.IsDemoMode)
            {
                ShowDemoHome();
                return;
            }

            // By default, open POS for Cashiers/Staff, or Report Dashboard for Manager/Admin
            if (SessionManager.IsManager)
            {
                if (btnReports != null)
                {
                    HighlightNavButton(btnReports);
                    SafeShowChildForm("Báo Cáo Thống Kê", () => new ReportForm());
                }
                else
                {
                    HighlightNavButton(btnPOS);
                    SafeShowChildForm("Bán Hàng (POS)", () => new POSForm());
                }
            }
            else
            {
                HighlightNavButton(btnPOS);
                SafeShowChildForm("Bán Hàng (POS)", () => new POSForm());
            }
        }

        private void SafeShowChildForm(string moduleName, Func<Form> childFormFactory)
        {
            try
            {
                Form childForm = childFormFactory();
                ShowChildForm(childForm);
            }
            catch (Exception ex)
            {
                ShowModuleError(moduleName, ex);
            }
        }

        private void ShowModuleError(string moduleName, Exception ex)
        {
            Panel errorPanel = new Panel();
            errorPanel.Dock = DockStyle.Fill;
            errorPanel.BackColor = Color.FromArgb(245, 246, 250);

            Label title = new Label();
            title.Text = "Không mở được " + moduleName;
            title.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(231, 76, 60);
            title.Size = new Size(800, 40);
            title.Location = new Point(40, 45);

            Label detail = new Label();
            detail.Text = ex.Message + "\n\n" + ex.StackTrace;
            detail.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            detail.ForeColor = Color.FromArgb(71, 84, 103);
            detail.Size = new Size(850, 180);
            detail.Location = new Point(42, 95);

            Label hint = new Label();
            hint.Text = "Kiểm tra kết nối MySQL, dữ liệu import và cấu hình App.config. Ứng dụng vẫn đang chạy, bạn có thể chọn module khác ở sidebar.";
            hint.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            hint.ForeColor = Color.FromArgb(127, 140, 141);
            hint.Size = new Size(850, 60);
            hint.Location = new Point(42, 285);

            errorPanel.Controls.Add(title);
            errorPanel.Controls.Add(detail);
            errorPanel.Controls.Add(hint);

            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(errorPanel);

            MessageBox.Show(
                "Không mở được " + moduleName + ":\n" + ex.Message + "\n\n" + ex.StackTrace,
                "Lỗi module",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void ShowDemoHome()
        {
            if (btnReports != null)
            {
                HighlightNavButton(btnReports);
            }
            else
            {
                HighlightNavButton(btnPOS);
            }

            Panel demoPanel = new Panel();
            demoPanel.Dock = DockStyle.Fill;
            demoPanel.BackColor = Color.FromArgb(245, 246, 250);

            Label title = new Label();
            title.Text = "GIAO DIỆN QUẢN LÝ CỬA HÀNG TIỆN LỢI";
            title.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            title.ForeColor = Color.FromArgb(44, 62, 80);
            title.Size = new Size(900, 45);
            title.Location = new Point(40, 45);

            Label subtitle = new Label();
            subtitle.Text = "Đang chạy ở chế độ demo giao diện. Có thể xem bố cục chính trước khi kết nối MySQL và chuyển dần sang mô hình 3 lớp.";
            subtitle.Font = new Font("Segoe UI", 10.5f, FontStyle.Regular);
            subtitle.ForeColor = Color.FromArgb(91, 105, 120);
            subtitle.Size = new Size(940, 30);
            subtitle.Location = new Point(42, 92);

            string[] modules = new string[]
            {
                "Bán hàng POS",
                "Ca làm việc",
                "Khách hàng",
                "Sản phẩm",
                "Tồn kho & nhập hàng",
                "Khuyến mãi",
                "Nhân viên",
                "Báo cáo"
            };

            int left = 42;
            int top = 150;
            int cardWidth = 230;
            int cardHeight = 95;

            for (int i = 0; i < modules.Length; i++)
            {
                Panel card = CreateDemoCard(modules[i], left + (i % 3) * (cardWidth + 20), top + (i / 3) * (cardHeight + 20), cardWidth, cardHeight);
                demoPanel.Controls.Add(card);
            }

            Label note = new Label();
            note.Text = "Bước tiếp theo: import database, xác thực đăng nhập thật, sau đó tách code sang BLL/DAL/Models để dễ bảo trì.";
            note.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            note.ForeColor = Color.FromArgb(127, 140, 141);
            note.Size = new Size(900, 30);
            note.Location = new Point(42, top + 3 * (cardHeight + 20) + 20);

            demoPanel.Controls.Add(title);
            demoPanel.Controls.Add(subtitle);
            demoPanel.Controls.Add(note);

            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(demoPanel);
        }

        private Panel CreateDemoCard(string title, int left, int top, int width, int height)
        {
            Panel card = new Panel();
            card.Size = new Size(width, height);
            card.Location = new Point(left, top);
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.FixedSingle;

            Label name = new Label();
            name.Text = title;
            name.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            name.ForeColor = Color.FromArgb(44, 62, 80);
            name.Location = new Point(15, 18);
            name.Size = new Size(width - 30, 24);

            Label status = new Label();
            status.Text = "Đã có form giao diện";
            status.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            status.ForeColor = Color.FromArgb(26, 188, 156);
            status.Location = new Point(15, 50);
            status.Size = new Size(width - 30, 22);

            card.Controls.Add(name);
            card.Controls.Add(status);
            return card;
        }

        private string GetRoleDisplayName(string role)
        {
            switch (role)
            {
                case "Admin": return "Quản trị viên";
                case "Manager": return "Quản lý";
                case "Cashier": return "Thu ngân";
                case "Staff": return "Nhân viên";
                default: return role;
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                SessionManager.Logout();
                this.Close();
            }
        }
    }
}

