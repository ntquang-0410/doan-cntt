using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ConvenienceStoreApp.Forms
{
    public class ReportForm : Form
    {
        private TabControl tabControl;
        private TabPage tabRevenue;
        private TabPage tabTopProducts;
        private TabPage tabAuditLogs;

        // Date Range Selector (common for reports)
        private Panel pnlHeader;
        private DateTimePicker dtpStart;
        private DateTimePicker dtpEnd;
        private Button btnRefresh;

        // Revenue Tab UI
        private Label lblTotalRevenue;
        private Label lblTotalOrders;
        private Label lblTotalDiscount;
        private Label lblTotalProfit;
        private DataGridView dgvOrders;

        // Top Products Tab UI
        private DataGridView dgvTopProducts;

        // Audit Logs Tab UI
        private DataGridView dgvAuditLogs;

        public ReportForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Báo Cáo & Thống Kê - Cửa Hàng Tiện Lợi";
            this.BackColor = Color.FromArgb(240, 244, 248);

            // Header Panel for dates selection
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 60;
            pnlHeader.BackColor = Color.White;
            pnlHeader.Paint += PnlHeader_Paint;

            Label lblStart = new Label() { Text = "Từ ngày:", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 9.5f) };
            dtpStart = new DateTimePicker() { Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(-30), Size = new Size(120, 25), Location = new Point(85, 17), Font = new Font("Segoe UI", 9.5f) };
            
            Label lblEnd = new Label() { Text = "Đến ngày:", Location = new Point(230, 20), AutoSize = true, Font = new Font("Segoe UI", 9.5f) };
            dtpEnd = new DateTimePicker() { Format = DateTimePickerFormat.Short, Value = DateTime.Today, Size = new Size(120, 25), Location = new Point(300, 17), Font = new Font("Segoe UI", 9.5f) };

            btnRefresh = new Button() { Text = "📊 Xem Báo Cáo", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), BackColor = Color.FromArgb(26, 188, 156), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(130, 28), Location = new Point(440, 15), Cursor = Cursors.Hand };
            btnRefresh.Click += BtnRefresh_Click;

            pnlHeader.Controls.Add(lblStart);
            pnlHeader.Controls.Add(dtpStart);
            pnlHeader.Controls.Add(lblEnd);
            pnlHeader.Controls.Add(dtpEnd);
            pnlHeader.Controls.Add(btnRefresh);

            // Tab Control
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 10);

            // Tab 1: Revenue
            tabRevenue = new TabPage("📈 Doanh Thu & Lợi Nhuận");
            tabRevenue.BackColor = Color.FromArgb(245, 246, 250);
            SetupRevenueTab();

            // Tab 2: Top Products
            tabTopProducts = new TabPage("🏆 Sản Phẩm Bán Chạy");
            tabTopProducts.BackColor = Color.FromArgb(245, 246, 250);
            SetupTopProductsTab();

            tabControl.TabPages.Add(tabRevenue);
            tabControl.TabPages.Add(tabTopProducts);

            // Tab 3: Audit Logs (Admin only)
            if (SessionManager.IsAdmin)
            {
                tabAuditLogs = new TabPage("🔒 Nhật Ký Hoạt Động");
                tabAuditLogs.BackColor = Color.FromArgb(245, 246, 250);
                SetupAuditLogsTab();
                tabControl.TabPages.Add(tabAuditLogs);
            }

            this.Controls.Add(tabControl);
            this.Controls.Add(pnlHeader);
        }

        private void PnlHeader_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.FromArgb(220, 224, 230), 1))
            {
                e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
            }
        }

        private void SetupRevenueTab()
        {
            // Cards panel for KPI metrics
            Panel pnlKPIs = new Panel();
            pnlKPIs.Dock = DockStyle.Top;
            pnlKPIs.Height = 120;
            pnlKPIs.Padding = new Padding(10);

            int cardWidth = 260, cardHeight = 90, spacing = 20, startX = 15;

            // Revenue Card
            Panel c1 = CreateKPICard("TỔNG DOANH THU", "0đ", Color.FromArgb(52, 152, 219), startX, 10, cardWidth, cardHeight, out lblTotalRevenue);
            startX += cardWidth + spacing;

            // Profit Card
            Panel c2 = CreateKPICard("LỢI NHUẬN GỘP", "0đ", Color.FromArgb(46, 204, 113), startX, 10, cardWidth, cardHeight, out lblTotalProfit);
            startX += cardWidth + spacing;

            // Orders count Card
            Panel c3 = CreateKPICard("SỐ HÓA ĐƠN", "0", Color.FromArgb(155, 89, 182), startX, 10, cardWidth, cardHeight, out lblTotalOrders);
            startX += cardWidth + spacing;

            // Discounts Card
            Panel c4 = CreateKPICard("CHIẾT KHẤU ĐÃ TẶNG", "0đ", Color.FromArgb(230, 126, 34), startX, 10, cardWidth, cardHeight, out lblTotalDiscount);

            pnlKPIs.Controls.Add(c1);
            pnlKPIs.Controls.Add(c2);
            pnlKPIs.Controls.Add(c3);
            pnlKPIs.Controls.Add(c4);

            // Orders list grid
            GroupBox grpOrdersList = new GroupBox() { Text = "Danh Sách Hóa Đơn Bán Hàng", Font = new Font("Segoe UI", 9.75f, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80), Dock = DockStyle.Fill, Padding = new Padding(10) };
            
            dgvOrders = new DataGridView();
            dgvOrders.Dock = DockStyle.Fill;
            dgvOrders.BackgroundColor = Color.White;
            dgvOrders.BorderStyle = BorderStyle.None;
            dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvOrders.AllowUserToAddRows = false;
            dgvOrders.ReadOnly = true;
            dgvOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrders.RowHeadersVisible = false;
            dgvOrders.Font = new Font("Segoe UI", 9.5f);

            grpOrdersList.Controls.Add(dgvOrders);

            tabRevenue.Controls.Add(grpOrdersList);
            tabRevenue.Controls.Add(pnlKPIs);
        }

        private Panel CreateKPICard(string title, string defaultVal, Color color, int x, int y, int w, int h, out Label valueLabel)
        {
            Panel card = new Panel() { Size = new Size(w, h), Location = new Point(x, y), BackColor = color };
            
            Label lblTitle = new Label() { Text = title, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(15, 15), Size = new Size(w - 30, 20) };
            
            valueLabel = new Label() { Text = defaultVal, ForeColor = Color.White, Font = new Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(15, 40), Size = new Size(w - 30, 35) };

            card.Controls.Add(lblTitle);
            card.Controls.Add(valueLabel);

            return card;
        }

        private void SetupTopProductsTab()
        {
            GroupBox grpTop = new GroupBox() { Text = "Sản Phẩm Bán Chạy Nhất", Font = new Font("Segoe UI", 9.75f, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80), Dock = DockStyle.Fill, Padding = new Padding(10) };

            dgvTopProducts = new DataGridView();
            dgvTopProducts.Dock = DockStyle.Fill;
            dgvTopProducts.BackgroundColor = Color.White;
            dgvTopProducts.BorderStyle = BorderStyle.None;
            dgvTopProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTopProducts.AllowUserToAddRows = false;
            dgvTopProducts.ReadOnly = true;
            dgvTopProducts.RowHeadersVisible = false;
            dgvTopProducts.Font = new Font("Segoe UI", 9.5f);

            grpTop.Controls.Add(dgvTopProducts);
            tabTopProducts.Controls.Add(grpTop);
        }

        private void SetupAuditLogsTab()
        {
            GroupBox grpAudit = new GroupBox() { Text = "Nhật Ký Hoạt Động Hệ Thống", Font = new Font("Segoe UI", 9.75f, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80), Dock = DockStyle.Fill, Padding = new Padding(10) };

            dgvAuditLogs = new DataGridView();
            dgvAuditLogs.Dock = DockStyle.Fill;
            dgvAuditLogs.BackgroundColor = Color.White;
            dgvAuditLogs.BorderStyle = BorderStyle.None;
            dgvAuditLogs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAuditLogs.AllowUserToAddRows = false;
            dgvAuditLogs.ReadOnly = true;
            dgvAuditLogs.RowHeadersVisible = false;
            dgvAuditLogs.Font = new Font("Segoe UI", 9.5f);

            grpAudit.Controls.Add(dgvAuditLogs);
            tabAuditLogs.Controls.Add(grpAudit);
        }

        // --- DATA LOADING & ANALYTICS ---
        private void LoadData()
        {
            DateTime start = dtpStart.Value.Date;
            DateTime end = dtpEnd.Value.Date.AddDays(1).AddSeconds(-1); // End of day

            MySqlParameter[] prs = new MySqlParameter[] {
                new MySqlParameter("@start", start),
                new MySqlParameter("@end", end)
            };

            LoadRevenueTab(prs);
            LoadTopProductsTab(prs);
            if (SessionManager.IsAdmin)
            {
                LoadAuditLogsTab(prs);
            }
        }

        private void LoadRevenueTab(MySqlParameter[] datePrs)
        {
            try
            {
                // KPI counts
                string kpiSql = @"
                    SELECT 
                        COALESCE(SUM(total_amount), 0) as Revenue,
                        COUNT(id) as OrdersCount,
                        COALESCE(SUM(discount_amount), 0) as Discount
                    FROM orders
                    WHERE created_at BETWEEN @start AND @end AND order_status = 'completed'";

                DataTable dtKpi = DatabaseHelper.ExecuteQuery(kpiSql, 
                    new MySqlParameter("@start", datePrs[0].Value),
                    new MySqlParameter("@end", datePrs[1].Value)
                );

                decimal rev = 0, disc = 0;
                int orders = 0;

                if (dtKpi.Rows.Count > 0)
                {
                    rev = Convert.ToDecimal(dtKpi.Rows[0]["Revenue"]);
                    orders = Convert.ToInt32(dtKpi.Rows[0]["OrdersCount"]);
                    disc = Convert.ToDecimal(dtKpi.Rows[0]["Discount"]);
                }

                lblTotalRevenue.Text = string.Format("{0:N0} VND", rev);
                lblTotalOrders.Text = orders.ToString("N0");
                lblTotalDiscount.Text = string.Format("-{0:N0} VND", disc);

                // Gross profit calculation
                // profit = (selling_price - cost_price) * qty - discount
                string profitSql = @"
                    SELECT 
                        SUM((oi.unit_price - p.cost_price) * oi.quantity - oi.discount) as Profit
                    FROM order_items oi
                    INNER JOIN products p ON oi.product_id = p.id
                    INNER JOIN orders o ON oi.order_id = o.id
                    WHERE o.created_at BETWEEN @start AND @end AND o.order_status = 'completed'";

                object profitObj = DatabaseHelper.ExecuteScalar(profitSql,
                    new MySqlParameter("@start", datePrs[0].Value),
                    new MySqlParameter("@end", datePrs[1].Value)
                );

                decimal profit = profitObj != null && profitObj != DBNull.Value ? Convert.ToDecimal(profitObj) : 0;
                lblTotalProfit.Text = string.Format("{0:N0} VND", profit);

                // Orders list
                string ordersSql = @"
                    SELECT o.id as MaHD, u.full_name as ThuNgan, 
                           COALESCE(c.name, 'Khách vãng lai') as KhachHang,
                           o.subtotal as TạmTính, o.discount_amount as ChiếtKhấu,
                           o.tax_amount as Thuế, o.total_amount as TổngTiền,
                           o.payment_method as PTThanhToán, o.created_at as NgayGiaoDich
                    FROM orders o
                    INNER JOIN users u ON o.staff_id = u.id
                    LEFT JOIN customers c ON o.customer_id = c.id
                    WHERE o.created_at BETWEEN @start AND @end
                    ORDER BY o.id DESC";

                DataTable dtOrders = DatabaseHelper.ExecuteQuery(ordersSql,
                    new MySqlParameter("@start", datePrs[0].Value),
                    new MySqlParameter("@end", datePrs[1].Value)
                );

                dgvOrders.DataSource = dtOrders;
                dgvOrders.Columns["TạmTính"].DefaultCellStyle.Format = "N0";
                dgvOrders.Columns["ChiếtKhấu"].DefaultCellStyle.Format = "N0";
                dgvOrders.Columns["Thuế"].DefaultCellStyle.Format = "N0";
                dgvOrders.Columns["TổngTiền"].DefaultCellStyle.Format = "N0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi phân tích doanh thu: " + ex.Message);
            }
        }

        private void LoadTopProductsTab(MySqlParameter[] datePrs)
        {
            try
            {
                string sql = @"
                    SELECT p.barcode as Barcode, p.name as TenSanPham, 
                           SUM(oi.quantity) as SoLuongDaBan, 
                           SUM(oi.quantity * oi.unit_price - oi.discount) as DoanhThuMangLai
                    FROM order_items oi
                    INNER JOIN products p ON oi.product_id = p.id
                    INNER JOIN orders o ON oi.order_id = o.id
                    WHERE o.created_at BETWEEN @start AND @end AND o.order_status = 'completed'
                    GROUP BY p.id, p.barcode, p.name
                    ORDER BY SoLuongDaBan DESC
                    LIMIT 20";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql,
                    new MySqlParameter("@start", datePrs[0].Value),
                    new MySqlParameter("@end", datePrs[1].Value)
                );
                dgvTopProducts.DataSource = dt;
                dgvTopProducts.Columns["SoLuongDaBan"].DefaultCellStyle.Format = "N0";
                dgvTopProducts.Columns["DoanhThuMangLai"].DefaultCellStyle.Format = "N0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy danh sách sản phẩm bán chạy: " + ex.Message);
            }
        }

        private void LoadAuditLogsTab(MySqlParameter[] datePrs)
        {
            try
            {
                string sql = @"
                    SELECT al.id, u.username as TaiKhoan, al.action as HanhDong, 
                           al.table_name as BangTacDong, al.record_id as ID_Dong, 
                           al.new_values as MoTaChiTiet, al.created_at as ThoiGian
                    FROM audit_logs al
                    INNER JOIN users u ON al.user_id = u.id
                    WHERE al.created_at BETWEEN @start AND @end
                    ORDER BY al.id DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql,
                    new MySqlParameter("@start", datePrs[0].Value),
                    new MySqlParameter("@end", datePrs[1].Value)
                );
                dgvAuditLogs.DataSource = dt;
                dgvAuditLogs.Columns["id"].Visible = false;
            }
            catch { }
        }

        // --- BUTTON EVENTS ---
        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}

