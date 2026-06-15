-- ============================================
-- File: 02_seed_data.sql
-- Mô tả: Dữ liệu khởi tạo tối thiểu (idempotent - chạy nhiều lần OK)
-- Yêu cầu: Chạy SAU khi đã import 01_schema.sql
-- ============================================

USE convenience_store;

SET FOREIGN_KEY_CHECKS = 0;

-- ============================================
-- 1. USERS - Tài khoản hệ thống
-- ⚠️ ĐỔI PASSWORD NGAY sau khi setup!
-- ============================================
INSERT IGNORE INTO users (id, username, password, full_name, phone, role, is_active) VALUES
(1, 'admin',    '$2a$10$YourBCryptHashHere123456789012345678901234567890ABC', 'Quản trị viên',   '0900000001', 'Admin',   TRUE),
(2, 'manager',  '$2a$10$YourBCryptHashHere123456789012345678901234567890ABC', 'Quản lý cửa hàng', '0900000002', 'Manager', TRUE),
(3, 'cashier',  '$2a$10$YourBCryptHashHere123456789012345678901234567890ABC', 'Nhân viên thu ngân','0900000003', 'Cashier', TRUE);

-- ============================================
-- 2. CATEGORIES - Danh mục sản phẩm
-- ============================================
INSERT IGNORE INTO categories (id, name, parent_id, icon, is_active) VALUES
(1, 'Đồ uống',           NULL, 'drink.png',    TRUE),
(2, 'Thực phẩm',         NULL, 'food.png',     TRUE),
(3, 'Đồ dùng cá nhân',   NULL, 'personal.png', TRUE),
(4, 'Thuốc lá',          NULL, 'tobacco.png',  TRUE),
(5, 'Văn phòng phẩm',    NULL, 'office.png',   TRUE),
(6, 'Nước ngọt',         1,    NULL,           TRUE),
(7, 'Cà phê',            1,    NULL,           TRUE),
(8, 'Mì gói',            2,    NULL,           TRUE),
(9, 'Bánh kẹo',          2,    NULL,           TRUE),
(10,'Sữa & sản phẩm từ sữa', 2, NULL,          TRUE);

-- ============================================
-- 3. SETTINGS - Cấu hình hệ thống
-- ============================================
INSERT IGNORE INTO settings (setting_key, setting_value, description) VALUES
('store_name',      'Cửa hàng Tiện lợi ABC',           'Tên cửa hàng'),
('store_address',   '123 Nguyễn Văn Linh, Q.7, TP.HCM','Địa chỉ cửa hàng'),
('store_phone',     '0901234567',                      'SĐT liên hệ'),
('tax_rate',        '8',                               'Thuế VAT (%)'),
('loyalty_rate',    '10000',                           'X đồng = 1 điểm tích lũy'),
('low_stock_alert', 'true',                            'Bật cảnh báo hàng sắp hết'),
('currency',        'VND',                             'Đơn vị tiền tệ'),
('receipt_footer',  'Cảm ơn quý khách - Hẹn gặp lại!', 'Footer in hóa đơn');

SET FOREIGN_KEY_CHECKS = 1;

-- Verify
SELECT 'users' AS bang, COUNT(*) AS so_dong FROM users
UNION ALL SELECT 'categories', COUNT(*) FROM categories
UNION ALL SELECT 'settings', COUNT(*) FROM settings;