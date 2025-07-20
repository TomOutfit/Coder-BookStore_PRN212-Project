# BookStore Management System

![BookStore UI Screenshot](./screenshot-main.png)

## 🏆 Giới thiệu

**BookStore Management** là ứng dụng quản lý nhà sách hiện đại, phát triển bằng WPF (C# .NET), giao diện đẹp, thân thiện, đồng bộ dữ liệu hai chiều, hỗ trợ đầy đủ các nghiệp vụ quản lý sách, người dùng, đơn hàng, thể loại, và hồ sơ cá nhân.

---

## 🚀 Tính năng nổi bật

- **Giao diện WPF hiện đại**: Sử dụng style Cosmo, màu sắc tươi sáng, nút bấm nổi bật, DataGrid bo góc, bóng đổ, responsive.
- **Quản lý Sách, Người dùng, Đơn hàng, Thể loại**: CRUD đầy đủ, thao tác nhanh, đồng bộ dữ liệu ngay lập tức giữa các tab.
- **Phân trang thông minh**: Chọn số bản ghi/trang, chuyển trang, đi tới trang bất kỳ, UI phân trang đẹp mắt.
- **Tìm kiếm, lọc dữ liệu**: Tìm kiếm nhanh theo tên, trạng thái, ...
- **Đăng nhập bảo mật**: Chỉ cần nhập đúng username/password như trong database (plain text).
- **Cập nhật hồ sơ cá nhân**: Đổi thông tin, đổi mật khẩu, đồng bộ dữ liệu ngay lập tức.
- **Thông báo thành công/thất bại rõ ràng**: Giao diện popup thân thiện.
- **Đồng bộ hai chiều**: Thao tác ở tab nào, các tab khác tự động cập nhật dữ liệu mới nhất.
- **Dữ liệu mẫu phong phú**: Có sẵn script SQL tạo và seed dữ liệu mẫu.

---

## 🖼️ Giao diện mẫu

> ![Dashboard](./screenshot-dashboard.png)
> ![Quản lý sách](./screenshot-books.png)
> ![Quản lý đơn hàng](./screenshot-orders.png)
> ![Hồ sơ cá nhân](./screenshot-profile.png)

---

## 🛠️ Công nghệ sử dụng
- **.NET 9.0** (WPF, C#)
- **SQL Server** (script tạo DB: `BookStoreDB_PRN.sql`)
- **MVVM Pattern**
- **Dependency Injection**
- **Custom Resource Styles** (CosmoTheme.xaml)

---

## ⚡ Hướng dẫn cài đặt & chạy thử

1. **Clone project:**
   ```bash
   git clone <repo-url>
   ```
2. **Khởi tạo database:**
   - Mở `BookStoreDB_PRN.sql` bằng SSMS hoặc Azure Data Studio, chạy toàn bộ script để tạo và seed dữ liệu mẫu.
3. **Mở solution:**
   - Mở `BookStore/BookStore.sln` bằng Visual Studio 2022 trở lên.
4. **Build & Run:**
   - Chọn project `PresentationLayer` làm startup, nhấn F5 để chạy.
5. **Đăng nhập:**
   - Dùng tài khoản mẫu trong DB, ví dụ:
     - Username: `admin`  | Password: `hashed_admin_pass`
     - Username: `staff1` | Password: `hashed_staff1`
     - ...

---

## 💡 Lưu ý
- **Mật khẩu hiện tại lưu dạng plain text** (để dễ test, nâng cấp hash ở bản sau).
- **Sau khi cập nhật profile, nếu không nhập mật khẩu mới thì mật khẩu cũ vẫn giữ nguyên.**
- **Nếu lỡ cập nhật mật khẩu thành chuỗi hash, hãy sửa lại trong DB về plain text.**

---

## 📂 Cấu trúc thư mục
```
BookStore/
├── BusinessLayer/         // Xử lý nghiệp vụ, service
├── DataLayer/             // Truy xuất dữ liệu, repository
├── Entities/              // Định nghĩa entity (Book, User, ...)
├── PresentationLayer/     // Giao diện WPF, View, ViewModel, Resource
│   ├── Views/             // XAML UI
│   ├── ViewModels/        // Logic MVVM
│   ├── Resources/         // Style, theme
│   └── Helpers/           // PasswordBox binding, ...
├── BookStoreDB_PRN.sql    // Script tạo DB và seed dữ liệu
└── README.md
```

---

## ✨ Đóng góp & nâng cấp
- Chào đón mọi ý tưởng nâng cấp: hash mật khẩu, phân quyền, báo cáo thống kê, xuất file, ...
- Giao diện, style, UX luôn sẵn sàng tối ưu thêm!

---

## 📧 Liên hệ
- **Tác giả:** [Your Name]
- **Email:** your.email@example.com

---

> **BookStore Management – Giao diện đẹp, trải nghiệm mượt, quản lý nhà sách dễ dàng!**
