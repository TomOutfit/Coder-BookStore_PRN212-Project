-- Tạo cơ sở dữ liệu nếu chưa tồn tại
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'BookStoreDB_PRN')
BEGIN
    CREATE DATABASE [BookStoreDB_PRN];
END
GO

USE [BookStoreDB_PRN];
GO

-- Tạo bảng Books
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Books' and xtype='U')
BEGIN
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
END;
GO

-- Tạo bảng Users
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' and xtype='U')
BEGIN
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
END;
GO

-- Tạo bảng Categories
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categories' and xtype='U')
BEGIN
    CREATE TABLE Categories (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) UNIQUE NOT NULL,
        Description NVARCHAR(MAX) NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
    );
END;
GO

-- Tạo bảng Orders
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Orders' and xtype='U')
BEGIN
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
END;
GO

-- Tạo bảng OrderDetails
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='OrderDetails' and xtype='U')
BEGIN
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
END;
GO

-- Tạo bảng Roles (Quản lý vai trò động)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Roles' and xtype='U')
BEGIN
    CREATE TABLE Roles (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(50) UNIQUE NOT NULL,
        Description NVARCHAR(MAX) NULL
    );
END;
GO

-- Xóa trigger cũ nếu tồn tại để tạo lại
IF OBJECT_ID('trg_Books_Update', 'TR') IS NOT NULL DROP TRIGGER trg_Books_Update; 
GO
IF OBJECT_ID('trg_Users_Update', 'TR') IS NOT NULL DROP TRIGGER trg_Users_Update; 
GO
IF OBJECT_ID('trg_Categories_Update', 'TR') IS NOT NULL DROP TRIGGER trg_Categories_Update; 
GO
IF OBJECT_ID('trg_Orders_Update', 'TR') IS NOT NULL DROP TRIGGER trg_Orders_Update; 
GO
IF OBJECT_ID('trg_OrderDetails_Update', 'TR') IS NOT NULL DROP TRIGGER trg_OrderDetails_Update; 
GO

-- Tạo trigger để tự động cập nhật UpdatedAt cho các bảng
CREATE TRIGGER trg_Books_Update ON Books AFTER UPDATE AS BEGIN UPDATE Books SET UpdatedAt = GETDATE() FROM Books INNER JOIN inserted ON Books.Id = inserted.Id; END; 
GO
CREATE TRIGGER trg_Users_Update ON Users AFTER UPDATE AS BEGIN UPDATE Users SET UpdatedAt = GETDATE() FROM Users INNER JOIN inserted ON Users.Id = inserted.Id; END; 
GO
CREATE TRIGGER trg_Categories_Update ON Categories AFTER UPDATE AS BEGIN UPDATE Categories SET UpdatedAt = GETDATE() FROM Categories INNER JOIN inserted ON Categories.Id = inserted.Id; END; 
GO
CREATE TRIGGER trg_Orders_Update ON Orders AFTER UPDATE AS BEGIN UPDATE Orders SET UpdatedAt = GETDATE() FROM Orders INNER JOIN inserted ON Orders.Id = inserted.Id; END;
GO
CREATE TRIGGER trg_OrderDetails_Update ON OrderDetails AFTER UPDATE AS BEGIN UPDATE OrderDetails SET UpdatedAt = GETDATE() FROM OrderDetails INNER JOIN inserted ON OrderDetails.Id = inserted.Id; END;
GO

-- Xóa dữ liệu cũ trước khi chèn dữ liệu mới
DELETE FROM OrderDetails;
DELETE FROM Orders;
DELETE FROM Books;
DELETE FROM Categories;
DELETE FROM Users;
DELETE FROM Roles;
GO
-- Reset Identity cho các bảng
DBCC CHECKIDENT ('OrderDetails', RESEED, 0);
DBCC CHECKIDENT ('Orders', RESEED, 0);
DBCC CHECKIDENT ('Books', RESEED, 0);
DBCC CHECKIDENT ('Categories', RESEED, 0);
DBCC CHECKIDENT ('Users', RESEED, 0);
DBCC CHECKIDENT ('Roles', RESEED, 0);

-- Chèn dữ liệu vào Roles
INSERT INTO Roles (Name, Description) VALUES
('Admin', 'Quản trị viên với quyền truy cập toàn phần'),
('User', 'Người dùng thông thường với quyền hạn cơ bản'),
('Staff', 'Nhân viên với quyền quản lý hạn chế');
GO

-- Chèn dữ liệu vào Users (20 bản ghi thực tế)
INSERT INTO Users (Username, Password, Email, FirstName, LastName, Address, City, State, ZipCode, PhoneNumber, Role, IsActive) VALUES
('admin_hanoi', 'admin123', 'admin.hanoi@bookstore.vn', 'Nguyễn', 'Hải', '12 Đường Thành, Hoàn Kiếm', 'Hà Nội', 'VN', '100000', '0981234567', 'Admin', 1),
('user_hcm', 'user123', 'nguyen.van.a@hcm.vn', 'Nguyễn', 'Văn A', '34 Lê Lợi, Quận 1', 'Hồ Chí Minh', 'VN', '700000', '0912345678', 'User', 1),
('staff_dn', 'staff123', 'le.thi.b@danang.vn', 'Lê', 'Thị B', '56 Nguyễn Văn Linh, Hải Châu', 'Đà Nẵng', 'VN', '550000', '0908765432', 'Staff', 1),
('user_ct', 'user456', 'tran.van.c@cantho.vn', 'Trần', 'Văn C', '78 Trần Phú, Ninh Kiều', 'Cần Thơ', 'VN', '940000', '0934567890', 'User', 1),
('admin_hp', 'admin456', 'pham.thi.d@haiphong.vn', 'Phạm', 'Thị D', '90 Lạch Tray, Ngô Quyền', 'Hải Phòng', 'VN', '180000', '0945678901', 'Admin', 1),
('user_hue', 'user789', 'hoang.van.e@hue.vn', 'Hoàng', 'Văn E', '22 Lê Lợi, TP Huế', 'Huế', 'VN', '530000', '0978901234', 'User', 1),
('staff_nt', 'staff789', 'vo.minh.f@nhatrang.vn', 'Võ', 'Minh F', '45 Trần Phú, Nha Trang', 'Nha Trang', 'VN', '650000', '0965432109', 'Staff', 1),
('user_vt', 'user101', 'bui.thi.g@vungtau.vn', 'Bùi', 'Thị G', '67 Bà Rịa, Vũng Tàu', 'Vũng Tàu', 'VN', '790000', '0956789012', 'User', 1),
('user_pq', 'user112', 'dang.van.h@phuquoc.vn', 'Đặng', 'Văn H', '89 Đường 30/4, Phú Quốc', 'Phú Quốc', 'VN', '910000', '0923456789', 'User', 1),
('staff_qn', 'staff112', 'duong.thi.i@quangninh.vn', 'Đường', 'Thị I', '101 Hạ Long, Hạ Long', 'Quảng Ninh', 'VN', '200000', '0918765432', 'Staff', 1),
('user_la', 'user223', 'truong.van.k@longan.vn', 'Trương', 'Văn K', '23 Nguyễn Huệ, Tân An', 'Long An', 'VN', '850000', '0987654321', 'User', 1),
('user_bn', 'user334', 'nguyen.thi.l@bacninh.vn', 'Nguyễn', 'Thị L', '45 Lý Thái Tổ, Bắc Ninh', 'Bắc Ninh', 'VN', '160000', '0932109876', 'User', 1),
('staff_tn', 'staff334', 'do.minh.m@tayninh.vn', 'Đỗ', 'Minh M', '67 Quốc lộ 22, Tây Ninh', 'Tây Ninh', 'VN', '800000', '0943210987', 'Staff', 1),
('user_dn1', 'user445', 'phan.van.n@dongnai.vn', 'Phan', 'Văn N', '89 Đồng Khởi, Biên Hòa', 'Đồng Nai', 'VN', '760000', '0976543210', 'User', 1),
('user_gl', 'user556', 'tran.thi.o@gialai.vn', 'Trần', 'Thị O', '12 Phan Chu Trinh, Pleiku', 'Gia Lai', 'VN', '600000', '0921098765', 'User', 1),
('staff_dl', 'staff556', 'le.van.p@daklak.vn', 'Lê', 'Văn P', '34 Nguyễn Tất Thành, Buôn Ma Thuột', 'Đắk Lắk', 'VN', '630000', '0910987654', 'Staff', 1),
('user_ld', 'user667', 'vo.thi.q@lamdong.vn', 'Võ', 'Thị Q', '56 Yersin, Đà Lạt', 'Lâm Đồng', 'VN', '660000', '0944321098', 'User', 1),
('user_kh', 'user778', 'huynh.van.r@khanhhoa.vn', 'Huỳnh', 'Văn R', '78 Trần Hưng Đạo, Nha Trang', 'Khánh Hòa', 'VN', '650000', '0938765432', 'User', 1),
('user_bd', 'user889', 'cao.minh.s@binhduong.vn', 'Cao', 'Minh S', '90 Quốc lộ 13, Thủ Dầu Một', 'Bình Dương', 'VN', '750000', '0921987654', 'User', 1),
('staff_brvt', 'staff889', 'nguyen.quang.t@baria.vn', 'Nguyễn', 'Quang T', '12 Lê Hồng Phong, Vũng Tàu', 'Bà Rịa - Vũng Tàu', 'VN', '720000', '0912345670', 'Staff', 1);
GO

-- Chèn dữ liệu vào Categories
INSERT INTO Categories (Name, Description) VALUES
('Lập trình', 'Sách về các ngôn ngữ lập trình và kỹ thuật lập trình.'),
('Khoa học viễn tưởng', 'Sách về các chủ đề khoa học viễn tưởng và tương lai.'),
('Kỳ ảo', 'Sách về thế giới kỳ ảo, phép thuật và huyền bí.'),
('Lịch sử', 'Sách về lịch sử thế giới và các nền văn minh.'),
('Tự giúp bản thân', 'Sách hướng dẫn phát triển bản thân và kỹ năng sống.'),
('Tiểu sử', 'Sách về cuộc đời của các nhân vật nổi tiếng.'),
('Kinh doanh', 'Sách về kinh doanh, marketing và quản lý.'),
('Tâm lý học', 'Sách nghiên cứu về tâm lý con người và hành vi xã hội.'),
('Hồi hộp', 'Sách thể loại kinh dị, hồi hộp và bí ẩn.'),
('Thiếu niên', 'Sách dành cho lứa tuổi thiếu niên với các câu chuyện phiêu lưu.');
GO

-- Chèn dữ liệu vào Books (200 bản ghi thực tế)
INSERT INTO Books (Title, Author, Price, PublishedDate, ISBN, Genre, Description, Stock, Publisher, Language) VALUES
('Lập trình C# cơ bản', 'Nguyễn Văn A', 150.00, '2020-01-15', '978-3-16-148410-0', 'Lập trình', 'Hướng dẫn lập trình C# từ cơ bản đến nâng cao cho người mới.', 100, 'Nhà xuất bản Giáo dục Việt Nam', 'Tiếng Việt'),
('Khoa học viễn tưởng: Hành tinh đỏ', 'Trần Thị B', 200.00, '2019-05-20', '978-1-23-456789-0', 'Khoa học viễn tưởng', 'Cuộc phiêu lưu trên sao Hỏa với công nghệ tương lai.', 50, 'Nhà xuất bản Kim Đồng', 'Tiếng Việt'),
('Kỳ ảo: Phép thuật rừng xanh', 'Lê Văn C', 180.00, '2021-03-10', '978-0-12-345678-9', 'Kỳ ảo', 'Câu chuyện về phép thuật và cuộc sống trong rừng sâu.', 75, 'Nhà xuất bản Trẻ', 'Tiếng Việt'),
('Lịch sử thế giới cổ đại', 'Phạm Thị D', 250.00, '2018-07-25', '978-4-56-789012-3', 'Lịch sử', 'Khám phá các nền văn minh cổ đại từ Ai Cập đến Hy Lạp.', 30, 'Nhà xuất bản Tổng hợp TP.HCM', 'Tiếng Việt'),
('Tự giúp bản thân: Nghệ thuật sống đơn giản', 'Nguyễn Minh E', 120.00, '2022-02-05', '978-5-67-890123-4', 'Tự giúp bản thân', 'Hướng dẫn sống tối giản và hạnh phúc trong đời sống hiện đại.', 200, 'Nhà xuất bản Lao động - Xã hội', 'Tiếng Việt'),
('Tiểu sử Steve Jobs', 'Trần Văn F', 300.00, '2017-11-30', '978-6-78-901234-5', 'Tiểu sử', 'Hành trình sáng tạo của Steve Jobs tại Apple.', 80, 'Nhà xuất bản Phụ nữ Việt Nam', 'Tiếng Việt'),
('Kinh doanh: Khởi nghiệp từ con số 0', 'Lê Thị G', 220.00, '2020-09-15', '978-7-89-012345-6', 'Kinh doanh', 'Hướng dẫn khởi nghiệp và xây dựng doanh nghiệp bền vững.', 60, 'Nhà xuất bản Thế giới', 'Tiếng Việt'),
('Tâm lý học: Hiểu tâm trí con người', 'Phạm Văn H', 140.00, '2019-04-20', '978-8-90-123456-7', 'Tâm lý học', 'Nghiên cứu sâu về tâm trí và hành vi xã hội.', 90, 'Nhà xuất bản Đại học Quốc gia Hà Nội', 'Tiếng Việt'),
('Hồi hộp: Ngôi nhà ma ám', 'Nguyễn Thị I', 160.00, '2021-06-10', '978-9-01-234567-8', 'Hồi hộp', 'Truyện kinh dị về bí ẩn trong ngôi nhà cổ.', 40, 'Nhà xuất bản Văn học', 'Tiếng Việt'),
('Thiếu niên: Phiêu lưu nhóm bạn trẻ', 'Trần Minh J', 130.00, '2022-08-05', '978-0-12-345678-0', 'Thiếu niên', 'Hành trình phiêu lưu đầy thú vị của nhóm bạn trẻ.', 150, 'Nhà xuất bản Kim Đồng', 'Tiếng Việt'),
('Lập trình Python cơ bản', 'Nguyễn Văn K', 170.00, '2020-01-15', '978-3-16-148410-1', 'Lập trình', 'Học Python từ cơ bản đến lập trình thực tế.', 120, 'Nhà xuất bản Giáo dục Việt Nam', 'Tiếng Việt'),
('Khoa học viễn tưởng: Cuộc chiến sao', 'Trần Thị L', 210.00, '2019-05-20', '978-1-23-456789-1', 'Khoa học viễn tưởng', 'Trận chiến giữa các vì sao trong vũ trụ.', 55, 'Nhà xuất bản Kim Đồng', 'Tiếng Việt'),
('Kỳ ảo: Vương quốc phép thuật', 'Lê Văn M', 190.00, '2021-03-10', '978-0-12-345679-0', 'Kỳ ảo', 'Hành trình khám phá vương quốc phép thuật huyền bí.', 80, 'Nhà xuất bản Trẻ', 'Tiếng Việt'),
('Lịch sử Việt Nam', 'Phạm Thị N', 260.00, '2018-07-25', '978-4-56-789012-4', 'Lịch sử', 'Lịch sử Việt Nam qua các triều đại.', 35, 'Nhà xuất bản Tổng hợp TP.HCM', 'Tiếng Việt'),
('Tự giúp bản thân: Bí quyết thành công', 'Nguyễn Minh O', 130.00, '2022-02-05', '978-5-67-890123-5', 'Tự giúp bản thân', 'Bí quyết đạt thành công trong cuộc sống hàng ngày.', 210, 'Nhà xuất bản Lao động - Xã hội', 'Tiếng Việt'),
('Tiểu sử Bill Gates', 'Trần Văn P', 310.00, '2017-11-30', '978-6-78-901234-6', 'Tiểu sử', 'Hành trình sáng lập Microsoft của Bill Gates.', 85, 'Nhà xuất bản Phụ nữ Việt Nam', 'Tiếng Việt'),
('Kinh doanh: Chiến lược marketing', 'Lê Thị Q', 230.00, '2020-09-15', '978-7-89-012345-7', 'Kinh doanh', 'Chiến lược marketing để phát triển thương hiệu.', 65, 'Nhà xuất bản Thế giới', 'Tiếng Việt'),
('Tâm lý học: Hành vi con người', 'Phạm Văn R', 150.00, '2019-04-20', '978-8-90-123456-8', 'Tâm lý học', 'Nghiên cứu hành vi con người trong xã hội hiện đại.', 95, 'Nhà xuất bản Đại học Quốc gia Hà Nội', 'Tiếng Việt'),
('Hồi hộp: Rừng ma ám', 'Nguyễn Thị S', 170.00, '2021-06-10', '978-9-01-234567-9', 'Hồi hộp', 'Bí ẩn kinh dị trong khu rừng ma ám.', 45, 'Nhà xuất bản Văn học', 'Tiếng Việt'),
('Thiếu niên: Nhóm bạn siêu nhân', 'Trần Minh T', 140.00, '2022-08-05', '978-0-12-345679-1', 'Thiếu niên', 'Phiêu lưu của nhóm bạn trẻ với siêu năng lực.', 160, 'Nhà xuất bản Kim Đồng', 'Tiếng Việt'),
('Lập trình Java cơ bản', 'Nguyễn Văn U', 160.00, '2020-03-10', '978-3-16-148410-2', 'Lập trình', 'Học Java từ cơ bản đến ứng dụng thực tế.', 110, 'Nhà xuất bản Giáo dục Việt Nam', 'Tiếng Việt'),
('Khoa học viễn tưởng: Sao Thủy', 'Trần Thị V', 190.00, '2019-06-15', '978-1-23-456789-2', 'Khoa học viễn tưởng', 'Khám phá hành tinh Sao Thủy trong tương lai.', 60, 'Nhà xuất bản Kim Đồng', 'Tiếng Việt'),
('Kỳ ảo: Hòn đảo thần bí', 'Lê Văn X', 170.00, '2021-04-20', '978-0-12-345679-2', 'Kỳ ảo', 'Cuộc phiêu lưu trên hòn đảo đầy phép thuật.', 70, 'Nhà xuất bản Trẻ', 'Tiếng Việt'),
('Lịch sử Trung Quốc', 'Phạm Thị Y', 240.00, '2018-08-10', '978-4-56-789012-5', 'Lịch sử', 'Lịch sử phát triển của Trung Quốc qua các triều đại.', 40, 'Nhà xuất bản Tổng hợp TP.HCM', 'Tiếng Việt'),
('Tự giúp bản thân: Quản lý thời gian', 'Nguyễn Minh Z', 110.00, '2022-03-15', '978-5-67-890123-6', 'Tự giúp bản thân', 'Hướng dẫn quản lý thời gian hiệu quả.', 180, 'Nhà xuất bản Lao động - Xã hội', 'Tiếng Việt'),
('Tiểu sử Elon Musk', 'Trần Văn AA', 290.00, '2017-12-05', '978-6-78-901234-7', 'Tiểu sử', 'Hành trình sáng tạo của Elon Musk tại Tesla và SpaceX.', 90, 'Nhà xuất bản Phụ nữ Việt Nam', 'Tiếng Việt'),
('Kinh doanh: Quản lý tài chính', 'Lê Thị BB', 210.00, '2020-10-20', '978-7-89-012345-8', 'Kinh doanh', 'Hướng dẫn quản lý tài chính cho doanh nghiệp nhỏ.', 70, 'Nhà xuất bản Thế giới', 'Tiếng Việt'),
('Tâm lý học: Tình yêu và mối quan hệ', 'Phạm Văn CC', 130.00, '2019-05-25', '978-8-90-123456-9', 'Tâm lý học', 'Nghiên cứu về tình yêu và các mối quan hệ xã hội.', 100, 'Nhà xuất bản Đại học Quốc gia Hà Nội', 'Tiếng Việt'),
('Hồi hộp: Lâu đài ma quỷ', 'Nguyễn Thị DD', 150.00, '2021-07-15', '978-9-01-234568-0', 'Hồi hộp', 'Bí ẩn kinh dị trong lâu đài cổ kính.', 50, 'Nhà xuất bản Văn học', 'Tiếng Việt'),
('Thiếu niên: Bí mật rừng sâu', 'Trần Minh EE', 120.00, '2022-09-10', '978-0-12-345679-3', 'Thiếu niên', 'Cuộc phiêu lưu khám phá rừng sâu của nhóm bạn trẻ.', 140, 'Nhà xuất bản Kim Đồng', 'Tiếng Việt'),
('Lập trình C++ nâng cao', 'Nguyễn Văn FF', 180.00, '2020-04-20', '978-3-16-148410-3', 'Lập trình', 'Học C++ nâng cao với các dự án thực tế.', 115, 'Nhà xuất bản Giáo dục Việt Nam', 'Tiếng Việt'),
('Khoa học viễn tưởng: Hành tinh X', 'Trần Thị GG', 200.00, '2019-07-10', '978-1-23-456789-3', 'Khoa học viễn tưởng', 'Khám phá hành tinh X với công nghệ tiên tiến.', 65, 'Nhà xuất bản Kim Đồng', 'Tiếng Việt'),
('Kỳ ảo: Thành phố ánh sáng', 'Lê Văn HH', 175.00, '2021-05-15', '978-0-12-345679-4', 'Kỳ ảo', 'Hành trình tại thành phố ánh sáng huyền bí.', 85, 'Nhà xuất bản Trẻ', 'Tiếng Việt'),
('Lịch sử châu Âu', 'Phạm Thị II', 230.00, '2018-09-05', '978-4-56-789012-6', 'Lịch sử', 'Lịch sử phát triển của châu Âu qua các thời kỳ.', 45, 'Nhà xuất bản Tổng hợp TP.HCM', 'Tiếng Việt'),
('Tự giúp bản thân: Sức khỏe tinh thần', 'Nguyễn Minh JJ', 115.00, '2022-04-10', '978-5-67-890123-7', 'Tự giúp bản thân', 'Hướng dẫn chăm sóc sức khỏe tinh thần.', 190, 'Nhà xuất bản Lao động - Xã hội', 'Tiếng Việt'),
('Tiểu sử Nelson Mandela', 'Trần Văn KK', 280.00, '2017-12-20', '978-6-78-901234-8', 'Tiểu sử', 'Hành trình đấu tranh tự do của Nelson Mandela.', 95, 'Nhà xuất bản Phụ nữ Việt Nam', 'Tiếng Việt'),
('Kinh doanh: Thương mại điện tử', 'Lê Thị LL', 200.00, '2020-11-15', '978-7-89-012345-9', 'Kinh doanh', 'Hướng dẫn phát triển kinh doanh online hiệu quả.', 75, 'Nhà xuất bản Thế giới', 'Tiếng Việt'),
('Tâm lý học: Trí nhớ và học tập', 'Phạm Văn MM', 140.00, '2019-06-20', '978-8-90-123457-0', 'Tâm lý học', 'Nghiên cứu về trí nhớ và phương pháp học tập.', 105, 'Nhà xuất bản Đại học Quốc gia Hà Nội', 'Tiếng Việt'),
('Hồi hộp: Hầm mộ cổ', 'Nguyễn Thị NN', 155.00, '2021-08-10', '978-9-01-234568-1', 'Hồi hộp', 'Bí ẩn kinh dị trong hầm mộ cổ xưa.', 55, 'Nhà xuất bản Văn học', 'Tiếng Việt'),
('Thiếu niên: Cuộc đua kỳ thú', 'Trần Minh OO', 125.00, '2022-10-05', '978-0-12-345679-5', 'Thiếu niên', 'Hành trình đua xe kỳ thú của nhóm bạn trẻ.', 145, 'Nhà xuất bản Kim Đồng', 'Tiếng Việt'),
('Lập trình JavaScript', 'Nguyễn Văn PP', 165.00, '2020-05-25', '978-3-16-148410-4', 'Lập trình', 'Học JavaScript với các dự án web thực tế.', 110, 'Nhà xuất bản Giáo dục Việt Nam', 'Tiếng Việt'),
('Khoa học viễn tưởng: Sao Mộc', 'Trần Thị QQ', 195.00, '2019-08-15', '978-1-23-456789-4', 'Khoa học viễn tưởng', 'Khám phá sao Mộc với công nghệ vũ trụ.', 70, 'Nhà xuất bản Kim Đồng', 'Tiếng Việt'),
('Kỳ ảo: Đỉnh núi thần thánh', 'Lê Văn RR', 180.00, '2021-06-20', '978-0-12-345679-6', 'Kỳ ảo', 'Hành trình lên đỉnh núi thần thánh huyền bí.', 90, 'Nhà xuất bản Trẻ', 'Tiếng Việt'),
('Lịch sử Nhật Bản', 'Phạm Thị SS', 245.00, '2018-10-10', '978-4-56-789012-7', 'Lịch sử', 'Lịch sử phát triển của Nhật Bản qua các thời kỳ.', 50, 'Nhà xuất bản Tổng hợp TP.HCM', 'Tiếng Việt'),
('Tự giúp bản thân: Thiền định', 'Nguyễn Minh TT', 110.00, '2022-05-15', '978-5-67-890123-8', 'Tự giúp bản thân', 'Hướng dẫn thiền định để giảm căng thẳng.', 185, 'Nhà xuất bản Lao động - Xã hội', 'Tiếng Việt'),
('Tiểu sử Marie Curie', 'Trần Văn UU', 275.00, '2017-12-25', '978-6-78-901234-9', 'Tiểu sử', 'Hành trình nghiên cứu khoa học của Marie Curie.', 100, 'Nhà xuất bản Phụ nữ Việt Nam', 'Tiếng Việt'),
('Kinh doanh: Đầu tư chứng khoán', 'Lê Thị VV', 205.00, '2020-12-10', '978-7-89-012346-0', 'Kinh doanh', 'Hướng dẫn đầu tư chứng khoán hiệu quả.', 80, 'Nhà xuất bản Thế giới', 'Tiếng Việt'),
('Tâm lý học: Giấc mơ và ý nghĩa', 'Phạm Văn WW', 135.00, '2019-07-25', '978-8-90-123457-1', 'Tâm lý học', 'Nghiên cứu giấc mơ và ý nghĩa tâm lý.', 110, 'Nhà xuất bản Đại học Quốc gia Hà Nội', 'Tiếng Việt'),
('Hồi hộp: Con tàu ma', 'Nguyễn Thị XX', 160.00, '2021-09-15', '978-9-01-234568-2', 'Hồi hộp', 'Bí ẩn kinh dị trên con tàu ma trôi dạt.', 60, 'Nhà xuất bản Văn học', 'Tiếng Việt'),
('Thiếu niên: Hành trình vũ trụ', 'Trần Minh YY', 130.00, '2022-11-10', '978-0-12-345679-7', 'Thiếu niên', 'Phiêu lưu vũ trụ của nhóm bạn trẻ.', 150, 'Nhà xuất bản Kim Đồng', 'Tiếng Việt'),
('Lập trình PHP cơ bản', 'Nguyễn Văn ZZ', 170.00, '2020-06-20', '978-3-16-148410-5', 'Lập trình', 'Học PHP với các dự án web động.', 115, 'Nhà xuất bản Giáo dục Việt Nam', 'Tiếng Việt'),
('The Lord of the Rings', 'J.R.R. Tolkien', 25.99, '1954-07-29', '978-0-261-10325-2', 'Kỳ ảo', 'Hành trình ép buộc ở Trung Địa.', 100, 'HarperCollins', 'English'),
('1984', 'George Orwell', 9.99, '1949-06-08', '978-0-452-28423-4', 'Khoa học viễn tưởng', 'Tiểu thuyết về xã hội độc tài.', 120, 'Secker & Warburg', 'English'),
('To Kill a Mockingbird', 'Harper Lee', 10.75, '1960-07-11', '978-0-06-112008-4', 'Tiểu thuyết', 'Câu chuyện về công lý và bất công chủng tộc.', 110, 'J.B. Lippincott & Co.', 'English'),
('The Great Gatsby', 'F. Scott Fitzgerald', 8.99, '1925-04-10', '978-0-7432-7356-5', 'Tiểu thuyết', 'Hình ảnh thời kỳ Jazz ở Mỹ.', 130, 'Scribner', 'English'),
('Sapiens', 'Yuval Noah Harari', 18.00, '2014-09-04', '978-0-7710-3850-1', 'Lịch sử', 'Lịch sử loài người từ thời cổ đại.', 150, 'Harvill Secker', 'English'),
('Atomic Habits', 'James Clear', 15.25, '2018-10-16', '978-0-7352-1129-3', 'Tự giúp bản thân', 'Xây dựng thói quen tốt.', 140, 'Avery', 'English'),
('The Hobbit', 'J.R.R. Tolkien', 14.50, '1937-09-21', '978-0-261-10357-4', 'Kỳ ảo', 'Cuộc phiêu lưu của Bilbo Baggins.', 90, 'George Allen & Unwin', 'English'),
('Pride and Prejudice', 'Jane Austen', 12.50, '1813-01-28', '978-0-14-143951-9', 'Tiểu thuyết', 'Tác phẩm kinh điển về tình yêu.', 75, 'Penguin Classics', 'English'),
('Dune', 'Frank Herbert', 16.00, '1965-08-01', '978-0-441-00590-2', 'Khoa học viễn tưởng', 'Câu chuyện về quyền lực và sa mạc.', 95, 'Chilton Books', 'English'),
('The Alchemist', 'Paulo Coelho', 10.00, '1988-01-01', '978-0-06-112241-6', 'Tiểu thuyết', 'Hành trình tìm kiếm giấc mơ của Santiago.', 180, 'HarperOne', 'English'),
('Clean Code', 'Robert C. Martin', 30.00, '2008-08-11', '978-0-13-235088-5', 'Lập trình', 'Hướng dẫn viết mã sạch.', 70, 'Prentice Hall', 'English'),
('The Pragmatic Programmer', 'Andrew Hunt', 28.00, '1999-10-20', '978-0-201-61622-5', 'Lập trình', 'Hành trình thành bậc thầy lập trình.', 65, 'Addison-Wesley', 'English'),
('Design Patterns', 'Erich Gamma', 45.00, '1994-10-21', '978-0-201-63361-1', 'Lập trình', 'Các mẫu thiết kế hướng đối tượng.', 50, 'Addison-Wesley', 'English'),
('Thinking, Fast and Slow', 'Daniel Kahneman', 21.00, '2011-10-25', '978-0-14-103357-1', 'Tâm lý học', 'Hai hệ thống tư duy con người.', 115, 'Farrar, Straus and Giroux', 'English'),
('The Intelligent Investor', 'Benjamin Graham', 24.00, '1949-01-01', '978-0-06-055566-6', 'Kinh doanh', 'Kinh điển về đầu tư giá trị.', 100, 'HarperBusiness', 'English'),
('Rich Dad Poor Dad', 'Robert T. Kiyosaki', 14.50, '1997-04-01', '978-0-7407-0332-1', 'Kinh doanh', 'Giáo dục tài chính giữa giàu và nghèo.', 180, 'Warner Books', 'English'),
('The 7 Habits of Highly Effective People', 'Stephen R. Covey', 14.00, '1989-08-15', '978-0-671-70863-5', 'Tự giúp bản thân', 'Thói quen của người thành công.', 155, 'Free Press', 'English'),
('Becoming', 'Michelle Obama', 19.00, '2018-11-13', '978-0-525-63576-2', 'Tiểu sử', 'Hồi ký của cựu Đệ nhất phu nhân Mỹ.', 145, 'Crown', 'English'),
('Educated', 'Tara Westover', 16.50, '2018-02-20', '978-0-399-59050-5', 'Tiểu sử', 'Hành trình tự học từ gia đình biệt lập.', 130, 'Random House', 'English'),
('The Nightingale', 'Kristin Hannah', 15.50, '2015-03-03', '978-0-312-57085-4', 'Tiểu thuyết lịch sử', 'Chuyện hai chị em trong Thế chiến II.', 160, 'St. Martin''s Press', 'English'),
('Where the Crawdads Sing', 'Delia Owens', 14.99, '2018-08-14', '978-0-7352-1909-1', 'Tiểu thuyết', 'Câu chuyện về cô gái sống trong đầm lầy.', 170, 'G.P. Putnam''s Sons', 'English'),
('The Subtle Art of Not Giving a F*ck', 'Mark Manson', 12.00, '2016-09-13', '978-0-06-245771-5', 'Tự giúp bản thân', 'Sống không quan tâm đến điều không quan trọng.', 170, 'HarperOne', 'English'),
('Atomic Habits', 'James Clear', 15.25, '2018-10-16', '978-0-7352-1129-4', 'Tự giúp bản thân', 'Xây dựng thói quen tốt và loại bỏ thói xấu.', 140, 'Avery', 'English'),
('The Power of Now', 'Eckhart Tolle', 13.50, '1997-08-19', '978-0-931442-72-1', 'Tự giúp bản thân', 'Sống trong hiện tại để tìm hạnh phúc.', 150, 'New World Library', 'English'),
('Man''s Search for Meaning', 'Viktor E. Frankl', 11.00, '1946-10-01', '978-0-8070-1427-2', 'Tâm lý học', 'Tìm ý nghĩa cuộc sống qua trải nghiệm Holocaust.', 160, 'Beacon Press', 'English'),
('Meditations', 'Marcus Aurelius', 8.00, '0180-01-01', '978-0-14-044933-5', 'Triết học', 'Suy ngẫm của Hoàng đế La Mã.', 170, 'Penguin Classics', 'English'),
('The Art of War', 'Sun Tzu', 8.00, '0500-01-01', '978-0-486-42957-4', 'Chiến lược', 'Tác phẩm kinh điển về chiến thuật quân sự.', 160, 'Dover Publications', 'English'),
('Good to Great', 'Jim Collins', 22.00, '2001-10-16', '978-0-06-662099-3', 'Kinh doanh', 'Yếu tố tạo nên công ty xuất sắc.', 110, 'HarperBusiness', 'English'),
('Start with Why', 'Simon Sinek', 15.00, '2009-10-29', '978-0-14-104698-2', 'Kinh doanh', 'Lý do dẫn dắt hành động thành công.', 140, 'Portfolio', 'English'),
('Dare to Lead', 'Brené Brown', 17.00, '2018-10-09', '978-0-399-59252-3', 'Lãnh đạo', 'Lãnh đạo với lòng dũng cảm và sự đồng cảm.', 120, 'Random House', 'English'),
('The Design of Everyday Things', 'Don Norman', 21.00, '2013-11-05', '978-0-465-05065-0', 'Thiết kế', 'Thiết kế thân thiện với người dùng.', 100, 'Basic Books', 'English'),
('Thinking in Systems', 'Donella H. Meadows', 20.00, '2008-11-26', '978-1-60358-055-8', 'Khoa học', 'Hướng dẫn tư duy hệ thống.', 95, 'Chelsea Green Publishing', 'English'),
('The Phoenix Project', 'Gene Kim', 28.00, '2013-01-10', '978-0-9882625-9-2', 'DevOps', 'Tiểu thuyết về quản lý IT và DevOps.', 90, 'IT Revolution Press', 'English'),
('Site Reliability Engineering', 'Betsy Beyer', 45.00, '2016-03-23', '978-1-49192-912-5', 'DevOps', 'Cách Google vận hành hệ thống.', 60, 'O''Reilly Media', 'English'),
('Clean Architecture', 'Robert C. Martin', 28.00, '2017-09-20', '978-0-13-449416-7', 'Lập trình', 'Hướng dẫn kiến trúc phần mềm bền vững.', 90, 'Prentice Hall', 'English'),
('Refactoring', 'Martin Fowler', 30.00, '1999-07-08', '978-0-201-48567-8', 'Lập trình', 'Cải tiến mã nguồn hiện có.', 90, 'Addison-Wesley', 'English'),
('The Lean Startup', 'Eric Ries', 20.00, '2011-09-13', '978-0-307-88789-5', 'Kinh doanh', 'Phương pháp khởi nghiệp tinh gọn.', 130, 'Crown Business', 'English'),
('Deep Work', 'Cal Newport', 16.00, '2016-01-05', '978-0-451-49799-2', 'Tự giúp bản thân', 'Tập trung sâu để đạt hiệu quả cao.', 140, 'Grand Central Publishing', 'English'),
('Educated', 'Tara Westover', 16.50, '2018-02-20', '978-0-399-59050-6', 'Tiểu sử', 'Hành trình tự học từ gia đình biệt lập.', 130, 'Random House', 'English'),
('The Body Keeps the Score', 'Bessel van der Kolk', 18.00, '2014-09-25', '978-0-670-78591-4', 'Tâm lý học', 'Trauma và cách chữa lành.', 110, 'Viking', 'English'),
('The Power of Habit', 'Charles Duhigg', 14.00, '2012-02-28', '978-0-8129-8385-3', 'Tự giúp bản thân', 'Khoa học về thói quen và thay đổi.', 150, 'Random House', 'English'),
('The Four Agreements', 'Don Miguel Ruiz', 12.00, '1997-11-07', '978-1-878424-31-1', 'Tự giúp bản thân', 'Bốn nguyên tắc sống hạnh phúc.', 160, 'Amber-Allen Publishing', 'English'),
('Man''s Search for Meaning', 'Viktor E. Frankl', 11.00, '1946-10-01', '978-0-8070-1427-3', 'Tâm lý học', 'Ý nghĩa cuộc sống qua trải nghiệm Holocaust.', 160, 'Beacon Press', 'English'),
('The War of Art', 'Steven Pressfield', 13.00, '2002-06-01', '978-0-446-69143-8', 'Tự giúp bản thân', 'Vượt qua nỗi sợ để sáng tạo.', 140, 'Warner Books', 'English'),
('Atomic Habits', 'James Clear', 15.25, '2018-10-16', '978-0-7352-1129-5', 'Tự giúp bản thân', 'Xây dựng thói quen tốt và loại bỏ thói xấu.', 140, 'Avery', 'English'),
('The 5 Love Languages', 'Gary Chapman', 12.50, '1992-01-01', '978-0-8024-7315-9', 'Tự giúp bản thân', 'Ngôn ngữ yêu thương trong các mối quan hệ.', 170, 'Northfield Publishing', 'English'),
('The Obstacle Is the Way', 'Ryan Holiday', 14.00, '2014-05-01', '978-0-7181-7992-2', 'Tự giúp bản thân', 'Biến trở ngại thành cơ hội thành công.', 150, 'Portfolio', 'English'),
('Can''t Hurt Me', 'David Goggins', 16.00, '2018-12-04', '978-1-5445-1228-3', 'Tự giúp bản thân', 'Vượt qua giới hạn bản thân.', 120, 'Lioncrest Publishing', 'English'),
('The Millionaire Next Door', 'Thomas J. Stanley', 15.00, '1996-10-01', '978-0-671-01520-7', 'Kinh doanh', 'Bí mật tài chính của người giàu.', 130, 'Longstreet Press', 'English'),
('The E-Myth Revisited', 'Michael E. Gerber', 17.00, '1995-03-01', '978-0-88730-728-8', 'Kinh doanh', 'Xây dựng doanh nghiệp thành công.', 110, 'HarperBusiness', 'English'),
('Crucial Conversations', 'Patterson, Grenny', 18.00, '2002-09-01', '978-0-07-140194-5', 'Tự giúp bản thân', 'Kỹ năng giao tiếp trong tình huống quan trọng.', 140, 'McGraw-Hill', 'English'),
('Influence', 'Robert B. Cialdini', 16.50, '1984-01-01', '978-0-06-124189-6', 'Tâm lý học', 'Nghệ thuật thuyết phục hiệu quả.', 125, 'Harper Business', 'English'),
('Grit', 'Angela Duckworth', 15.00, '2016-05-03', '978-1-5011-1111-1', 'Tâm lý học', 'Sức mạnh của đam mê và kiên trì.', 140, 'Scribner', 'English'),
('The 4-Hour Workweek', 'Tim Ferriss', 16.00, '2007-04-24', '978-0-307-46535-2', 'Tự giúp bản thân', 'Làm việc ít, sống nhiều hơn.', 140, 'Crown Publishing', 'English'),
('Tools of Titans', 'Tim Ferriss', 22.00, '2016-12-06', '978-1-328-68378-7', 'Tự giúp bản thân', 'Thói quen của những người xuất chúng.', 90, 'Houghton Mifflin Harcourt', 'English'),
('Extreme Ownership', 'Jocko Willink', 15.00, '2015-10-20', '978-1-250-06705-3', 'Lãnh đạo', 'Trách nhiệm tuyệt đối trong lãnh đạo.', 115, 'St. Martin''s Press', 'English'),
('Leaders Eat Last', 'Simon Sinek', 16.00, '2014-01-07', '978-1-59184-801-2', 'Lãnh đạo', 'Xây dựng đội nhóm gắn kết.', 130, 'Portfolio', 'English'),
('The Hard Thing About Hard Things', 'Ben Horowitz', 19.00, '2014-03-04', '978-0-06-227320-9', 'Kinh doanh', 'Thực tế về quản lý công ty khởi nghiệp.', 100, 'HarperBusiness', 'English'),
('Zero to One', 'Peter Thiel', 18.00, '2014-09-16', '978-0-7535-5645-2', 'Kinh doanh', 'Tạo ra giá trị độc nhất trong kinh doanh.', 110, 'Virgin Books', 'English'),
('The Tipping Point', 'Malcolm Gladwell', 15.00, '2000-03-01', '978-0-316-31696-6', 'Kinh doanh', 'Cách ý tưởng lan tỏa trong xã hội.', 130, 'Little, Brown and Company', 'English'),
('Outliers', 'Malcolm Gladwell', 14.00, '2008-11-18', '978-0-316-01792-4', 'Tâm lý học', 'Bí mật thành công của những người xuất chúng.', 140, 'Little, Brown and Company', 'English'),
('Blink', 'Malcolm Gladwell', 13.00, '2005-01-11', '978-0-316-17232-6', 'Tâm lý học', 'Sức mạnh của trực giác nhanh chóng.', 150, 'Little, Brown and Company', 'English'),
('David and Goliath', 'Malcolm Gladwell', 16.00, '2013-10-01', '978-0-316-20436-2', 'Tâm lý học', 'Lợi thế của kẻ yếu thế.', 120, 'Little, Brown and Company', 'English'),
('The Power of Vulnerability', 'Brené Brown', 17.00, '2012-06-01', '978-1-59179-857-8', 'Tâm lý học', 'Sức mạnh của sự yếu đuối và đồng cảm.', 110, 'Sounds True', 'English'),
('Daring Greatly', 'Brené Brown', 16.00, '2012-09-11', '978-0-679-64524-2', 'Tự giúp bản thân', 'Can đảm sống với sự tổn thương.', 130, 'Gotham Books', 'English'),
('Rising Strong', 'Brené Brown', 15.00, '2015-08-25', '978-0-8129-8580-2', 'Tự giúp bản thân', 'Khởi dậy mạnh mẽ sau thất bại.', 140, 'Spiegel & Grau', 'English'),
('Braving the Wilderness', 'Brené Brown', 14.50, '2017-09-12', '978-1-5011-7240-2', 'Tự giúp bản thân', 'Tìm sự kết nối trong cô đơn.', 150, 'Random House', 'English'),
('The Gifts of Imperfection', 'Brené Brown', 13.00, '2010-08-27', '978-1-59285-989-5', 'Tự giúp bản thân', 'Chấp nhận bản thân không hoàn hảo.', 160, 'Hazelden Publishing', 'English'),
('Big Magic', 'Elizabeth Gilbert', 15.00, '2015-09-22', '978-1-59463-471-1', 'Tự giúp bản thân', 'Sáng tạo và sống với cảm hứng.', 140, 'Riverhead Books', 'English'),
('Eat Pray Love', 'Elizabeth Gilbert', 14.00, '2006-02-16', '978-0-14-303841-3', 'Tiểu sử', 'Hành trình tìm kiếm bản thân qua Ý, Ấn Độ và Indonesia.', 170, 'Viking', 'English'),
('The Signature of All Things', 'Elizabeth Gilbert', 16.00, '2013-10-01', '978-0-670-02485-9', 'Tiểu thuyết', 'Câu chuyện về một nhà thực vật học thế kỷ 19.', 130, 'Viking', 'English'),
('City of Girls', 'Elizabeth Gilbert', 17.00, '2019-06-04', '978-1-59463-473-5', 'Tiểu thuyết', 'Hành trình trưởng thành của một phụ nữ ở New York.', 120, 'Riverhead Books', 'English'),
('The Last American Man', 'Elizabeth Gilbert', 13.50, '2002-05-07', '978-0-670-03066-1', 'Tiểu sử', 'Câu chuyện về một người đàn ông sống hoang dã.', 140, 'Viking', 'English'),
('Stern Men', 'Elizabeth Gilbert', 12.00, '2000-06-01', '978-0-618-05754-7', 'Tiểu thuyết', 'Cuộc sống của người dân chài ở Maine.', 150, 'Houghton Mifflin Harcourt', 'English'),
('Pilgrim at Tinker Creek', 'Annie Dillard', 14.00, '1974-04-01', '978-0-06-123332-1', 'Khoa học', 'Suy ngẫm về thiên nhiên và cuộc sống.', 130, 'Harper Perennial Modern Classics', 'English'),
('Teaching a Stone to Talk', 'Annie Dillard', 12.50, '1982-01-01', '978-0-06-091540-5', 'Khoa học', 'Những bài luận về thiên nhiên và sự sống.', 140, 'Harper & Row', 'English'),
('For the Time Being', 'Annie Dillard', 13.00, '1999-10-01', '978-0-375-40380-2', 'Khoa học', 'Suy ngẫm về thời gian và sự tồn tại.', 120, 'Knopf', 'English'),
('Holy the Firm', 'Annie Dillard', 11.00, '1977-01-01', '978-0-06-091543-6', 'Khoa học', 'Nhật ký thiêng liêng về thiên nhiên.', 150, 'Harper & Row', 'English'),
('The Writing Life', 'Annie Dillard', 12.00, '1989-10-01', '978-0-06-091988-5', 'Tự giúp bản thân', 'Hành trình sáng tác và cuộc sống của một nhà văn.', 130, 'Harper & Row', 'English')
('The Art of Happiness', 'Dalai Lama', 15.00, '1998-01-01', '978-0-7679-0551-0', 'Tâm lý học', 'Hạnh phúc từ góc nhìn Phật giáo.', 140, 'Riverhead Books', 'English'),
('The Tao of Pooh', 'Benjamin Hoff', 12.00, '1982-10-01', '978-0-14-006747-7', 'Triết học', 'Philosophy through the lens of Winnie the Pooh.', 150, 'Penguin Books', 'English'),
('The Power of Now', 'Eckhart Tolle', 13.50, '1997-08-19', '978-0-931442-72-1', 'Tự giúp bản thân', 'Sống trong hiện tại để tìm hạnh phúc.', 150, 'New World Library', 'English');
('The Subtle Art of Not Giving a F*ck', 'Mark Manson', 12.00, '2016-09-13', '978-0-06-245771-5', 'Tự giúp bản thân', 'Sống không quan tâm đến điều không quan trọng.', 170, 'HarperOne', 'English');
('The 7 Habits of Highly Effective People', 'Stephen R. Covey', 14.00, '1989-08-15', '978-0-671-70863-5', 'Tự giúp bản thân', 'Thói quen của người thành công.', 155, 'Free Press', 'English');
('The 48 Laws of Power', 'Robert Greene', 18.00, '1998-09-01', '978-0-14-028019-7', 'Kinh doanh', 'Chiến lược quyền lực trong xã hội.', 120, 'Penguin Books', 'English'),
('The Art of War', 'Sun Tzu', 8.00, '0500-01-01', '978-0-486-42957-4', 'Chiến lược', 'Tác phẩm kinh điển về chiến thuật quân sự.', 160, 'Dover Publications', 'English');
('The Lean Startup', 'Eric Ries', 20.00, '2011-09-13', '978-0-307-88789-5', 'Kinh doanh', 'Phương pháp khởi nghiệp tinh gọn.', 130, 'Crown Business', 'English'),
('Good to Great', 'Jim Collins', 22.00, '2001-10-16', '978-0-06-662099-3', 'Kinh doanh', 'Yếu tố tạo nên công ty xuất sắc.', 110, 'HarperBusiness', 'English');
('The Innovator''s Dilemma', 'Clayton M. Christensen', 20.00, '1997-09-23', '978-0-06-662059-7', 'Kinh doanh', 'Tại sao các công ty lớn thất bại trong đổi mới.', 130, 'Harvard Business Review Press', 'English'),
('The Hard Thing About Hard Things', 'Ben Horowitz', 19.00, '2014-03-04', '978-0-06-227320-9', 'Kinh doanh', 'Thực tế về quản lý công ty khởi nghiệp.', 100, 'HarperBusiness', 'English'),
('Zero to One', 'Peter Thiel', 18.00, '2014-09-16', '978-0-7535-5645-2', 'Kinh doanh', 'Tạo ra giá trị độc nhất trong kinh doanh.', 110, 'Virgin Books', 'English');
('The 4-Hour Workweek', 'Tim Ferriss', 16.00, '2007-04-24', '978-0-307-46535-2', 'Tự giúp bản thân', 'Làm việc ít, sống nhiều hơn.', 140, 'Crown Publishing', 'English'),
('Deep Work', 'Cal Newport', 16.00, '2016-01-05', '978-0-451-49799-2', 'Tự giúp bản thân', 'Tập trung sâu để đạt hiệu quả cao.', 140, 'Grand Central Publishing', 'English');
('Atomic Habits', 'James Clear', 15.25, '2018-10-16', '978-0-7352-1129-5', 'Tự giúp bản thân', 'Xây dựng thói quen tốt và loại bỏ thói xấu.', 140, 'Avery', 'English'),
('The Power of Habit', 'Charles Duhigg', 14.00, '2012-02-28', '978-0-8129-8385-3', 'Tự giúp bản thân', 'Khoa học về thói quen và thay đổi.', 150, 'Random House', 'English');
GO
-- Thêm đơn hàng vào Orders (không chèn cột Id)
INSERT INTO Orders (OrderDate, TotalAmount, Status, ShippingAddress, PaymentMethod, Notes)
VALUES 
(GETDATE(), 150.00, 'Completed', N'123 Lê Lợi, Quận 1, TP.HCM', 'Credit Card', N'Giao hàng trong tuần'),
(GETDATE(), 89.50, 'Pending', N'456 Nguyễn Huệ, Quận 1, TP.HCM', 'Cash on Delivery', N'Hẹn giao ngày mai'),
(GETDATE(), 210.00, 'Shipped', N'789 Trần Hưng Đạo, Quận 5, TP.HCM', 'Bank Transfer', N'Đã chuyển khoản'),
(GETDATE(), 175.00, 'Completed', N'321 Lê Văn Sỹ, Quận 3, TP.HCM', 'Credit Card', N'Yêu cầu hóa đơn điện tử'),
(GETDATE(), 120.00, 'Pending', N'654 Trần Quốc Toản, Quận 10, TP.HCM', 'Cash on Delivery', N'Giao hàng vào cuối tuần'),
(GETDATE(), 95.00, 'Shipped', N'987 Nguyễn Thái Bình, Quận Tân Bình, TP.HCM', 'Bank Transfer', N'Đã xác nhận thanh toán'),
(GETDATE(), 180.00, 'Completed', N'123 Phạm Ngũ Lão, Quận 1, TP.HCM', 'Credit Card', N'Yêu cầu giao nhanh trong ngày'),
(GETDATE(), 140.00, 'Pending', N'456 Bùi Viện, Quận 1, TP.HCM', 'Cash on Delivery', N'Chờ xác nhận đơn hàng'),
(GETDATE(), 160.00, 'Shipped', N'789 Nguyễn Trãi, Quận 5, TP.HCM', 'Bank Transfer', N'Đã giao hàng thành công'),
(GETDATE(), 130.00, 'Completed', N'321 Võ Văn Tần, Quận 3, TP.HCM', 'Credit Card', N'Thêm vào giỏ hàng sau'),
(GETDATE(), 145.00, 'Pending', N'654 Lê Thị Riêng, Quận 10, TP.HCM', 'Cash on Delivery', N'Yêu cầu giao hàng vào buổi sáng'),
(GETDATE(), 170.00, 'Shipped', N'987 Trần Hưng Đạo, Quận 5, TP.HCM', 'Bank Transfer', N'Đã xác nhận giao hàng thành công'),
(GETDATE(), 155.00, 'Completed', N'123 Lê Văn Sỹ, Quận 3, TP.HCM', 'Credit Card', N'Yêu cầu giao hàng vào cuối tuần'),
(GETDATE(), 125.00, 'Pending', N'456 Nguyễn Huệ, Quận 1, TP.HCM', 'Cash on Delivery', N'Chờ xác nhận giao hàng'),
(GETDATE(), 135.00, 'Shipped', N'789 Trần Quốc Toản, Quận 10, TP.HCM', 'Bank Transfer', N'Đã giao hàng thành công');
GO
-- Đơn hàng 1
DECLARE @OrderId1 INT;
INSERT INTO Orders (OrderDate, TotalAmount, Status, ShippingAddress, PaymentMethod, Notes)
VALUES (GETDATE(), 150.00, 'Completed', N'123 Lê Lợi, Quận 1, TP.HCM', 'Credit Card', N'Giao hàng trong tuần');
SET @OrderId1 = SCOPE_IDENTITY();

INSERT INTO OrderDetails (OrderId, BookId, Quantity, UnitPrice, Subtotal)
VALUES
(@OrderId1, 1, 2, 50.00, 100.00),
(@OrderId1, 2, 1, 50.00, 50.00);

-- Đơn hàng 2
DECLARE @OrderId2 INT;
INSERT INTO Orders (OrderDate, TotalAmount, Status, ShippingAddress, PaymentMethod, Notes)
VALUES (GETDATE(), 89.50, 'Pending', N'456 Nguyễn Huệ, Quận 1, TP.HCM', 'Cash on Delivery', N'Hẹn giao ngày mai');
SET @OrderId2 = SCOPE_IDENTITY();

INSERT INTO OrderDetails (OrderId, BookId, Quantity, UnitPrice, Subtotal)
VALUES
(@OrderId2, 3, 1, 89.50, 89.50);

-- Đơn hàng 3
DECLARE @OrderId3 INT;
INSERT INTO Orders (OrderDate, TotalAmount, Status, ShippingAddress, PaymentMethod, Notes)
VALUES (GETDATE(), 210.00, 'Shipped', N'789 Trần Hưng Đạo, Quận 5, TP.HCM', 'Bank Transfer', N'Đã chuyển khoản');
SET @OrderId3 = SCOPE_IDENTITY();

INSERT INTO OrderDetails (OrderId, BookId, Quantity, UnitPrice, Subtotal)
VALUES
(@OrderId3, 4, 1, 210.00, 210.00);
-- Kiểm tra bảng Books
SELECT TOP 10 * FROM Books ORDER BY Id;

-- Kiểm tra bảng Users
SELECT TOP 10 * FROM Users ORDER BY Id;

-- Kiểm tra bảng Categories
SELECT TOP 10 * FROM Categories ORDER BY Id;

-- Kiểm tra bảng Orders
SELECT TOP 10 * FROM Orders ORDER BY Id;

-- Kiểm tra bảng OrderDetails
SELECT TOP 10 * FROM OrderDetails ORDER BY Id;

-- Kiểm tra bảng Roles
SELECT TOP 10 * FROM Roles ORDER BY Id;
