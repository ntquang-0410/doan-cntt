# Convenience Store WinForms

Ứng dụng WinForms quản lý cửa hàng tiện lợi cho môn Đồ án CNTT. Dự án ưu tiên chạy ổn định, có giao diện thao tác được và kết nối MySQL thật.

## Công nghệ

```text
WinForms .NET Framework 4.8
MySQL
ADO.NET / MySql.Data.dll
Mô hình 3 lớp từng bước: Forms -> BLL -> DAL
```

Phạm vi hiện tại không dùng SQL Server và không bắt buộc Entity Framework. Một phần module `Customer` đã được tách mẫu sang `BLL`, `DAL`, `Models`, `DTOs`; các module còn lại vẫn dùng `DatabaseHelper` trực tiếp để giữ app ổn định trong phạm vi đồ án.

## Chức năng chính

- Đăng nhập theo tài khoản và phân quyền `Admin`, `Manager`, `Cashier`, `Staff`.
- Chế độ xem giao diện demo không cần MySQL.
- POS bán hàng:
  - Tìm sản phẩm theo tên/barcode.
  - Thêm sản phẩm vào giỏ.
  - Tăng/giảm số lượng bằng nút `+`/`-`.
  - Kiểm tra tồn kho.
  - Thanh toán và tạo hóa đơn.
  - Trừ tồn kho bằng trigger database.
- Quản lý ca làm việc.
- Quản lý khách hàng.
- Quản lý sản phẩm và danh mục.
- Quản lý tồn kho và nhập hàng:
  - Xem/tìm/lọc tồn kho.
  - Tạo đơn nhập hàng.
  - Sửa/xóa dòng sản phẩm trong đơn đang tạo.
  - Xác nhận nhận hàng để cộng tồn kho.
  - Hủy đơn pending.
  - Hoàn tác đơn đã nhập, trừ lại tồn kho và ghi lịch sử.
  - Điều chỉnh tồn kho thủ công.
- Quản lý nhân viên.
- Quản lý khuyến mãi.
- Báo cáo doanh thu, sản phẩm bán chạy, nhật ký hoạt động.

## Database

Thư mục `database` có các file quan trọng:

```text
database/schema-convenience_store-202605120907.sql
database/dump-convenience_store-clean-20260620.sql
database/cleanup_duplicate_products.sql
database/backup-before-cleanup-20260620.sql
```

Nên dùng file dump sạch mới:

```text
database/dump-convenience_store-clean-20260620.sql
```

File này đã dọn các sản phẩm mẫu bị trùng như `Coca Cola lon 330ml`, `Cà phê G7 hộp 20 gói`, `Bánh Oreo gói 137g`.

## Cấu hình kết nối

Connection string nằm trong `ConvenienceStoreApp/App.config`:

```xml
Server=localhost;Port=3306;Database=convenience_store;Uid=store_app;Pwd=store123;Charset=utf8mb4;
```

Tạo user MySQL cho app:

```sql
CREATE USER IF NOT EXISTS 'store_app'@'localhost'
IDENTIFIED BY 'store123';

GRANT ALL PRIVILEGES ON convenience_store.*
TO 'store_app'@'localhost';

FLUSH PRIVILEGES;
```

## Cài đặt database

Trong MySQL Workbench:

1. Tạo database:

```sql
CREATE DATABASE IF NOT EXISTS convenience_store
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;
```

2. Import file:

```text
database/dump-convenience_store-clean-20260620.sql
```

3. Kiểm tra dữ liệu:

```sql
USE convenience_store;
SHOW TABLES;
SELECT username, role, is_active FROM users;
```

Tài khoản đăng nhập mẫu:

```text
admin01 / 123456
staff01 / 123456
```

## Build và chạy

Build bằng MSBuild:

```powershell
& C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe ConvenienceStoreApp\ConvenienceStoreApp.csproj /t:Build /p:Configuration=Debug
```

Chạy app:

```text
ConvenienceStoreApp/bin/Debug/ConvenienceStoreApp.exe
```

Nếu máy báo thiếu .NET Framework 4.8 Targeting Pack khi build, cài thêm `.NET Framework 4.8 Developer Pack`. App hiện vẫn build được trên máy đang phát triển, chỉ còn warning về targeting pack/architecture.

## Luồng test đề xuất

1. Đăng nhập `admin01/123456`.
2. Mở `Tồn Kho & Nhập Hàng`, kiểm tra danh sách tồn kho không còn sản phẩm trùng.
3. Tạo đơn nhập hàng, thêm sản phẩm, sửa/xóa dòng, lưu đơn.
4. Xác nhận nhận hàng, kiểm tra tồn kho tăng.
5. Hoàn tác đơn đã nhập, kiểm tra tồn kho giảm lại.
6. Mở POS, thêm sản phẩm theo barcode/tên.
7. Tăng/giảm số lượng bằng `+`/`-`.
8. Thanh toán, kiểm tra hóa đơn được tạo và tồn kho giảm.
9. Chuyển module khi POS còn giỏ hàng để kiểm tra cảnh báo.
10. Xem báo cáo doanh thu/sản phẩm bán chạy.

## Ghi chú báo cáo đồ án

Có thể trình bày kiến trúc như sau:

- `Forms`: giao diện WinForms, sự kiện, bind dữ liệu.
- `BLL`: lớp nghiệp vụ, đã tách mẫu ở module Customer.
- `DAL`: repository truy cập MySQL bằng ADO.NET/MySql.Data.
- `Models/DTOs`: model và dữ liệu hiển thị cho form.
- `DatabaseHelper`: helper dùng chung để kết nối và chạy SQL.

Do phạm vi đồ án nhỏ, nhóm ưu tiên hoàn thiện chức năng, giao diện và luồng nghiệp vụ. Mô hình 3 lớp được áp dụng từng bước và có thể tiếp tục tách các module còn lại sau.

