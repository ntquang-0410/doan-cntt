using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ConvenienceStoreApp.Forms
{
    public class InventoryForm : Form
    {
        private TabControl tabControl;
        private TabPage tabStock;
        private TabPage tabPurchase;

        // Stock Tab UI
        private DataGridView dgvStock;
        private TextBox txtStockSearch;
        private Button btnStockSearch;
        private CheckBox chkLowStockOnly;
        private CheckBox chkExpiredOnly;

        // Purchase Tab UI
        private DataGridView dgvPurchaseOrders;
        private Button btnCreatePO;
        private Button btnReceivePO;

        // Create PO Panel Overlay
        private Panel pnlCreatePO;
        private ComboBox cboPOSupplier;
        private TextBox txtPONote;
        private DataGridView dgvPOItems;
        private DataTable poItemsTable;
        private Button btnAddPOItem;
        private Button btnSavePO;
        private Button btnCancelPO;
        private Label lblPOTotal;
        private decimal poTotalAmount = 0;

        // Helper lists for PO Creation
        private ComboBox cboPOProducts;
        private NumericUpDown numPOQty;
        private NumericUpDown numPOCost;

        public InventoryForm()
        {
            InitializeComponent();
            LoadStock();
            LoadPurchaseOrders();
            LoadPODropdowns();
            SetupPOItemsTable();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Kho Hàng & Nhập Hàng";
            this.BackColor = Color.FromArgb(240, 244, 248);

            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 10);

            tabStock = new TabPage("📦 Tồn Kho Hiện Tại");
            tabStock.BackColor = Color.FromArgb(245, 246, 250);
            InitializeStockTab();

            tabPurchase = new TabPage("🚚 Nhập Hàng (Purchase Orders)");
            tabPurchase.BackColor = Color.FromArgb(245, 246, 250);
            InitializePurchaseTab();

            tabControl.TabPages.Add(tabStock);
            tabControl.TabPages.Add(tabPurchase);

            // Create PO Panel Overlay setup
            SetupCreatePOPanel();

            this.Controls.Add(tabControl);
        }

        private void InitializeStockTab()
        {
            Panel pnlFilter = new Panel();
            pnlFilter.Dock = DockStyle.Top;
            pnlFilter.Height = 50;

            txtStockSearch = new TextBox();
            txtStockSearch.Size = new Size(250, 25);
            txtStockSearch.Location = new Point(15, 12);
            txtStockSearch.PlaceholderText = "Tìm theo tên sản phẩm hoặc barcode...";
            txtStockSearch.KeyDown += TxtStockSearch_KeyDown;

            btnStockSearch = new Button();
            btnStockSearch.Text = "Tìm kiếm";
            btnStockSearch.BackColor = Color.FromArgb(52, 73, 94);
            btnStockSearch.ForeColor = Color.White;
            btnStockSearch.FlatStyle = FlatStyle.Flat;
            btnStockSearch.FlatAppearance.BorderSize = 0;
            btnStockSearch.Size = new Size(90, 26);
            btnStockSearch.Location = new Point(275, 11);
            btnStockSearch.Click += BtnStockSearch_Click;

            chkLowStockOnly = new CheckBox();
            chkLowStockOnly.Text = "⚠️ Sắp hết hàng (Tồn <= Tối thiểu)";
            chkLowStockOnly.AutoSize = true;
            chkLowStockOnly.Location = new Point(390, 14);
            chkLowStockOnly.CheckedChanged += ChkStockFilter_CheckedChanged;

            chkExpiredOnly = new CheckBox();
            chkExpiredOnly.Text = "⏳ Sắp hết hạn sử dụng";
            chkExpiredOnly.AutoSize = true;
            chkExpiredOnly.Location = new Point(620, 14);
            chkExpiredOnly.CheckedChanged += ChkStockFilter_CheckedChanged;

            pnlFilter.Controls.Add(txtStockSearch);
            pnlFilter.Controls.Add(btnStockSearch);
            pnlFilter.Controls.Add(chkLowStockOnly);
            pnlFilter.Controls.Add(chkExpiredOnly);

            dgvStock = new DataGridView();
            dgvStock.Dock = DockStyle.Fill;
            dgvStock.BackgroundColor = Color.White;
            dgvStock.BorderStyle = BorderStyle.None;
            dgvStock.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvStock.AllowUserToAddRows = false;
            dgvStock.ReadOnly = true;
            dgvStock.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStock.RowHeadersVisible = false;
            dgvStock.CellFormatting += DgvStock_CellFormatting;

            tabStock.Controls.Add(dgvStock);
            tabStock.Controls.Add(pnlFilter);
        }

        private void InitializePurchaseTab()
        {
            Panel pnlActions = new Panel();
            pnlActions.Dock = DockStyle.Top;
            pnlActions.Height = 50;

            btnCreatePO = new Button();
            btnCreatePO.Text = "➕ Tạo Đơn Nhập Hàng";
            btnCreatePO.Font = new Font("Segoe UI", 9.5pt, FontStyle.Bold);
            btnCreatePO.BackColor = Color.FromArgb(26, 188, 156);
            btnCreatePO.ForeColor = Color.White;
            btnCreatePO.FlatStyle = FlatStyle.Flat;
            btnCreatePO.FlatAppearance.BorderSize = 0;
            btnCreatePO.Size = new Size(160, 30);
            btnCreatePO.Location = new Point(15, 10);
            btnCreatePO.Click += BtnCreatePO_Click;

            btnReceivePO = new Button();
            btnReceivePO.Text = "✅ Xác Nhận Nhận Hàng (Nhập Kho)";
            btnReceivePO.Font = new Font("Segoe UI", 9.5pt, FontStyle.Bold);
            btnReceivePO.BackColor = Color.FromArgb(46, 204, 113);
            btnReceivePO.ForeColor = Color.White;
            btnReceivePO.FlatStyle = FlatStyle.Flat;
            btnReceivePO.FlatAppearance.BorderSize = 0;
            btnReceivePO.Size = new Size(240, 30);
            btnReceivePO.Location = new Point(190, 10);
            btnReceivePO.Click += BtnReceivePO_Click;

            pnlActions.Controls.Add(btnCreatePO);
            pnlActions.Controls.Add(btnReceivePO);

            dgvPurchaseOrders = new DataGridView();
            dgvPurchaseOrders.Dock = DockStyle.Fill;
            dgvPurchaseOrders.BackgroundColor = Color.White;
            dgvPurchaseOrders.BorderStyle = BorderStyle.None;
            dgvPurchaseOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPurchaseOrders.AllowUserToAddRows = false;
            dgvPurchaseOrders.ReadOnly = true;
            dgvPurchaseOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPurchaseOrders.RowHeadersVisible = false;

            tabPurchase.Controls.Add(dgvPurchaseOrders);
            tabPurchase.Controls.Add(pnlActions);
        }

        private void SetupCreatePOPanel()
        {
            pnlCreatePO = new Panel();
            pnlCreatePO.Size = new Size(680, 580);
            pnlCreatePO.BackColor = Color.White;
            pnlCreatePO.BorderStyle = BorderStyle.FixedSingle;
            pnlCreatePO.Visible = false;

            Label lblPOTitle = new Label();
            lblPOTitle.Text = "TẠO ĐƠN NHẬP HÀNG MỚI (PO)";
            lblPOTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblPOTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblPOTitle.Size = new Size(660, 25);
            lblPOTitle.Location = new Point(10, 15);
            lblPOTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Header fields
            Label lblSupplier = new Label() { Text = "Nhà cung cấp:", Location = new Point(20, 55), AutoSize = true };
            cboPOSupplier = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList, Size = new Size(200, 25), Location = new Point(120, 52) };

            Label lblNote = new Label() { Text = "Ghi chú đơn:", Location = new Point(340, 55), AutoSize = true };
            txtPONote = new TextBox() { Size = new Size(220, 25), Location = new Point(430, 52) };

            // Add item section
            GroupBox grpAddItem = new GroupBox() { Text = "Thêm sản phẩm", Size = new Size(640, 80), Location = new Point(20, 95) };
            
            Label lblProd = new Label() { Text = "Sản phẩm:", Location = new Point(10, 25), AutoSize = true };
            cboPOProducts = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList, Size = new Size(200, 25), Location = new Point(80, 22) };

            Label lblQty = new Label() { Text = "Số lượng:", Location = new Point(290, 25), AutoSize = true };
            numPOQty = new NumericUpDown() { Size = new Size(60, 25), Location = new Point(350, 22), Minimum = 1, Maximum = 100000, Value = 10 };

            Label lblCost = new Label() { Text = "Giá nhập:", Location = new Point(420, 25), AutoSize = true };
            numPOCost = new NumericUpDown() { Size = new Size(100, 25), Location = new Point(480, 22), Minimum = 0, Maximum = 1000000000, Value = 10000, ThousandsSeparator = true };

            btnAddPOItem = new Button() { Text = "Thêm", BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(50, 25), Location = new Point(585, 21) };
            btnAddPOItem.Click += BtnAddPOItem_Click;

            grpAddItem.Controls.Add(lblProd);
            grpAddItem.Controls.Add(cboPOProducts);
            grpAddItem.Controls.Add(lblQty);
            grpAddItem.Controls.Add(numPOQty);
            grpAddItem.Controls.Add(lblCost);
            grpAddItem.Controls.Add(numPOCost);
            grpAddItem.Controls.Add(btnAddPOItem);

            // Item grid
            dgvPOItems = new DataGridView();
            dgvPOItems.Size = new Size(640, 280);
            dgvPOItems.Location = new Point(20, 185);
            dgvPOItems.BackgroundColor = Color.White;
            dgvPOItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPOItems.AllowUserToAddRows = false;
            dgvPOItems.RowHeadersVisible = false;

            lblPOTotal = new Label();
            lblPOTotal.Text = "TỔNG CỘNG: 0 VND";
            lblPOTotal.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblPOTotal.ForeColor = Color.FromArgb(231, 76, 60);
            lblPOTotal.Size = new Size(300, 25);
            lblPOTotal.Location = new Point(20, 480);

            // Actions
            btnSavePO = new Button() { Text = "Lưu Đơn Hàng", BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(130, 35), Location = new Point(390, 520) };
            btnSavePO.Click += BtnSavePO_Click;

            btnCancelPO = new Button() { Text = "Hủy bỏ", BackColor = Color.FromArgb(189, 195, 199), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(100, 35), Location = new Point(530, 520) };
            btnCancelPO.Click += BtnCancelPO_Click;

            pnlCreatePO.Controls.Add(lblPOTitle);
            pnlCreatePO.Controls.Add(lblSupplier);
            pnlCreatePO.Controls.Add(cboPOSupplier);
            pnlCreatePO.Controls.Add(lblNote);
            pnlCreatePO.Controls.Add(txtPONote);
            pnlCreatePO.Controls.Add(grpAddItem);
            pnlCreatePO.Controls.Add(dgvPOItems);
            pnlCreatePO.Controls.Add(lblPOTotal);
            pnlCreatePO.Controls.Add(btnSavePO);
            pnlCreatePO.Controls.Add(btnCancelPO);

            this.Controls.Add(pnlCreatePO);
        }

        private void SetupPOItemsTable()
        {
            poItemsTable = new DataTable();
            poItemsTable.Columns.Add("ProductId", typeof(int));
            poItemsTable.Columns.Add("ProductName", typeof(string));
            poItemsTable.Columns.Add("Quantity", typeof(int));
            poItemsTable.Columns.Add("UnitCost", typeof(decimal));
            poItemsTable.Columns.Add("Total", typeof(decimal));

            dgvPOItems.DataSource = poItemsTable;

            dgvPOItems.Columns["ProductId"].Visible = false;
            dgvPOItems.Columns["UnitCost"].DefaultCellStyle.Format = "N0";
            dgvPOItems.Columns["Total"].DefaultCellStyle.Format = "N0";
        }

        // --- DATA LOADING ---

        private void LoadStock()
        {
            try
            {
                string sql = @"
                    SELECT p.id as ProductId, p.barcode as Barcode, p.name as TenSanPham, 
                           COALESCE(v.variant_name, 'Mặc định') as PhanLoai,
                           COALESCE(i.quantity, 0) as TonKho, 
                           COALESCE(i.min_quantity, 10) as DinhMucToiThieu,
                           i.expiry_date as HanSuDung
                    FROM products p
                    LEFT JOIN product_variants v ON p.id = v.product_id
                    LEFT JOIN inventory i ON p.id = i.product_id AND (v.id = i.variant_id OR (v.id IS NULL AND i.variant_id IS NULL))
                    WHERE p.is_active = 1";

                List<MySqlParameter> prs = new List<MySqlParameter>();

                string search = txtStockSearch.Text.Trim();
                if (!string.IsNullOrEmpty(search))
                {
                    sql += " AND (p.name LIKE @kw OR p.barcode = @kwExact OR v.barcode = @kwExact)";
                    prs.Add(new MySqlParameter("@kw", "%" + search + "%"));
                    prs.Add(new MySqlParameter("@kwExact", search));
                }

                if (chkLowStockOnly.Checked)
                {
                    sql += " AND COALESCE(i.quantity, 0) <= COALESCE(i.min_quantity, 10)";
                }

                if (chkExpiredOnly.Checked)
                {
                    sql += " AND i.expiry_date IS NOT NULL AND i.expiry_date <= DATE_ADD(CURDATE(), INTERVAL 30 DAY)";
                }

                DataTable dt = DatabaseHelper.ExecuteQuery(sql, prs.ToArray());
                dgvStock.DataSource = dt;

                dgvStock.Columns["ProductId"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải tồn kho: " + ex.Message);
            }
        }

        private void LoadPurchaseOrders()
        {
            try
            {
                string sql = @"
                    SELECT po.id as PO_ID, s.name as NhaCungCap, u.full_name as NguoiTao, 
                           po.total_amount as TongTien, po.status as TrangThai, 
                           po.ordered_at as NgayDat, po.received_at as NgayNhan, po.note as GhiChu
                    FROM purchase_orders po
                    INNER JOIN suppliers s ON po.supplier_id = s.id
                    INNER JOIN users u ON po.staff_id = u.id
                    ORDER BY po.id DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql);
                dgvPurchaseOrders.DataSource = dt;

                dgvPurchaseOrders.Columns["TongTien"].DefaultCellStyle.Format = "N0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải hóa đơn nhập hàng: " + ex.Message);
            }
        }

        private void LoadPODropdowns()
        {
            try
            {
                // Supplier dropdown
                DataTable dtSup = DatabaseHelper.ExecuteQuery("SELECT id, name FROM suppliers");
                cboPOSupplier.DataSource = dtSup;
                cboPOSupplier.DisplayMember = "name";
                cboPOSupplier.ValueMember = "id";

                // Product dropdown for PO items
                DataTable dtProd = DatabaseHelper.ExecuteQuery("SELECT id, name FROM products WHERE is_active = 1");
                cboPOProducts.DataSource = dtProd;
                cboPOProducts.DisplayMember = "name";
                cboPOProducts.ValueMember = "id";
            }
            catch { }
        }

        // --- STOCK EVENTS ---
        private void BtnStockSearch_Click(object sender, EventArgs e)
        {
            LoadStock();
        }

        private void TxtStockSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadStock();
                e.SuppressKeyPress = true;
            }
        }

        private void ChkStockFilter_CheckedChanged(object sender, EventArgs e)
        {
            LoadStock();
        }

        private void DgvStock_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Highlight low stock (in red)
            if (e.RowIndex >= 0 && dgvStock.Columns[e.ColumnIndex].Name == "TonKho")
            {
                int qty = Convert.ToInt32(dgvStock.Rows[e.RowIndex].Cells["TonKho"].Value);
                int min = Convert.ToInt32(dgvStock.Rows[e.RowIndex].Cells["DinhMucToiThieu"].Value);
                
                if (qty <= min)
                {
                    e.CellStyle.ForeColor = Color.Red;
                    e.CellStyle.Font = new Font(dgvStock.Font, FontStyle.Bold);
                }
            }

            // Highlight expiring items
            if (e.RowIndex >= 0 && dgvStock.Columns[e.ColumnIndex].Name == "HanSuDung")
            {
                object expiryObj = dgvStock.Rows[e.RowIndex].Cells["HanSuDung"].Value;
                if (expiryObj != null && expiryObj != DBNull.Value)
                {
                    DateTime expiry = Convert.ToDateTime(expiryObj);
                    if (expiry <= DateTime.Today.AddDays(7))
                    {
                        e.CellStyle.BackColor = Color.LightPink;
                    }
                    else if (expiry <= DateTime.Today.AddDays(30))
                    {
                        e.CellStyle.BackColor = Color.LightYellow;
                    }
                }
            }
        }

        // --- PURCHASE ORDER ACTIONS ---
        private void BtnCreatePO_Click(object sender, EventArgs e)
        {
            txtPONote.Text = "";
            poItemsTable.Rows.Clear();
            CalculatePOTotal();

            pnlCreatePO.Location = new Point((tabControl.Width - pnlCreatePO.Width) / 2, (tabControl.Height - pnlCreatePO.Height) / 2);
            pnlCreatePO.Visible = true;
            pnlCreatePO.BringToFront();
        }

        private void BtnAddPOItem_Click(object sender, EventArgs e)
        {
            if (cboPOProducts.SelectedValue == null) return;
            
            int prodId = Convert.ToInt32(cboPOProducts.SelectedValue);
            string prodName = cboPOProducts.Text;
            int qty = (int)numPOQty.Value;
            decimal cost = numPOCost.Value;

            // Check if already in PO
            foreach (DataRow row in poItemsTable.Rows)
            {
                if (Convert.ToInt32(row["ProductId"]) == prodId)
                {
                    row["Quantity"] = Convert.ToInt32(row["Quantity"]) + qty;
                    row["Total"] = Convert.ToInt32(row["Quantity"]) * Convert.ToDecimal(row["UnitCost"]);
                    CalculatePOTotal();
                    return;
                }
            }

            DataRow r = poItemsTable.NewRow();
            r["ProductId"] = prodId;
            r["ProductName"] = prodName;
            r["Quantity"] = qty;
            r["UnitCost"] = cost;
            r["Total"] = qty * cost;
            poItemsTable.Rows.Add(r);

            CalculatePOTotal();
        }

        private void CalculatePOTotal()
        {
            poTotalAmount = 0;
            foreach (DataRow r in poItemsTable.Rows)
            {
                poTotalAmount += Convert.ToDecimal(r["Total"]);
            }
            lblPOTotal.Text = string.Format("TỔNG CỘNG: {0:N0} VND", poTotalAmount);
        }

        private void BtnSavePO_Click(object sender, EventArgs e)
        {
            if (cboPOSupplier.SelectedValue == null) return;
            if (poItemsTable.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm sản phẩm vào đơn nhập hàng.", "Đơn trống");
                return;
            }

            int supId = Convert.ToInt32(cboPOSupplier.SelectedValue);
            string note = txtPONote.Text.Trim();

            using (MySqlConnection conn = DatabaseHelper.GetConnection())
            {
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Insert into purchase_orders (status is pending initially)
                        string poSql = @"
                            INSERT INTO purchase_orders 
                                (supplier_id, staff_id, total_amount, status, note) 
                            VALUES 
                                (@sup, @staff, @total, 'pending', @note);
                            SELECT LAST_INSERT_ID();";

                        MySqlCommand cmdPO = new MySqlCommand(poSql, conn, trans);
                        cmdPO.Parameters.AddWithValue("@sup", supId);
                        cmdPO.Parameters.AddWithValue("@staff", SessionManager.UserId);
                        cmdPO.Parameters.AddWithValue("@total", poTotalAmount);
                        cmdPO.Parameters.AddWithValue("@note", string.IsNullOrEmpty(note) ? "Nhập hàng từ nhà cung cấp" : note);

                        int poId = Convert.ToInt32(cmdPO.ExecuteScalar());

                        // 2. Insert into purchase_order_items
                        foreach (DataRow row in poItemsTable.Rows)
                        {
                            string itemSql = @"
                                INSERT INTO purchase_order_items 
                                    (order_id, product_id, variant_id, quantity, unit_cost) 
                                VALUES 
                                    (@oid, @pid, NULL, @qty, @cost)";
                            
                            MySqlCommand cmdItem = new MySqlCommand(itemSql, conn, trans);
                            cmdItem.Parameters.AddWithValue("@oid", poId);
                            cmdItem.Parameters.AddWithValue("@pid", Convert.ToInt32(row["ProductId"]));
                            cmdItem.Parameters.AddWithValue("@qty", Convert.ToInt32(row["Quantity"]));
                            cmdItem.Parameters.AddWithValue("@cost", Convert.ToDecimal(row["UnitCost"]));
                            cmdItem.ExecuteNonQuery();
                        }

                        trans.Commit();
                        pnlCreatePO.Visible = false;
                        LoadPurchaseOrders();
                        MessageBox.Show("Tạo đơn nhập hàng #" + poId + " thành công! Vui lòng bấm 'Xác Nhận Nhận Hàng' khi hàng về để nhập kho.", "Thành công");
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show("Tạo đơn thất bại: " + ex.Message);
                    }
                }
            }
        }

        private void BtnCancelPO_Click(object sender, EventArgs e)
        {
            pnlCreatePO.Visible = false;
        }

        private void BtnReceivePO_Click(object sender, EventArgs e)
        {
            if (dgvPurchaseOrders.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn đơn nhập hàng cần nhận.", "Chọn đơn hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int poId = Convert.ToInt32(dgvPurchaseOrders.CurrentRow.Cells["PO_ID"].Value);
            string status = dgvPurchaseOrders.CurrentRow.Cells["TrangThai"].Value.ToString();

            if (status == "received")
            {
                MessageBox.Show("Đơn hàng này đã được nhập kho trước đó.", "Đã nhận rồi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (status == "cancelled")
            {
                MessageBox.Show("Đơn hàng này đã bị hủy.", "Đã hủy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dr = MessageBox.Show("Xác nhận đã nhận đủ hàng từ nhà cung cấp và nhập kho đơn hàng #" + poId + "?", "Xác nhận nhập kho", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No) return;

            try
            {
                // Setting status to 'received' triggers the BEFORE UPDATE trigger `trg_purchase_received` in MySQL!
                // This will automatically increase inventory levels and log stock movements.
                string sql = "UPDATE purchase_orders SET status = 'received' WHERE id = @id";
                DatabaseHelper.ExecuteNonQuery(sql, new MySqlParameter("@id", poId));

                MessageBox.Show("Nhập kho thành công! Số lượng sản phẩm tồn kho đã được tự động cộng thêm.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPurchaseOrders();
                LoadStock(); // Refresh stock tab
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nhập kho: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
