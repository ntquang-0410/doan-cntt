# Kế hoạch triển khai ứng dụng WinForms quản lý cửa hàng tiện lợi

## Định hướng mới

Dự án không bắt buộc dùng SQL Server hoặc Entity Framework. Hướng triển khai được chốt là:

```text
WinForms .NET Framework 4.8
  -> Presentation Layer: Forms
  -> Business Logic Layer: Services/BLL
  -> Data Access Layer: Repositories/DAL + ADO.NET/MySql.Data
  -> MySQL
```

Ưu tiên triển khai là **WinForms + MySQL + ADO.NET**, tổ chức theo mô hình 3 lớp từng bước. Trước mắt giữ app ổn định, giao diện thao tác được, sau đó tách dần code SQL trực tiếp trong Form sang BLL/DAL.

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
- POS đã chỉnh được số lượng bằng nút cộng/trừ trong giỏ hàng và có cảnh báo khi chuyển module nếu còn giỏ chưa thanh toán.
- Inventory đã có tạo đơn nhập, sửa/xóa dòng trong đơn đang tạo, xác nhận nhập kho, hủy đơn pending, hoàn tác đơn đã nhập và điều chỉnh tồn kho thủ công.
- Luồng sản phẩm đã chốt theo cách đơn giản: mỗi barcode là một sản phẩm độc lập; không dùng `product_variants` trong luồng chính.
- Đã dọn dữ liệu mẫu trùng sản phẩm/barcode trong MySQL và tạo dump sạch `database/dump-convenience_store-clean-20260620.sql`.
- Đã cập nhật README theo công nghệ hiện tại, cách setup, tài khoản mẫu, luồng test và ghi chú báo cáo đồ án.

## Thống kê đã hoàn thành

- Build ứng dụng WinForms thành công bằng MSBuild.
- Có giao diện demo để xem app mà chưa cần database.
- Có database dump/schema phục vụ chạy nghiệp vụ thật.
- Có hướng dẫn setup MySQL và user `store_app` trong README.
- Đã phân quyền menu theo role `Admin`, `Manager`, `Cashier`, `Staff`.
- Đã có các form nghiệp vụ chính.
- Đã bắt đầu mô hình 3 lớp với module `Customer`.
- Đã gia cố luồng mở `MainForm`/module con: nếu POS hoặc form khác lỗi, app hiển thị thông báo thay vì tự đóng.
- POS đã thao tác bán hàng cơ bản tốt hơn: tìm barcode, thêm giỏ, tăng/giảm số lượng, kiểm tra tồn, thanh toán.
- Inventory đã hoàn thiện thêm các thao tác chính cho đồ án: nhập hàng, nhận hàng, hủy/hoàn tác đơn nhập, điều chỉnh tồn.

## Thống kê chưa hoàn thành

- POS vẫn còn SQL trực tiếp trong Form, chưa tách BLL/DAL.
- Employee, Promotion, Product, Inventory, Report chưa tách sang mô hình 3 lớp.
- Chưa cần Entity Framework 6; hướng chính hiện tại là ADO.NET/MySql.Data.
- Không triển khai quản lý biến thể sản phẩm trong giai đoạn nộp đồ án; mỗi barcode là một sản phẩm riêng.
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

## Milestone 2 - Tách mô hình 3 lớp từng bước

Tạo và chuyển dần sang cấu trúc:

```text
ConvenienceStoreApp/
├── Forms/              # Presentation Layer
├── BLL/                # Business logic/services
├── DAL/                # Repositories dùng ADO.NET/MySql.Data
├── Models/             # Domain/data models
├── DTOs/               # View/data transfer models
└── Common/             # Session, validation, formatting, helpers
```

Nguyên tắc:

- Form chỉ xử lý giao diện, sự kiện, bind dữ liệu, hiển thị thông báo.
- BLL xử lý nghiệp vụ: đăng nhập, phân quyền, tính tiền, tồn kho, khuyến mãi, ca làm việc.
- DAL xử lý truy cập dữ liệu bằng ADO.NET/MySql.Data thông qua repository.
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
- DAL vẫn đang dùng `DatabaseHelper` và SQL trực tiếp để giữ app ổn định. Đây là hướng phù hợp với công nghệ đã chốt: WinForms + MySQL + ADO.NET.

## Milestone 3 - Hoàn thiện 3 lớp với ADO.NET/MySQL

- Không đưa EF6 vào giai đoạn hiện tại.
- Tạo repository/service cho các nhóm nghiệp vụ chính:
  - `User`
  - `Category`
  - `Product`
  - `Inventory`
  - `Customer`
  - `Order`
  - `OrderItem`
  - `Promotion`
  - `PurchaseOrder`
  - `StockMovement`
- Chuyển module đơn giản trước:
  - Customer
  - Employee
  - Promotion
- Sau đó chuyển module phức tạp:
  - Product/Category
  - Inventory/PurchaseOrder
  - POS/Order/Payment

## Việc còn lại gần nhất

1. Rà lần cuối luồng POS: mở ca, thêm hàng, tăng/giảm số lượng, thanh toán, tồn kho giảm.
2. Rà lần cuối luồng Inventory: tạo đơn nhập, sửa/xóa dòng, nhận hàng, tồn kho tăng, hủy/hoàn tác đơn.
3. Chuẩn bị tài liệu nộp đồ án: ERD, use case, sơ đồ 3 lớp, hướng dẫn cài đặt/chạy app, tài khoản demo.
4. Nếu còn thời gian, chuyển module `Employee` sang cùng pattern 3 lớp như `Customer`.
5. Sau khi Customer/Employee ổn, có thể tiếp tục Promotion, Product, Inventory, POS.

## Tiêu chí hoàn thành

- App mở được giao diện demo không cần database.
- App đăng nhập thật được khi database đã import đúng.
- Các form chính thao tác được với dữ liệu thật.
- Code không còn SQL trực tiếp trong Form sau khi hoàn tất refactor.
- Dự án trình bày rõ mô hình 3 lớp: Presentation, Business Logic, Data Access.
- Công nghệ được trình bày thống nhất: WinForms + MySQL + ADO.NET/MySql.Data, không dùng EF6 trong phạm vi hiện tại.
