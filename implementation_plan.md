# Kế hoạch triển khai ứng dụng WinForms quản lý cửa hàng tiện lợi

Dự án này là một ứng dụng Windows Forms (WinForms) bằng ngôn ngữ C# để quản lý cửa hàng tiện lợi, kết nối với cơ sở dữ liệu MySQL thông qua thư viện `MySql.Data.dll` (v8.0.30) có sẵn trên máy. Hệ thống phân chia thành hai vai trò chính: **Quản trị viên (Admin/Manager)** và **Nhân viên bán hàng (Cashier/Staff)**.

---

## User Review Required

> [!IMPORTANT]
> **Phương án Biên dịch & Cấu hình:**
> - Máy hiện tại chưa cài đặt **.NET SDK** nhưng có sẵn **.NET 9.0 Desktop Runtime**, **.NET 6.0 Desktop Runtime** và **MSBuild v4.0.30319** (thuộc .NET Framework 4.8).
> - Để thuận tiện nhất, chúng tôi đề xuất xây dựng ứng dụng dựa trên **.NET Framework 4.8** (sử dụng MSBuild có sẵn để biên dịch trực tiếp, không yêu cầu cài đặt thêm phần mềm).
> - Nếu bạn muốn viết code theo chuẩn .NET Core / .NET 9 mới nhất, bạn cần cài đặt thêm **.NET SDK 9.0** hoặc **.NET SDK 8.0**. Sau đó chúng tôi có thể dùng lệnh `dotnet build` để biên dịch.

> [!WARNING]
> **Kết nối Cơ sở dữ liệu (MySQL):**
> - Chúng tôi phát hiện MySQL đang chạy thông qua service `MySQL80` (tiến trình `mysqld`).
> - Tuy nhiên, tài khoản `root` hiện tại yêu cầu mật khẩu. Bạn vui lòng cung cấp thông tin kết nối (Host, Port, Username, Password, Database Name) hoặc xác nhận mật khẩu để chúng tôi cấu hình file kết nối `App.config` và kiểm tra cấu trúc bảng thực tế.

---

## Open Questions

> [!IMPORTANT]
> 1. **Mật khẩu root MySQL của bạn là gì?** Hoặc bạn có tài khoản MySQL nào khác để chúng tôi kết nối và import/xác thực schema không?
> 2. **Bạn muốn phát triển trên phiên bản .NET nào?**
>    - **Lựa chọn A (Đề xuất):** `.NET Framework 4.8` - Biên dịch được ngay bằng MSBuild có sẵn (`C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe`) không cần cài thêm gì.
>    - **Lựa chọn B:** `.NET 9.0` (hoặc `.NET 8.0`) - Bạn sẽ cài đặt thêm .NET SDK trên máy, giúp code hiện đại hơn và hỗ trợ NuGet tự động dễ dàng hơn.

---

## Proposed Changes

Tất cả các file mã nguồn ứng dụng sẽ được tạo trong thư mục `d:\New folder\doan-cntt\ConvenienceStoreApp`.

### 1. Thư viện & Cấu hình dự án (Project & Configurations)

#### [NEW] [ConvenienceStoreApp.csproj](file:///d:/New%20folder/doan-cntt/ConvenienceStoreApp/ConvenienceStoreApp.csproj)
Tạo file dự án định dạng MSBuild tương thích với .NET Framework 4.8, tham chiếu các thư viện WinForms chuẩn và `MySql.Data.dll` được trích xuất từ đường dẫn hệ thống.

#### [NEW] [App.config](file:///d:/New%20folder/doan-cntt/ConvenienceStoreApp/App.config)
Cấu hình chuỗi kết nối MySQL (Connection String) và các cài đặt mặc định của ứng dụng.

### 2. Lớp Tiện ích & Kết nối Cơ sở Dữ liệu (Database Layer)

#### [NEW] [DatabaseHelper.cs](file:///d:/New%20folder/doan-cntt/ConvenienceStoreApp/DatabaseHelper.cs)
Lớp tĩnh thực hiện kết nối cơ sở dữ liệu MySQL, thực thi các truy vấn SQL (`ExecuteNonQuery`, `ExecuteScalar`, `ExecuteReader`) và quản lý Transaction để đảm bảo tính toàn vẹn dữ liệu (đặc biệt khi lưu hóa đơn kèm chi tiết hóa đơn).

#### [NEW] [SessionManager.cs](file:///d:/New%20folder/doan-cntt/ConvenienceStoreApp/SessionManager.cs)
Quản lý thông tin đăng nhập hiện tại của người dùng (Username, Vai trò, ID nhân viên) để phân quyền hiển thị chức năng trên giao diện.

### 3. Giao diện Người dùng (UI Forms)

Giao diện sẽ được thiết kế theo phong cách hiện đại (Flat Design, tông màu hài hòa, có sidebar menu điều hướng mượt mà, hạn chế dùng điều khiển mặc định thô sơ).

#### [NEW] [LoginForm.cs](file:///d:/New%20folder/doan-cntt/ConvenienceStoreApp/Forms/LoginForm.cs)
Màn hình đăng nhập yêu cầu Username và Password. Kiểm tra đối chiếu bảng `users`. Phân quyền truy cập dựa trên vai trò (`role`).

#### [NEW] [MainForm.cs](file:///d:/New%20folder/doan-cntt/ConvenienceStoreApp/Forms/MainForm.cs)
Giao diện chính chứa khung điều hướng (Sidebar) và vùng hiển thị nội dung (Content Panel). Tự động thay đổi các chức năng hiển thị tùy theo quyền của User đang đăng nhập.

#### [NEW] [POSForm.cs](file:///d:/New%20folder/doan-cntt/ConvenienceStoreApp/Forms/POSForm.cs)
*Giao diện dành cho nhân viên bán hàng (Cashier):*
- Ô nhập/quét Barcode để thêm nhanh sản phẩm vào giỏ hàng.
- Danh sách giỏ hàng hiển thị tên sản phẩm, số lượng (cho phép điều chỉnh trực tiếp), đơn giá, chiết khấu, thành tiền.
- Khu vực tìm kiếm nhanh sản phẩm theo tên hoặc danh mục.
- Thông tin khách hàng thành viên (tìm theo SĐT, hiển thị số điểm tích lũy, cho phép trừ điểm/tích điểm mới).
- Tính toán tổng tiền, thuế VAT, giảm giá khuyến mãi (từ bảng `promotions`), tiền khách đưa, tiền thối lại.
- Nút bấm thanh toán bằng Tiền mặt (Cash), Thẻ (Card), Ví điện tử (E-wallet) và in hóa đơn ảo.

#### [NEW] [DailyShiftForm.cs](file:///d:/New%20folder/doan-cntt/ConvenienceStoreApp/Forms/DailyShiftForm.cs)
*Giao diện quản lý ca làm việc:*
- Nhân viên khai báo số tiền đầu ca (Opening Balance) khi mở két.
- Kết thúc ca làm việc khai báo số tiền mặt thực tế thu được (Closing Balance) để hệ thống tự động đối chiếu doanh thu trên phần mềm, tính toán chênh lệch (Expected Cash vs Closing Balance). Ghi nhận vào bảng `daily_cash_register`.

#### [NEW] [ProductManagementForm.cs](file:///d:/New%20folder/doan-cntt/ConvenienceStoreApp/Forms/ProductManagementForm.cs)
*Giao diện quản trị sản phẩm & danh mục (Admin):*
- Xem danh sách sản phẩm với bộ lọc theo danh mục, nhà cung cấp, trạng thái.
- Thêm, sửa, xóa (hoặc ngừng kích hoạt) sản phẩm và biến thể sản phẩm (bảng `products` và `product_variants`).
- Quản lý danh mục sản phẩm đa cấp (bảng `categories`).

#### [NEW] [InventoryForm.cs](file:///d:/New%20folder/doan-cntt/ConvenienceStoreApp/Forms/InventoryForm.cs)
*Giao diện quản lý kho hàng (Admin/Manager):*
- Xem tồn kho hiện tại (bảng `inventory`), lọc sản phẩm sắp hết hàng (tồn < `min_quantity`) hoặc sắp hết hạn sử dụng (`expiry_date`).
- Tạo đơn nhập hàng từ nhà cung cấp (bảng `purchase_orders` và `purchase_order_items`).
- Theo dõi lịch sử biến động kho (bảng `stock_movements`).

#### [NEW] [EmployeeForm.cs](file:///d:/New%20folder/doan-cntt/ConvenienceStoreApp/Forms/EmployeeForm.cs)
*Giao diện quản lý nhân viên (chỉ dành cho Admin):*
- Quản lý danh sách tài khoản nhân viên (bảng `users`).
- Thêm tài khoản mới, phân quyền (Admin, Manager, Cashier, Staff), kích hoạt/khóa tài khoản.

#### [NEW] [CustomerForm.cs](file:///d:/New%20folder/doan-cntt/ConvenienceStoreApp/Forms/CustomerForm.cs)
*Giao diện quản lý khách hàng thành viên (Admin/Cashier):*
- Đăng ký thông tin khách hàng mới (Tên, SĐT, Email).
- Xem lịch sử mua hàng và lịch sử biến động điểm tích lũy (bảng `loyalty_transactions`).

#### [NEW] [PromotionForm.cs](file:///d:/New%20folder/doan-cntt/ConvenienceStoreApp/Forms/PromotionForm.cs)
*Giao diện quản lý khuyến mãi (Admin/Manager):*
- Thiết lập các chương trình khuyến mãi (theo phần trăm, giảm tiền cố định, hoặc mua X tặng Y).
- Quản lý thời hạn áp dụng (`start_date`, `end_date`) và trạng thái kích hoạt.

#### [NEW] [ReportForm.cs](file:///d:/New%20folder/doan-cntt/ConvenienceStoreApp/Forms/ReportForm.cs)
*Giao diện thống kê báo cáo (Admin/Manager):*
- Thống kê doanh thu, lợi nhuận theo khoảng thời gian.
- Biểu đồ top sản phẩm bán chạy nhất.
- Xem nhật ký hệ thống (bảng `audit_logs`).

---

## Verification Plan

### Automated Tests
- Biên dịch ứng dụng bằng lệnh MSBuild để đảm bảo không lỗi cú pháp:
  ```powershell
  & C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe d:\New%20folder\doan-cntt\ConvenienceStoreApp\ConvenienceStoreApp.csproj /t:Build /p:Configuration=Debug
  ```
- Kiểm tra tính tương thích và kết nối cơ sở dữ liệu khi ứng dụng khởi chạy.

### Manual Verification
1. **Đăng nhập:** Thử đăng nhập bằng tài khoản `admin` (vai trò Admin) và `cashier` (vai trò Cashier) để xác nhận phân quyền giao diện hiển thị đúng.
2. **Bán hàng (POS):** Thực hiện quét/thêm sản phẩm vào giỏ hàng, chọn khách hàng thành viên, áp dụng khuyến mãi, thanh toán và kiểm tra xem điểm tích lũy của khách hàng có tăng lên, tồn kho sản phẩm có tự động trừ đi (thông qua TRIGGER trong database).
3. **Quản lý tồn kho:** Tạo một phiếu nhập kho mới từ nhà cung cấp, kiểm tra số lượng tồn kho tự động tăng lên sau khi xác nhận nhận hàng.
4. **Báo cáo doanh thu:** Kiểm tra xem doanh thu sau khi bán hàng có cập nhật chính xác trên biểu đồ báo cáo hay không.
