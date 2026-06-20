using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ConvenienceStoreApp.Forms
{
    public class ProductManagementForm : Form
    {
        private SplitContainer splitContainer;
        
        // Category Controls (Left Panel)
        private GroupBox grpCategories;
        private TreeView tvCategories;
        private Button btnAddCategory;
        private Button btnEditCategory;
        private Button btnDeleteCategory;

        // Product Controls (Right Panel)
        private GroupBox grpProducts;
        private TextBox txtSearchProduct;
        private Button btnSearch;
        private Button btnAddProduct;
        private Button btnEditProduct;
        private Button btnToggleActiveProduct;
        private DataGridView dgvProducts;

        // Add/Edit Product Editor Panel (Form overlays)
        private Panel pnlEditor;
        private TextBox txtProdName, txtProdBarcode, txtProdSKU, txtProdUnit, txtProdCost, txtProdSelling, txtProdDesc;
        private ComboBox cboProdCategory, cboProdSupplier;
        private CheckBox chkProdActive;
        private Button btnSaveProduct, btnCancelProduct;
        private Label lblEditorTitle;
        private int editingProductId = -1;

        public ProductManagementForm()
        {
            InitializeComponent();
            LoadCategories();
            LoadProducts();
            LoadEditorDropdowns();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Danh Mục & Sản Phẩm";
            this.BackColor = Color.FromArgb(240, 244, 248);

            splitContainer = new SplitContainer();
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.SplitterDistance = 280;

            // --- LEFT PANEL: CATEGORIES ---
            grpCategories = new GroupBox();
            grpCategories.Text = "Danh Mục Sản Phẩm";
            grpCategories.Font = new Font("Segoe UI", 9.75f, FontStyle.Bold);
            grpCategories.ForeColor = Color.FromArgb(44, 62, 80);
            grpCategories.Dock = DockStyle.Fill;
            grpCategories.Padding = new Padding(10);

            tvCategories = new TreeView();
            tvCategories.Dock = DockStyle.Fill;
            tvCategories.Font = new Font("Segoe UI", 10);
            tvCategories.AfterSelect += TvCategories_AfterSelect;

            Panel pnlCatActions = new Panel();
            pnlCatActions.Dock = DockStyle.Bottom;
            pnlCatActions.Height = 45;
            pnlCatActions.Padding = new Padding(0, 10, 0, 0);

            btnAddCategory = new Button();
            btnAddCategory.Text = "➕";
            btnAddCategory.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnAddCategory.BackColor = Color.FromArgb(26, 188, 156);
            btnAddCategory.ForeColor = Color.White;
            btnAddCategory.FlatStyle = FlatStyle.Flat;
            btnAddCategory.FlatAppearance.BorderSize = 0;
            btnAddCategory.Size = new Size(70, 30);
            btnAddCategory.Location = new Point(5, 10);
            btnAddCategory.Cursor = Cursors.Hand;
            btnAddCategory.Click += BtnAddCategory_Click;

            btnEditCategory = new Button();
            btnEditCategory.Text = "✏️";
            btnEditCategory.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnEditCategory.BackColor = Color.FromArgb(52, 152, 219);
            btnEditCategory.ForeColor = Color.White;
            btnEditCategory.FlatStyle = FlatStyle.Flat;
            btnEditCategory.FlatAppearance.BorderSize = 0;
            btnEditCategory.Size = new Size(70, 30);
            btnEditCategory.Location = new Point(80, 10);
            btnEditCategory.Cursor = Cursors.Hand;
            btnEditCategory.Click += BtnEditCategory_Click;

            btnDeleteCategory = new Button();
            btnDeleteCategory.Text = "🗑️";
            btnDeleteCategory.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnDeleteCategory.BackColor = Color.FromArgb(231, 76, 60);
            btnDeleteCategory.ForeColor = Color.White;
            btnDeleteCategory.FlatStyle = FlatStyle.Flat;
            btnDeleteCategory.FlatAppearance.BorderSize = 0;
            btnDeleteCategory.Size = new Size(70, 30);
            btnDeleteCategory.Location = new Point(155, 10);
            btnDeleteCategory.Cursor = Cursors.Hand;
            btnDeleteCategory.Click += BtnDeleteCategory_Click;

            pnlCatActions.Controls.Add(btnAddCategory);
            pnlCatActions.Controls.Add(btnEditCategory);
            pnlCatActions.Controls.Add(btnDeleteCategory);

            grpCategories.Controls.Add(tvCategories);
            grpCategories.Controls.Add(pnlCatActions);
            splitContainer.Panel1.Controls.Add(grpCategories);

            // --- RIGHT PANEL: PRODUCTS ---
            grpProducts = new GroupBox();
            grpProducts.Text = "Danh Sách Sản Phẩm";
            grpProducts.Font = new Font("Segoe UI", 9.75f, FontStyle.Bold);
            grpProducts.ForeColor = Color.FromArgb(44, 62, 80);
            grpProducts.Dock = DockStyle.Fill;
            grpProducts.Padding = new Padding(10);

            Panel pnlProdSearch = new Panel();
            pnlProdSearch.Dock = DockStyle.Top;
            pnlProdSearch.Height = 45;

            txtSearchProduct = new TextBox();
            txtSearchProduct.Font = new Font("Segoe UI", 10);
            txtSearchProduct.Size = new Size(300, 25);
            txtSearchProduct.Location = new Point(5, 10);
            txtSearchProduct.KeyDown += TxtSearchProduct_KeyDown;

            btnSearch = new Button();
            btnSearch.Text = "Tìm kiếm";
            btnSearch.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnSearch.BackColor = Color.FromArgb(52, 73, 94);
            btnSearch.ForeColor = Color.White;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Size = new Size(90, 26);
            btnSearch.Location = new Point(315, 9);
            btnSearch.Cursor = Cursors.Hand;
            btnSearch.Click += BtnSearch_Click;

            btnAddProduct = new Button();
            btnAddProduct.Text = "➕ Thêm Sản Phẩm";
            btnAddProduct.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnAddProduct.BackColor = Color.FromArgb(26, 188, 156);
            btnAddProduct.ForeColor = Color.White;
            btnAddProduct.FlatStyle = FlatStyle.Flat;
            btnAddProduct.FlatAppearance.BorderSize = 0;
            btnAddProduct.Size = new Size(135, 26);
            btnAddProduct.Location = new Point(415, 9);
            btnAddProduct.Cursor = Cursors.Hand;
            btnAddProduct.Click += BtnAddProduct_Click;

            btnEditProduct = new Button();
            btnEditProduct.Text = "✏️ Sửa";
            btnEditProduct.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnEditProduct.BackColor = Color.FromArgb(52, 152, 219);
            btnEditProduct.ForeColor = Color.White;
            btnEditProduct.FlatStyle = FlatStyle.Flat;
            btnEditProduct.FlatAppearance.BorderSize = 0;
            btnEditProduct.Size = new Size(80, 26);
            btnEditProduct.Location = new Point(560, 9);
            btnEditProduct.Cursor = Cursors.Hand;
            btnEditProduct.Click += BtnEditProduct_Click;

            btnToggleActiveProduct = new Button();
            btnToggleActiveProduct.Text = "📴 Bật/Tắt";
            btnToggleActiveProduct.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnToggleActiveProduct.BackColor = Color.FromArgb(127, 140, 141);
            btnToggleActiveProduct.ForeColor = Color.White;
            btnToggleActiveProduct.FlatStyle = FlatStyle.Flat;
            btnToggleActiveProduct.FlatAppearance.BorderSize = 0;
            btnToggleActiveProduct.Size = new Size(90, 26);
            btnToggleActiveProduct.Location = new Point(650, 9);
            btnToggleActiveProduct.Cursor = Cursors.Hand;
            btnToggleActiveProduct.Click += BtnToggleActiveProduct_Click;

            pnlProdSearch.Controls.Add(txtSearchProduct);
            pnlProdSearch.Controls.Add(btnSearch);
            pnlProdSearch.Controls.Add(btnAddProduct);
            pnlProdSearch.Controls.Add(btnEditProduct);
            pnlProdSearch.Controls.Add(btnToggleActiveProduct);

            dgvProducts = new DataGridView();
            dgvProducts.Dock = DockStyle.Fill;
            dgvProducts.BackgroundColor = Color.White;
            dgvProducts.BorderStyle = BorderStyle.FixedSingle;
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProducts.AllowUserToAddRows = false;
            dgvProducts.ReadOnly = true;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.MultiSelect = false;
            dgvProducts.RowHeadersVisible = false;
            dgvProducts.Font = new Font("Segoe UI", 9.5f);

            grpProducts.Controls.Add(dgvProducts);
            grpProducts.Controls.Add(pnlProdSearch);
            splitContainer.Panel2.Controls.Add(grpProducts);

            // --- EDITOR PANEL overlay ---
            SetupEditorPanel();

            this.Controls.Add(splitContainer);
            splitContainer.BringToFront();
        }

        private void SetupEditorPanel()
        {
            pnlEditor = new Panel();
            pnlEditor.Size = new Size(400, 500);
            pnlEditor.BackColor = Color.White;
            pnlEditor.BorderStyle = BorderStyle.FixedSingle;
            pnlEditor.Visible = false;

            lblEditorTitle = new Label();
            lblEditorTitle.Text = "THÊM SẢN PHẨM MỚI";
            lblEditorTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblEditorTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblEditorTitle.Size = new Size(380, 25);
            lblEditorTitle.Location = new Point(10, 15);
            lblEditorTitle.TextAlign = ContentAlignment.MiddleCenter;

            int rowY = 55, spacing = 42, labelX = 20, inputX = 140, inputWidth = 230;

            CreateEditorLabel("Tên sản phẩm:", rowY, labelX);
            txtProdName = CreateEditorTextBox(rowY, inputX, inputWidth);
            rowY += spacing;

            CreateEditorLabel("Mã vạch (Barcode):", rowY, labelX);
            txtProdBarcode = CreateEditorTextBox(rowY, inputX, inputWidth);
            rowY += spacing;

            CreateEditorLabel("SKU (Mã kho):", rowY, labelX);
            txtProdSKU = CreateEditorTextBox(rowY, inputX, inputWidth);
            rowY += spacing;

            CreateEditorLabel("Đơn vị tính:", rowY, labelX);
            txtProdUnit = CreateEditorTextBox(rowY, inputX, inputWidth);
            txtProdUnit.Text = "cái";
            rowY += spacing;

            CreateEditorLabel("Giá nhập:", rowY, labelX);
            txtProdCost = CreateEditorTextBox(rowY, inputX, inputWidth);
            txtProdCost.Text = "0";
            rowY += spacing;

            CreateEditorLabel("Giá bán:", rowY, labelX);
            txtProdSelling = CreateEditorTextBox(rowY, inputX, inputWidth);
            txtProdSelling.Text = "0";
            rowY += spacing;

            CreateEditorLabel("Danh mục:", rowY, labelX);
            cboProdCategory = new ComboBox();
            cboProdCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cboProdCategory.Size = new Size(inputWidth, 25);
            cboProdCategory.Location = new Point(inputX, rowY);
            pnlEditor.Controls.Add(cboProdCategory);
            rowY += spacing;

            CreateEditorLabel("Nhà cung cấp:", rowY, labelX);
            cboProdSupplier = new ComboBox();
            cboProdSupplier.DropDownStyle = ComboBoxStyle.DropDownList;
            cboProdSupplier.Size = new Size(inputWidth, 25);
            cboProdSupplier.Location = new Point(inputX, rowY);
            pnlEditor.Controls.Add(cboProdSupplier);
            rowY += spacing;

            chkProdActive = new CheckBox();
            chkProdActive.Text = "Còn kinh doanh (Active)";
            chkProdActive.Font = new Font("Segoe UI", 9.5f);
            chkProdActive.Checked = true;
            chkProdActive.Location = new Point(inputX, rowY);
            chkProdActive.Size = new Size(200, 20);
            pnlEditor.Controls.Add(chkProdActive);
            rowY += 30;

            btnSaveProduct = new Button();
            btnSaveProduct.Text = "Lưu lại";
            btnSaveProduct.BackColor = Color.FromArgb(46, 204, 113);
            btnSaveProduct.ForeColor = Color.White;
            btnSaveProduct.FlatStyle = FlatStyle.Flat;
            btnSaveProduct.FlatAppearance.BorderSize = 0;
            btnSaveProduct.Size = new Size(100, 32);
            btnSaveProduct.Location = new Point(140, rowY);
            btnSaveProduct.Click += BtnSaveProduct_Click;

            btnCancelProduct = new Button();
            btnCancelProduct.Text = "Hủy bỏ";
            btnCancelProduct.BackColor = Color.FromArgb(189, 195, 199);
            btnCancelProduct.ForeColor = Color.White;
            btnCancelProduct.FlatStyle = FlatStyle.Flat;
            btnCancelProduct.FlatAppearance.BorderSize = 0;
            btnCancelProduct.Size = new Size(100, 32);
            btnCancelProduct.Location = new Point(255, rowY);
            btnCancelProduct.Click += BtnCancelProduct_Click;

            pnlEditor.Controls.Add(lblEditorTitle);
            pnlEditor.Controls.Add(btnSaveProduct);
            pnlEditor.Controls.Add(btnCancelProduct);

            this.Controls.Add(pnlEditor);
        }

        private void CreateEditorLabel(string text, int top, int left)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 9.5f);
            lbl.ForeColor = Color.FromArgb(71, 84, 103);
            lbl.Location = new Point(left, top + 3);
            lbl.AutoSize = true;
            pnlEditor.Controls.Add(lbl);
        }

        private TextBox CreateEditorTextBox(int top, int left, int width)
        {
            TextBox txt = new TextBox();
            txt.Font = new Font("Segoe UI", 9.5f);
            txt.Size = new Size(width, 25);
            txt.Location = new Point(left, top);
            pnlEditor.Controls.Add(txt);
            return txt;
        }

        // --- DATABASE OPERATIONS ---

        private void LoadCategories()
        {
            try
            {
                tvCategories.Nodes.Clear();
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT id, name, parent_id FROM categories WHERE is_active = 1");
                
                // Add Root Nodes (parent_id IS NULL)
                TreeNode rootNode = new TreeNode("Tất Cả Sản Phẩm");
                rootNode.Tag = -1; // -1 represents all
                tvCategories.Nodes.Add(rootNode);

                AddCategoryNodes(rootNode.Nodes, dt, DBNull.Value);
                tvCategories.ExpandAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message);
            }
        }

        private void AddCategoryNodes(TreeNodeCollection nodes, DataTable dt, object parentId)
        {
            string filter = parentId == DBNull.Value ? "parent_id IS NULL" : "parent_id = " + parentId;
            DataRow[] rows = dt.Select(filter);
            
            foreach (DataRow row in rows)
            {
                TreeNode node = new TreeNode(row["name"].ToString());
                node.Tag = Convert.ToInt32(row["id"]);
                nodes.Add(node);
                
                // Add children recursively
                AddCategoryNodes(node.Nodes, dt, row["id"]);
            }
        }

        private void LoadProducts(string searchKw = "", int categoryId = -1)
        {
            try
            {
                string sql = @"
                    SELECT p.id, p.barcode as Barcode, p.name as TenSanPham, p.sku as SKU, 
                           c.name as DanhMuc, s.name as NhaCungCap, p.unit as DVT, 
                           p.cost_price as GiaNhap, p.selling_price as GiaBan, 
                           IF(p.is_active = 1, 'Đang bán', 'Ngừng bán') as TrangThai
                    FROM products p
                    INNER JOIN categories c ON p.category_id = c.id
                    LEFT JOIN suppliers s ON p.supplier_id = s.id
                    WHERE 1 = 1";

                List<MySqlParameter> parameters = new List<MySqlParameter>();

                if (categoryId != -1)
                {
                    // Filter by selected category or its children
                    sql += " AND (p.category_id = @catId OR c.parent_id = @catId)";
                    parameters.Add(new MySqlParameter("@catId", categoryId));
                }

                if (!string.IsNullOrEmpty(searchKw))
                {
                    sql += " AND (p.name LIKE @kw OR p.barcode = @kwExact OR p.sku = @kwExact)";
                    parameters.Add(new MySqlParameter("@kw", "%" + searchKw + "%"));
                    parameters.Add(new MySqlParameter("@kwExact", searchKw));
                }

                sql += " ORDER BY p.id DESC";

                DataTable dt = DatabaseHelper.ExecuteQuery(sql, parameters.ToArray());
                dgvProducts.DataSource = dt;
                
                // Styling grid
                dgvProducts.Columns["id"].Visible = false;
                dgvProducts.Columns["GiaNhap"].DefaultCellStyle.Format = "N0";
                dgvProducts.Columns["GiaBan"].DefaultCellStyle.Format = "N0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải sản phẩm: " + ex.Message);
            }
        }

        private void LoadEditorDropdowns()
        {
            try
            {
                // Categories
                DataTable dtCat = DatabaseHelper.ExecuteQuery("SELECT id, name FROM categories WHERE is_active = 1");
                cboProdCategory.DataSource = dtCat;
                cboProdCategory.DisplayMember = "name";
                cboProdCategory.ValueMember = "id";

                // Suppliers
                DataTable dtSup = DatabaseHelper.ExecuteQuery("SELECT id, name FROM suppliers");
                cboProdSupplier.DataSource = dtSup;
                cboProdSupplier.DisplayMember = "name";
                cboProdSupplier.ValueMember = "id";
            }
            catch { }
        }

        // --- TREEVIEW EVENT ---
        private void TvCategories_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvCategories.SelectedNode != null)
            {
                int catId = Convert.ToInt32(tvCategories.SelectedNode.Tag);
                LoadProducts(txtSearchProduct.Text.Trim(), catId);
            }
        }

        // --- PRODUCT SEARCH EVENTS ---
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            int catId = tvCategories.SelectedNode != null ? Convert.ToInt32(tvCategories.SelectedNode.Tag) : -1;
            LoadProducts(txtSearchProduct.Text.Trim(), catId);
        }

        private void TxtSearchProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnSearch_Click(this, EventArgs.Empty);
                e.SuppressKeyPress = true;
            }
        }

        // --- PRODUCT ACTIONS ---
        private void BtnAddProduct_Click(object sender, EventArgs e)
        {
            editingProductId = -1;
            lblEditorTitle.Text = "THÊM SẢN PHẨM MỚI";
            txtProdName.Text = "";
            txtProdBarcode.Text = "";
            txtProdSKU.Text = "";
            txtProdUnit.Text = "cái";
            txtProdCost.Text = "0";
            txtProdSelling.Text = "0";
            chkProdActive.Checked = true;

            // Overlay in center
            pnlEditor.Location = new Point((this.ClientSize.Width - pnlEditor.Width) / 2, (this.ClientSize.Height - pnlEditor.Height) / 2);
            pnlEditor.Visible = true;
            pnlEditor.BringToFront();
            txtProdName.Focus();
        }

        private void BtnEditProduct_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm cần sửa.", "Chọn sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            editingProductId = Convert.ToInt32(dgvProducts.CurrentRow.Cells["id"].Value);

            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM products WHERE id = @id", new MySqlParameter("@id", editingProductId));
                if (dt.Rows.Count > 0)
                {
                    DataRow r = dt.Rows[0];
                    txtProdName.Text = r["name"].ToString();
                    txtProdBarcode.Text = r["barcode"].ToString();
                    txtProdSKU.Text = r["sku"].ToString();
                    txtProdUnit.Text = r["unit"].ToString();
                    txtProdCost.Text = Convert.ToDecimal(r["cost_price"]).ToString("F0");
                    txtProdSelling.Text = Convert.ToDecimal(r["selling_price"]).ToString("F0");
                    cboProdCategory.SelectedValue = Convert.ToInt32(r["category_id"]);
                    
                    if (r["supplier_id"] != DBNull.Value)
                        cboProdSupplier.SelectedValue = Convert.ToInt32(r["supplier_id"]);

                    chkProdActive.Checked = Convert.ToBoolean(r["is_active"]);

                    lblEditorTitle.Text = "SỬA SẢN PHẨM KHÓA #" + editingProductId;
                    pnlEditor.Location = new Point((this.ClientSize.Width - pnlEditor.Width) / 2, (this.ClientSize.Height - pnlEditor.Height) / 2);
                    pnlEditor.Visible = true;
                    pnlEditor.BringToFront();
                    txtProdName.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy thông tin sản phẩm: " + ex.Message);
            }
        }

        private void BtnToggleActiveProduct_Click(object sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow == null) return;
            int id = Convert.ToInt32(dgvProducts.CurrentRow.Cells["id"].Value);
            string status = dgvProducts.CurrentRow.Cells["TrangThai"].Value.ToString();

            int newActive = (status == "Đang bán") ? 0 : 1;

            try
            {
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE products SET is_active = @act WHERE id = @id",
                    new MySqlParameter("@act", newActive),
                    new MySqlParameter("@id", id)
                );
                
                // Refresh
                int catId = tvCategories.SelectedNode != null ? Convert.ToInt32(tvCategories.SelectedNode.Tag) : -1;
                LoadProducts(txtSearchProduct.Text.Trim(), catId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật trạng thái: " + ex.Message);
            }
        }

        private void BtnSaveProduct_Click(object sender, EventArgs e)
        {
            string name = txtProdName.Text.Trim();
            string barcode = txtProdBarcode.Text.Trim();
            string sku = txtProdSKU.Text.Trim();
            string unit = txtProdUnit.Text.Trim();
            decimal cost = 0, selling = 0;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Vui lòng điền tên sản phẩm.");
                return;
            }

            if (!decimal.TryParse(txtProdCost.Text, out cost) || !decimal.TryParse(txtProdSelling.Text, out selling))
            {
                MessageBox.Show("Giá nhập và Giá bán phải là số hợp lệ.");
                return;
            }

            int catId = Convert.ToInt32(cboProdCategory.SelectedValue);
            object supId = cboProdSupplier.SelectedValue != null ? cboProdSupplier.SelectedValue : DBNull.Value;

            try
            {
                if (editingProductId == -1)
                {
                    // Add new product
                    string sql = @"
                        INSERT INTO products 
                            (category_id, supplier_id, name, barcode, sku, unit, cost_price, selling_price, is_active) 
                        VALUES 
                            (@cat, @sup, @name, @bc, @sku, @unit, @cost, @sell, @act)";
                    
                    MySqlParameter[] prs = new MySqlParameter[] {
                        new MySqlParameter("@cat", catId),
                        new MySqlParameter("@sup", supId),
                        new MySqlParameter("@name", name),
                        new MySqlParameter("@bc", string.IsNullOrEmpty(barcode) ? DBNull.Value : (object)barcode),
                        new MySqlParameter("@sku", string.IsNullOrEmpty(sku) ? DBNull.Value : (object)sku),
                        new MySqlParameter("@unit", unit),
                        new MySqlParameter("@cost", cost),
                        new MySqlParameter("@sell", selling),
                        new MySqlParameter("@act", chkProdActive.Checked ? 1 : 0)
                    };

                    DatabaseHelper.ExecuteNonQuery(sql, prs);
                }
                else
                {
                    // Edit existing product
                    string sql = @"
                        UPDATE products 
                        SET category_id = @cat, supplier_id = @sup, name = @name, barcode = @bc, sku = @sku, 
                            unit = @unit, cost_price = @cost, selling_price = @sell, is_active = @act 
                        WHERE id = @id";
                    
                    MySqlParameter[] prs = new MySqlParameter[] {
                        new MySqlParameter("@cat", catId),
                        new MySqlParameter("@sup", supId),
                        new MySqlParameter("@name", name),
                        new MySqlParameter("@bc", string.IsNullOrEmpty(barcode) ? DBNull.Value : (object)barcode),
                        new MySqlParameter("@sku", string.IsNullOrEmpty(sku) ? DBNull.Value : (object)sku),
                        new MySqlParameter("@unit", unit),
                        new MySqlParameter("@cost", cost),
                        new MySqlParameter("@sell", selling),
                        new MySqlParameter("@act", chkProdActive.Checked ? 1 : 0),
                        new MySqlParameter("@id", editingProductId)
                    };

                    DatabaseHelper.ExecuteNonQuery(sql, prs);
                }

                pnlEditor.Visible = false;
                
                // Refresh
                int selCat = tvCategories.SelectedNode != null ? Convert.ToInt32(tvCategories.SelectedNode.Tag) : -1;
                LoadProducts(txtSearchProduct.Text.Trim(), selCat);
                MessageBox.Show("Lưu thông tin sản phẩm thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu sản phẩm: " + ex.Message, "Lỗi cơ sở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancelProduct_Click(object sender, EventArgs e)
        {
            pnlEditor.Visible = false;
        }

        // --- CATEGORY ACTIONS ---
        private void BtnAddCategory_Click(object sender, EventArgs e)
        {
            string name = PromptDialog.ShowDialog("Nhập tên danh mục mới:", "Thêm Danh Mục");
            if (string.IsNullOrEmpty(name)) return;

            int? parentId = null;
            if (tvCategories.SelectedNode != null && Convert.ToInt32(tvCategories.SelectedNode.Tag) != -1)
            {
                DialogResult dr = MessageBox.Show("Bạn có muốn đặt danh mục này làm con của danh mục '" + tvCategories.SelectedNode.Text + "' không?", "Danh mục cha-con", MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Yes) parentId = Convert.ToInt32(tvCategories.SelectedNode.Tag);
                else if (dr == DialogResult.Cancel) return;
            }

            try
            {
                DatabaseHelper.ExecuteNonQuery(
                    "INSERT INTO categories (name, parent_id, is_active) VALUES (@name, @parent, 1)",
                    new MySqlParameter("@name", name),
                    new MySqlParameter("@parent", parentId.HasValue ? (object)parentId.Value : DBNull.Value)
                );
                LoadCategories();
                LoadEditorDropdowns();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm danh mục: " + ex.Message);
            }
        }

        private void BtnEditCategory_Click(object sender, EventArgs e)
        {
            if (tvCategories.SelectedNode == null || Convert.ToInt32(tvCategories.SelectedNode.Tag) == -1)
            {
                MessageBox.Show("Vui lòng chọn danh mục cần sửa.");
                return;
            }

            int id = Convert.ToInt32(tvCategories.SelectedNode.Tag);
            string oldName = tvCategories.SelectedNode.Text;

            string name = PromptDialog.ShowDialog("Nhập tên danh mục mới:", "Sửa Danh Mục", oldName);
            if (string.IsNullOrEmpty(name)) return;

            try
            {
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE categories SET name = @name WHERE id = @id",
                    new MySqlParameter("@name", name),
                    new MySqlParameter("@id", id)
                );
                LoadCategories();
                LoadEditorDropdowns();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật danh mục: " + ex.Message);
            }
        }

        private void BtnDeleteCategory_Click(object sender, EventArgs e)
        {
            if (tvCategories.SelectedNode == null || Convert.ToInt32(tvCategories.SelectedNode.Tag) == -1)
            {
                MessageBox.Show("Vui lòng chọn danh mục cần ẩn.");
                return;
            }

            int id = Convert.ToInt32(tvCategories.SelectedNode.Tag);
            DialogResult dr = MessageBox.Show("Bạn có chắc chắn muốn ngưng kích hoạt danh mục '" + tvCategories.SelectedNode.Text + "'?", "Xác nhận ẩn danh mục", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No) return;

            try
            {
                DatabaseHelper.ExecuteNonQuery("UPDATE categories SET is_active = 0 WHERE id = @id", new MySqlParameter("@id", id));
                LoadCategories();
                LoadEditorDropdowns();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ẩn danh mục: " + ex.Message);
            }
        }
    }

    // Helper static class to show simple prompt dialog inputs
    public static class PromptDialog
    {
        public static string ShowDialog(string text, string caption, string defaultValue = "")
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 160,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false
            };
            Label textLabel = new Label() { Left = 20, Top = 20, Text = text, Width = 350 };
            TextBox textBox = new TextBox() { Left = 20, Top = 45, Width = 350, Text = defaultValue };
            Button confirmation = new Button() { Text = "Xác nhận", Left = 170, Width = 100, Top = 80, DialogResult = DialogResult.OK, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(26, 188, 156), ForeColor = Color.White };
            Button cancel = new Button() { Text = "Hủy bỏ", Left = 280, Width = 80, Top = 80, DialogResult = DialogResult.Cancel, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(189, 195, 199), ForeColor = Color.White };
            
            confirmation.Click += (sender, e) => { prompt.Close(); };
            cancel.Click += (sender, e) => { prompt.Close(); };
            
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}

