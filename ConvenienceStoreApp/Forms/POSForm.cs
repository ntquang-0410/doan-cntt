using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ConvenienceStoreApp.Forms
{
    public class POSForm : Form
    {
        // UI Controls
        private Panel leftPanel;
        private Panel rightPanel;
        
        // Cart controls
        private DataGridView dgvCart;
        private Label lblCartTitle;
        private Button btnRemoveItem;
        private Button btnClearCart;
        private NumericUpDown numQty;
        private Label lblQty;

        // Search and Info controls
        private GroupBox grpProductSearch;
        private TextBox txtBarcodeSearch;
        private Button btnAddProduct;
        private ListBox lstProductResults;

        // Customer controls
        private GroupBox grpCustomer;
        private TextBox txtCustomerPhone;
        private Button btnSearchCustomer;
        private Label lblCustomerInfo;
        private int selectedCustomerId = -1;
        private int customerPoints = 0;

        // Checkout controls
        private GroupBox grpCheckout;
        private Label lblSubtotalValue;
        private Label lblDiscountValue;
        private Label lblTaxValue;
        private Label lblTotalValue;
        private ComboBox cboPaymentMethod;
        private TextBox txtNote;
        private Button btnPay;

        // Data Table for Cart
        private DataTable cartTable;
        
        // Settings
        private decimal taxRate = 8.00m; // 8% default
        private decimal subtotal = 0.00m;
        private decimal discountAmount = 0.00m;
        private decimal taxAmount = 0.00m;
        private decimal totalAmount = 0.00m;
        private int selectedPromotionId = -1;

        public POSForm()
        {
            InitializeComponent();
            SetupCartTable();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "Quầy Bán Hàng (POS) - Cửa Hàng Tiện Lợi";
            this.BackColor = Color.FromArgb(240, 244, 248);

            // Split into Left (Cart) and Right (Info/Checkout)
            leftPanel = new Panel();
            leftPanel.Dock = DockStyle.Fill;
            leftPanel.Padding = new Padding(15);

            rightPanel = new Panel();
            rightPanel.Width = 450;
            rightPanel.Dock = DockStyle.Right;
            rightPanel.Padding = new Padding(15);
            rightPanel.BackColor = Color.White;
            rightPanel.Paint += RightPanel_Paint;

            // --- LEFT PANEL CONTROLS (Cart) ---
            lblCartTitle = new Label();
            lblCartTitle.Text = "🛒 GIỎ HÀNG THANH TOÁN";
            lblCartTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblCartTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblCartTitle.Height = 25;
            lblCartTitle.Dock = DockStyle.Top;

            dgvCart = new DataGridView();
            dgvCart.Dock = DockStyle.Top;
            dgvCart.Height = 450;
            dgvCart.BackgroundColor = Color.White;
            dgvCart.BorderStyle = BorderStyle.FixedSingle;
            dgvCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCart.AutoGenerateColumns = false;
            dgvCart.AllowUserToAddRows = false;
            dgvCart.ReadOnly = true;
            dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCart.MultiSelect = false;
            dgvCart.RowHeadersVisible = false;
            dgvCart.Font = new Font("Segoe UI", 10);
            dgvCart.CellClick += DgvCart_CellClick;

            Panel cartActionsPanel = new Panel();
            cartActionsPanel.Dock = DockStyle.Top;
            cartActionsPanel.Height = 50;
            cartActionsPanel.Padding = new Padding(0, 10, 0, 0);

            lblQty = new Label();
            lblQty.Text = "Số lượng:";
            lblQty.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            lblQty.Location = new Point(5, 15);
            lblQty.Width = 60;

            numQty = new NumericUpDown();
            numQty.Font = new Font("Segoe UI", 10);
            numQty.Location = new Point(70, 13);
            numQty.Width = 60;
            numQty.Minimum = 1;
            numQty.Maximum = 1000;
            numQty.Value = 1;
            numQty.ValueChanged += NumQty_ValueChanged;

            btnRemoveItem = new Button();
            btnRemoveItem.Text = "❌ Xóa Khỏi Giỏ";
            btnRemoveItem.Font = new Font("Segoe UI", 9.75f, FontStyle.Bold);
            btnRemoveItem.ForeColor = Color.White;
            btnRemoveItem.BackColor = Color.FromArgb(231, 76, 60);
            btnRemoveItem.FlatStyle = FlatStyle.Flat;
            btnRemoveItem.FlatAppearance.BorderSize = 0;
            btnRemoveItem.Cursor = Cursors.Hand;
            btnRemoveItem.Size = new Size(130, 30);
            btnRemoveItem.Location = new Point(150, 10);
            btnRemoveItem.Click += BtnRemoveItem_Click;

            btnClearCart = new Button();
            btnClearCart.Text = "🗑️ Hủy Giỏ Hàng";
            btnClearCart.Font = new Font("Segoe UI", 9.75f, FontStyle.Bold);
            btnClearCart.ForeColor = Color.White;
            btnClearCart.BackColor = Color.FromArgb(127, 140, 141);
            btnClearCart.FlatStyle = FlatStyle.Flat;
            btnClearCart.FlatAppearance.BorderSize = 0;
            btnClearCart.Cursor = Cursors.Hand;
            btnClearCart.Size = new Size(130, 30);
            btnClearCart.Location = new Point(290, 10);
            btnClearCart.Click += BtnClearCart_Click;

            cartActionsPanel.Controls.Add(lblQty);
            cartActionsPanel.Controls.Add(numQty);
            cartActionsPanel.Controls.Add(btnRemoveItem);
            cartActionsPanel.Controls.Add(btnClearCart);

            leftPanel.Controls.Add(cartActionsPanel);
            leftPanel.Controls.Add(dgvCart);
            leftPanel.Controls.Add(lblCartTitle);

            // --- RIGHT PANEL CONTROLS (Search & Checkout) ---
            
            // 1. Product Search Group
            grpProductSearch = new GroupBox();
            grpProductSearch.Text = "Tìm Kiếm Sản Phẩm (Tên hoặc Barcode)";
            grpProductSearch.Font = new Font("Segoe UI", 9.75f, FontStyle.Bold);
            grpProductSearch.ForeColor = Color.FromArgb(44, 62, 80);
            grpProductSearch.Size = new Size(420, 160);
            grpProductSearch.Location = new Point(15, 15);

            txtBarcodeSearch = new TextBox();
            txtBarcodeSearch.Font = new Font("Segoe UI", 11);
            txtBarcodeSearch.Location = new Point(15, 25);
            txtBarcodeSearch.Size = new Size(290, 27);
            txtBarcodeSearch.TextChanged += TxtBarcodeSearch_TextChanged;
            txtBarcodeSearch.KeyDown += TxtBarcodeSearch_KeyDown;

            btnAddProduct = new Button();
            btnAddProduct.Text = "Thêm";
            btnAddProduct.Font = new Font("Segoe UI", 9.75f, FontStyle.Bold);
            btnAddProduct.BackColor = Color.FromArgb(26, 188, 156);
            btnAddProduct.ForeColor = Color.White;
            btnAddProduct.FlatStyle = FlatStyle.Flat;
            btnAddProduct.FlatAppearance.BorderSize = 0;
            btnAddProduct.Size = new Size(80, 27);
            btnAddProduct.Location = new Point(320, 25);
            btnAddProduct.Click += BtnAddProduct_Click;

            lstProductResults = new ListBox();
            lstProductResults.Font = new Font("Segoe UI", 9.75f);
            lstProductResults.Location = new Point(15, 60);
            lstProductResults.Size = new Size(385, 80);
            lstProductResults.Visible = false;
            lstProductResults.DoubleClick += LstProductResults_DoubleClick;

            grpProductSearch.Controls.Add(txtBarcodeSearch);
            grpProductSearch.Controls.Add(btnAddProduct);
            grpProductSearch.Controls.Add(lstProductResults);

            // 2. Customer Member Group
            grpCustomer = new GroupBox();
            grpCustomer.Text = "Khách Hàng Thành Viên";
            grpCustomer.Font = new Font("Segoe UI", 9.75f, FontStyle.Bold);
            grpCustomer.ForeColor = Color.FromArgb(44, 62, 80);
            grpCustomer.Size = new Size(420, 110);
            grpCustomer.Location = new Point(15, 190);

            txtCustomerPhone = new TextBox();
            txtCustomerPhone.Font = new Font("Segoe UI", 11);
            txtCustomerPhone.Location = new Point(15, 25);
            txtCustomerPhone.Size = new Size(240, 27);

            btnSearchCustomer = new Button();
            btnSearchCustomer.Text = "Tìm kiếm";
            btnSearchCustomer.Font = new Font("Segoe UI", 9.75f, FontStyle.Bold);
            btnSearchCustomer.BackColor = Color.FromArgb(52, 152, 219);
            btnSearchCustomer.ForeColor = Color.White;
            btnSearchCustomer.FlatStyle = FlatStyle.Flat;
            btnSearchCustomer.FlatAppearance.BorderSize = 0;
            btnSearchCustomer.Size = new Size(130, 27);
            btnSearchCustomer.Location = new Point(270, 25);
            btnSearchCustomer.Click += BtnSearchCustomer_Click;

            lblCustomerInfo = new Label();
            lblCustomerInfo.Text = "Khách vãng lai (Không tích điểm)";
            lblCustomerInfo.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            lblCustomerInfo.ForeColor = Color.FromArgb(127, 140, 141);
            lblCustomerInfo.Location = new Point(15, 65);
            lblCustomerInfo.Size = new Size(385, 35);

            grpCustomer.Controls.Add(txtCustomerPhone);
            grpCustomer.Controls.Add(btnSearchCustomer);
            grpCustomer.Controls.Add(lblCustomerInfo);

            // 3. Checkout Summary Group
            grpCheckout = new GroupBox();
            grpCheckout.Text = "Chi Tiết Thanh Toán";
            grpCheckout.Font = new Font("Segoe UI", 9.75f, FontStyle.Bold);
            grpCheckout.ForeColor = Color.FromArgb(44, 62, 80);
            grpCheckout.Size = new Size(420, 390);
            grpCheckout.Location = new Point(15, 315);

            int labelX1 = 20, labelX2 = 200, rowY = 30, rowHeight = 35;

            CreateCheckoutLabel("Tiền hàng:", rowY, labelX1);
            lblSubtotalValue = CreateCheckoutValue("0 VND", rowY, labelX2);
            rowY += rowHeight;

            CreateCheckoutLabel("Giảm giá (Khuyến mãi):", rowY, labelX1);
            lblDiscountValue = CreateCheckoutValue("0 VND", rowY, labelX2);
            rowY += rowHeight;

            CreateCheckoutLabel("Thuế VAT (" + taxRate + "%):", rowY, labelX1);
            lblTaxValue = CreateCheckoutValue("0 VND", rowY, labelX2);
            rowY += rowHeight;

            CreateCheckoutLabel("TỔNG CỘNG:", rowY, labelX1, 12, Color.FromArgb(231, 76, 60));
            lblTotalValue = CreateCheckoutValue("0 VND", rowY, labelX2, 14, Color.FromArgb(231, 76, 60));
            rowY += 40;

            CreateCheckoutLabel("PT Thanh Toán:", rowY, labelX1);
            cboPaymentMethod = new ComboBox();
            cboPaymentMethod.Font = new Font("Segoe UI", 10);
            cboPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPaymentMethod.Items.AddRange(new string[] { "Tiền mặt (cash)", "Thẻ (card)", "Ví điện tử (e_wallet)" });
            cboPaymentMethod.SelectedIndex = 0;
            cboPaymentMethod.Size = new Size(200, 25);
            cboPaymentMethod.Location = new Point(200, rowY - 3);
            grpCheckout.Controls.Add(cboPaymentMethod);
            rowY += rowHeight;

            CreateCheckoutLabel("Ghi chú:", rowY, labelX1);
            txtNote = new TextBox();
            txtNote.Font = new Font("Segoe UI", 10);
            txtNote.Size = new Size(200, 45);
            txtNote.Multiline = true;
            txtNote.Location = new Point(200, rowY - 3);
            grpCheckout.Controls.Add(txtNote);
            rowY += 55;

            btnPay = new Button();
            btnPay.Text = "💳 THANH TOÁN (IN HÓA ĐƠN)";
            btnPay.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnPay.BackColor = Color.FromArgb(46, 204, 113); // Green
            btnPay.ForeColor = Color.White;
            btnPay.FlatStyle = FlatStyle.Flat;
            btnPay.FlatAppearance.BorderSize = 0;
            btnPay.Cursor = Cursors.Hand;
            btnPay.Size = new Size(380, 50);
            btnPay.Location = new Point(20, rowY);
            btnPay.Click += BtnPay_Click;

            grpCheckout.Controls.Add(btnPay);

            rightPanel.Controls.Add(grpProductSearch);
            rightPanel.Controls.Add(grpCustomer);
            rightPanel.Controls.Add(grpCheckout);

            this.Controls.Add(leftPanel);
            this.Controls.Add(rightPanel);
        }

        private void SetupCartTable()
        {
            cartTable = new DataTable();
            cartTable.Columns.Add("ProductId", typeof(int));
            DataColumn variantColumn = cartTable.Columns.Add("VariantId", typeof(int));
            variantColumn.AllowDBNull = true;
            cartTable.Columns.Add("Barcode", typeof(string));
            cartTable.Columns.Add("ProductName", typeof(string));
            cartTable.Columns.Add("VariantName", typeof(string));
            cartTable.Columns.Add("Quantity", typeof(int));
            cartTable.Columns.Add("UnitPrice", typeof(decimal));
            cartTable.Columns.Add("Discount", typeof(decimal));
            cartTable.Columns.Add("Total", typeof(decimal));

            SetupCartGridColumns();
            dgvCart.DataSource = cartTable;
        }

        private void SetupCartGridColumns()
        {
            dgvCart.Columns.Clear();

            AddCartTextColumn("ProductId", "ProductId", "ProductId", false, 0, null);
            AddCartTextColumn("VariantId", "VariantId", "VariantId", false, 0, null);
            AddCartTextColumn("Barcode", "Barcode", "Mã Vạch", true, 90, null);
            AddCartTextColumn("ProductName", "ProductName", "Sản Phẩm", true, 180, null);
            AddCartTextColumn("VariantName", "VariantName", "Phân Loại", true, 90, null);
            AddCartTextColumn("Quantity", "Quantity", "SL", true, 45, null);
            AddCartTextColumn("UnitPrice", "UnitPrice", "Đơn Giá", true, 90, "N0");
            AddCartTextColumn("Discount", "Discount", "Khấu Trừ", true, 90, "N0");
            AddCartTextColumn("Total", "Total", "Thành Tiền", true, 100, "N0");
        }

        private void AddCartTextColumn(string name, string dataPropertyName, string headerText, bool visible, int width, string format)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.Name = name;
            column.DataPropertyName = dataPropertyName;
            column.HeaderText = headerText;
            column.Visible = visible;
            if (width > 0)
            {
                column.Width = width;
            }
            if (!string.IsNullOrEmpty(format))
            {
                column.DefaultCellStyle.Format = format;
            }
            dgvCart.Columns.Add(column);
        }

        private void LoadSettings()
        {
            try
            {
                object taxValObj = DatabaseHelper.ExecuteScalar("SELECT setting_value FROM settings WHERE setting_key = 'tax_rate'");
                if (taxValObj != null)
                {
                    taxRate = decimal.Parse(taxValObj.ToString());
                }
            }
            catch { }
        }

        private void RightPanel_Paint(object sender, PaintEventArgs e)
        {
            // Left gray border
            using (Pen pen = new Pen(Color.FromArgb(220, 224, 230), 1))
            {
                e.Graphics.DrawLine(pen, 0, 0, 0, rightPanel.Height);
            }
        }

        private void CreateCheckoutLabel(string text, int top, int left, int fontSize = 10, Color? color = null)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", fontSize, fontSize > 10 ? FontStyle.Bold : FontStyle.Regular);
            lbl.ForeColor = color ?? Color.FromArgb(71, 84, 103);
            lbl.Location = new Point(left, top);
            lbl.AutoSize = true;
            grpCheckout.Controls.Add(lbl);
        }

        private Label CreateCheckoutValue(string text, int top, int left, int fontSize = 10, Color? color = null)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
            lbl.ForeColor = color ?? Color.FromArgb(44, 62, 80);
            lbl.Location = new Point(left, top);
            lbl.Width = 200;
            lbl.TextAlign = ContentAlignment.MiddleRight;
            grpCheckout.Controls.Add(lbl);
            return lbl;
        }

        // --- PRODUCT SEARCH EVENTS ---
        private void TxtBarcodeSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtBarcodeSearch.Text.Trim();
            if (keyword.Length >= 2)
            {
                try
                {
                    // Search in products and variants
                    string sql = @"
                        SELECT p.id as product_id, v.id as variant_id, p.name, v.variant_name, 
                               COALESCE(v.barcode, p.barcode) as barcode, 
                               (p.selling_price + COALESCE(v.price_adjustment, 0)) as price
                        FROM products p
                        LEFT JOIN product_variants v ON p.id = v.product_id
                        WHERE p.is_active = 1 
                          AND (p.name LIKE @kw 
                               OR p.barcode = @kwExact 
                               OR v.barcode = @kwExact 
                               OR v.variant_name LIKE @kw)";
                    
                    MySqlParameter[] prs = new MySqlParameter[] {
                        new MySqlParameter("@kw", "%" + keyword + "%"),
                        new MySqlParameter("@kwExact", keyword)
                    };

                    DataTable dt = DatabaseHelper.ExecuteQuery(sql, prs);
                    lstProductResults.Items.Clear();
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string itemText = string.Format("{0} | {1} - {2:N0}đ ({3})",
                                row["barcode"], row["name"], row["price"], row["variant_name"]);
                            
                            // Tag row data
                            lstProductResults.Items.Add(new ProductSearchItem {
                                ProductId = Convert.ToInt32(row["product_id"]),
                                VariantId = row["variant_id"] != DBNull.Value ? Convert.ToInt32(row["variant_id"]) : (int?)null,
                                Barcode = row["barcode"].ToString(),
                                ProductName = row["name"].ToString(),
                                VariantName = row["variant_name"] != DBNull.Value ? row["variant_name"].ToString() : "",
                                Price = Convert.ToDecimal(row["price"])
                            });
                        }
                        lstProductResults.Visible = true;
                        lstProductResults.BringToFront();
                    }
                    else
                    {
                        lstProductResults.Visible = false;
                    }
                }
                catch { }
            }
            else
            {
                lstProductResults.Visible = false;
            }
        }

        private class ProductSearchItem
        {
            public int ProductId { get; set; }
            public int? VariantId { get; set; }
            public string Barcode { get; set; }
            public string ProductName { get; set; }
            public string VariantName { get; set; }
            public decimal Price { get; set; }

            public override string ToString()
            {
                return string.IsNullOrEmpty(VariantName) 
                    ? string.Format("{0} - {1:N0} VND", ProductName, Price)
                    : string.Format("{0} ({1}) - {2:N0} VND", ProductName, VariantName, Price);
            }
        }

        private void TxtBarcodeSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (lstProductResults.Visible && lstProductResults.Items.Count > 0)
                {
                    AddSearchItemToCart((ProductSearchItem)lstProductResults.Items[0]);
                    txtBarcodeSearch.Text = "";
                    lstProductResults.Visible = false;
                }
                else
                {
                    // Direct barcode scan match
                    BtnAddProduct_Click(this, EventArgs.Empty);
                }
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Down && lstProductResults.Visible)
            {
                lstProductResults.Focus();
                if (lstProductResults.Items.Count > 0) lstProductResults.SelectedIndex = 0;
            }
        }

        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            string barcode = txtBarcodeSearch.Text.Trim();
            if (string.IsNullOrEmpty(barcode)) return;

            try
            {
                string sql = @"
                    SELECT p.id as product_id, v.id as variant_id, p.name, v.variant_name, 
                           COALESCE(v.barcode, p.barcode) as barcode, 
                           (p.selling_price + COALESCE(v.price_adjustment, 0)) as price
                    FROM products p
                    LEFT JOIN product_variants v ON p.id = v.product_id
                    WHERE p.is_active = 1 
                      AND (p.barcode = @bc OR v.barcode = @bc)
                    LIMIT 1";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql, new MySqlParameter("@bc", barcode));

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    ProductSearchItem item = new ProductSearchItem {
                        ProductId = Convert.ToInt32(row["product_id"]),
                        VariantId = row["variant_id"] != DBNull.Value ? Convert.ToInt32(row["variant_id"]) : (int?)null,
                        Barcode = row["barcode"].ToString(),
                        ProductName = row["name"].ToString(),
                        VariantName = row["variant_name"] != DBNull.Value ? row["variant_name"].ToString() : "",
                        Price = Convert.ToDecimal(row["price"])
                    };
                    AddSearchItemToCart(item);
                    txtBarcodeSearch.Text = "";
                    lstProductResults.Visible = false;
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sản phẩm có mã vạch: " + barcode, "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LstProductResults_DoubleClick(object sender, EventArgs e)
        {
            ProductSearchItem item = lstProductResults.SelectedItem as ProductSearchItem;
            if (item != null)
            {
                AddSearchItemToCart(item);
                txtBarcodeSearch.Text = "";
                lstProductResults.Visible = false;
                txtBarcodeSearch.Focus();
            }
        }

        private void AddSearchItemToCart(ProductSearchItem item)
        {
            // Check if item already exists in cart
            foreach (DataRow row in cartTable.Rows)
            {
                int pId = Convert.ToInt32(row["ProductId"]);
                int? vId = row["VariantId"] != DBNull.Value ? Convert.ToInt32(row["VariantId"]) : (int?)null;

                if (pId == item.ProductId && vId == item.VariantId)
                {
                    // Increase Qty
                    row["Quantity"] = Convert.ToInt32(row["Quantity"]) + 1;
                    row["Total"] = Convert.ToInt32(row["Quantity"]) * Convert.ToDecimal(row["UnitPrice"]) - Convert.ToDecimal(row["Discount"]);
                    RecalculateTotals();
                    return;
                }
            }

            // Verify inventory quantity first
            try
            {
                string sql = "SELECT quantity FROM inventory WHERE product_id = @pid AND (variant_id = @vid OR (variant_id IS NULL AND @vid IS NULL))";
                object qtyObj = DatabaseHelper.ExecuteScalar(sql, 
                    new MySqlParameter("@pid", item.ProductId),
                    new MySqlParameter("@vid", item.VariantId.HasValue ? (object)item.VariantId.Value : DBNull.Value)
                );
                int stockQty = qtyObj != null ? Convert.ToInt32(qtyObj) : 0;
                if (stockQty <= 0)
                {
                    DialogResult dr = MessageBox.Show(string.Format("Sản phẩm này đã hết hàng trong kho. Bạn có muốn tiếp tục bán không?", item.ProductName), "Hết hàng", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dr == DialogResult.No) return;
                }
            }
            catch { }

            // Add new row to cart
            DataRow newRow = cartTable.NewRow();
            newRow["ProductId"] = item.ProductId;
            newRow["VariantId"] = item.VariantId.HasValue ? (object)item.VariantId.Value : DBNull.Value;
            newRow["Barcode"] = item.Barcode;
            newRow["ProductName"] = item.ProductName;
            newRow["VariantName"] = item.VariantName;
            newRow["Quantity"] = 1;
            newRow["UnitPrice"] = item.Price;
            newRow["Discount"] = 0.00m;
            newRow["Total"] = item.Price;

            cartTable.Rows.Add(newRow);
            RecalculateTotals();
        }

        // --- CART INTERACTIONS ---
        private void DgvCart_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCart.CurrentRow != null)
            {
                int qty = Convert.ToInt32(dgvCart.CurrentRow.Cells["Quantity"].Value);
                numQty.Value = qty;
            }
        }

        private void NumQty_ValueChanged(object sender, EventArgs e)
        {
            if (dgvCart.CurrentRow != null)
            {
                int newQty = (int)numQty.Value;
                int rowIndex = dgvCart.CurrentRow.Index;
                DataRow row = cartTable.Rows[rowIndex];

                row["Quantity"] = newQty;
                row["Total"] = newQty * Convert.ToDecimal(row["UnitPrice"]) - Convert.ToDecimal(row["Discount"]);
                RecalculateTotals();
            }
        }

        private void BtnRemoveItem_Click(object sender, EventArgs e)
        {
            if (dgvCart.CurrentRow != null)
            {
                int rowIndex = dgvCart.CurrentRow.Index;
                cartTable.Rows.RemoveAt(rowIndex);
                RecalculateTotals();
            }
        }

        private void BtnClearCart_Click(object sender, EventArgs e)
        {
            if (cartTable.Rows.Count > 0)
            {
                DialogResult dr = MessageBox.Show("Bạn có chắc chắn muốn hủy toàn bộ giỏ hàng?", "Xác nhận hủy", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    cartTable.Rows.Clear();
                    RecalculateTotals();
                }
            }
        }

        private void RecalculateTotals()
        {
            subtotal = 0.00m;
            foreach (DataRow row in cartTable.Rows)
            {
                subtotal += Convert.ToDecimal(row["Total"]);
            }

            // Apply automatic promotions based on subtotal
            discountAmount = 0.00m;
            selectedPromotionId = -1;
            try
            {
                // Find active promotion applicable for this order value
                string promoSql = @"
                    SELECT id, name, type, value 
                    FROM promotions 
                    WHERE is_active = 1 
                      AND min_order_value <= @sub 
                      AND start_date <= CURDATE() 
                      AND end_date >= CURDATE()
                    ORDER BY min_order_value DESC, value DESC
                    LIMIT 1";
                
                DataTable dtPromo = DatabaseHelper.ExecuteQuery(promoSql, new MySqlParameter("@sub", subtotal));
                if (dtPromo.Rows.Count > 0)
                {
                    DataRow pRow = dtPromo.Rows[0];
                    selectedPromotionId = Convert.ToInt32(pRow["id"]);
                    string type = pRow["type"].ToString();
                    decimal val = Convert.ToDecimal(pRow["value"]);

                    if (type == "percent")
                    {
                        discountAmount = subtotal * (val / 100m);
                    }
                    else if (type == "fixed")
                    {
                        discountAmount = val;
                    }
                    
                    // Cap discount
                    if (discountAmount > subtotal) discountAmount = subtotal;
                }
            }
            catch { }

            taxAmount = Math.Round((subtotal - discountAmount) * (taxRate / 100m), 0);
            totalAmount = subtotal - discountAmount + taxAmount;
            if (totalAmount < 0) totalAmount = 0;

            // Update Labels
            lblSubtotalValue.Text = string.Format("{0:N0} VND", subtotal);
            lblDiscountValue.Text = string.Format("-{0:N0} VND", discountAmount);
            lblTaxValue.Text = string.Format("{0:N0} VND", taxAmount);
            lblTotalValue.Text = string.Format("{0:N0} VND", totalAmount);
        }

        // --- CUSTOMER SEARCH ---
        private void BtnSearchCustomer_Click(object sender, EventArgs e)
        {
            string phone = txtCustomerPhone.Text.Trim();
            if (string.IsNullOrEmpty(phone))
            {
                selectedCustomerId = -1;
                customerPoints = 0;
                lblCustomerInfo.Text = "Khách vãng lai (Không tích điểm)";
                lblCustomerInfo.ForeColor = Color.FromArgb(127, 140, 141);
                return;
            }

            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(
                    "SELECT id, name, loyalty_points FROM customers WHERE phone = @phone AND is_active = 1",
                    new MySqlParameter("@phone", phone)
                );

                if (dt.Rows.Count > 0)
                {
                    selectedCustomerId = Convert.ToInt32(dt.Rows[0]["id"]);
                    customerPoints = Convert.ToInt32(dt.Rows[0]["loyalty_points"]);
                    string name = dt.Rows[0]["name"].ToString();
                    lblCustomerInfo.Text = string.Format("👤 {0}\nĐiểm tích lũy: {1} điểm", name, customerPoints);
                    lblCustomerInfo.ForeColor = Color.FromArgb(46, 204, 113); // Green
                }
                else
                {
                    selectedCustomerId = -1;
                    customerPoints = 0;
                    lblCustomerInfo.Text = "Không tìm thấy khách hàng. Đang hiển thị: Khách vãng lai";
                    lblCustomerInfo.ForeColor = Color.FromArgb(231, 76, 60); // Red
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi truy vấn khách hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- CHECKOUT SUBMISSION ---
        private void BtnPay_Click(object sender, EventArgs e)
        {
            if (cartTable.Rows.Count == 0)
            {
                MessageBox.Show("Giỏ hàng rỗng. Vui lòng thêm sản phẩm trước khi thanh toán.", "Giỏ hàng rỗng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verify Cash Register is open for shift
            // If SessionManager.CurrentShiftId is -1, cashier needs to open shift first
            if (SessionManager.Role == "Cashier" || SessionManager.Role == "Staff")
            {
                try
                {
                    object openShift = DatabaseHelper.ExecuteScalar(
                        "SELECT id FROM daily_cash_register WHERE staff_id = @sid AND closed_at IS NULL LIMIT 1",
                        new MySqlParameter("@sid", SessionManager.UserId)
                    );

                    if (openShift == null)
                    {
                        MessageBox.Show("Bạn chưa bắt đầu ca làm việc (Mở két). Vui lòng vào màn hình 'Ca Làm Việc' để khai báo số dư đầu ca trước.", "Chưa mở ca làm việc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        SessionManager.CurrentShiftId = Convert.ToInt32(openShift);
                    }
                }
                catch { }
            }

            DialogResult dr = MessageBox.Show(string.Format("Xác nhận thanh toán đơn hàng tổng cộng {0:N0} VND?", totalAmount), "Xác nhận thanh toán", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No) return;

            btnPay.Enabled = false;
            btnPay.Text = "Đang xử lý...";

            using (MySqlConnection conn = DatabaseHelper.GetConnection())
            {
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Insert into orders
                        string orderSql = @"
                            INSERT INTO orders 
                                (staff_id, customer_id, subtotal, discount_amount, tax_amount, total_amount, 
                                 payment_method, payment_status, order_status, note) 
                            VALUES 
                                (@staff_id, @cust_id, @sub, @disc, @tax, @total, @pay_method, 'paid', 'completed', @note);
                            SELECT LAST_INSERT_ID();";

                        MySqlCommand cmdOrder = new MySqlCommand(orderSql, conn, trans);
                        cmdOrder.Parameters.AddWithValue("@staff_id", SessionManager.UserId);
                        cmdOrder.Parameters.AddWithValue("@cust_id", selectedCustomerId != -1 ? (object)selectedCustomerId : DBNull.Value);
                        cmdOrder.Parameters.AddWithValue("@sub", subtotal);
                        cmdOrder.Parameters.AddWithValue("@disc", discountAmount);
                        cmdOrder.Parameters.AddWithValue("@tax", taxAmount);
                        cmdOrder.Parameters.AddWithValue("@total", totalAmount);
                        
                        string method = "cash";
                        if (cboPaymentMethod.SelectedIndex == 1) method = "card";
                        else if (cboPaymentMethod.SelectedIndex == 2) method = "e_wallet";
                        
                        cmdOrder.Parameters.AddWithValue("@pay_method", method);
                        cmdOrder.Parameters.AddWithValue("@note", txtNote.Text.Trim());

                        int orderId = Convert.ToInt32(cmdOrder.ExecuteScalar());

                        // 2. Insert into order_items
                        // Insertion will automatically fire `trg_order_item_inserted` trigger in MySQL!
                        // This trigger handles: inventory subtraction, stock_movements log, loyalty points addition.
                        foreach (DataRow row in cartTable.Rows)
                        {
                            string itemSql = @"
                                INSERT INTO order_items 
                                    (order_id, product_id, variant_id, quantity, unit_price, discount) 
                                VALUES 
                                    (@oid, @pid, @vid, @qty, @price, @disc);";

                            MySqlCommand cmdItem = new MySqlCommand(itemSql, conn, trans);
                            cmdItem.Parameters.AddWithValue("@oid", orderId);
                            cmdItem.Parameters.AddWithValue("@pid", Convert.ToInt32(row["ProductId"]));
                            cmdItem.Parameters.AddWithValue("@vid", row["VariantId"] != DBNull.Value ? (object)Convert.ToInt32(row["VariantId"]) : DBNull.Value);
                            cmdItem.Parameters.AddWithValue("@qty", Convert.ToInt32(row["Quantity"]));
                            cmdItem.Parameters.AddWithValue("@price", Convert.ToDecimal(row["UnitPrice"]));
                            cmdItem.Parameters.AddWithValue("@disc", Convert.ToDecimal(row["Discount"]));

                            cmdItem.ExecuteNonQuery();
                        }

                        // 3. Insert into order_promotions if promo applied
                        if (selectedPromotionId != -1)
                        {
                            string promoSql = "INSERT INTO order_promotions (order_id, promotion_id, discount_value) VALUES (@oid, @pid, @disc)";
                            MySqlCommand cmdPromo = new MySqlCommand(promoSql, conn, trans);
                            cmdPromo.Parameters.AddWithValue("@oid", orderId);
                            cmdPromo.Parameters.AddWithValue("@pid", selectedPromotionId);
                            cmdPromo.Parameters.AddWithValue("@disc", discountAmount);
                            cmdPromo.ExecuteNonQuery();
                        }

                        trans.Commit();

                        MessageBox.Show("Thanh toán thành công! Hóa đơn #" + orderId + " đã được tạo.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        // Clear Cart
                        cartTable.Rows.Clear();
                        RecalculateTotals();
                        txtNote.Text = "";
                        txtCustomerPhone.Text = "";
                        selectedCustomerId = -1;
                        customerPoints = 0;
                        lblCustomerInfo.Text = "Khách vãng lai (Không tích điểm)";
                        lblCustomerInfo.ForeColor = Color.FromArgb(127, 140, 141);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show("Thanh toán thất bại. Lỗi CSDL: " + ex.Message, "Lỗi thanh toán", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        btnPay.Enabled = true;
                        btnPay.Text = "💳 THANH TOÁN (IN HÓA ĐƠN)";
                    }
                }
            }
        }
    }
}

