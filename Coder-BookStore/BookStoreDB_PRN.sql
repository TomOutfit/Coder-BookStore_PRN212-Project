-- Tạo cơ sở dữ liệu
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'BookStoreDB_PRN')
    CREATE DATABASE [BookStoreDB_PRN];
GO

USE [BookStoreDB_PRN];
GO

-- Tạo bảng Books
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'Books' AND type = 'U')
    CREATE TABLE Books (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Title NVARCHAR(255) NOT NULL,
        Author NVARCHAR(255) NULL,
        Price DECIMAL(18, 2) NOT NULL,
        PublishedDate DATE NULL,
        ISBN NVARCHAR(20) UNIQUE NOT NULL,
        Genre NVARCHAR(50) NULL,
        Description NVARCHAR(MAX) NULL,
        Stock INT NOT NULL DEFAULT 0,
        Publisher NVARCHAR(100) NULL,
        Language NVARCHAR(50) NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
    );
GO

-- Tạo bảng Users
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'Users' AND type = 'U')
    CREATE TABLE Users (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Username NVARCHAR(50) UNIQUE NOT NULL,
        Password NVARCHAR(255) NOT NULL,
        Email NVARCHAR(100) UNIQUE NOT NULL,
        FirstName NVARCHAR(50) NULL,
        LastName NVARCHAR(50) NULL,
        Address NVARCHAR(255) NULL,
        City NVARCHAR(100) NULL,
        State NVARCHAR(100) NULL,
        ZipCode NVARCHAR(20) NULL,
        PhoneNumber NVARCHAR(20) NULL,
        Role NVARCHAR(20) NOT NULL DEFAULT 'User',
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
    );
GO

-- Tạo bảng Categories
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'Categories' AND type = 'U')
    CREATE TABLE Categories (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) UNIQUE NOT NULL,
        Description NVARCHAR(MAX) NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
    );
GO

-- Tạo bảng Orders
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'Orders' AND type = 'U')
    CREATE TABLE Orders (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NULL,
        OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
        TotalAmount DECIMAL(18, 2) NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        ShippingAddress NVARCHAR(255) NULL,
        PaymentMethod NVARCHAR(50) NULL,
        Notes NVARCHAR(MAX) NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE SET NULL
    );
GO

-- Tạo bảng OrderDetails
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'OrderDetails' AND type = 'U')
    CREATE TABLE OrderDetails (
        Id INT PRIMARY KEY IDENTITY(1,1),
        OrderId INT NOT NULL,
        BookId INT NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18, 2) NOT NULL,
        Subtotal DECIMAL(18, 2) NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
        FOREIGN KEY (BookId) REFERENCES Books(Id) ON DELETE CASCADE
    );
GO

-- Tạo bảng Roles
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'Roles' AND type = 'U')
    CREATE TABLE Roles (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(50) UNIQUE NOT NULL,
        Description NVARCHAR(MAX) NULL
    );
GO

-- Xóa trigger cũ nếu tồn tại
IF OBJECT_ID('trg_Books_Update', 'TR') IS NOT NULL DROP TRIGGER trg_Books_Update;
IF OBJECT_ID('trg_Users_Update', 'TR') IS NOT NULL DROP TRIGGER trg_Users_Update;
IF OBJECT_ID('trg_Categories_Update', 'TR') IS NOT NULL DROP TRIGGER trg_Categories_Update;
IF OBJECT_ID('trg_Orders_Update', 'TR') IS NOT NULL DROP TRIGGER trg_Orders_Update;
IF OBJECT_ID('trg_OrderDetails_Update', 'TR') IS NOT NULL DROP TRIGGER trg_OrderDetails_Update;
GO

-- Tạo trigger cập nhật UpdatedAt
CREATE TRIGGER trg_Books_Update ON Books AFTER UPDATE AS 
    UPDATE Books SET UpdatedAt = GETDATE() FROM Books INNER JOIN inserted ON Books.Id = inserted.Id;
GO
CREATE TRIGGER trg_Users_Update ON Users AFTER UPDATE AS 
    UPDATE Users SET UpdatedAt = GETDATE() FROM Users INNER JOIN inserted ON Users.Id = inserted.Id;
GO
CREATE TRIGGER trg_Categories_Update ON Categories AFTER UPDATE AS 
    UPDATE Categories SET UpdatedAt = GETDATE() FROM Categories INNER JOIN inserted ON Categories.Id = inserted.Id;
GO
CREATE TRIGGER trg_Orders_Update ON Orders AFTER UPDATE AS 
    UPDATE Orders SET UpdatedAt = GETDATE() FROM Orders INNER JOIN inserted ON Orders.Id = inserted.Id;
GO
CREATE TRIGGER trg_OrderDetails_Update ON OrderDetails AFTER UPDATE AS 
    UPDATE OrderDetails SET UpdatedAt = GETDATE() FROM OrderDetails INNER JOIN inserted ON OrderDetails.Id = inserted.Id;
GO

-- Xóa dữ liệu cũ
DELETE FROM OrderDetails;
DELETE FROM Orders;
DELETE FROM Books;
DELETE FROM Categories;
DELETE FROM Users;
DELETE FROM Roles;
GO

-- Reset Identity
DBCC CHECKIDENT ('OrderDetails', RESEED, 0);
DBCC CHECKIDENT ('Orders', RESEED, 0);
DBCC CHECKIDENT ('Books', RESEED, 0);
DBCC CHECKIDENT ('Categories', RESEED, 0);
DBCC CHECKIDENT ('Users', RESEED, 0);
DBCC CHECKIDENT ('Roles', RESEED, 0);
GO

-- Chèn dữ liệu vào Roles
INSERT INTO Roles (Name, Description) VALUES
('Admin', N'Quản trị viên với quyền truy cập toàn phần'),
('User', N'Người dùng thông thường với quyền hạn cơ bản'),
('Staff', N'Nhân viên với quyền quản lý hạn chế');
GO

-- Chèn dữ liệu vào Users (15 bản ghi)
INSERT INTO Users (Username, Password, Email, FirstName, LastName, Address, City, State, ZipCode, PhoneNumber, Role, IsActive) VALUES
('admin_hanoi', 'admin123', 'admin.hanoi@bookstore.vn', N'Nguyễn', N'Hải', N'12 Đường Thành, Hoàn Kiếm', N'Hà Nội', 'VN', '100000', '0981234567', 'Admin', 1),
('user_hcm', 'user123', 'nguyen.van.a@hcm.vn', N'Nguyễn', N'Văn A', N'34 Lê Lợi, Quận 1', N'Hồ Chí Minh', 'VN', '700000', '0912345678', 'User', 1),
('staff_dn', 'staff123', 'le.thi.b@danang.vn', N'Lê', N'Thị B', N'56 Nguyễn Văn Linh, Hải Châu', N'Đà Nẵng', 'VN', '550000', '0908765432', 'Staff', 1),
('user_ct', 'user456', 'tran.van.c@cantho.vn', N'Trần', N'Văn C', N'78 Trần Phú, Ninh Kiều', N'Cần Thơ', 'VN', '940000', '0934567890', 'User', 1),
('admin_hp', 'admin456', 'pham.thi.d@haiphong.vn', N'Phạm', N'Thị D', N'90 Lạch Tray, Ngô Quyền', N'Hải Phòng', 'VN', '180000', '0945678901', 'Admin', 1),
('user_hue', 'user789', 'hoang.van.e@hue.vn', N'Hoàng', N'Văn E', N'22 Lê Lợi, TP Huế', N'Huế', 'VN', '530000', '0978901234', 'User', 1),
('staff_nt', 'staff789', 'vo.minh.f@nhatrang.vn', N'Võ', N'Minh F', N'45 Trần Phú, Nha Trang', N'Nha Trang', 'VN', '650000', '0965432109', 'Staff', 1),
('user_vt', 'user101', 'bui.thi.g@vungtau.vn', N'Bùi', N'Thị G', N'67 Bà Rịa, Vũng Tàu', N'Vũng Tàu', 'VN', '790000', '0956789012', 'User', 1),
('user_pq', 'user112', 'dang.van.h@phuquoc.vn', N'Đặng', N'Văn H', N'89 Đường 30/4, Phú Quốc', N'Phú Quốc', 'VN', '910000', '0923456789', 'User', 1),
('staff_qn', 'staff112', 'duong.thi.i@quangninh.vn', N'Đường', N'Thị I', N'101 Hạ Long, Hạ Long', N'Quảng Ninh', 'VN', '200000', '0918765432', 'Staff', 1),
('user_la', 'user223', 'truong.van.k@longan.vn', N'Trương', N'Văn K', N'23 Nguyễn Huệ, Tân An', N'Long An', 'VN', '850000', '0987654321', 'User', 1),
('user_bn', 'user334', 'nguyen.thi.l@bacninh.vn', N'Nguyễn', N'Thị L', N'45 Lý Thái Tổ, Bắc Ninh', N'Bắc Ninh', 'VN', '160000', '0932109876', 'User', 1),
('staff_tn', 'staff334', 'do.minh.m@tayninh.vn', N'Đỗ', N'Minh M', N'67 Quốc lộ 22, Tây Ninh', N'Tây Ninh', 'VN', '800000', '0943210987', 'Staff', 1),
('user_dn1', 'user445', 'phan.van.n@dongnai.vn', N'Phan', N'Văn N', N'89 Đồng Khởi, Biên Hòa', N'Đồng Nai', 'VN', '760000', '0976543210', 'User', 1),
('user_gl', 'user556', 'tran.thi.o@gialai.vn', N'Trần', N'Thị O', N'12 Phan Chu Trinh, Pleiku', N'Gia Lai', 'VN', '600000', '0921098765', 'User', 1);
GO

-- Chèn dữ liệu vào Categories
INSERT INTO Categories (Name, Description) VALUES
(N'Lập trình', N'Sách về các ngôn ngữ lập trình và kỹ thuật lập trình.'),
(N'Khoa học viễn tưởng', N'Sách về các chủ đề khoa học viễn tưởng và tương lai.'),
(N'Kỳ ảo', N'Sách về thế giới kỳ ảo, phép thuật và huyền bí.'),
(N'Lịch sử', N'Sách về lịch sử thế giới và các nền văn minh.'),
(N'Tự giúp bản thân', N'Sách hướng dẫn phát triển bản thân và kỹ năng sống.'),
(N'Tiểu sử', N'Sách về cuộc đời của các nhân vật nổi tiếng.'),
(N'Kinh doanh', N'Sách về kinh doanh, marketing và quản lý.'),
(N'Tâm lý học', N'Sách nghiên cứu về tâm lý con người và hành vi xã hội.'),
(N'Hồi hộp', N'Sách thể loại kinh dị, hồi hộp và bí ẩn.'),
(N'Thiếu niên', N'Sách dành cho lứa tuổi thiếu niên với các câu chuyện phiêu lưu.');
GO

-- Chèn dữ liệu vào Books (100 bản ghi)
INSERT INTO Books (Title, Author, Price, PublishedDate, ISBN, Genre, Description, Stock, Publisher, Language) VALUES
(N'Lập trình C# cơ bản', N'Nguyễn Văn A', 150.00, '2020-01-15', '978-3-16-148410-0', N'Lập trình', N'Hướng dẫn lập trình C# từ cơ bản đến nâng cao.', 100, N'NXB Giáo dục Việt Nam', N'Tiếng Việt'),
(N'Khoa học viễn tưởng: Hành tinh đỏ', N'Trần Thị B', 200.00, '2019-05-20', '978-1-23-456789-0', N'Khoa học viễn tưởng', N'Cuộc phiêu lưu trên sao Hỏa.', 50, N'NXB Kim Đồng', N'Tiếng Việt'),
(N'Kỳ ảo: Phép thuật rừng xanh', N'Lê Văn C', 180.00, '2021-03-10', '978-0-12-345678-9', N'Kỳ ảo', N'Câu chuyện về phép thuật trong rừng sâu.', 75, N'NXB Trẻ', N'Tiếng Việt'),
(N'Lịch sử thế giới cổ đại', N'Phạm Thị D', 250.00, '2018-07-25', '978-4-56-789012-3', N'Lịch sử', N'Khám phá các nền văn minh cổ đại.', 30, N'NXB Tổng hợp TP.HCM', N'Tiếng Việt'),
(N'Tự giúp bản thân: Nghệ thuật sống đơn giản', N'Nguyễn Minh E', 120.00, '2022-02-05', '978-5-67-890123-4', N'Tự giúp bản thân', N'Hướng dẫn sống tối giản và hạnh phúc.', 200, N'NXB Lao động - Xã hội', N'Tiếng Việt'),
(N'Tiểu sử Steve Jobs', N'Trần Văn F', 300.00, '2017-11-30', '978-6-78-901234-5', N'Tiểu sử', N'Hành trình sáng tạo của Steve Jobs.', 80, N'NXB Phụ nữ Việt Nam', N'Tiếng Việt'),
(N'Kinh doanh: Khởi nghiệp từ con số 0', N'Lê Thị G', 220.00, '2020-09-15', '978-7-89-012345-6', N'Kinh doanh', N'Hướng dẫn khởi nghiệp bền vững.', 60, N'NXB Thế giới', N'Tiếng Việt'),
(N'Tâm lý học: Hiểu tâm trí con người', N'Phạm Văn H', 140.00, '2019-04-20', '978-8-90-123456-7', N'Tâm lý học', N'Nghiên cứu tâm trí và hành vi xã hội.', 90, N'NXB Đại học Quốc gia Hà Nội', N'Tiếng Việt'),
(N'Hồi hộp: Ngôi nhà ma ám', N'Nguyễn Thị I', 160.00, '2021-06-10', '978-9-01-234567-8', N'Hồi hộp', N'Truyện kinh dị về ngôi nhà cổ.', 40, N'NXB Văn học', N'Tiếng Việt'),
(N'Thiếu niên: Phiêu lưu nhóm bạn trẻ', N'Trần Minh J', 130.00, '2022-08-05', '978-0-12-345678-0', N'Thiếu niên', N'Hành trình phiêu lưu của nhóm bạn trẻ.', 150, N'NXB Kim Đồng', N'Tiếng Việt'),
(N'Lập trình Python cơ bản', N'Nguyễn Văn K', 170.00, '2020-01-15', '978-3-16-148410-1', N'Lập trình', N'Học Python từ cơ bản đến thực tế.', 120, N'NXB Giáo dục Việt Nam', N'Tiếng Việt'),
(N'Khoa học viễn tưởng: Cuộc chiến sao', N'Trần Thị L', 210.00, '2019-05-20', '978-1-23-456789-1', N'Khoa học viễn tưởng', N'Trận chiến giữa các vì sao.', 55, N'NXB Kim Đồng', N'Tiếng Việt'),
(N'Kỳ ảo: Vương quốc phép thuật', N'Lê Văn M', 190.00, '2021-03-10', '978-0-12-345679-0', N'Kỳ ảo', N'Khám phá vương quốc phép thuật.', 80, N'NXB Trẻ', N'Tiếng Việt'),
(N'Lịch sử Việt Nam', N'Phạm Thị N', 260.00, '2018-07-25', '978-4-56-789012-4', N'Lịch sử', N'Lịch sử Việt Nam qua các triều đại.', 35, N'NXB Tổng hợp TP.HCM', N'Tiếng Việt'),
(N'Tự giúp bản thân: Bí quyết thành công', N'Nguyễn Minh O', 130.00, '2022-02-05', '978-5-67-890123-5', N'Tự giúp bản thân', N'Bí quyết đạt thành công.', 210, N'NXB Lao động - Xã hội', N'Tiếng Việt'),
(N'Tiểu sử Bill Gates', N'Trần Văn P', 310.00, '2017-11-30', '978-6-78-901234-6', N'Tiểu sử', N'Hành trình sáng lập Microsoft.', 85, N'NXB Phụ nữ Việt Nam', N'Tiếng Việt'),
(N'Kinh doanh: Chiến lược marketing', N'Lê Thị Q', 230.00, '2020-09-15', '978-7-89-012345-7', N'Kinh doanh', N'Chiến lược marketing phát triển thương hiệu.', 65, N'NXB Thế giới', N'Tiếng Việt'),
(N'Tâm lý học: Hành vi con người', N'Phạm Văn R', 150.00, '2019-04-20', '978-8-90-123456-8', N'Tâm lý học', N'Nghiên cứu hành vi con người.', 95, N'NXB Đại học Quốc gia Hà Nội', N'Tiếng Việt'),
(N'Hồi hộp: Rừng ma ám', N'Nguyễn Thị S', 170.00, '2021-06-10', '978-9-01-234567-9', N'Hồi hộp', N'Bí ẩn trong khu rừng ma ám.', 45, N'NXB Văn học', N'Tiếng Việt'),
(N'Thiếu niên: Nhóm bạn siêu nhân', N'Trần Minh T', 140.00, '2022-08-05', '978-0-12-345679-1', N'Thiếu niên', N'Phiêu lưu của nhóm bạn có siêu năng lực.', 160, N'NXB Kim Đồng', N'Tiếng Việt'),
(N'Lập trình Java cơ bản', N'Nguyễn Văn U', 160.00, '2020-03-10', '978-3-16-148410-2', N'Lập trình', N'Học Java từ cơ bản đến ứng dụng.', 110, N'NXB Giáo dục Việt Nam', N'Tiếng Việt'),
(N'Khoa học viễn tưởng: Sao Thủy', N'Trần Thị V', 190.00, '2019-06-15', '978-1-23-456789-2', N'Khoa học viễn tưởng', N'Khám phá sao Thủy trong tương lai.', 60, N'NXB Kim Đồng', N'Tiếng Việt'),
(N'Kỳ ảo: Hòn đảo thần bí', N'Lê Văn X', 170.00, '2021-04-20', '978-0-12-345679-2', N'Kỳ ảo', N'Phiêu lưu trên hòn đảo phép thuật.', 70, N'NXB Trẻ', N'Tiếng Việt'),
(N'Lịch sử Trung Quốc', N'Phạm Thị Y', 240.00, '2018-08-10', '978-4-56-789012-5', N'Lịch sử', N'Lịch sử Trung Quốc qua các triều đại.', 40, N'NXB Tổng hợp TP.HCM', N'Tiếng Việt'),
(N'Tự giúp bản thân: Quản lý thời gian', N'Nguyễn Minh Z', 110.00, '2022-03-15', '978-5-67-890123-6', N'Tự giúp bản thân', N'Hướng dẫn quản lý thời gian hiệu quả.', 180, N'NXB Lao động - Xã hội', N'Tiếng Việt'),
(N'Tiểu sử Elon Musk', N'Trần Văn AA', 290.00, '2017-12-05', '978-6-78-901234-7', N'Tiểu sử', N'Hành trình sáng tạo của Elon Musk.', 90, N'NXB Phụ nữ Việt Nam', N'Tiếng Việt'),
(N'Kinh doanh: Quản lý tài chính', N'Lê Thị BB', 210.00, '2020-12-10', '978-7-89-012346-0', N'Kinh doanh', N'Quản lý tài chính cho doanh nghiệp nhỏ.', 80, N'NXB Thế giới', N'Tiếng Việt'),
(N'Tâm lý học: Giấc mơ và ý nghĩa', N'Phạm Văn CC', 135.00, '2019-07-25', '978-8-90-123457-1', N'Tâm lý học', N'Nghiên cứu giấc mơ và ý nghĩa tâm lý.', 110, N'NXB Đại học Quốc gia Hà Nội', N'Tiếng Việt'),
(N'Hồi hộp: Con tàu ma', N'Nguyễn Thị DD', 160.00, '2021-09-15', '978-9-01-234568-2', N'Hồi hộp', N'Bí ẩn trên con tàu ma trôi dạt.', 60, N'NXB Văn học', N'Tiếng Việt'),
(N'Thiếu niên: Hành trình vũ trụ', N'Trần Minh EE', 130.00, '2022-11-10', '978-0-12-345679-7', N'Thiếu niên', N'Phiêu lưu vũ trụ của nhóm bạn trẻ.', 150, N'NXB Kim Đồng', N'Tiếng Việt'),
(N'Lập trình C++ nâng cao', N'Nguyễn Văn FF', 180.00, '2020-04-20', '978-3-16-148410-3', N'Lập trình', N'Học C++ với các dự án thực tế.', 115, N'NXB Giáo dục Việt Nam', N'Tiếng Việt'),
(N'Khoa học viễn tưởng: Sao Mộc', N'Trần Thị GG', 195.00, '2019-08-15', '978-1-23-456789-4', N'Khoa học viễn tưởng', N'Khám phá sao Mộc với công nghệ vũ trụ.', 70, N'NXB Kim Đồng', N'Tiếng Việt'),
(N'Kỳ ảo: Đỉnh núi thần thánh', N'Lê Văn HH', 180.00, '2021-06-20', '978-0-12-345679-6', N'Kỳ ảo', N'Hành trình lên đỉnh núi thần thánh.', 90, N'NXB Trẻ', N'Tiếng Việt'),
(N'Lịch sử Nhật Bản', N'Phạm Thị SS', 245.00, '2018-10-10', '978-4-56-789012-7', N'Lịch sử', N'Lịch sử phát triển của Nhật Bản.', 50, N'NXB Tổng hợp TP.HCM', N'Tiếng Việt'),
(N'Tự giúp bản thân: Thiền định', N'Nguyễn Minh TT', 110.00, '2022-05-15', '978-5-67-890123-8', N'Tự giúp bản thân', N'Hướng dẫn thiền định để giảm căng thẳng.', 185, N'NXB Lao động - Xã hội', N'Tiếng Việt'),
(N'Tiểu sử Marie Curie', N'Trần Văn UU', 275.00, '2017-12-25', '978-6-78-901234-9', N'Tiểu sử', N'Nghiên cứu khoa học của Marie Curie.', 100, N'NXB Phụ nữ Việt Nam', N'Tiếng Việt'),
(N'Kinh doanh: Đầu tư chứng khoán', N'Lê Thị VV', 205.00, '2020-12-10', '978-7-89-012346-1', N'Kinh doanh', N'Hướng dẫn đầu tư chứng khoán hiệu quả.', 80, N'NXB Thế giới', N'Tiếng Việt'),
(N'Tâm lý học: Trí nhớ và học tập', N'Phạm Văn MM', 140.00, '2019-06-20', '978-8-90-123457-0', N'Tâm lý học', N'Nghiên cứu trí nhớ và phương pháp học tập.', 105, N'NXB Đại học Quốc gia Hà Nội', N'Tiếng Việt'),
(N'Hồi hộp: Hầm mộ cổ', N'Nguyễn Thị NN', 155.00, '2021-08-10', '978-9-01-234568-1', N'Hồi hộp', N'Bí ẩn trong hầm mộ cổ xưa.', 55, N'NXB Văn học', N'Tiếng Việt'),
(N'Thiếu niên: Cuộc đua kỳ thú', N'Trần Minh OO', 125.00, '2022-10-05', '978-0-12-345679-5', N'Thiếu niên', N'Hành trình đua xe kỳ thú của nhóm bạn trẻ.', 145, N'NXB Kim Đồng', N'Tiếng Việt'),
(N'Lập trình JavaScript', N'Nguyễn Văn PP', 165.00, '2020-05-25', '978-3-16-148410-4', N'Lập trình', N'Học JavaScript với các dự án web.', 110, N'NXB Giáo dục Việt Nam', N'Tiếng Việt'),
(N'Khoa học viễn tưởng: Hành tinh X', N'Trần Thị QQ', 200.00, '2019-07-10', '978-1-23-456789-3', N'Khoa học viễn tưởng', N'Khám phá hành tinh X với công nghệ tiên tiến.', 65, N'NXB Kim Đồng', N'Tiếng Việt'),
(N'Kỳ ảo: Thành phố ánh sáng', N'Lê Văn HH', 175.00, '2021-05-15', '978-0-12-345679-4', N'Kỳ ảo', N'Hành trình tại thành phố ánh sáng huyền bí.', 85, N'NXB Trẻ', N'Tiếng Việt'),
(N'Lịch sử châu Âu', N'Phạm Thị II', 230.00, '2018-09-05', '978-4-56-789012-6', N'Lịch sử', N'Lịch sử phát triển của châu Âu.', 45, N'NXB Tổng hợp TP.HCM', N'Tiếng Việt'),
(N'Tự giúp bản thân: Sức khỏe tinh thần', N'Nguyễn Minh JJ', 115.00, '2022-04-10', '978-5-67-890123-7', N'Tự giúp bản thân', N'Chăm sóc sức khỏe tinh thần.', 190, N'NXB Lao động - Xã hội', N'Tiếng Việt'),
(N'Tiểu sử Nelson Mandela', N'Trần Văn KK', 280.00, '2017-12-20', '978-6-78-901234-8', N'Tiểu sử', N'Hành trình đấu tranh tự do của Nelson Mandela.', 95, N'NXB Phụ nữ Việt Nam', N'Tiếng Việt'),
(N'Kinh doanh: Thương mại điện tử', N'Lê Thị LL', 200.00, '2020-11-15', '978-7-89-012345-9', N'Kinh doanh', N'Phát triển kinh doanh online hiệu quả.', 75, N'NXB Thế giới', N'Tiếng Việt'),
(N'Tâm lý học: Tình yêu và mối quan hệ', N'Phạm Văn CC', 130.00, '2019-05-25', '978-8-90-123457-2', N'Tâm lý học', N'Nghiên cứu về tình yêu và mối quan hệ.', 100, N'NXB Đại học Quốc gia Hà Nội', N'Tiếng Việt'),
(N'Hồi hộp: Lâu đài ma quỷ', N'Nguyễn Thị DD', 150.00, '2021-07-15', '978-9-01-234568-0', N'Hồi hộp', N'Bí ẩn trong lâu đài cổ.', 50, N'NXB Văn học', N'Tiếng Việt'),
(N'Thiếu niên: Bí mật rừng sâu', N'Trần Minh EE', 120.00, '2022-09-10', '978-0-12-345679-3', N'Thiếu niên', N'Khám phá rừng sâu của nhóm bạn trẻ.', 140, N'NXB Kim Đồng', N'Tiếng Việt'),
('The Lord of the Rings', 'J.R.R. Tolkien', 25.99, '1954-07-29', '978-0-261-10325-2', N'Kỳ ảo', N'Hành trình sử thi ở Trung Địa.', 100, 'HarperCollins', 'English'),
('1984', 'George Orwell', 9.99, '1949-06-08', '978-0-452-28423-4', N'Khoa học viễn tưởng', N'Tiểu thuyết về xã hội độc tài.', 120, 'Secker & Warburg', 'English'),
('To Kill a Mockingbird', 'Harper Lee', 10.75, '1960-07-11', '978-0-06-112008-4', N'Tiểu thuyết', N'Câu chuyện về công lý và bất công.', 110, 'J.B. Lippincott & Co.', 'English'),
('The Great Gatsby', 'F. Scott Fitzgerald', 8.99, '1925-04-10', '978-0-7432-7356-5', N'Tiểu thuyết', N'Hình ảnh thời kỳ Jazz ở Mỹ.', 130, 'Scribner', 'English'),
('Sapiens', 'Yuval Noah Harari', 18.00, '2014-09-04', '978-0-7710-3850-1', N'Lịch sử', N'Lịch sử loài người từ thời cổ đại.', 150, 'Harvill Secker', 'English'),
('Atomic Habits', 'James Clear', 15.25, '2018-10-16', '978-0-7352-1129-3', N'Tự giúp bản thân', N'Xây dựng thói quen tốt.', 140, 'Avery', 'English'),
('The Hobbit', 'J.R.R. Tolkien', 14.50, '1937-09-21', '978-0-261-10357-4', N'Kỳ ảo', N'Cuộc phiêu lưu của Bilbo Baggins.', 90, 'George Allen & Unwin', 'English'),
('Pride and Prejudice', 'Jane Austen', 12.50, '1813-01-28', '978-0-14-143951-9', N'Tiểu thuyết', N'Tác phẩm kinh điển về tình yêu.', 75, 'Penguin Classics', 'English'),
('Dune', 'Frank Herbert', 16.00, '1965-08-01', '978-0-441-00590-2', N'Khoa học viễn tưởng', N'Câu chuyện về quyền lực và sa mạc.', 95, 'Chilton Books', 'English'),
('The Alchemist', 'Paulo Coelho', 10.00, '1988-01-01', '978-0-06-112241-6', N'Tiểu thuyết', N'Hành trình tìm kiếm giấc mơ.', 180, 'HarperOne', 'English'),
('Clean Code', 'Robert C. Martin', 30.00, '2008-08-11', '978-0-13-235088-5', N'Lập trình', N'Hướng dẫn viết mã sạch.', 70, 'Prentice Hall', 'English'),
('The Pragmatic Programmer', 'Andrew Hunt', 28.00, '1999-10-20', '978-0-201-61622-5', N'Lập trình', N'Hành trình trở thành bậc thầy lập trình.', 65, 'Addison-Wesley', 'English'),
('Design Patterns', 'Erich Gamma', 45.00, '1994-10-21', '978-0-201-63361-1', N'Lập trình', N'Các mẫu thiết kế hướng đối tượng.', 50, 'Addison-Wesley', 'English'),
('Thinking, Fast and Slow', 'Daniel Kahneman', 21.00, '2011-10-25', '978-0-14-103357-1', N'Tâm lý học', N'Hai hệ thống tư duy con người.', 115, 'Farrar, Straus and Giroux', 'English'),
('The Intelligent Investor', 'Benjamin Graham', 24.00, '1949-01-01', '978-0-06-055566-6', N'Kinh doanh', N'Kinh điển về đầu tư giá trị.', 100, 'HarperBusiness', 'English'),
('Rich Dad Poor Dad', 'Robert T. Kiyosaki', 14.50, '1997-04-01', '978-0-7407-0332-1', N'Kinh doanh', N'Giáo dục tài chính giữa giàu và nghèo.', 180, 'Warner Books', 'English'),
('The 7 Habits of Highly Effective People', 'Stephen R. Covey', 14.00, '1989-08-15', '978-0-671-70863-5', N'Tự giúp bản thân', N'Thói quen của người thành công.', 155, 'Free Press', 'English'),
('Becoming', 'Michelle Obama', 19.00, '2018-11-13', '978-0-525-63576-2', N'Tiểu sử', N'Hồi ký của cựu Đệ nhất phu nhân Mỹ.', 145, 'Crown', 'English'),
('Educated', 'Tara Westover', 16.50, '2018-02-20', '978-0-399-59050-5', N'Tiểu sử', N'Hành trình tự học từ gia đình biệt lập.', 130, 'Random House', 'English'),
('The Nightingale', 'Kristin Hannah', 15.50, '2015-03-03', '978-0-312-57085-4', N'Tiểu thuyết lịch sử', N'Chuyện hai chị em trong Thế chiến II.', 160, 'St. Martin''s Press', 'English'),
('Where the Crawdads Sing', 'Delia Owens', 14.99, '2018-08-14', '978-0-7352-1909-1', N'Tiểu thuyết', N'Câu chuyện về cô gái sống trong đầm lầy.', 170, 'G.P. Putnam''s Sons', 'English'),
('The Subtle Art of Not Giving a F*ck', 'Mark Manson', 12.00, '2016-09-13', '978-0-06-245771-5', N'Tự giúp bản thân', N'Sống không quan tâm đến điều không quan trọng.', 170, 'HarperOne', 'English'),
('The Power of Now', 'Eckhart Tolle', 13.50, '1997-08-19', '978-0-931442-72-1', N'Tự giúp bản thân', N'Sống trong hiện tại để tìm hạnh phúc.', 150, 'New World Library', 'English'),
('Man''s Search for Meaning', 'Viktor E. Frankl', 11.00, '1946-10-01', '978-0-8070-1427-2', N'Tâm lý học', N'Tìm ý nghĩa cuộc sống qua Holocaust.', 160, 'Beacon Press', 'English'),
('Meditations', 'Marcus Aurelius', 8.00, '0180-01-01', '978-0-14-044933-5', N'Triết học', N'Suy ngẫm của Hoàng đế La Mã.', 170, 'Penguin Classics', 'English'),
('The Art of War', 'Sun Tzu', 8.00, '0500-01-01', '978-0-486-42957-4', N'Chiến lược', N'Tác phẩm kinh điển về chiến thuật quân sự.', 160, 'Dover Publications', 'English'),
('Good to Great', 'Jim Collins', 22.00, '2001-10-16', '978-0-06-662099-3', N'Kinh doanh', N'Yếu tố tạo nên công ty xuất sắc.', 110, 'HarperBusiness', 'English'),
('Start with Why', 'Simon Sinek', 15.00, '2009-10-29', '978-0-14-104698-2', N'Kinh doanh', N'Lý do dẫn dắt hành động thành công.', 140, 'Portfolio', 'English'),
('Dare to Lead', 'Brené Brown', 17.00, '2018-10-09', '978-0-399-59252-3', N'Lãnh đạo', N'Lãnh đạo với lòng dũng cảm.', 120, 'Random House', 'English'),
('The Design of Everyday Things', 'Don Norman', 21.00, '2013-11-05', '978-0-465-05065-0', N'Thiết kế', N'Thiết kế thân thiện với người dùng.', 100, 'Basic Books', 'English'),
('Thinking in Systems', 'Donella H. Meadows', 20.00, '2008-11-26', '978-1-60358-055-8', N'Khoa học', N'Hướng dẫn tư duy hệ thống.', 95, 'Chelsea Green Publishing', 'English'),
('The Phoenix Project', 'Gene Kim', 28.00, '2013-01-10', '978-0-9882625-9-2', N'DevOps', N'Tiểu thuyết về quản lý IT và DevOps.', 90, 'IT Revolution Press', 'English'),
('Site Reliability Engineering', 'Betsy Beyer', 45.00, '2016-03-23', '978-1-49192-912-5', N'DevOps', N'Cách Google vận hành hệ thống.', 60, 'O''Reilly Media', 'English'),
('Clean Architecture', 'Robert C. Martin', 28.00, '2017-09-20', '978-0-13-449416-7', N'Lập trình', N'Kiến trúc phần mềm bền vững.', 90, 'Prentice Hall', 'English'),
('Refactoring', 'Martin Fowler', 30.00, '1999-07-08', '978-0-201-48567-8', N'Lập trình', N'Cải tiến mã nguồn hiện có.', 90, 'Addison-Wesley', 'English'),
('The Lean Startup', 'Eric Ries', 20.00, '2011-09-13', '978-0-307-88789-5', N'Kinh doanh', N'Phương pháp khởi nghiệp tinh gọn.', 130, 'Crown Business', 'English'),
('Deep Work', 'Cal Newport', 16.00, '2016-01-05', '978-0-451-49799-2', N'Tự giúp bản thân', N'Tập trung sâu để đạt hiệu quả cao.', 140, 'Grand Central Publishing', 'English'),
('The Body Keeps the Score', 'Bessel van der Kolk', 18.00, '2014-09-25', '978-0-670-78591-4', N'Tâm lý học', N'Trauma và cách chữa lành.', 110, 'Viking', 'English'),
('The Power of Habit', 'Charles Duhigg', 14.00, '2012-02-28', '978-0-8129-8385-3', N'Tự giúp bản thân', N'Khoa học về thói quen và thay đổi.', 150, 'Random House', 'English'),
('The Four Agreements', 'Don Miguel Ruiz', 12.00, '1997-11-07', '978-1-878424-31-1', N'Tự giúp bản thân', N'Bốn nguyên tắc sống hạnh phúc.', 160, 'Amber-Allen Publishing', 'English'),
('The War of Art', 'Steven Pressfield', 13.00, '2002-06-01', '978-0-446-69143-8', N'Tự giúp bản thân', N'Vượt qua nỗi sợ để sáng tạo.', 140, 'Warner Books', 'English'),
('The 5 Love Languages', 'Gary Chapman', 12.50, '1992-01-01', '978-0-8024-7315-9', N'Tự giúp bản thân', N'Ngôn ngữ yêu thương trong mối quan hệ.', 170, 'Northfield Publishing', 'English'),
('The Obstacle Is the Way', 'Ryan Holiday', 14.00, '2014-05-01', '978-0-7181-7992-2', N'Tự giúp bản thân', N'Biến trở ngại thành cơ hội.', 150, 'Portfolio', 'English'),
('Can''t Hurt Me', 'David Goggins', 16.00, '2018-12-04', '978-1-5445-1228-3', N'Tự giúp bản thân', N'Vượt qua giới hạn bản thân.', 120, 'Lioncrest Publishing', 'English'),
('The Millionaire Next Door', 'Thomas J. Stanley', 15.00, '1996-10-01', '978-0-671-01520-7', N'Kinh doanh', N'Bí mật tài chính của người giàu.', 130, 'Longstreet Press', 'English'),
('The E-Myth Revisited', 'Michael E. Gerber', 17.00, '1995-03-01', '978-0-88730-728-8', N'Kinh doanh', N'Xây dựng doanh nghiệp thành công.', 110, 'HarperBusiness', 'English'),
('Crucial Conversations', 'Patterson, Grenny', 18.00, '2002-09-01', '978-0-07-140194-5', N'Tự giúp bản thân', N'Kỹ năng giao tiếp trong tình huống quan trọng.', 140, 'McGraw-Hill', 'English'),
('Influence', 'Robert B. Cialdini', 16.50, '1984-01-01', '978-0-06-124189-6', N'Tâm lý học', N'Nghệ thuật thuyết phục hiệu quả.', 125, 'Harper Business', 'English'),
('Grit', 'Angela Duckworth', 15.00, '2016-05-03', '978-1-5011-1111-1', N'Tâm lý học', N'Sức mạnh của đam mê và kiên trì.', 140, 'Scribner', 'English'),
('The 4-Hour Workweek', 'Tim Ferriss', 16.00, '2007-04-24', '978-0-307-46535-2', N'Tự giúp bản thân', N'Làm việc ít, sống nhiều hơn.', 140, 'Crown Publishing', 'English'),
('Tools of Titans', 'Tim Ferriss', 22.00, '2016-12-06', '978-1-328-68378-7', N'Tự giúp bản thân', N'Thói quen của những người xuất chúng.', 90, 'Houghton Mifflin Harcourt', 'English'),
('Extreme Ownership', 'Jocko Willink', 15.00, '2015-10-20', '978-1-250-06705-3', N'Lãnh đạo', N'Trách nhiệm tuyệt đối trong lãnh đạo.', 115, 'St. Martin''s Press', 'English'),
('Leaders Eat Last', 'Simon Sinek', 16.00, '2014-01-07', '978-1-59184-801-2', N'Lãnh đạo', N'Xây dựng đội nhóm gắn kết.', 130, 'Portfolio', 'English'),
('The Hard Thing About Hard Things', 'Ben Horowitz', 19.00, '2014-03-04', '978-0-06-227320-9', N'Kinh doanh', N'Quản lý công ty khởi nghiệp.', 100, 'HarperBusiness', 'English'),
('Zero to One', 'Peter Thiel', 18.00, '2014-09-16', '978-0-7535-5645-2', N'Kinh doanh', N'Tạo giá trị độc nhất trong kinh doanh.', 110, 'Virgin Books', 'English'),
('The Tipping Point', 'Malcolm Gladwell', 15.00, '2000-03-01', '978-0-316-31696-6', N'Kinh doanh', N'Cách ý tưởng lan tỏa trong xã hội.', 130, 'Little, Brown and Company', 'English'),
('Outliers', 'Malcolm Gladwell', 14.00, '2008-11-18', '978-0-316-01792-4', N'Tâm lý học', N'Bí mật thành công của người xuất chúng.', 140, 'Little, Brown and Company', 'English'),
('Blink', 'Malcolm Gladwell', 13.00, '2005-01-11', '978-0-316-17232-6', N'Tâm lý học', N'Sức mạnh của trực giác nhanh chóng.', 150, 'Little, Brown and Company', 'English'),
('David and Goliath', 'Malcolm Gladwell', 16.00, '2013-10-01', '978-0-316-20436-2', N'Tâm lý học', N'Lợi thế của kẻ yếu thế.', 120, 'Little, Brown and Company', 'English'),
('The Power of Vulnerability', 'Brené Brown', 17.00, '2012-06-01', '978-1-59179-857-8', N'Tâm lý học', N'Sức mạnh của sự yếu đuối và đồng cảm.', 110, 'Sounds True', 'English'),
('Daring Greatly', 'Brené Brown', 16.00, '2012-09-11', '978-0-679-64524-2', N'Tự giúp bản thân', N'Can đảm sống với sự tổn thương.', 130, 'Gotham Books', 'English'),
('Rising Strong', 'Brené Brown', 15.00, '2015-08-25', '978-0-8129-8580-2', N'Tự giúp bản thân', N'Khởi dậy mạnh mẽ sau thất bại.', 140, 'Spiegel & Grau', 'English'),
('Braving the Wilderness', 'Brené Brown', 14.50, '2017-09-12', '978-1-5011-7240-2', N'Tự giúp bản thân', N'Tìm sự kết nối trong cô đơn.', 150, 'Random House', 'English'),
('The Gifts of Imperfection', 'Brené Brown', 13.00, '2010-08-27', '978-1-59285-989-5', N'Tự giúp bản thân', N'Chấp nhận bản thân không hoàn hảo.', 160, 'Hazelden Publishing', 'English'),
('Big Magic', 'Elizabeth Gilbert', 15.00, '2015-09-22', '978-1-59463-471-1', N'Tự giúp bản thân', N'Sáng tạo và sống với cảm hứng.', 140, 'Riverhead Books', 'English'),
('Eat Pray Love', 'Elizabeth Gilbert', 14.00, '2006-02-16', '978-0-14-303841-3', N'Tiểu sử', N'Hành trình tìm kiếm bản thân qua Ý, Ấn Độ.', 170, 'Viking', 'English'),
('The Signature of All Things', 'Elizabeth Gilbert', 16.00, '2013-10-01', '978-0-670-02485-9', N'Tiểu thuyết', N'Câu chuyện về nhà thực vật học thế kỷ 19.', 130, 'Viking', 'English'),
('City of Girls', 'Elizabeth Gilbert', 17.00, '2019-06-04', '978-1-59463-473-5', N'Tiểu thuyết', N'Hành trình trưởng thành ở New York.', 120, 'Riverhead Books', 'English'),
('The Last American Man', 'Elizabeth Gilbert', 13.50, '2002-05-07', '978-0-670-03066-1', N'Tiểu sử', N'Câu chuyện về người sống hoang dã.', 140, 'Viking', 'English'),
('Stern Men', 'Elizabeth Gilbert', 12.00, '2000-06-01', '978-0-618-05754-7', N'Tiểu thuyết', N'Cuộc sống của người dân chài ở Maine.', 150, 'Houghton Mifflin Harcourt', 'English'),
('Pilgrim at Tinker Creek', 'Annie Dillard', 14.00, '1974-04-01', '978-0-06-123332-6', N'Khoa học', N'Suy ngẫm về thiên nhiên và cuộc sống.', 130, 'Harper Perennial', 'English'),
('Teaching a Stone to Talk', 'Annie Dillard', 12.50, '1982-01-01', '978-0-06-091541-3', N'Khoa học', N'Những bài luận về thiên nhiên và sự sống.', 140, 'Harper & Row', 'English'),
('For the Time Being', 'Annie Dillard', 13.00, '1999-10-01', '978-0-375-40380-3', N'Khoa học', N'Suy ngẫm về thời gian và sự tồn tại.', 120, 'Knopf', 'English'),
('Holy the Firm', 'Annie Dillard', 11.00, '1977-01-01', '978-0-06-091543-8', N'Khoa học', N'Nhật ký thiêng liêng về thiên nhiên.', 150, 'Harper & Row', 'English'),
('The Writing Life', 'Annie Dillard', 12.00, '1989-10-01', '978-0-06-091988-7', N'Tự giúp bản thân', N'Hành trình sáng tác của một nhà văn.', 130, 'Harper & Row', 'English'),
('The Art of Happiness', 'Dalai Lama', 15.00, '1998-01-01', '978-0-7679-0551-9', N'Tâm lý học', N'Hạnh phúc từ góc nhìn Phật giáo.', 140, 'Riverhead Books', 'English'),
('The Tao of Pooh', 'Benjamin Hoff', 12.00, '1982-10-01', '978-0-14-006747-7', N'Triết học', N'Triết lý qua giáo dục của Winnie the Pooh.', 150, 'Penguin Books', 'English'),
('The 48 Laws of Power', 'Robert Greene', 18.00, '1998-09-01', '978-0-14-028019-7', N'Kinh doanh', N'Chiến lược quyền lực trong xã hội.', 120, 'Penguin Books', 'English'),
('The Innovator''s Dilemma', 'Clayton M. Christensen', 20.00, '1997-09-23', '978-0-06-052199-8', N'Kinh doanh', N'Tại sao các công ty lớn thất bại trong đổi mới.', 130, 'Harvard Business Review Press', 'English');
GO

-- Chèn dữ liệu vào Orders
DECLARE @OrderId1 INT, @OrderId2 INT, @OrderId3 INT, @OrderId4 INT, @OrderId5 INT;

-- Kiểm tra xem bảng Users có đủ dữ liệu không
IF NOT EXISTS (SELECT 1 FROM Users WHERE Id IN (1, 2, 3, 4, 5))
BEGIN
    RAISERROR ('Not enough Users in the database to satisfy foreign key constraint.', 16, 1);
    RETURN;
END

-- Chèn từng đơn hàng
INSERT INTO Orders (UserId, OrderDate, TotalAmount, Status, ShippingAddress, PaymentMethod, Notes) VALUES
(1, GETDATE(), 150.00, 'Completed', N'123 Lê Lợi, Quận 1, TP.HCM', 'Credit Card', N'Giao hàng trong tuần');
SET @OrderId1 = SCOPE_IDENTITY();

INSERT INTO Orders (UserId, OrderDate, TotalAmount, Status, ShippingAddress, PaymentMethod, Notes) VALUES
(2, GETDATE(), 89.50, 'Pending', N'456 Nguyễn Huệ, Quận 1, TP.HCM', 'Cash on Delivery', N'Hẹn giao ngày mai');
SET @OrderId2 = SCOPE_IDENTITY();

INSERT INTO Orders (UserId, OrderDate, TotalAmount, Status, ShippingAddress, PaymentMethod, Notes) VALUES
(3, GETDATE(), 210.00, 'Shipped', N'789 Trần Hưng Đạo, Quận 5, TP.HCM', 'Bank Transfer', N'Đã chuyển khoản');
SET @OrderId3 = SCOPE_IDENTITY();

INSERT INTO Orders (UserId, OrderDate, TotalAmount, Status, ShippingAddress, PaymentMethod, Notes) VALUES
(4, GETDATE(), 175.00, 'Completed', N'321 Lê Văn Sỹ, Quận 3, TP.HCM', 'Credit Card', N'Yêu cầu hóa đơn điện tử');
SET @OrderId4 = SCOPE_IDENTITY();

INSERT INTO Orders (UserId, OrderDate, TotalAmount, Status, ShippingAddress, PaymentMethod, Notes) VALUES
(5, GETDATE(), 120.00, 'Pending', N'654 Trần Quốc Toản, Quận 10, TP.HCM', 'Cash on Delivery', N'Giao hàng vào cuối tuần');
SET @OrderId5 = SCOPE_IDENTITY();

-- Chèn dữ liệu vào OrderDetails
INSERT INTO OrderDetails (OrderId, BookId, Quantity, UnitPrice, Subtotal) VALUES
(@OrderId1, 1, 2, 50.00, 100.00),
(@OrderId1, 2, 1, 50.00, 50.00),
(@OrderId2, 3, 1, 89.50, 89.50),
(@OrderId3, 4, 1, 210.00, 210.00),
(@OrderId4, 5, 1, 120.00, 120.00),
(@OrderId4, 6, 1, 55.00, 55.00),
(@OrderId5, 7, 1, 120.00, 120.00);
GO

-- Kiểm tra dữ liệu
SELECT TOP 10 * FROM Books ORDER BY Id;
SELECT TOP 15 * FROM Users ORDER BY Id;
SELECT TOP 10 * FROM Categories ORDER BY Id;
SELECT TOP 10 * FROM Orders ORDER BY Id;
SELECT TOP 10 * FROM OrderDetails ORDER BY Id;
SELECT TOP 10 * FROM Roles ORDER BY Id;
GO