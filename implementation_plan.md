# Kế hoạch triển khai ứng dụng WinForms quản lý cửa hàng tiện lợi

## Định hướng mới

Dự án không bắt buộc dùng SQL Server. Hướng triển khai được chốt là:

```text
WinForms .NET Framework 4.8
  -> Presentation Layer: Forms
  -> Business Logic Layer: Services/BLL
  -> Data Access Layer: Repositories/DAL + Entity Framework 6
  -> MySQL
```

Ưu tiên trước mắt là **có giao diện chạy được để xem và góp ý**, sau đó mới tách dần code sang mô hình 3 lớp và thay `DatabaseHelper`/SQL trực tiếp bằng EF6.

## Trạng thái hiện tại

- Project `ConvenienceStoreApp` đã build được bằng MSBuild.
- File chạy: `ConvenienceStoreApp\bin\Debug\ConvenienceStoreApp.exe`.
- Đã có các form chính:
  - Login
  - Main
  - POS
  - Daily Shift
  - Product Management
  - Inventory
  - Employee
  - Customer
  - Promotion
  - Report
- Đã thêm chế độ **Xem giao diện demo** ở `LoginForm`:
  - Không cần kết nối MySQL.
  - Tự đăng nhập bằng user nội bộ `demo.admin`.
  - Mở `MainForm` với quyền `Admin`.
  - Mặc định hiển thị trang tổng quan giao diện tĩnh để tránh lỗi database khi mới mở app.
- Database MySQL đã có schema/dump trong thư mục `database`.
- Service `MySQL80` đang chạy, nhưng máy chưa có `mysql` CLI trong PATH nên chưa xác thực import database bằng dòng lệnh.

## Milestone 1 - Hoàn thiện giao diện trước

- Giữ toàn bộ form WinForms hiện có.
- Đảm bảo app mở được từ `ConvenienceStoreApp.exe`.
- Giữ nút đăng nhập thật cho database.
- Giữ nút demo để xem giao diện khi database chưa sẵn sàng.
- Rà layout từng màn hình:
  - Login: rõ tài khoản demo và tài khoản database mẫu.
  - Main: sidebar đầy đủ module.
  - POS: giỏ hàng, tìm sản phẩm, khách hàng, thanh toán.
  - Inventory: tồn kho, đơn nhập hàng.
  - Employee/Customer/Promotion/Product/Report: đủ vùng danh sách và form nhập liệu.

## Milestone 2 - Tách mô hình 3 lớp

Tạo và chuyển dần sang cấu trúc:

```text
ConvenienceStoreApp/
├── Forms/              # Presentation Layer
├── BLL/                # Business logic/services
├── DAL/                # DbContext, repositories, unit of work
├── Models/             # Entity classes map database
├── DTOs/               # View/data transfer models
└── Common/             # Session, validation, formatting, helpers
```

Nguyên tắc:

- Form chỉ xử lý giao diện, sự kiện, bind dữ liệu, hiển thị thông báo.
- BLL xử lý nghiệp vụ: đăng nhập, phân quyền, tính tiền, tồn kho, khuyến mãi, ca làm việc.
- DAL xử lý truy cập dữ liệu bằng EF6.
- Models map với bảng MySQL.
- DTOs phục vụ DataGridView và dữ liệu màn hình, tránh để Form tự join/query phức tạp.

## Milestone 3 - Entity Framework 6 + MySQL

- Thêm provider EF6 cho MySQL:
  - `MySql.Data`
  - `MySql.Data.EntityFramework` hoặc provider EF6 tương thích phiên bản đang dùng.
- Tạo `DAL/AppDbContext.cs`.
- Tạo entity cho các bảng chính:
  - `User`
  - `Category`
  - `Product`
  - `ProductVariant`
  - `Inventory`
  - `Customer`
  - `Order`
  - `OrderItem`
  - `Promotion`
  - `PurchaseOrder`
  - `StockMovement`
- Chuyển CRUD đơn giản trước:
  - Customer
  - Employee
  - Promotion
- Sau đó chuyển module phức tạp:
  - Product/Category/Variant
  - Inventory/PurchaseOrder
  - POS/Order/Payment

## Việc còn lại gần nhất

1. Chạy và rà giao diện demo.
2. Sửa layout nếu có màn hình bị lệch, khó nhìn, hoặc thao tác chưa rõ.
3. Import database MySQL.
4. Kiểm thử đăng nhập thật bằng `admin01/123456` và `staff01/123456`.
5. Tạo thư mục BLL/DAL/Models/DTOs/Common và chuyển module `Customer` hoặc `Employee` đầu tiên sang 3 lớp.
6. Sau khi module đầu tiên ổn, áp dụng cùng pattern cho các module còn lại.

## Tiêu chí hoàn thành

- App mở được giao diện demo không cần database.
- App đăng nhập thật được khi database đã import đúng.
- Các form chính thao tác được với dữ liệu thật.
- Code không còn SQL trực tiếp trong Form sau khi hoàn tất refactor.
- Dự án trình bày rõ mô hình 3 lớp: Presentation, Business Logic, Data Access.
