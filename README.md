# doan-cntt

Ứng dụng WinForms quản lý cửa hàng tiện lợi, viết bằng C# trên .NET Framework 4.8 và dùng MySQL qua `MySql.Data.dll`.

## Trạng thái hiện tại

- Đã build được bằng MSBuild có sẵn của .NET Framework.
- File chạy sau build: `ConvenienceStoreApp\bin\Debug\ConvenienceStoreApp.exe`.
- Đã có các màn hình chính: đăng nhập, bán hàng POS, ca làm việc, sản phẩm, kho, nhân viên, khách hàng, khuyến mãi, báo cáo.
- Có nút **Xem giao diện demo** ở màn hình đăng nhập để mở giao diện trước khi cấu hình database.
- Đã có schema/dump/seed database trong thư mục `database`.
- Luồng chạy thật dùng MySQL local, database `convenience_store`, user ứng dụng `store_app`.

## Kiến trúc mục tiêu

Dự án sẽ đi theo hướng:

```text
WinForms .NET Framework 4.8
  -> Presentation Layer: Forms
  -> Business Logic Layer: BLL/Services
  -> Data Access Layer: DAL/Repositories + Entity Framework 6
  -> MySQL
```

Hiện tại code nghiệp vụ vẫn còn nằm nhiều trong Form và gọi `DatabaseHelper` trực tiếp. Các bước refactor BLL/DAL/EF6 sẽ làm dần sau khi giao diện ổn định.

## Cấu hình database

Connection string nằm trong `ConvenienceStoreApp\App.config`:

```xml
Server=localhost;Port=3306;Database=convenience_store;Uid=store_app;Pwd=store123;Charset=utf8mb4;
```

Không nên dùng trực tiếp tài khoản `root` trong app. Tạo user riêng `store_app` theo hướng dẫn bên dưới.

## Setup sau khi clone từ Git

Máy mới cần có:

- MySQL Server
- MySQL Workbench
- .NET Framework 4.8 Runtime
- .NET Framework 4.8 Developer Pack/Targeting Pack nếu muốn build bằng MSBuild

### 1. Tạo database

Mở MySQL Workbench, vào connection local, mở Query tab và chạy:

```sql
CREATE DATABASE IF NOT EXISTS convenience_store
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE convenience_store;
```

### 2. Import database mẫu

Trong MySQL Workbench:

```text
Server > Data Import > Import from Self-Contained File
```

Chọn file:

```text
database\dump-convenience_store-202605120906.sql
```

Ở `Default Target Schema`, chọn:

```text
convenience_store
```

Sau đó bấm `Start Import`.

### 3. Tạo user cho app

Chạy trong Query tab:

```sql
CREATE USER IF NOT EXISTS 'store_app'@'localhost'
IDENTIFIED BY 'store123';

GRANT ALL PRIVILEGES ON convenience_store.*
TO 'store_app'@'localhost';

FLUSH PRIVILEGES;
```

Nếu user đã tồn tại, chạy:

```sql
ALTER USER 'store_app'@'localhost'
IDENTIFIED BY 'store123';

GRANT ALL PRIVILEGES ON convenience_store.*
TO 'store_app'@'localhost';

FLUSH PRIVILEGES;
```

### 4. Kiểm tra dữ liệu

Chạy:

```sql
USE convenience_store;

SHOW TABLES;
SELECT * FROM users;
```

Nếu thấy `admin01` và `staff01`, database đã sẵn sàng.

## Import database

Dùng MySQL Workbench như phần trên, hoặc đường dẫn thật đến `mysql.exe` để import:

```powershell
mysql -u root -p convenience_store < database\dump-convenience_store-202605120906.sql
```

Nên dùng file dump nếu muốn có dữ liệu mẫu đầy đủ. Tài khoản đăng nhập phần mềm có trong dump:

- `admin01` / `123456`
- `staff01` / `123456`

## Build

```powershell
& C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe ConvenienceStoreApp\ConvenienceStoreApp.csproj /t:Build /p:Configuration=Debug
```

Build hiện thành công, nhưng có warning vì máy thiếu .NET Framework 4.8 Targeting Pack. Nếu muốn build chuẩn hơn, cài .NET Framework 4.8 Developer Pack/Targeting Pack.

## Xem giao diện trước

1. Build app.
2. Mở `ConvenienceStoreApp\bin\Debug\ConvenienceStoreApp.exe`.
3. Ở màn hình đăng nhập, chọn **Xem giao diện demo**.

Chế độ demo dùng tài khoản nội bộ `demo.admin`, quyền `Admin`, không cần kết nối MySQL để vào màn hình chính.

## Việc còn lại

- Xác thực login và các luồng nghiệp vụ với database thật.
- Tách dần Form -> BLL -> DAL theo mô hình 3 lớp.
- Thêm Entity Framework 6 provider cho MySQL và tạo `AppDbContext`.
- Kiểm tra POS: mở ca, bán hàng, tạo hóa đơn, trừ tồn kho, tích điểm khách hàng.
- Kiểm tra nhập hàng: tạo purchase order, xác nhận nhận hàng, trigger cộng tồn kho.
- Hoàn thiện quản lý biến thể sản phẩm trong Product Management.
- Bổ sung xử lý khuyến mãi `buy_x_get_y`; hiện UI chủ yếu hỗ trợ `percent` và `fixed`.
- Kiểm thử giao diện và xử lý lỗi kết nối database thân thiện hơn.
