using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ConvenienceStoreApp.Forms
{
    public class LoginForm : Form
    {
        private Panel cardPanel;
        private Label titleLabel;
        private Label subtitleLabel;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblError;
        private Label lblUsername;
        private Label lblPassword;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Đăng Nhập Hệ Thống - Cửa Hàng Tiện Lợi";
            this.Size = new Size(800, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 244, 248); // Light grayish blue background

            // Card panel in center
            cardPanel = new Panel();
            cardPanel.Size = new Size(400, 420);
            cardPanel.Location = new Point((this.ClientSize.Width - cardPanel.Width) / 2, (this.ClientSize.Height - cardPanel.Height) / 2);
            cardPanel.BackColor = Color.White;
            cardPanel.BorderStyle = BorderStyle.None;
            // Draw a subtle shadow border manually
            cardPanel.Paint += CardPanel_Paint;

            // Brand/App Title
            titleLabel = new Label();
            titleLabel.Text = "CONVENIENCE STORE";
            titleLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(44, 62, 80); // Dark Slate
            titleLabel.Size = new Size(360, 35);
            titleLabel.Location = new Point(20, 30);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;

            subtitleLabel = new Label();
            subtitleLabel.Text = "Đăng nhập để bắt đầu ca làm việc";
            subtitleLabel.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            subtitleLabel.ForeColor = Color.FromArgb(127, 143, 166); // Muted Blue Gray
            subtitleLabel.Size = new Size(360, 20);
            subtitleLabel.Location = new Point(20, 70);
            subtitleLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Username Field
            lblUsername = new Label();
            lblUsername.Text = "Tên đăng nhập";
            lblUsername.Font = new Font("Segoe UI", 9.75f, FontStyle.Bold);
            lblUsername.ForeColor = Color.FromArgb(71, 84, 103);
            lblUsername.Location = new Point(40, 115);
            lblUsername.Size = new Size(320, 20);

            txtUsername = new TextBox();
            txtUsername.Font = new Font("Segoe UI", 12);
            txtUsername.Location = new Point(40, 140);
            txtUsername.Size = new Size(320, 30);
            txtUsername.BorderStyle = BorderStyle.FixedSingle;

            // Password Field
            lblPassword = new Label();
            lblPassword.Text = "Mật khẩu";
            lblPassword.Font = new Font("Segoe UI", 9.75f, FontStyle.Bold);
            lblPassword.ForeColor = Color.FromArgb(71, 84, 103);
            lblPassword.Location = new Point(40, 185);
            lblPassword.Size = new Size(320, 20);

            txtPassword = new TextBox();
            txtPassword.Font = new Font("Segoe UI", 12);
            txtPassword.Location = new Point(40, 210);
            txtPassword.Size = new Size(320, 30);
            txtPassword.PasswordChar = '●';
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.KeyDown += TxtPassword_KeyDown;

            // Error Message
            lblError = new Label();
            lblError.Text = "";
            lblError.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            lblError.ForeColor = Color.FromArgb(231, 76, 60); // Red
            lblError.Location = new Point(40, 250);
            lblError.Size = new Size(320, 35);
            lblError.TextAlign = ContentAlignment.MiddleLeft;

            // Login Button
            btnLogin = new Button();
            btnLogin.Text = "ĐĂNG NHẬP";
            btnLogin.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.BackColor = Color.FromArgb(26, 188, 156); // Turquoise Teal
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Location = new Point(40, 295);
            btnLogin.Size = new Size(320, 45);
            btnLogin.Click += BtnLogin_Click;

            // Help Label
            Label helpLabel = new Label();
            helpLabel.Text = "Tài khoản mẫu: admin (pass: admin), cashier (pass: cashier)";
            helpLabel.Font = new Font("Segoe UI", 8, FontStyle.Regular);
            helpLabel.ForeColor = Color.FromArgb(189, 195, 199);
            helpLabel.Size = new Size(360, 20);
            helpLabel.Location = new Point(20, 370);
            helpLabel.TextAlign = ContentAlignment.MiddleCenter;

            cardPanel.Controls.Add(titleLabel);
            cardPanel.Controls.Add(subtitleLabel);
            cardPanel.Controls.Add(lblUsername);
            cardPanel.Controls.Add(txtUsername);
            cardPanel.Controls.Add(lblPassword);
            cardPanel.Controls.Add(txtPassword);
            cardPanel.Controls.Add(lblError);
            cardPanel.Controls.Add(btnLogin);
            cardPanel.Controls.Add(helpLabel);

            this.Controls.Add(cardPanel);
        }

        private void CardPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw a subtle border outline
            using (Pen pen = new Pen(Color.FromArgb(220, 224, 230), 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, cardPanel.Width - 1, cardPanel.Height - 1);
            }
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnLogin_Click(this, EventArgs.Empty);
                e.SuppressKeyPress = true; // prevent beep
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Vui lòng điền đầy đủ Tên đăng nhập và Mật khẩu.";
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "Đang xác thực...";
            lblError.Text = "";

            try
            {
                string query = "SELECT id, username, password, full_name, role, is_active FROM users WHERE username = @username";
                MySqlParameter[] prs = new MySqlParameter[] {
                    new MySqlParameter("@username", username)
                };

                DataTable dt = DatabaseHelper.ExecuteQuery(query, prs);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    bool isActive = Convert.ToBoolean(row["is_active"]);
                    string dbHash = row["password"].ToString();
                    string fullName = row["full_name"].ToString();
                    string role = row["role"].ToString();
                    int userId = Convert.ToInt32(row["id"]);

                    if (!isActive)
                    {
                        lblError.Text = "Tài khoản của bạn đã bị vô hiệu hóa.";
                        btnLogin.Enabled = true;
                        btnLogin.Text = "ĐĂNG NHẬP";
                        return;
                    }

                    if (DatabaseHelper.VerifyPassword(password, dbHash, username))
                    {
                        // Save Session Info
                        SessionManager.Login(userId, username, fullName, role);
                        
                        // Write to Audit Log (Silent)
                        try
                        {
                            DatabaseHelper.ExecuteNonQuery(
                                "INSERT INTO audit_logs (user_id, action, table_name, record_id, new_values) VALUES (@uid, 'login', 'users', @uid, @vals)",
                                new MySqlParameter("@uid", userId),
                                new MySqlParameter("@vals", "{\"username\": \"" + username + "\", \"ip\": \"local\"}")
                            );
                        }
                        catch { }

                        // Transition to MainForm
                        this.Hide();
                        MainForm mainForm = new MainForm();
                        mainForm.FormClosed += (s, args) => this.Close();
                        mainForm.Show();
                    }
                    else
                    {
                        lblError.Text = "Mật khẩu không chính xác.";
                        btnLogin.Enabled = true;
                        btnLogin.Text = "ĐĂNG NHẬP";
                    }
                }
                else
                {
                    lblError.Text = "Tài khoản không tồn tại.";
                    btnLogin.Enabled = true;
                    btnLogin.Text = "ĐĂNG NHẬP";
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Lỗi kết nối CSDL: " + ex.Message;
                btnLogin.Enabled = true;
                btnLogin.Text = "ĐĂNG NHẬP";
            }
        }
    }
}

