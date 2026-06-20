using System;
using System.Drawing;
using System.Windows.Forms;
using ConvenienceStoreApp.BLL;
using ConvenienceStoreApp.Models;

namespace ConvenienceStoreApp.Forms
{
    public class CustomerForm : Form
    {
        private SplitContainer splitContainer;
        
        // Customer list (Top panel)
        private GroupBox grpCustomers;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnAdd;
        private Button btnEdit;
        private DataGridView dgvCustomers;

        // Transaction history (Bottom panel)
        private GroupBox grpHistory;
        private DataGridView dgvHistory;

        // Editor overlay panel
        private Panel pnlEditor;
        private Label lblEditorTitle;
        private TextBox txtName;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private CheckBox chkActive;
        private Button btnSave;
        private Button btnCancel;
        private int editingCustomerId = -1;
        private readonly CustomerService customerService = new CustomerService();

        public CustomerForm()
        {
            InitializeComponent();
            LoadCustomers();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Khách Hàng Thành Viên";
            this.BackColor = Color.FromArgb(240, 244, 248);

            splitContainer = new SplitContainer();
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Orientation = Orientation.Horizontal;
            splitContainer.SplitterDistance = 450;

            // --- TOP PANEL: CUSTOMERS GRID ---
            grpCustomers = new GroupBox();
            grpCustomers.Text = "Thành Viên Tích Điểm";
            grpCustomers.Font = new Font("Segoe UI", 9.75f, FontStyle.Bold);
            grpCustomers.ForeColor = Color.FromArgb(44, 62, 80);
            grpCustomers.Dock = DockStyle.Fill;
            grpCustomers.Padding = new Padding(10);

            Panel pnlFilter = new Panel();
            pnlFilter.Dock = DockStyle.Top;
            pnlFilter.Height = 45;

            txtSearch = new TextBox();
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.Size = new Size(250, 25);
            txtSearch.Location = new Point(5, 10);
            txtSearch.KeyDown += TxtSearch_KeyDown;

            btnSearch = new Button() { Text = "Tìm kiếm", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), BackColor = Color.FromArgb(52, 73, 94), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(90, 26), Location = new Point(265, 9), Cursor = Cursors.Hand };
            btnSearch.Click += BtnSearch_Click;

            btnAdd = new Button() { Text = "➕ Đăng Ký Mới", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), BackColor = Color.FromArgb(26, 188, 156), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(130, 26), Location = new Point(365, 9), Cursor = Cursors.Hand };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button() { Text = "✏️ Sửa Thành Viên", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(140, 26), Location = new Point(505, 9), Cursor = Cursors.Hand };
            btnEdit.Click += BtnEdit_Click;

            pnlFilter.Controls.Add(txtSearch);
            pnlFilter.Controls.Add(btnSearch);
            pnlFilter.Controls.Add(btnAdd);
            pnlFilter.Controls.Add(btnEdit);

            dgvCustomers = new DataGridView();
            dgvCustomers.Dock = DockStyle.Fill;
            dgvCustomers.BackgroundColor = Color.White;
            dgvCustomers.BorderStyle = BorderStyle.None;
            dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCustomers.AllowUserToAddRows = false;
            dgvCustomers.ReadOnly = true;
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.RowHeadersVisible = false;
            dgvCustomers.Font = new Font("Segoe UI", 9.5f);
            dgvCustomers.SelectionChanged += DgvCustomers_SelectionChanged;

            grpCustomers.Controls.Add(dgvCustomers);
            grpCustomers.Controls.Add(pnlFilter);
            splitContainer.Panel1.Controls.Add(grpCustomers);

            // --- BOTTOM PANEL: TRANSACTION HISTORY GRID ---
            grpHistory = new GroupBox();
            grpHistory.Text = "Lịch Sử Điểm Tích Lũy (Loyalty Log)";
            grpHistory.Font = new Font("Segoe UI", 9.75f, FontStyle.Bold);
            grpHistory.ForeColor = Color.FromArgb(44, 62, 80);
            grpHistory.Dock = DockStyle.Fill;
            grpHistory.Padding = new Padding(10);

            dgvHistory = new DataGridView();
            dgvHistory.Dock = DockStyle.Fill;
            dgvHistory.BackgroundColor = Color.White;
            dgvHistory.BorderStyle = BorderStyle.None;
            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistory.AllowUserToAddRows = false;
            dgvHistory.ReadOnly = true;
            dgvHistory.RowHeadersVisible = false;
            dgvHistory.Font = new Font("Segoe UI", 9.5f);

            grpHistory.Controls.Add(dgvHistory);
            splitContainer.Panel2.Controls.Add(grpHistory);

            // Setup Editor panel
            SetupEditorPanel();

            this.Controls.Add(splitContainer);
            splitContainer.BringToFront();
        }

        private void SetupEditorPanel()
        {
            pnlEditor = new Panel();
            pnlEditor.Size = new Size(380, 320);
            pnlEditor.BackColor = Color.White;
            pnlEditor.BorderStyle = BorderStyle.FixedSingle;
            pnlEditor.Visible = false;

            lblEditorTitle = new Label();
            lblEditorTitle.Text = "ĐĂNG KÝ THÀNH VIÊN MỚI";
            lblEditorTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblEditorTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblEditorTitle.Size = new Size(360, 25);
            lblEditorTitle.Location = new Point(10, 15);
            lblEditorTitle.TextAlign = ContentAlignment.MiddleCenter;

            int rowY = 55, spacing = 45, labelX = 25, inputX = 130, inputWidth = 220;

            CreateEditorLabel("Họ tên:", rowY, labelX);
            txtName = CreateEditorTextBox(rowY, inputX, inputWidth);
            rowY += spacing;

            CreateEditorLabel("Số điện thoại:", rowY, labelX);
            txtPhone = CreateEditorTextBox(rowY, inputX, inputWidth);
            rowY += spacing;

            CreateEditorLabel("Email:", rowY, labelX);
            txtEmail = CreateEditorTextBox(rowY, inputX, inputWidth);
            rowY += spacing;

            chkActive = new CheckBox();
            chkActive.Text = "Thành viên hoạt động (Active)";
            chkActive.Font = new Font("Segoe UI", 9.5f);
            chkActive.Checked = true;
            chkActive.Location = new Point(inputX, rowY);
            chkActive.Size = new Size(220, 20);
            pnlEditor.Controls.Add(chkActive);
            rowY += 35;

            btnSave = new Button() { Text = "Lưu lại", BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(90, 32), Location = new Point(130, rowY) };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button() { Text = "Hủy bỏ", BackColor = Color.FromArgb(189, 195, 199), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(90, 32), Location = new Point(235, rowY) };
            btnCancel.Click += BtnCancel_Click;

            pnlEditor.Controls.Add(lblEditorTitle);
            pnlEditor.Controls.Add(btnSave);
            pnlEditor.Controls.Add(btnCancel);

            this.Controls.Add(pnlEditor);
        }

        private void CreateEditorLabel(string text, int top, int left)
        {
            Label lbl = new Label() { Text = text, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(71, 84, 103), Location = new Point(left, top + 3), AutoSize = true };
            pnlEditor.Controls.Add(lbl);
        }

        private TextBox CreateEditorTextBox(int top, int left, int width)
        {
            TextBox txt = new TextBox() { Font = new Font("Segoe UI", 9.5f), Size = new Size(width, 25), Location = new Point(left, top) };
            pnlEditor.Controls.Add(txt);
            return txt;
        }

        // --- DATA LOADING ---
        private void LoadCustomers()
        {
            try
            {
                dgvCustomers.DataSource = customerService.SearchCustomers(txtSearch.Text.Trim());
                dgvCustomers.Columns["Id"].Visible = false;
                dgvCustomers.Columns["TongChiTieu"].DefaultCellStyle.Format = "N0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải khách hàng: " + ex.Message);
            }
        }

        private void LoadLoyaltyHistory(int customerId)
        {
            try
            {
                dgvHistory.DataSource = customerService.GetLoyaltyHistory(customerId);
                dgvHistory.Columns["LogId"].Visible = false;
            }
            catch { }
        }

        // --- SEARCH EVENTS ---
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadCustomers();
                e.SuppressKeyPress = true;
            }
        }

        // --- SELECTION EVENT ---
        private void DgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow != null)
            {
                int customerId = Convert.ToInt32(dgvCustomers.CurrentRow.Cells["Id"].Value);
                LoadLoyaltyHistory(customerId);
            }
            else
            {
                dgvHistory.DataSource = null;
            }
        }

        // --- CRUD ACTIONS ---
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            editingCustomerId = -1;
            lblEditorTitle.Text = "ĐĂNG KÝ THÀNH VIÊN MỚI";
            txtName.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            chkActive.Checked = true;

            pnlEditor.Location = new Point((this.Width - pnlEditor.Width) / 2, (this.Height - pnlEditor.Height) / 2);
            pnlEditor.Visible = true;
            pnlEditor.BringToFront();
            txtName.Focus();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần sửa.", "Chọn khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            editingCustomerId = Convert.ToInt32(dgvCustomers.CurrentRow.Cells["Id"].Value);

            try
            {
                Customer customer = customerService.GetCustomer(editingCustomerId);
                if (customer != null)
                {
                    txtName.Text = customer.Name;
                    txtPhone.Text = customer.Phone;
                    txtEmail.Text = customer.Email;
                    chkActive.Checked = customer.IsActive;

                    lblEditorTitle.Text = "SỬA HỘI VIÊN KHÓA #" + editingCustomerId;
                    pnlEditor.Location = new Point((this.Width - pnlEditor.Width) / 2, (this.Height - pnlEditor.Height) / 2);
                    pnlEditor.Visible = true;
                    pnlEditor.BringToFront();
                    txtName.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy thông tin: " + ex.Message);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string email = txtEmail.Text.Trim();

            try
            {
                customerService.SaveCustomer(editingCustomerId, name, phone, email, chkActive.Checked);
                pnlEditor.Visible = false;
                LoadCustomers();
                MessageBox.Show("Lưu thông tin khách hàng thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi dữ liệu. SĐT phải là duy nhất. Chi tiết lỗi: " + ex.Message, "Lỗi ghi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            pnlEditor.Visible = false;
        }
    }
}

