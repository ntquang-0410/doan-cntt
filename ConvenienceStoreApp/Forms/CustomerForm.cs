using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

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
            grpCustomers.Font = new Font("Segoe UI", 9.75pt, FontStyle.Bold);
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
            txtSearch.PlaceholderText = "Nhập tên hoặc số điện thoại...";
            txtSearch.KeyDown += TxtSearch_KeyDown;

            btnSearch = new Button() { Text = "Tìm kiếm", Font = new Font("Segoe UI", 9.5pt, FontStyle.Bold), BackColor = Color.FromArgb(52, 73, 94), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(90, 26), Location = new Point(265, 9), Cursor = Cursors.Hand };
            btnSearch.Click += BtnSearch_Click;

            btnAdd = new Button() { Text = "➕ Đăng Ký Mới", Font = new Font("Segoe UI", 9.5pt, FontStyle.Bold), BackColor = Color.FromArgb(26, 188, 156), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(130, 26), Location = new Point(365, 9), Cursor = Cursors.Hand };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button() { Text = "✏️ Sửa Thành Viên", Font = new Font("Segoe UI", 9.5pt, FontStyle.Bold), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(140, 26), Location = new Point(505, 9), Cursor = Cursors.Hand };
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
            dgvCustomers.Font = new Font("Segoe UI", 9.5pt);
            dgvCustomers.SelectionChanged += DgvCustomers_SelectionChanged;

            grpCustomers.Controls.Add(dgvCustomers);
            grpCustomers.Controls.Add(pnlFilter);
            splitContainer.Panel1.Controls.Add(grpCustomers);

            // --- BOTTOM PANEL: TRANSACTION HISTORY GRID ---
            grpHistory = new GroupBox();
            grpHistory.Text = "Lịch Sử Điểm Tích Lũy (Loyalty Log)";
            grpHistory.Font = new Font("Segoe UI", 9.75pt, FontStyle.Bold);
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
            dgvHistory.Font = new Font("Segoe UI", 9.5pt);

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
            chkActive.Font = new Font("Segoe UI", 9.5pt);
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
            Label lbl = new Label() { Text = text, Font = new Font("Segoe UI", 9.5pt), ForeColor = Color.FromArgb(71, 84, 103), Location = new Point(left, top + 3), AutoSize = true };
            pnlEditor.Controls.Add(lbl);
        }

        private TextBox CreateEditorTextBox(int top, int left, int width)
        {
            TextBox txt = new TextBox() { Font = new Font("Segoe UI", 9.5pt), Size = new Size(width, 25), Location = new Point(left, top) };
            pnlEditor.Controls.Add(txt);
            return txt;
        }

        // --- DATA LOADING ---
        private void LoadCustomers()
        {
            try
            {
                string sql = "SELECT id, name as HoTen, phone as SoDienThoai, email as Email, loyalty_points as DiemTichLuy, total_spent as TongChiTieu, IF(is_active = 1, 'Hoạt động', 'Ngưng') as TrangThai FROM customers WHERE 1=1";
                List<MySqlParameter> prs = new List<MySqlParameter>();

                string search = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(search))
                {
                    sql += " AND (name LIKE @kw OR phone = @kwExact)";
                    prs.Add(new MySqlParameter("@kw", "%" + search + "%"));
                    prs.Add(new MySqlParameter("@kwExact", search));
                }

                sql += " ORDER BY id DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql, prs.ToArray());
                dgvCustomers.DataSource = dt;
                dgvCustomers.Columns["id"].Visible = false;
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
                string sql = @"
                    SELECT lt.id as LogId, lt.order_id as DonHang, lt.points as SoDiem, 
                           CASE lt.transaction_type 
                               WHEN 'earn' THEN 'Cộng điểm (+)' 
                               WHEN 'redeem' THEN 'Trừ điểm (-)' 
                               WHEN 'expire' THEN 'Hết hạn' 
                               ELSE 'Điều chỉnh' 
                           END as LoaiGiaoDich, 
                           lt.description as MoTa, lt.created_at as ThoiGian
                    FROM loyalty_transactions lt
                    WHERE lt.customer_id = @cid
                    ORDER BY lt.created_at DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql, new MySqlParameter("@cid", customerId));
                dgvHistory.DataSource = dt;
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
                int customerId = Convert.ToInt32(dgvCustomers.CurrentRow.Cells["id"].Value);
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

            editingCustomerId = Convert.ToInt32(dgvCustomers.CurrentRow.Cells["id"].Value);

            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM customers WHERE id = @id", new MySqlParameter("@id", editingCustomerId));
                if (dt.Rows.Count > 0)
                {
                    DataRow r = dt.Rows[0];
                    txtName.Text = r["name"].ToString();
                    txtPhone.Text = r["phone"].ToString();
                    txtEmail.Text = r["email"].ToString();
                    chkActive.Checked = Convert.ToBoolean(r["is_active"]);

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
            int active = chkActive.Checked ? 1 : 0;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Vui lòng điền Họ tên và Số điện thoại khách hàng.");
                return;
            }

            try
            {
                if (editingCustomerId == -1)
                {
                    // Create customer
                    string sql = "INSERT INTO customers (name, phone, email, is_active) VALUES (@name, @phone, @email, @act)";
                    MySqlParameter[] prs = new MySqlParameter[] {
                        new MySqlParameter("@name", name),
                        new MySqlParameter("@phone", phone),
                        new MySqlParameter("@email", string.IsNullOrEmpty(email) ? DBNull.Value : (object)email),
                        new MySqlParameter("@act", active)
                    };
                    DatabaseHelper.ExecuteNonQuery(sql, prs);
                }
                else
                {
                    // Update customer
                    string sql = "UPDATE customers SET name = @name, phone = @phone, email = @email, is_active = @act WHERE id = @id";
                    MySqlParameter[] prs = new MySqlParameter[] {
                        new MySqlParameter("@name", name),
                        new MySqlParameter("@phone", phone),
                        new MySqlParameter("@email", string.IsNullOrEmpty(email) ? DBNull.Value : (object)email),
                        new MySqlParameter("@act", active),
                        new MySqlParameter("@id", editingCustomerId)
                    };
                    DatabaseHelper.ExecuteNonQuery(sql, prs);
                }

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
