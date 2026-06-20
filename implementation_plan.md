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
- Cấu hình kết nối app đang dùng user MySQL riêng `store_app/store123`.
- Đã thêm xử lý lỗi toàn cục và màn hình báo lỗi module để app không tự thoát khi một form như POS gặp lỗi khởi tạo/kết nối.

## Thống kê đã hoàn thành

- Build ứng dụng WinForms thành công bằng MSBuild.
- Có giao diện demo để xem app mà chưa cần database.
- Có database dump/schema phục vụ chạy nghiệp vụ thật.
- Có hướng dẫn setup MySQL và user `store_app` trong README.
- Đã phân quyền menu theo role `Admin`, `Manager`, `Cashier`, `Staff`.
- Đã có các form nghiệp vụ chính.
- Đã bắt đầu mô hình 3 lớp với module `Customer`.
- Đã gia cố luồng mở `MainForm`/module con: nếu POS hoặc form khác lỗi, app hiển thị thông báo thay vì tự đóng.

## Thống kê chưa hoàn thành

- Chưa xác nhận toàn bộ luồng nghiệp vụ thật sau khi import database trên máy đang chạy.
- Chưa kiểm thử xong đăng nhập nhân viên `staff01/123456` và mở POS sau khi database kết nối ổn định.
- POS vẫn còn SQL trực tiếp trong Form, chưa tách BLL/DAL.
- Employee, Promotion, Product, Inventory, Report chưa tách sang mô hình 3 lớp.
- Chưa đưa Entity Framework 6 vào DAL.
- Chưa hoàn thiện quản lý biến thể sản phẩm.
- Chưa hoàn thiện khuyến mãi `buy_x_get_y`.
- Chưa có in/xuất hóa đơn thật.
- Chưa có tài liệu báo cáo đồ án đầy đủ như ERD, mô tả use case, sơ đồ kiến trúc, hướng dẫn sử dụng.

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

Trạng thái hiện tại của Milestone 2:

- Đã tạo khung thư mục `BLL`, `DAL`, `Models`, `DTOs`, `Common`.
- Đã tách module `Customer` đầu tiên:
  - `Models/Customer.cs`
  - `DTOs/CustomerListItemDto.cs`
  - `DTOs/LoyaltyHistoryDto.cs`
  - `DAL/CustomerRepository.cs`
  - `BLL/CustomerService.cs`
- `CustomerForm` hiện chỉ gọi `CustomerService`, không còn viết SQL trực tiếp trong Form.
- DAL vẫn đang dùng `DatabaseHelper` và SQL trực tiếp để giữ app ổn định trước khi đưa EF6 vào.

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
3. Import database MySQL và tạo user `store_app`.
4. Kiểm thử đăng nhập thật bằng `admin01/123456` và `staff01/123456`.
5. Kiểm thử mở POS bằng tài khoản Staff/Cashier; nếu còn lỗi, ghi lại nội dung thông báo mới trên màn hình.
6. Kiểm thử module `Customer` với database thật sau khi import dump.
7. Chuyển module `Employee` sang cùng pattern 3 lớp.
8. Sau khi Customer/Employee ổn, tiếp tục Promotion, Product, Inventory, POS.

## Tiêu chí hoàn thành

- App mở được giao diện demo không cần database.
- App đăng nhập thật được khi database đã import đúng.
- Các form chính thao tác được với dữ liệu thật.
- Code không còn SQL trực tiếp trong Form sau khi hoàn tất refactor.
- Dự án trình bày rõ mô hình 3 lớp: Presentation, Business Logic, Data Access.
