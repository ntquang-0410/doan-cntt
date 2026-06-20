using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ConvenienceStoreApp.Forms
{
    public class EmployeeForm : Form
    {
        private Panel actionPanel;
        private DataGridView dgvEmployees;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnToggleStatus;

        // Editor overlay panel
        private Panel pnlEditor;
        private Label lblEditorTitle;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtFullName;
        private TextBox txtPhone;
        private ComboBox cboRole;
        private CheckBox chkActive;
        private Button btnSave;
        private Button btnCancel;
        private Label lblPasswordHelp;
        private int editingUserId = -1;

        public EmployeeForm()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Nhân Viên";
            this.BackColor = Color.FromArgb(240, 244, 248);

            actionPanel = new Panel();
            actionPanel.Dock = DockStyle.Top;
            actionPanel.Height = 50;

            btnAdd = new Button();
            btnAdd.Text = "➕ Thêm Nhân Viên";
            btnAdd.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnAdd.BackColor = Color.FromArgb(26, 188, 156);
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Size = new Size(150, 30);
            btnAdd.Location = new Point(15, 10);
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button();
            btnEdit.Text = "✏️ Sửa Thông Tin";
            btnEdit.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnEdit.BackColor = Color.FromArgb(52, 152, 219);
            btnEdit.ForeColor = Color.White;
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Size = new Size(130, 30);
            btnEdit.Location = new Point(175, 10);
            btnEdit.Cursor = Cursors.Hand;
            btnEdit.Click += BtnEdit_Click;

            btnToggleStatus = new Button();
            btnToggleStatus.Text = "🔒 Khóa/Mở Khóa";
            btnToggleStatus.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnToggleStatus.BackColor = Color.FromArgb(127, 140, 141);
            btnToggleStatus.ForeColor = Color.White;
            btnToggleStatus.FlatStyle = FlatStyle.Flat;
            btnToggleStatus.FlatAppearance.BorderSize = 0;
            btnToggleStatus.Size = new Size(130, 30);
            btnToggleStatus.Location = new Point(315, 10);
            btnToggleStatus.Cursor = Cursors.Hand;
            btnToggleStatus.Click += BtnToggleStatus_Click;

            actionPanel.Controls.Add(btnAdd);
            actionPanel.Controls.Add(btnEdit);
            actionPanel.Controls.Add(btnToggleStatus);

            dgvEmployees = new DataGridView();
            dgvEmployees.Dock = DockStyle.Fill;
            dgvEmployees.BackgroundColor = Color.White;
            dgvEmployees.BorderStyle = BorderStyle.None;
            dgvEmployees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEmployees.AllowUserToAddRows = false;
            dgvEmployees.ReadOnly = true;
            dgvEmployees.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEmployees.RowHeadersVisible = false;
            dgvEmployees.Font = new Font("Segoe UI", 9.5f);

            // Set up Editor Panel Overlay
            SetupEditorPanel();

            this.Controls.Add(dgvEmployees);
            this.Controls.Add(actionPanel);
        }

        private void SetupEditorPanel()
        {
            pnlEditor = new Panel();
            pnlEditor.Size = new Size(400, 420);
            pnlEditor.BackColor = Color.White;
            pnlEditor.BorderStyle = BorderStyle.FixedSingle;
            pnlEditor.Visible = false;

            lblEditorTitle = new Label();
            lblEditorTitle.Text = "THÊM TÀI KHOẢN MỚI";
            lblEditorTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblEditorTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblEditorTitle.Size = new Size(380, 25);
            lblEditorTitle.Location = new Point(10, 15);
            lblEditorTitle.TextAlign = ContentAlignment.MiddleCenter;

            int rowY = 55, spacing = 45, labelX = 25, inputX = 140, inputWidth = 230;

            CreateEditorLabel("Username:", rowY, labelX);
            txtUsername = CreateEditorTextBox(rowY, inputX, inputWidth);
            rowY += spacing;

            CreateEditorLabel("Mật khẩu:", rowY, labelX);
            txtPassword = CreateEditorTextBox(rowY, inputX, inputWidth);
            txtPassword.PasswordChar = '●';
            rowY += 25;

            lblPasswordHelp = new Label();
            lblPasswordHelp.Text = "(Để trống nếu không muốn đổi mật khẩu)";
            lblPasswordHelp.Font = new Font("Segoe UI", 7.5f, FontStyle.Italic);
            lblPasswordHelp.ForeColor = Color.Gray;
            lblPasswordHelp.Size = new Size(230, 15);
            lblPasswordHelp.Location = new Point(inputX, rowY);
            pnlEditor.Controls.Add(lblPasswordHelp);
            rowY += 20;

            CreateEditorLabel("Họ và tên:", rowY, labelX);
            txtFullName = CreateEditorTextBox(rowY, inputX, inputWidth);
            rowY += spacing;

            CreateEditorLabel("Số điện thoại:", rowY, labelX);
            txtPhone = CreateEditorTextBox(rowY, inputX, inputWidth);
            rowY += spacing;

            CreateEditorLabel("Vai trò:", rowY, labelX);
            cboRole = new ComboBox();
            cboRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cboRole.Items.AddRange(new string[] { "Admin", "Manager", "Cashier", "Staff" });
            cboRole.SelectedIndex = 2; // Cashier default
            cboRole.Size = new Size(inputWidth, 25);
            cboRole.Location = new Point(inputX, rowY);
            pnlEditor.Controls.Add(cboRole);
            rowY += spacing;

            chkActive = new CheckBox();
            chkActive.Text = "Kích hoạt hoạt động";
            chkActive.Font = new Font("Segoe UI", 9.5f);
            chkActive.Checked = true;
            chkActive.Location = new Point(inputX, rowY);
            chkActive.Size = new Size(200, 20);
            pnlEditor.Controls.Add(chkActive);
            rowY += 35;

            btnSave = new Button() { Text = "Lưu Lại", BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(100, 32), Location = new Point(140, rowY) };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button() { Text = "Hủy bỏ", BackColor = Color.FromArgb(189, 195, 199), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(100, 32), Location = new Point(255, rowY) };
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
        private void LoadEmployees()
        {
            try
            {
                string sql = "SELECT id, username as TaiKhoan, full_name as HoVaTen, phone as SoDienThoai, role as VaiTro, IF(is_active = 1, 'Kích hoạt', 'Bị khóa') as TrangThai FROM users ORDER BY id ASC";
                DataTable dt = DatabaseHelper.ExecuteQuery(sql);
                dgvEmployees.DataSource = dt;
                dgvEmployees.Columns["id"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách nhân viên: " + ex.Message);
            }
        }

        // --- ACTIONS ---
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            editingUserId = -1;
            lblEditorTitle.Text = "THÊM TÀI KHOẢN MỚI";
            txtUsername.Text = "";
            txtUsername.ReadOnly = false;
            txtPassword.Text = "";
            lblPasswordHelp.Visible = false;
            txtFullName.Text = "";
            txtPhone.Text = "";
            cboRole.SelectedIndex = 2; // Cashier
            chkActive.Checked = true;

            pnlEditor.Location = new Point((this.Width - pnlEditor.Width) / 2, (this.Height - pnlEditor.Height) / 2);
            pnlEditor.Visible = true;
            pnlEditor.BringToFront();
            txtUsername.Focus();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa.", "Chọn nhân viên", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            editingUserId = Convert.ToInt32(dgvEmployees.CurrentRow.Cells["id"].Value);

            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM users WHERE id = @id", new MySqlParameter("@id", editingUserId));
                if (dt.Rows.Count > 0)
                {
                    DataRow r = dt.Rows[0];
                    txtUsername.Text = r["username"].ToString();
                    txtUsername.ReadOnly = true; // Cannot edit username
                    txtPassword.Text = ""; // Leave blank to not change password
                    lblPasswordHelp.Visible = true;
                    txtFullName.Text = r["full_name"].ToString();
                    txtPhone.Text = r["phone"].ToString();
                    cboRole.SelectedItem = r["role"].ToString();
                    chkActive.Checked = Convert.ToBoolean(r["is_active"]);

                    lblEditorTitle.Text = "SỬA TÀI KHOẢN KHÓA #" + editingUserId;
                    pnlEditor.Location = new Point((this.Width - pnlEditor.Width) / 2, (this.Height - pnlEditor.Height) / 2);
                    pnlEditor.Visible = true;
                    pnlEditor.BringToFront();
                    txtFullName.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy thông tin: " + ex.Message);
            }
        }

        private void BtnToggleStatus_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.CurrentRow == null) return;
            int id = Convert.ToInt32(dgvEmployees.CurrentRow.Cells["id"].Value);
            string user = dgvEmployees.CurrentRow.Cells["TaiKhoan"].Value.ToString();
            string status = dgvEmployees.CurrentRow.Cells["TrangThai"].Value.ToString();

            if (user == SessionManager.Username)
            {
                MessageBox.Show("Bạn không thể tự khóa tài khoản của chính mình!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int newActive = status == "Kích hoạt" ? 0 : 1;
            string actionText = newActive == 1 ? "mở khóa" : "khóa";

            DialogResult dr = MessageBox.Show(string.Format("Xác nhận {0} tài khoản '{1}'?", actionText, user), "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No) return;

            try
            {
                DatabaseHelper.ExecuteNonQuery("UPDATE users SET is_active = @act WHERE id = @id", new MySqlParameter("@act", newActive), new MySqlParameter("@id", id));
                LoadEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string fullName = txtFullName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string role = cboRole.SelectedItem.ToString();
            int active = chkActive.Checked ? 1 : 0;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(fullName))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Username và Họ tên.");
                return;
            }

            try
            {
                if (editingUserId == -1)
                {
                    // Create account
                    if (string.IsNullOrEmpty(password))
                    {
                        MessageBox.Show("Vui lòng nhập mật khẩu cho tài khoản mới.");
                        return;
                    }

                    // Hash password
                    string hashed = DatabaseHelper.HashPassword(password);

                    string sql = "INSERT INTO users (username, password, full_name, phone, role, is_active) VALUES (@user, @pass, @name, @phone, @role, @act)";
                    MySqlParameter[] prs = new MySqlParameter[] {
                        new MySqlParameter("@user", username),
                        new MySqlParameter("@pass", hashed),
                        new MySqlParameter("@name", fullName),
                        new MySqlParameter("@phone", string.IsNullOrEmpty(phone) ? DBNull.Value : (object)phone),
                        new MySqlParameter("@role", role),
                        new MySqlParameter("@act", active)
                    };

                    DatabaseHelper.ExecuteNonQuery(sql, prs);
                }
                else
                {
                    // Update account
                    string sql;
                    List<MySqlParameter> prs = new List<MySqlParameter>() {
                        new MySqlParameter("@name", fullName),
                        new MySqlParameter("@phone", string.IsNullOrEmpty(phone) ? DBNull.Value : (object)phone),
                        new MySqlParameter("@role", role),
                        new MySqlParameter("@act", active),
                        new MySqlParameter("@id", editingUserId)
                    };

                    if (!string.IsNullOrEmpty(password))
                    {
                        // Update password too
                        string hashed = DatabaseHelper.HashPassword(password);
                        sql = "UPDATE users SET full_name = @name, phone = @phone, role = @role, is_active = @act, password = @pass WHERE id = @id";
                        prs.Add(new MySqlParameter("@pass", hashed));
                    }
                    else
                    {
                        sql = "UPDATE users SET full_name = @name, phone = @phone, role = @role, is_active = @act WHERE id = @id";
                    }

                    DatabaseHelper.ExecuteNonQuery(sql, prs.ToArray());
                }

                pnlEditor.Visible = false;
                LoadEmployees();
                MessageBox.Show("Lưu thông tin nhân viên thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            pnlEditor.Visible = false;
        }
    }
}

