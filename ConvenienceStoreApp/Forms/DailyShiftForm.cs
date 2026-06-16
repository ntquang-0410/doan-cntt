using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ConvenienceStoreApp.Forms
{
    public class DailyShiftForm : Form
    {
        private Panel contentCard;
        private Label lblTitle;
        private Label lblStatus;
        
        // Form states
        private bool hasActiveShift = false;
        private int activeShiftId = -1;
        private DateTime shiftOpenedAt;
        private decimal shiftOpeningBalance = 0;

        // Open shift UI elements
        private Label lblOpenBalance;
        private NumericUpDown numOpenBalance;
        private Label lblOpenNote;
        private TextBox txtOpenNote;
        private Button btnOpenShift;

        // Close shift UI elements
        private Label lblExpectedCashLabel;
        private Label lblExpectedCashVal;
        private Label lblCloseBalance;
        private NumericUpDown numCloseBalance;
        private Label lblCloseNote;
        private TextBox txtCloseNote;
        private Button btnCloseShift;

        public DailyShiftForm()
        {
            InitializeComponent();
            CheckShiftStatus();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Ca Làm Việc - Cửa Hàng Tiện Lợi";
            this.BackColor = Color.FromArgb(240, 244, 248);

            contentCard = new Panel();
            contentCard.Size = new Size(550, 480);
            contentCard.Location = new Point((this.ClientSize.Width - contentCard.Width) / 2, 50);
            contentCard.Anchor = AnchorStyles.Top;
            contentCard.BackColor = Color.White;
            contentCard.Paint += ContentCard_Paint;

            lblTitle = new Label();
            lblTitle.Text = "⏰ CA LÀM VIỆC & KÉT TIỀN MẶT";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblTitle.Size = new Size(510, 30);
            lblTitle.Location = new Point(20, 20);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            lblStatus = new Label();
            lblStatus.Text = "Đang kiểm tra trạng thái ca...";
            lblStatus.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            lblStatus.ForeColor = Color.FromArgb(127, 140, 141);
            lblStatus.Size = new Size(510, 25);
            lblStatus.Location = new Point(20, 55);
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;

            contentCard.Controls.Add(lblTitle);
            contentCard.Controls.Add(lblStatus);

            this.Controls.Add(contentCard);
        }

        private void ContentCard_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.FromArgb(220, 224, 230), 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, contentCard.Width - 1, contentCard.Height - 1);
            }
        }

        private void CheckShiftStatus()
        {
            try
            {
                // Find if this staff member has an open shift
                string sql = "SELECT id, opened_at, opening_balance FROM daily_cash_register WHERE staff_id = @sid AND closed_at IS NULL LIMIT 1";
                DataTable dt = DatabaseHelper.ExecuteQuery(sql, new MySqlParameter("@sid", SessionManager.UserId));

                // Clear previous controls except title and status
                for (int i = contentCard.Controls.Count - 1; i >= 0; i--)
                {
                    Control c = contentCard.Controls[i];
                    if (c != lblTitle && c != lblStatus)
                    {
                        contentCard.Controls.RemoveAt(i);
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    hasActiveShift = true;
                    activeShiftId = Convert.ToInt32(dt.Rows[0]["id"]);
                    shiftOpenedAt = Convert.ToDateTime(dt.Rows[0]["opened_at"]);
                    shiftOpeningBalance = Convert.ToDecimal(dt.Rows[0]["opening_balance"]);
                    SessionManager.CurrentShiftId = activeShiftId;

                    lblStatus.Text = string.Format("🟢 Đang hoạt động (Mở ca lúc: {0:dd/MM/yyyy HH:mm:ss})", shiftOpenedAt);
                    lblStatus.ForeColor = Color.FromArgb(46, 204, 113);

                    SetupCloseShiftUI();
                }
                else
                {
                    hasActiveShift = false;
                    activeShiftId = -1;
                    SessionManager.CurrentShiftId = -1;

                    lblStatus.Text = "🔴 Chưa mở ca (Két tiền mặt hiện đang đóng)";
                    lblStatus.ForeColor = Color.FromArgb(231, 76, 60);

                    SetupOpenShiftUI();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi truy vấn CSDL: " + ex.Message;
                lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
            }
        }

        private void SetupOpenShiftUI()
        {
            lblOpenBalance = new Label();
            lblOpenBalance.Text = "Tiền mặt bàn giao đầu ca (Opening Balance):";
            lblOpenBalance.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblOpenBalance.ForeColor = Color.FromArgb(71, 84, 103);
            lblOpenBalance.Size = new Size(510, 20);
            lblOpenBalance.Location = new Point(40, 110);

            numOpenBalance = new NumericUpDown();
            numOpenBalance.Font = new Font("Segoe UI", 12);
            numOpenBalance.Size = new Size(470, 30);
            numOpenBalance.Location = new Point(40, 135);
            numOpenBalance.Maximum = 1000000000;
            numOpenBalance.Increment = 50000;
            numOpenBalance.ThousandsSeparator = true;
            numOpenBalance.Value = 200000; // Default opening cash float: 200,000 VND

            lblOpenNote = new Label();
            lblOpenNote.Text = "Ghi chú ca:";
            lblOpenNote.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblOpenNote.ForeColor = Color.FromArgb(71, 84, 103);
            lblOpenNote.Size = new Size(510, 20);
            lblOpenNote.Location = new Point(40, 185);

            txtOpenNote = new TextBox();
            txtOpenNote.Font = new Font("Segoe UI", 11);
            txtOpenNote.Size = new Size(470, 80);
            txtOpenNote.Multiline = true;
            txtOpenNote.Location = new Point(40, 210);

            btnOpenShift = new Button();
            btnOpenShift.Text = "🚀 BẮT ĐẦU CA MỚI (MỞ KÉT)";
            btnOpenShift.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnOpenShift.BackColor = Color.FromArgb(26, 188, 156);
            btnOpenShift.ForeColor = Color.White;
            btnOpenShift.FlatStyle = FlatStyle.Flat;
            btnOpenShift.FlatAppearance.BorderSize = 0;
            btnOpenShift.Cursor = Cursors.Hand;
            btnOpenShift.Size = new Size(470, 45);
            btnOpenShift.Location = new Point(40, 320);
            btnOpenShift.Click += BtnOpenShift_Click;

            contentCard.Controls.Add(lblOpenBalance);
            contentCard.Controls.Add(numOpenBalance);
            contentCard.Controls.Add(lblOpenNote);
            contentCard.Controls.Add(txtOpenNote);
            contentCard.Controls.Add(btnOpenShift);
        }

        private void SetupCloseShiftUI()
        {
            decimal cashSales = GetCashSalesSinceOpen();
            decimal expectedCash = shiftOpeningBalance + cashSales;

            lblExpectedCashLabel = new Label();
            lblExpectedCashLabel.Text = "Số tiền mặt lý thuyết trong két (Expected Cash):";
            lblExpectedCashLabel.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblExpectedCashLabel.ForeColor = Color.FromArgb(71, 84, 103);
            lblExpectedCashLabel.Size = new Size(300, 20);
            lblExpectedCashLabel.Location = new Point(40, 110);

            lblExpectedCashVal = new Label();
            lblExpectedCashVal.Text = string.Format("{0:N0} VND\n(Đầu ca: {1:N0}đ + Doanh thu mặt: {2:N0}đ)", expectedCash, shiftOpeningBalance, cashSales);
            lblExpectedCashVal.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblExpectedCashVal.ForeColor = Color.FromArgb(44, 62, 80);
            lblExpectedCashVal.Size = new Size(470, 40);
            lblExpectedCashVal.Location = new Point(40, 130);

            lblCloseBalance = new Label();
            lblCloseBalance.Text = "Tiền mặt kiểm kê thực tế cuối ca (Closing Balance):";
            lblCloseBalance.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblCloseBalance.ForeColor = Color.FromArgb(71, 84, 103);
            lblCloseBalance.Size = new Size(510, 20);
            lblCloseBalance.Location = new Point(40, 190);

            numCloseBalance = new NumericUpDown();
            numCloseBalance.Font = new Font("Segoe UI", 12);
            numCloseBalance.Size = new Size(470, 30);
            numCloseBalance.Location = new Point(40, 215);
            numCloseBalance.Maximum = 1000000000;
            numCloseBalance.Increment = 50000;
            numCloseBalance.ThousandsSeparator = true;
            numCloseBalance.Value = expectedCash; // Set expected cash as default

            lblCloseNote = new Label();
            lblCloseNote.Text = "Ghi chú kết thúc ca:";
            lblCloseNote.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblCloseNote.ForeColor = Color.FromArgb(71, 84, 103);
            lblCloseNote.Size = new Size(510, 20);
            lblCloseNote.Location = new Point(40, 260);

            txtCloseNote = new TextBox();
            txtCloseNote.Font = new Font("Segoe UI", 11);
            txtCloseNote.Size = new Size(470, 60);
            txtCloseNote.Multiline = true;
            txtCloseNote.Location = new Point(40, 285);

            btnCloseShift = new Button();
            btnCloseShift.Text = "🛑 BÀN GIAO & KẾT THÚC CA (ĐÓNG KÉT)";
            btnCloseShift.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnCloseShift.BackColor = Color.FromArgb(231, 76, 60);
            btnCloseShift.ForeColor = Color.White;
            btnCloseShift.FlatStyle = FlatStyle.Flat;
            btnCloseShift.FlatAppearance.BorderSize = 0;
            btnCloseShift.Cursor = Cursors.Hand;
            btnCloseShift.Size = new Size(470, 45);
            btnCloseShift.Location = new Point(40, 370);
            btnCloseShift.Click += BtnCloseShift_Click;

            contentCard.Controls.Add(lblExpectedCashLabel);
            contentCard.Controls.Add(lblExpectedCashVal);
            contentCard.Controls.Add(lblCloseBalance);
            contentCard.Controls.Add(numCloseBalance);
            contentCard.Controls.Add(lblCloseNote);
            contentCard.Controls.Add(txtCloseNote);
            contentCard.Controls.Add(btnCloseShift);
        }

        private decimal GetCashSalesSinceOpen()
        {
            try
            {
                // Sum total_amount from orders where payment_method = 'cash' and created_at >= shiftOpenedAt and staff_id = UserId
                string sql = "SELECT SUM(total_amount) FROM orders WHERE staff_id = @sid AND payment_method = 'cash' AND created_at >= @opened";
                MySqlParameter[] prs = new MySqlParameter[] {
                    new MySqlParameter("@sid", SessionManager.UserId),
                    new MySqlParameter("@opened", shiftOpenedAt)
                };

                object res = DatabaseHelper.ExecuteScalar(sql, prs);
                return res != DBNull.Value && res != null ? Convert.ToDecimal(res) : 0;
            }
            catch
            {
                return 0;
            }
        }

        private void BtnOpenShift_Click(object sender, EventArgs e)
        {
            decimal openVal = numOpenBalance.Value;
            string note = txtOpenNote.Text.Trim();

            DialogResult dr = MessageBox.Show(string.Format("Xác nhận mở két với số tiền {0:N0} VND?", openVal), "Xác nhận mở ca", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No) return;

            try
            {
                string sql = "INSERT INTO daily_cash_register (staff_id, shift_date, opening_balance, note) VALUES (@sid, CURDATE(), @bal, @note)";
                MySqlParameter[] prs = new MySqlParameter[] {
                    new MySqlParameter("@sid", SessionManager.UserId),
                    new MySqlParameter("@bal", openVal),
                    new MySqlParameter("@note", string.IsNullOrEmpty(note) ? "Bắt đầu ca" : note)
                };

                DatabaseHelper.ExecuteNonQuery(sql, prs);
                MessageBox.Show("Mở ca làm việc thành công! Bạn có thể sử dụng quầy POS ngay bây giờ.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CheckShiftStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mở ca thất bại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCloseShift_Click(object sender, EventArgs e)
        {
            decimal closeVal = numCloseBalance.Value;
            decimal cashSales = GetCashSalesSinceOpen();
            decimal expectedCash = shiftOpeningBalance + cashSales;
            decimal diff = closeVal - expectedCash;
            string note = txtCloseNote.Text.Trim();

            DialogResult dr = MessageBox.Show("Xác nhận đóng két và bàn giao ca làm việc?", "Xác nhận kết thúc ca", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No) return;

            try
            {
                string sql = @"
                    UPDATE daily_cash_register 
                    SET closing_balance = @close,
                        expected_cash = @expected,
                        cash_difference = @diff,
                        closed_at = NOW(),
                        note = @note
                    WHERE id = @id";

                MySqlParameter[] prs = new MySqlParameter[] {
                    new MySqlParameter("@close", closeVal),
                    new MySqlParameter("@expected", expectedCash),
                    new MySqlParameter("@diff", diff),
                    new MySqlParameter("@note", string.IsNullOrEmpty(note) ? "Bàn giao ca thành công" : note),
                    new MySqlParameter("@id", activeShiftId)
                };

                DatabaseHelper.ExecuteNonQuery(sql, prs);

                string diffMsg = diff == 0 
                    ? "Số dư két khớp 100%!" 
                    : (diff > 0 
                        ? string.Format("Lưu ý: Két DƯ {0:N0} VND so với sổ sách.", diff)
                        : string.Format("CẢNH BÁO: Két THIẾU {0:N0} VND so với sổ sách!", Math.Abs(diff)));

                MessageBox.Show("Đã đóng ca làm việc thành công!\n" + diffMsg, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CheckShiftStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đóng ca thất bại: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
