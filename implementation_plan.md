# Kế hoạch hoàn thiện ứng dụng WinForms quản lý cửa hàng tiện lợi

## Tổng quan

Dự án là ứng dụng Windows Forms bằng C# để quản lý cửa hàng tiện lợi, dùng MySQL và `MySql.Data.dll`. Hệ thống chia quyền theo các vai trò `Admin`, `Manager`, `Cashier`, `Staff`.

Trạng thái sau đợt sửa hiện tại: dự án đã build thành công bằng MSBuild, tạo được file `ConvenienceStoreApp\bin\Debug\ConvenienceStoreApp.exe`. Phần còn lại quan trọng nhất là xác thực database thật và kiểm thử các luồng nghiệp vụ.

## Đã hoàn thành

- Tạo project `.NET Framework 4.8` tại `ConvenienceStoreApp`.
- Cấu hình `App.config` với connection string MySQL mặc định:

  ```text
  Server=localhost;Port=3306;Database=convenience_store;Uid=root;Pwd=root;Charset=utf8mb4;
  ```

- Thêm `MySql.Data.dll` vào `ConvenienceStoreApp\lib`.
- Tạo các lớp nền:
  - `DatabaseHelper.cs`: kết nối database, query, non-query, scalar, hash/verify password cơ bản.
  - `SessionManager.cs`: lưu thông tin user, role, shift hiện tại.
- Tạo các màn hình chính:
  - `LoginForm`
  - `MainForm`
  - `POSForm`
  - `DailyShiftForm`
  - `ProductManagementForm`
  - `InventoryForm`
  - `EmployeeForm`
  - `CustomerForm`
  - `PromotionForm`
  - `ReportForm`
- Sửa lỗi compile:
  - Chuyển literal font sai kiểu như `9.75pt`, `9.5pt`, `10.5pt` sang `9.75f`, `9.5f`, `10.5f`.
  - Đổi pattern matching C# mới sang cú pháp tương thích compiler hiện có.
  - Loại bỏ `TextBox.PlaceholderText`, API không có trong WinForms .NET Framework.
  - Sửa tham số `DBNull.Value` trong POS để tránh lỗi kiểu dữ liệu.
  - Bổ sung `System.Collections.Generic` ở các form dùng `List<T>`.
- Build đã thành công bằng:

  ```powershell
  & C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe ConvenienceStoreApp\ConvenienceStoreApp.csproj /t:Build /p:Configuration=Debug
  ```

## Cảnh báo và giới hạn hiện tại

- Máy build thiếu .NET Framework 4.8 Targeting Pack, nên MSBuild cảnh báo `MSB3644`. Build vẫn thành công nhờ dùng assemblies từ GAC, nhưng nên cài .NET Framework 4.8 Developer Pack/Targeting Pack để build chuẩn hơn.
- Có warning kiến trúc `MSIL` với reference `AMD64`. Nếu gặp lỗi runtime trên máy khác, cân nhắc đặt `PlatformTarget` rõ ràng là `x64`.
- Máy có service `MySQL80` đang chạy, nhưng `mysql` CLI không có trong PATH nên chưa import/xác thực database bằng dòng lệnh được.
- `ProductManagementForm` hiện quản lý sản phẩm và danh mục, chưa có UI đầy đủ cho `product_variants`.
- `PromotionForm` chủ yếu hỗ trợ `percent` và `fixed`; schema có thêm `buy_x_get_y`, POS hiện chưa xử lý loại này.
- Chưa kiểm thử thủ công các luồng nghiệp vụ vì cần database đã import và connection string đúng mật khẩu.

## Việc cần làm tiếp

1. Xác thực database:
   - Tạo database `convenience_store` nếu chưa có.
   - Import `database\dump-convenience_store-202605120906.sql` để có dữ liệu mẫu đầy đủ, hoặc import schema rồi seed data.
   - Cập nhật `Pwd` trong `ConvenienceStoreApp\App.config` nếu mật khẩu root không phải `root`.

2. Kiểm thử đăng nhập:
   - Thử `admin01` / `123456`.
   - Thử `staff01` / `123456`.
   - Kiểm tra user inactive bị chặn.
   - Kiểm tra phân quyền menu theo role.

3. Kiểm thử POS và ca làm việc:
   - Cashier mở ca trong `DailyShiftForm`.
   - Thêm sản phẩm bằng barcode hoặc tìm kiếm.
   - Chọn khách hàng theo số điện thoại.
   - Thanh toán bằng `cash`, `card`, `e_wallet`.
   - Kiểm tra `orders`, `order_items`, `order_promotions`, tồn kho và điểm khách hàng sau giao dịch.

4. Kiểm thử kho:
   - Tạo purchase order.
   - Xác nhận nhận hàng.
   - Kiểm tra trigger `trg_purchase_received` cộng tồn kho và ghi `stock_movements`.

5. Kiểm thử CRUD quản trị:
   - Sản phẩm, danh mục, trạng thái active.
   - Nhân viên, role, khóa/mở tài khoản.
   - Khách hàng và lịch sử điểm.
   - Khuyến mãi percent/fixed.
   - Báo cáo doanh thu, top sản phẩm, audit log.

6. Hoàn thiện chức năng còn thiếu:
   - UI quản lý biến thể sản phẩm.
   - Logic khuyến mãi `buy_x_get_y`.
   - In hóa đơn hoặc xuất hóa đơn.
   - Thông báo lỗi kết nối database rõ hơn ở màn hình login.
   - Kiểm tra và chuẩn hóa dữ liệu đầu vào trước khi ghi database.

## Tiêu chí hoàn thành

- Build không lỗi.
- Database import được và app kết nối được.
- Tài khoản demo đăng nhập được.
- Cashier bán được một đơn hàng từ đầu đến cuối.
- Tồn kho tự động giảm khi bán hàng và tăng khi nhận hàng.
- Admin/Manager dùng được các màn hình quản trị chính.
- README phản ánh đúng cách build, cấu hình và chạy dự án.
