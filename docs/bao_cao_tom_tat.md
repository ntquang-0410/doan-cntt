# Báo Cáo Tóm Tắt Đồ Án

## Tên đề tài

Ứng dụng WinForms quản lý cửa hàng tiện lợi.

## Mục tiêu

Xây dựng phần mềm desktop hỗ trợ các nghiệp vụ cơ bản của cửa hàng tiện lợi:

- Đăng nhập và phân quyền người dùng.
- Bán hàng POS.
- Quản lý sản phẩm, khách hàng, nhân viên.
- Quản lý tồn kho và nhập hàng.
- Quản lý khuyến mãi.
- Xem báo cáo doanh thu và nhật ký hoạt động.

## Công nghệ sử dụng

- C# WinForms trên .NET Framework 4.8.
- MySQL.
- ADO.NET với `MySql.Data.dll`.
- Tổ chức theo hướng 3 lớp từng bước:
  - Presentation Layer: `Forms`.
  - Business Logic Layer: `BLL`.
  - Data Access Layer: `DAL`.

## Cơ sở dữ liệu

Database chính: `convenience_store`.

Một số nhóm bảng quan trọng:

- `users`: tài khoản và phân quyền.
- `products`, `categories`, `suppliers`: sản phẩm, danh mục, nhà cung cấp.
- `inventory`, `stock_movements`: tồn kho và lịch sử kho.
- `purchase_orders`, `purchase_order_items`: nhập hàng.
- `orders`, `order_items`, `order_promotions`: bán hàng.
- `customers`, `loyalty_transactions`: khách hàng và điểm tích lũy.
- `promotions`: khuyến mãi.
- `audit_logs`: nhật ký hoạt động.

File dump sạch dùng khi nộp/chạy lại:

```text
database/dump-convenience_store-clean-20260620.sql
```

## Các chức năng đã hoàn thành

- Đăng nhập bằng tài khoản trong database.
- Phân quyền menu theo vai trò.
- Chế độ xem giao diện demo không cần MySQL.
- POS bán hàng:
  - Tìm sản phẩm theo tên/barcode.
  - Thêm sản phẩm vào giỏ.
  - Tăng/giảm số lượng.
  - Thanh toán và tạo hóa đơn.
  - Tự động trừ tồn kho qua trigger.
- Tồn kho và nhập hàng:
  - Xem, tìm kiếm, lọc tồn kho.
  - Tạo đơn nhập hàng.
  - Xác nhận nhận hàng để cộng tồn kho.
  - Hủy đơn đang chờ.
  - Hoàn tác đơn đã nhập.
  - Điều chỉnh tồn kho thủ công và ghi lịch sử.
- Quản lý khách hàng.
- Quản lý nhân viên.
- Quản lý sản phẩm/danh mục.
- Quản lý khuyến mãi.
- Báo cáo doanh thu, sản phẩm bán chạy, nhật ký hoạt động.

## Kiến trúc hiện tại

Dự án ưu tiên hoàn thiện chức năng trong phạm vi môn học. Module `Customer` đã được tách mẫu theo hướng 3 lớp:

- `CustomerForm`: giao diện.
- `CustomerService`: nghiệp vụ.
- `CustomerRepository`: truy cập dữ liệu.
- `Customer`, `CustomerListItemDto`, `LoyaltyHistoryDto`: model/DTO.

Các module còn lại vẫn còn SQL trực tiếp trong Form thông qua `DatabaseHelper`. Đây là hướng chấp nhận được trong phạm vi đồ án nhỏ, và có thể tiếp tục refactor sau.

## Tài khoản mẫu

```text
admin01 / 123456
staff01 / 123456
```

## Hướng phát triển tiếp

- Tách tiếp các module `Employee`, `Promotion`, `Product`, `Inventory`, `POS` sang BLL/DAL.
- Bổ sung in hóa đơn thật.
- Hoàn thiện báo cáo theo nhiều khoảng thời gian.
- Bổ sung giao diện nhập dữ liệu nhanh bằng máy quét barcode.

