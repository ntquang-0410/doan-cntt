using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ConvenienceStoreApp.Forms
{
    public class PromotionForm : Form
    {
        private Panel actionPanel;
        private DataGridView dgvPromotions;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnToggleStatus;

        // Editor overlay panel
        private Panel pnlEditor;
        private Label lblEditorTitle;
        private TextBox txtPromoName;
        private ComboBox cboPromoType;
        private NumericUpDown numPromoValue;
        private NumericUpDown numMinOrderValue;
        private DateTimePicker dtpStart;
        private DateTimePicker dtpEnd;
        private CheckBox chkActive;
        private Button btnSave;
        private Button btnCancel;
        private int editingPromoId = -1;

        public PromotionForm()
        {
            InitializeComponent();
            LoadPromotions();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Chương Trình Khuyến Mãi";
            this.BackColor = Color.FromArgb(240, 244, 248);

            actionPanel = new Panel();
            actionPanel.Dock = DockStyle.Top;
            actionPanel.Height = 50;

            btnAdd = new Button() { Text = "➕ Tạo Khuyến Mãi", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), BackColor = Color.FromArgb(26, 188, 156), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(160, 30), Location = new Point(15, 10), Cursor = Cursors.Hand };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button() { Text = "✏️ Sửa Chiến Dịch", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(140, 30), Location = new Point(185, 10), Cursor = Cursors.Hand };
            btnEdit.Click += BtnEdit_Click;

            btnToggleStatus = new Button() { Text = "🔒 Bật/Tắt Áp Dụng", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), BackColor = Color.FromArgb(127, 140, 141), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(140, 30), Location = new Point(335, 10), Cursor = Cursors.Hand };
            btnToggleStatus.Click += BtnToggleStatus_Click;

            actionPanel.Controls.Add(btnAdd);
            actionPanel.Controls.Add(btnEdit);
            actionPanel.Controls.Add(btnToggleStatus);

            dgvPromotions = new DataGridView();
            dgvPromotions.Dock = DockStyle.Fill;
            dgvPromotions.BackgroundColor = Color.White;
            dgvPromotions.BorderStyle = BorderStyle.None;
            dgvPromotions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPromotions.AllowUserToAddRows = false;
            dgvPromotions.ReadOnly = true;
            dgvPromotions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPromotions.RowHeadersVisible = false;
            dgvPromotions.Font = new Font("Segoe UI", 9.5f);

            // Editor Panel Setup
            SetupEditorPanel();

            this.Controls.Add(dgvPromotions);
            this.Controls.Add(actionPanel);
        }

        private void SetupEditorPanel()
        {
            pnlEditor = new Panel();
            pnlEditor.Size = new Size(420, 430);
            pnlEditor.BackColor = Color.White;
            pnlEditor.BorderStyle = BorderStyle.FixedSingle;
            pnlEditor.Visible = false;

            lblEditorTitle = new Label();
            lblEditorTitle.Text = "TẠO CHƯƠNG TRÌNH KHUYẾN MÃI";
            lblEditorTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblEditorTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblEditorTitle.Size = new Size(400, 25);
            lblEditorTitle.Location = new Point(10, 15);
            lblEditorTitle.TextAlign = ContentAlignment.MiddleCenter;

            int rowY = 55, spacing = 45, labelX = 25, inputX = 160, inputWidth = 220;

            CreateEditorLabel("Tên chương trình:", rowY, labelX);
            txtPromoName = CreateEditorTextBox(rowY, inputX, inputWidth);
            rowY += spacing;

            CreateEditorLabel("Hình thức giảm:", rowY, labelX);
            cboPromoType = new ComboBox();
            cboPromoType.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPromoType.Items.AddRange(new string[] { "Phần trăm (%) - percent", "Giảm tiền mặt (đ) - fixed" });
            cboPromoType.SelectedIndex = 0;
            cboPromoType.Size = new Size(inputWidth, 25);
            cboPromoType.Location = new Point(inputX, rowY);
            pnlEditor.Controls.Add(cboPromoType);
            rowY += spacing;

            CreateEditorLabel("Giá trị giảm:", rowY, labelX);
            numPromoValue = new NumericUpDown() { Size = new Size(inputWidth, 25), Location = new Point(inputX, rowY), Minimum = 0, Maximum = 1000000000, ThousandsSeparator = true };
            pnlEditor.Controls.Add(numPromoValue);
            rowY += spacing;

            CreateEditorLabel("Giá trị đơn tối thiểu:", rowY, labelX);
            numMinOrderValue = new NumericUpDown() { Size = new Size(inputWidth, 25), Location = new Point(inputX, rowY), Minimum = 0, Maximum = 1000000000, ThousandsSeparator = true };
            pnlEditor.Controls.Add(numMinOrderValue);
            rowY += spacing;

            CreateEditorLabel("Ngày bắt đầu:", rowY, labelX);
            dtpStart = new DateTimePicker() { Format = DateTimePickerFormat.Short, Size = new Size(inputWidth, 25), Location = new Point(inputX, rowY) };
            pnlEditor.Controls.Add(dtpStart);
            rowY += spacing;

            CreateEditorLabel("Ngày kết thúc:", rowY, labelX);
            dtpEnd = new DateTimePicker() { Format = DateTimePickerFormat.Short, Size = new Size(inputWidth, 25), Location = new Point(inputX, rowY) };
            pnlEditor.Controls.Add(dtpEnd);
            rowY += spacing;

            chkActive = new CheckBox();
            chkActive.Text = "Kích hoạt áp dụng";
            chkActive.Font = new Font("Segoe UI", 9.5f);
            chkActive.Checked = true;
            chkActive.Location = new Point(inputX, rowY);
            chkActive.Size = new Size(200, 20);
            pnlEditor.Controls.Add(chkActive);
            rowY += 35;

            btnSave = new Button() { Text = "Lưu chiến dịch", BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(110, 32), Location = new Point(160, rowY) };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button() { Text = "Hủy bỏ", BackColor = Color.FromArgb(189, 195, 199), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(90, 32), Location = new Point(285, rowY) };
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
        private void LoadPromotions()
        {
            try
            {
                string sql = "SELECT id, name as TenChuongTrinh, IF(type = 'percent', 'Phần trăm (%)', 'Tiền mặt (VND)') as KieuGiam, value as GiaTriGiam, min_order_value as HoaDonToiThieu, start_date as NgayBatDau, end_date as NgayKetThuc, IF(is_active = 1, 'Kích hoạt', 'Tạm ngưng') as TrangThai FROM promotions ORDER BY id DESC";
                DataTable dt = DatabaseHelper.ExecuteQuery(sql);
                dgvPromotions.DataSource = dt;
                dgvPromotions.Columns["id"].Visible = false;
                dgvPromotions.Columns["GiaTriGiam"].DefaultCellStyle.Format = "N0";
                dgvPromotions.Columns["HoaDonToiThieu"].DefaultCellStyle.Format = "N0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải chương trình khuyến mãi: " + ex.Message);
            }
        }

        // --- ACTIONS ---
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            editingPromoId = -1;
            lblEditorTitle.Text = "TẠO CHƯƠNG TRÌNH KHUYẾN MÃI";
            txtPromoName.Text = "";
            cboPromoType.SelectedIndex = 0;
            numPromoValue.Value = 10; // 10%
            numMinOrderValue.Value = 100000; // 100,000 VND
            dtpStart.Value = DateTime.Today;
            dtpEnd.Value = DateTime.Today.AddDays(7);
            chkActive.Checked = true;

            pnlEditor.Location = new Point((this.Width - pnlEditor.Width) / 2, (this.Height - pnlEditor.Height) / 2);
            pnlEditor.Visible = true;
            pnlEditor.BringToFront();
            txtPromoName.Focus();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvPromotions.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn khuyến mãi cần sửa.", "Chọn chương trình", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            editingPromoId = Convert.ToInt32(dgvPromotions.CurrentRow.Cells["id"].Value);

            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM promotions WHERE id = @id", new MySqlParameter("@id", editingPromoId));
                if (dt.Rows.Count > 0)
                {
                    DataRow r = dt.Rows[0];
                    txtPromoName.Text = r["name"].ToString();
                    cboPromoType.SelectedIndex = r["type"].ToString() == "percent" ? 0 : 1;
                    numPromoValue.Value = Convert.ToDecimal(r["value"]);
                    numMinOrderValue.Value = Convert.ToDecimal(r["min_order_value"]);
                    dtpStart.Value = Convert.ToDateTime(r["start_date"]);
                    dtpEnd.Value = Convert.ToDateTime(r["end_date"]);
                    chkActive.Checked = Convert.ToBoolean(r["is_active"]);

                    lblEditorTitle.Text = "SỬA KHUYẾN MÃI KHÓA #" + editingPromoId;
                    pnlEditor.Location = new Point((this.Width - pnlEditor.Width) / 2, (this.Height - pnlEditor.Height) / 2);
                    pnlEditor.Visible = true;
                    pnlEditor.BringToFront();
                    txtPromoName.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy thông tin: " + ex.Message);
            }
        }

        private void BtnToggleStatus_Click(object sender, EventArgs e)
        {
            if (dgvPromotions.CurrentRow == null) return;
            int id = Convert.ToInt32(dgvPromotions.CurrentRow.Cells["id"].Value);
            string name = dgvPromotions.CurrentRow.Cells["TenChuongTrinh"].Value.ToString();
            string status = dgvPromotions.CurrentRow.Cells["TrangThai"].Value.ToString();

            int newActive = status == "Kích hoạt" ? 0 : 1;
            string actionText = newActive == 1 ? "kích hoạt lại" : "tạm ngưng";

            DialogResult dr = MessageBox.Show(string.Format("Xác nhận {0} chương trình '{1}'?", actionText, name), "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No) return;

            try
            {
                DatabaseHelper.ExecuteNonQuery("UPDATE promotions SET is_active = @act WHERE id = @id", new MySqlParameter("@act", newActive), new MySqlParameter("@id", id));
                LoadPromotions();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string name = txtPromoName.Text.Trim();
            string type = cboPromoType.SelectedIndex == 0 ? "percent" : "fixed";
            decimal val = numPromoValue.Value;
            decimal minVal = numMinOrderValue.Value;
            DateTime start = dtpStart.Value;
            DateTime end = dtpEnd.Value;
            int active = chkActive.Checked ? 1 : 0;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Vui lòng điền tên chương trình khuyến mãi.");
                return;
            }

            if (type == "percent" && val > 100)
            {
                MessageBox.Show("Giá trị giảm theo phần trăm không thể vượt quá 100%.");
                return;
            }

            if (start > end)
            {
                MessageBox.Show("Ngày bắt đầu không thể sau ngày kết thúc.");
                return;
            }

            try
            {
                if (editingPromoId == -1)
                {
                    // Create promotion
                    string sql = "INSERT INTO promotions (name, type, value, min_order_value, start_date, end_date, is_active) VALUES (@name, @type, @val, @minVal, @start, @end, @act)";
                    MySqlParameter[] prs = new MySqlParameter[] {
                        new MySqlParameter("@name", name),
                        new MySqlParameter("@type", type),
                        new MySqlParameter("@val", val),
                        new MySqlParameter("@minVal", minVal),
                        new MySqlParameter("@start", start.ToString("yyyy-MM-dd")),
                        new MySqlParameter("@end", end.ToString("yyyy-MM-dd")),
                        new MySqlParameter("@act", active)
                    };
                    DatabaseHelper.ExecuteNonQuery(sql, prs);
                }
                else
                {
                    // Update promotion
                    string sql = "UPDATE promotions SET name = @name, type = @type, value = @val, min_order_value = @minVal, start_date = @start, end_date = @end, is_active = @act WHERE id = @id";
                    MySqlParameter[] prs = new MySqlParameter[] {
                        new MySqlParameter("@name", name),
                        new MySqlParameter("@type", type),
                        new MySqlParameter("@val", val),
                        new MySqlParameter("@minVal", minVal),
                        new MySqlParameter("@start", start.ToString("yyyy-MM-dd")),
                        new MySqlParameter("@end", end.ToString("yyyy-MM-dd")),
                        new MySqlParameter("@act", active),
                        new MySqlParameter("@id", editingPromoId)
                    };
                    DatabaseHelper.ExecuteNonQuery(sql, prs);
                }

                pnlEditor.Visible = false;
                LoadPromotions();
                MessageBox.Show("Lưu chiến dịch khuyến mãi thành công!");
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

