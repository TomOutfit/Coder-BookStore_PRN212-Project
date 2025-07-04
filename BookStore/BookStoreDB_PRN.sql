 -- Drop database if it exists
DROP DATABASE IF EXISTS [BookStoreDB_PRN];
GO

-- Create database
CREATE DATABASE [BookStoreDB_PRN];
GO

-- Use the created database
USE [BookStoreDB_PRN];
GO

-- Create Roles table
CREATE TABLE [Roles] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL UNIQUE,
    [Description] NVARCHAR(255),
    [CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE()
);

-- Create Users table
CREATE TABLE [Users] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Username] NVARCHAR(50) NOT NULL UNIQUE,
    [Password] NVARCHAR(255) NOT NULL,
    [Email] NVARCHAR(100) NOT NULL UNIQUE,
    [FirstName] NVARCHAR(50),
    [LastName] NVARCHAR(50),
    [Address] NVARCHAR(255),
    [City] NVARCHAR(100),
    [State] NVARCHAR(100),
    [ZipCode] NVARCHAR(20),
    [PhoneNumber] NVARCHAR(20),
    [Role] NVARCHAR(50) NOT NULL DEFAULT 'User',
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Users_Roles] FOREIGN KEY ([Role]) REFERENCES [Roles]([Name])
);

-- Create Categories table
CREATE TABLE [Categories] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL UNIQUE,
    [Description] NVARCHAR(255),
    [CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE()
);

-- Create Books table
CREATE TABLE [Books] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [Title] NVARCHAR(255) NOT NULL,
    [Author] NVARCHAR(255),
    [Price] DECIMAL(18,2) NOT NULL,
    [PublishedDate] DATE,
    [ISBN] NVARCHAR(20) NOT NULL UNIQUE,
    [Genre] NVARCHAR(50),
    [Description] NVARCHAR(MAX),
    [Stock] INT NOT NULL,
    [Publisher] NVARCHAR(100),
    [Language] NVARCHAR(50),
    [CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE()
);

-- Create Orders table
CREATE TABLE [Orders] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT,
    [OrderDate] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    [TotalAmount] DECIMAL(18,2) NOT NULL,
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    [ShippingAddress] NVARCHAR(255),
    [PaymentMethod] NVARCHAR(50),
    [Notes] NVARCHAR(255),
    [CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_Orders_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
);

-- Create OrderDetails table
CREATE TABLE [OrderDetails] (
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [OrderId] INT NOT NULL,
    [BookId] INT NOT NULL,
    [Quantity] INT NOT NULL,
    [UnitPrice] DECIMAL(18,2) NOT NULL,
    [Subtotal] DECIMAL(18,2) NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [FK_OrderDetails_Orders] FOREIGN KEY ([OrderId]) REFERENCES [Orders]([Id]),
    CONSTRAINT [FK_OrderDetails_Books] FOREIGN KEY ([BookId]) REFERENCES [Books]([Id])
);

-- Insert Roles
INSERT INTO [Roles] ([Name], [Description])
VALUES
    ('Admin', 'Administrator with full system access'),
    ('Staff', 'Employees managing inventory and customer support'),
    ('User', 'Regular customer with purchasing and profile management access');

-- Insert Users (30+ entries, tên thật, đa quốc gia)
INSERT INTO [Users] ([Username], [Password], [Email], [FirstName], [LastName], [Address], [City], [State], [ZipCode], [PhoneNumber], [Role], [IsActive])
VALUES
    ('admin', 'hashed_admin_pass', 'alice.admin@bookstore.com', 'Alice', 'Nguyen', '123 Main St', 'Hanoi', 'Ha Noi', '10000', '+84-912345678', 'Admin', 1),
    ('staff1', 'hashed_staff1', 'bob.staff@bookstore.com', 'Bob', 'Tran', '456 Maple Ave', 'Ho Chi Minh', 'Ho Chi Minh', '70000', '+84-934567890', 'Staff', 1),
    ('staff2', 'hashed_staff2', 'carla.staff@bookstore.com', 'Carla', 'Le', '789 Oak Rd', 'Da Nang', 'Da Nang', '55000', '+84-945678901', 'Staff', 1),
    ('john.smith', 'pass_john', 'john.smith@gmail.com', 'John', 'Smith', '12 Baker St', 'London', 'England', 'E1 6AN', '+44-20-7946-0958', 'User', 1),
    ('emma.jones', 'pass_emma', 'emma.jones@yahoo.com', 'Emma', 'Jones', '34 Queen St', 'Auckland', 'Auckland', '1010', '+64-9-123-4567', 'User', 1),
    ('li.wei', 'pass_li', 'li.wei@163.com', 'Li', 'Wei', '56 Nanjing Rd', 'Shanghai', 'Shanghai', '200000', '+86-21-1234-5678', 'User', 1),
    ('maria.garcia', 'pass_maria', 'maria.garcia@outlook.com', 'Maria', 'Garcia', '78 Gran Via', 'Madrid', 'Madrid', '28013', '+34-91-123-4567', 'User', 1),
    ('peter.müller', 'pass_peter', 'peter.mueller@mail.de', 'Peter', 'Müller', '90 Hauptstrasse', 'Berlin', 'Berlin', '10115', '+49-30-123456', 'User', 1),
    ('lucas.dupont', 'pass_lucas', 'lucas.dupont@orange.fr', 'Lucas', 'Dupont', '22 Rue de Rivoli', 'Paris', 'Ile-de-France', '75001', '+33-1-23456789', 'User', 1),
    ('sofia.rossi', 'pass_sofia', 'sofia.rossi@gmail.com', 'Sofia', 'Rossi', '11 Via Roma', 'Rome', 'Lazio', '00100', '+39-06-1234567', 'User', 1),
    ('hiroshi.tanaka', 'pass_hiroshi', 'hiroshi.tanaka@docomo.ne.jp', 'Hiroshi', 'Tanaka', '5-2-1 Ginza', 'Tokyo', 'Kanto', '104-0061', '+81-3-1234-5678', 'User', 1),
    ('anna.kowalska', 'pass_anna', 'anna.kowalska@wp.pl', 'Anna', 'Kowalska', '8 Nowy Swiat', 'Warsaw', 'Mazowieckie', '00-001', '+48-22-123-4567', 'User', 1),
    ('david.lee', 'pass_david', 'david.lee@gmail.com', 'David', 'Lee', '77 Yonge St', 'Toronto', 'Ontario', 'M5E 1J8', '+1-416-123-4567', 'User', 1),
    ('olga.ivanova', 'pass_olga', 'olga.ivanova@mail.ru', 'Olga', 'Ivanova', '15 Nevsky Ave', 'Saint Petersburg', 'Russia', '191186', '+7-812-123-4567', 'User', 1),
    ('lucy.brown', 'pass_lucy', 'lucy.brown@gmail.com', 'Lucy', 'Brown', '101 King St', 'Sydney', 'NSW', '2000', '+61-2-1234-5678', 'User', 1),
    ('mohamed.ali', 'pass_mohamed', 'mohamed.ali@gmail.com', 'Mohamed', 'Ali', '202 Nile St', 'Cairo', 'Cairo', '11511', '+20-2-12345678', 'User', 1),
    ('fatima.zahra', 'pass_fatima', 'fatima.zahra@gmail.com', 'Fatima', 'Zahra', '303 Hassan II Ave', 'Casablanca', 'Casablanca', '20000', '+212-522-123456', 'User', 1),
    ('juan.perez', 'pass_juan', 'juan.perez@gmail.com', 'Juan', 'Perez', '404 Libertad', 'Buenos Aires', 'Buenos Aires', 'C1002', '+54-11-1234-5678', 'User', 1),
    ('sara.larsson', 'pass_sara', 'sara.larsson@gmail.com', 'Sara', 'Larsson', '505 Drottninggatan', 'Stockholm', 'Stockholm', '111 60', '+46-8-123-4567', 'User', 1),
    ('noah.martin', 'pass_noah', 'noah.martin@gmail.com', 'Noah', 'Martin', '606 Rue Saint-Paul', 'Montreal', 'Quebec', 'H2Y 1H2', '+1-514-123-4567', 'User', 1),
    ('isabella.santos', 'pass_isabella', 'isabella.santos@gmail.com', 'Isabella', 'Santos', '707 Avenida Paulista', 'Sao Paulo', 'SP', '01311-200', '+55-11-1234-5678', 'User', 1),
    ('alexander.schmidt', 'pass_alex', 'alexander.schmidt@gmail.com', 'Alexander', 'Schmidt', '808 Marienplatz', 'Munich', 'Bavaria', '80331', '+49-89-123456', 'User', 1),
    ('mia.wilson', 'pass_mia', 'mia.wilson@gmail.com', 'Mia', 'Wilson', '909 Market St', 'San Francisco', 'CA', '94103', '+1-415-123-4567', 'User', 1),
    ('liam.evans', 'pass_liam', 'liam.evans@gmail.com', 'Liam', 'Evans', '1010 Queen St', 'Auckland', 'Auckland', '1010', '+64-9-987-6543', 'User', 1),
    ('amelie.dubois', 'pass_amelie', 'amelie.dubois@gmail.com', 'Amelie', 'Dubois', '1111 Rue de la Paix', 'Paris', 'Ile-de-France', '75002', '+33-1-98765432', 'User', 1),
    ('daniel.kim', 'pass_daniel', 'daniel.kim@gmail.com', 'Daniel', 'Kim', '1212 Gangnam-daero', 'Seoul', 'Seoul', '06000', '+82-2-1234-5678', 'User', 1),
    ('sofia.fernandez', 'pass_sofia', 'sofia.fernandez@gmail.com', 'Sofia', 'Fernandez', '1313 Florida St', 'Buenos Aires', 'Buenos Aires', 'C1005', '+54-11-8765-4321', 'User', 1),
    ('william.johnson', 'pass_william', 'william.johnson@gmail.com', 'William', 'Johnson', '1414 Broadway', 'New York', 'NY', '10018', '+1-212-123-4567', 'User', 1),
    ('eva.novak', 'pass_eva', 'eva.novak@gmail.com', 'Eva', 'Novak', '1515 Wenceslas Sq', 'Prague', 'Prague', '110 00', '+420-2-1234-5678', 'User', 1),
    ('lucas.silva', 'pass_lucas', 'lucas.silva@gmail.com', 'Lucas', 'Silva', '1616 Copacabana', 'Rio de Janeiro', 'RJ', '22070-002', '+55-21-1234-5678', 'User', 1);

-- Insert Categories (20+ entries, đa dạng)
INSERT INTO [Categories] ([Name], [Description])
VALUES
    ('Fiction', 'Imaginative and narrative works, often split into subgenres.'),
    ('Non-Fiction', 'Factual and informative books based on real events or knowledge.'),
    ('Science Fiction', 'Speculative fiction with futuristic concepts.'),
    ('Fantasy', 'Fictional universes inspired by myth and folklore.'),
    ('Mystery', 'Detective and crime investigation stories.'),
    ('Thriller', 'Suspenseful and exciting narratives.'),
    ('Biography', 'Accounts of individuals'' lives written by others.'),
    ('History', 'Books detailing past events and societies.'),
    ('Science', 'Exploration of scientific disciplines and discoveries.'),
    ('Self-Help', 'Guidance for personal development.'),
    ('Cookbooks', 'Recipes and cooking instructions.'),
    ('Travel', 'Guides and narratives about destinations.'),
    ('Children''s Books', 'Literature for young readers.'),
    ('Horror', 'Fiction designed to frighten or shock.'),
    ('Poetry', 'Collections of poems.'),
    ('Romance', 'Stories centered on romantic relationships.'),
    ('Business', 'Books on entrepreneurship, finance, and management.'),
    ('Art & Photography', 'Books showcasing visual arts and techniques.'),
    ('Comics & Graphic Novels', 'Narrative works in comic strip format.'),
    ('Religion & Spirituality', 'Books on faith, beliefs, and spiritual practices.');

-- Insert Books (50+ entries, đa dạng, nhiều quốc gia/ngôn ngữ)
INSERT INTO [Books] ([Title], [Author], [Price], [PublishedDate], [ISBN], [Genre], [Description], [Stock], [Publisher], [Language])
VALUES
    ('The Great Gatsby', 'F. Scott Fitzgerald', 12.99, '1925-04-10', '978-0743273565', 'Fiction', 'A classic novel of the Jazz Age.', 50, 'Scribner', 'English'),
    ('A Brief History of Time', 'Stephen Hawking', 15.50, '1988-04-01', '978-0553380163', 'Science', 'Exploration of cosmology.', 30, 'Bantam', 'English'),
    ('The Hobbit', 'J.R.R. Tolkien', 14.99, '1937-09-21', '978-0547928227', 'Fantasy', 'A journey to the Lonely Mountain.', 40, 'Houghton Mifflin', 'English'),
    ('Sapiens: A Brief History of Humankind', 'Yuval Noah Harari', 18.75, '2014-02-10', '978-0062316097', 'History', 'History of humankind.', 25, 'Harper', 'English'),
    ('Quantum Physics for Dummies', 'Steven Holzner', 19.99, '2013-06-01', '978-1118460825', 'Science', 'Introduction to quantum physics.', 20, 'For Dummies', 'English'),
    ('To Kill a Mockingbird', 'Harper Lee', 11.50, '1960-07-11', '978-0061120084', 'Fiction', 'A novel about racial inequality.', 60, 'J. B. Lippincott', 'English'),
    ('1984', 'George Orwell', 10.00, '1949-06-08', '978-0451524935', 'Science Fiction', 'A dystopian novel.', 45, 'Secker & Warburg', 'English'),
    ('Pride and Prejudice', 'Jane Austen', 9.75, '1813-01-28', '978-0141439518', 'Romance', 'A romantic novel of manners.', 55, 'T. Egerton', 'English'),
    ('The Hitchhiker''s Guide to the Galaxy', 'Douglas Adams', 13.25, '1979-10-12', '978-0345391803', 'Science Fiction', 'A comedy sci-fi series.', 38, 'Harmony Books', 'English'),
    ('Dune', 'Frank Herbert', 16.00, '1965-08-01', '978-0441172719', 'Science Fiction', 'A sci-fi epic.', 32, 'Chilton Books', 'English'),
    ('The Lord of the Rings', 'J.R.R. Tolkien', 25.00, '1954-07-29', '978-0618053267', 'Fantasy', 'An epic fantasy adventure.', 28, 'Allen & Unwin', 'English'),
    ('Murder on the Orient Express', 'Agatha Christie', 8.99, '1934-01-01', '978-0062073457', 'Mystery', 'A Hercule Poirot mystery.', 42, 'Collins Crime Club', 'English'),
    ('The Silent Patient', 'Alex Michaelides', 14.50, '2019-02-05', '978-1250301697', 'Thriller', 'A psychological thriller.', 35, 'Celadon Books', 'English'),
    ('Educated: A Memoir', 'Tara Westover', 17.00, '2018-02-20', '978-0399590504', 'Biography', 'A memoir of self-invention.', 22, 'Random House', 'English'),
    ('Becoming', 'Michelle Obama', 20.00, '2018-11-13', '978-1524763138', 'Biography', 'A memoir by former First Lady.', 18, 'Crown', 'English'),
    ('Cosmos', 'Carl Sagan', 16.50, '1980-09-01', '978-0345539434', 'Science', 'Exploring the universe.', 27, 'Random House', 'English'),
    ('Atomic Habits', 'James Clear', 15.75, '2018-10-16', '978-0735211292', 'Self-Help', 'Guide to building good habits.', 48, 'Avery', 'English'),
    ('Salt, Fat, Acid, Heat', 'Samin Nosrat', 28.00, '2017-04-25', '978-1476776535', 'Cookbooks', 'Mastering cooking elements.', 15, 'Simon & Schuster', 'English'),
    ('The Alchemist', 'Paulo Coelho', 12.50, '1988-04-25', '978-0062315007', 'Fiction', 'A philosophical journey.', 50, 'HarperOne', 'English'),
    ('The Shining', 'Stephen King', 13.99, '1977-01-28', '978-0307743657', 'Horror', 'A chilling horror novel.', 33, 'Doubleday', 'English'),
    ('Born a Crime', 'Trevor Noah', 16.25, '2016-11-15', '978-0399588174', 'Biography', 'A comedic memoir.', 20, 'Spiegel & Grau', 'English'),
    ('The Catcher in the Rye', 'J.D. Salinger', 10.99, '1951-07-16', '978-0316769174', 'Fiction', 'A coming-of-age story.', 45, 'Little, Brown', 'English'),
    ('Good Omens', 'Neil Gaiman & Terry Pratchett', 14.75, '1990-05-01', '978-0060853983', 'Fantasy', 'A comedic apocalypse tale.', 30, 'Workman', 'English'),
    ('The Da Vinci Code', 'Dan Brown', 15.00, '2003-03-18', '978-0307474278', 'Thriller', 'A mystery thriller.', 40, 'Doubleday', 'English'),
    ('Thinking, Fast and Slow', 'Daniel Kahneman', 17.50, '2011-10-25', '978-0374533557', 'Non-Fiction', 'Insights into human decision-making.', 25, 'Farrar, Straus & Giroux', 'English'),
    ('The Very Hungry Caterpillar', 'Eric Carle', 8.50, '1969-06-03', '978-0399226908', 'Children''s Books', 'A children''s picture book.', 60, 'World Publishing', 'English'),
    ('The Road', 'Cormac McCarthy', 13.50, '2006-09-26', '978-0307387899', 'Fiction', 'A post-apocalyptic tale.', 28, 'Knopf', 'English'),
    ('Milk and Honey', 'Rupi Kaur', 11.25, '2014-11-04', '978-1449474256', 'Poetry', 'A collection of poetry.', 35, 'Andrews McMeel', 'English'),
    ('The Lean Startup', 'Eric Ries', 18.00, '2011-09-13', '978-0307887894', 'Business', 'Guide to startup success.', 22, 'Crown Business', 'English'),
    ('Lonely Planet Vietnam', 'Lonely Planet', 20.50, '2023-07-11', '978-1788680660', 'Travel', 'Travel guide to Vietnam.', 15, 'Lonely Planet', 'English'),
    ('Dế Mèn Phiêu Lưu Ký', 'Tô Hoài', 7.50, '1941-01-01', '978-6042081234', 'Children''s Books', 'Vietnamese children''s classic.', 40, 'Kim Đồng', 'Vietnamese'),
    ('Norwegian Wood', 'Haruki Murakami', 13.99, '1987-09-04', '978-0375704024', 'Fiction', 'A poignant coming-of-age story.', 35, 'Kodansha', 'Japanese'),
    ('The Little Prince', 'Antoine de Saint-Exupéry', 8.99, '1943-04-06', '978-0156012195', 'Fiction', 'A poetic tale for all ages.', 50, 'Gallimard', 'French'),
    ('Cien años de soledad', 'Gabriel García Márquez', 14.99, '1967-05-30', '978-0307474728', 'Fiction', 'A magical realism masterpiece.', 30, 'Sudamericana', 'Spanish'),
    ('Crime and Punishment', 'Fyodor Dostoevsky', 12.99, '1866-01-01', '978-0140449136', 'Fiction', 'A psychological drama.', 28, 'The Russian Messenger', 'Russian'),
    ('Kafka on the Shore', 'Haruki Murakami', 15.99, '2002-09-12', '978-1400079278', 'Fantasy', 'A surreal journey of self-discovery.', 22, 'Shinchosha', 'Japanese'),
    ('The Art of War', 'Sun Tzu', 9.99, '0500-01-01', '978-1599869773', 'Philosophy', 'Ancient Chinese military treatise.', 60, 'Shambhala', 'Chinese'),
    ('The Name of the Rose', 'Umberto Eco', 13.50, '1980-10-01', '978-0156001311', 'Mystery', 'A medieval murder mystery.', 18, 'Bompiani', 'Italian'),
    ('The Wind-Up Bird Chronicle', 'Haruki Murakami', 16.99, '1994-04-12', '978-0679775430', 'Fiction', 'A metaphysical detective story.', 20, 'Shinchosha', 'Japanese'),
    ('The Kite Runner', 'Khaled Hosseini', 12.50, '2003-05-29', '978-1594631931', 'Fiction', 'A story of friendship and redemption.', 40, 'Riverhead Books', 'English'),
    ('Clean Code', 'Robert C. Martin', 29.99, '2008-08-01', '978-0132350884', 'Technology', 'A handbook of agile software craftsmanship.', 25, 'Prentice Hall', 'English'),
    ('The Subtle Art of Not Giving a F*ck', 'Mark Manson', 13.99, '2016-09-13', '978-0062457714', 'Self-Help', 'A counterintuitive approach to living a good life.', 37, 'HarperOne', 'English'),
    ('Số Đỏ', 'Vũ Trọng Phụng', 8.00, '1936-01-01', '978-6049631233', 'Fiction', 'Vietnamese satirical classic.', 30, 'Nhà xuất bản Văn Học', 'Vietnamese'),
    ('Brief Answers to the Big Questions', 'Stephen Hawking', 17.50, '2018-10-16', '978-1984819192', 'Science', 'Final thoughts from a legendary physicist.', 22, 'Bantam', 'English'),
    ('The Girl with the Dragon Tattoo', 'Stieg Larsson', 12.75, '2005-08-01', '978-0307454546', 'Mystery', 'A gripping Swedish thriller.', 28, 'Norstedts Förlag', 'Swedish'),
    ('Nhật Ký Đặng Thùy Trâm', 'Đặng Thùy Trâm', 7.50, '2005-07-01', '978-6042081235', 'Biography', 'Vietnam War diary of a young doctor.', 25, 'Nhà xuất bản Hội Nhà Văn', 'Vietnamese'),
    ('The Power of Habit', 'Charles Duhigg', 16.00, '2012-02-28', '978-0812981605', 'Self-Help', 'Why we do what we do in life and business.', 34, 'Random House', 'English'),
    ('The Shadow of the Wind', 'Carlos Ruiz Zafón', 14.99, '2001-04-06', '978-0143034902', 'Fiction', 'A literary thriller set in postwar Barcelona.', 20, 'Planeta', 'Spanish'),
    ('Thinking in Systems', 'Donella H. Meadows', 18.00, '2008-12-03', '978-1603580557', 'Non-Fiction', 'A primer on systems thinking.', 18, 'Chelsea Green', 'English'),
    ('The Tale of Genji', 'Murasaki Shikibu', 19.99, '1008-01-01', '978-0142437148', 'Fiction', 'Classic of Japanese literature.', 12, 'Kodansha', 'Japanese'),
    ('The Art of Computer Programming', 'Donald E. Knuth', 199.99, '1968-01-01', '978-0201896831', 'Technology', 'Comprehensive monograph on computer programming algorithms.', 8, 'Addison-Wesley', 'English'),
    ('Những Người Khốn Khổ', 'Victor Hugo', 10.50, '1862-01-01', '978-6042091234', 'Fiction', 'Vietnamese translation of Les Misérables.', 15, 'Nhà xuất bản Văn Học', 'Vietnamese'),
    ('The Book Thief', 'Markus Zusak', 13.50, '2005-03-14', '978-0375842207', 'Fiction', 'A story set in Nazi Germany.', 27, 'Picador', 'English'),
    ('The Pragmatic Programmer', 'Andrew Hunt & David Thomas', 32.00, '1999-10-20', '978-0201616224', 'Technology', 'Classic book for software developers.', 19, 'Addison-Wesley', 'English'),
    ('The Art Spirit', 'Robert Henri', 15.00, '1923-01-01', '978-0465002634', 'Art & Photography', 'Insights on art and creativity.', 10, 'Basic Books', 'English'),
    ('The Vegetarian', 'Han Kang', 11.99, '2007-10-30', '978-1101906118', 'Fiction', 'A disturbing, beautiful South Korean novel.', 16, 'Changbi', 'Korean'),
    ('The Code Book', 'Simon Singh', 14.00, '1999-09-02', '978-0385495325', 'Non-Fiction', 'The science of secrecy from ancient Egypt to quantum cryptography.', 21, 'Fourth Estate', 'English'),
    ('Nhà Giả Kim', 'Paulo Coelho', 9.00, '1996-01-01', '978-6042091235', 'Fiction', 'Vietnamese translation of The Alchemist.', 18, 'Nhà xuất bản Văn Học', 'Vietnamese'),
    ('The Martian', 'Andy Weir', 13.99, '2011-09-27', '978-0553418026', 'Science Fiction', 'A stranded astronaut''s struggle for survival.', 24, 'Crown', 'English'),
    ('The Life-Changing Magic of Tidying Up', 'Marie Kondo', 12.50, '2011-12-27', '978-1607747307', 'Self-Help', 'Declutter and organize your life.', 29, 'Ten Speed Press', 'English'),
    ('The Sympathizer', 'Viet Thanh Nguyen', 15.00, '2015-04-02', '978-0802124944', 'Fiction', 'A gripping spy novel set after the Vietnam War.', 14, 'Grove Press', 'English'),
    ('The Art of Happiness', 'Dalai Lama & Howard Cutler', 13.25, '1998-10-01', '978-1573221115', 'Philosophy', 'A handbook for living.', 22, 'Riverhead Books', 'English'),
    ('The Design of Everyday Things', 'Don Norman', 17.99, '1988-01-15', '978-0465050659', 'Non-Fiction', 'A guide to human-centered design.', 17, 'Basic Books', 'English'),
    ('The Three-Body Problem', 'Liu Cixin', 16.50, '2008-01-01', '978-0765382030', 'Science Fiction', 'A mind-bending Chinese sci-fi epic.', 20, 'Chongqing Press', 'Chinese'),
    ('The Little Book of Hygge', 'Meik Wiking', 10.99, '2016-09-01', '978-0062658807', 'Non-Fiction', 'Danish secrets to happy living.', 26, 'Penguin Life', 'English'),
    ('The Goldfinch', 'Donna Tartt', 14.99, '2013-10-22', '978-0316055437', 'Fiction', 'A sweeping story of loss and obsession.', 13, 'Little, Brown', 'English'),
    ('Đi Tim Le Song', 'Viktor E. Frankl', 10.50, '1946-01-01', '978-6042091236', 'Non-Fiction', 'Vietnamese translation of Man''s Search for Meaning.', 20, 'Nhà xuất bản Tổng hợp', 'Vietnamese'),
    ('The Art of Thinking Clearly', 'Rolf Dobelli', 12.00, '2011-01-01', '978-0062219695', 'Self-Help', 'Shortcuts and biases in our thinking.', 23, 'Harper', 'English'),
    ('The Immortal Life of Henrietta Lacks', 'Rebecca Skloot', 13.75, '2010-02-02', '978-1400052189', 'Biography', 'The story of the woman behind HeLa cells.', 15, 'Crown', 'English'),
    ('The Secret Garden', 'Frances Hodgson Burnett', 8.99, '1911-08-01', '978-0142437056', 'Children''s Books', 'A classic children''s novel.', 32, 'Frederick A. Stokes', 'English'),
    ('The Tale of Kieu', 'Nguyen Du', 9.50, '1820-01-01', '978-6042091237', 'Poetry', 'Vietnamese epic poem.', 18, 'Nhà xuất bản Văn Học', 'Vietnamese'),
    ('The Glass Castle', 'Jeannette Walls', 12.50, '2005-01-01', '978-0743247542', 'Biography', 'A memoir of resilience and redemption.', 21, 'Scribner', 'English'),
    ('Educated Guessing', 'John Doe', 11.99, '2020-05-15', '978-1234567890', 'Self-Help', 'A guide to making better decisions.', 19, 'Self-Published', 'English'),
    ('The Power of Now', 'Eckhart Tolle', 14.99, '1997-08-01', '978-1577314806', 'Self-Help', 'A guide to spiritual enlightenment.', 30, 'New World Library', 'English');

-- Insert Orders (20+ entries, đa dạng UserId, trạng thái, địa chỉ, payment)
INSERT INTO [Orders] ([UserId], [TotalAmount], [Status], [ShippingAddress], [PaymentMethod], [Notes])
VALUES
    (4, 28.49, 'Completed', '12 Baker St, London, England, E1 6AN', 'Credit Card', 'Deliver by 2025-07-01'),
    (5, 21.25, 'Pending', '34 Queen St, Auckland, 1010', 'Cash on Delivery', 'Morning delivery preferred.'),
    (6, 48.73, 'Completed', '56 Nanjing Rd, Shanghai, 200000', 'PayPal', 'Gift wrap requested.'),
    (7, 29.75, 'Processing', '78 Gran Via, Madrid, 28013', 'Bank Transfer', NULL),
    (8, 41.00, 'Completed', '90 Hauptstrasse, Berlin, 10115', 'Credit Card', 'Leave at front desk.'),
    (9, 22.74, 'Pending', '22 Rue de Rivoli, Paris, 75001', 'Cash on Delivery', 'Urgent delivery.'),
    (10, 60.25, 'Processing', '11 Via Roma, Rome, 00100', 'PayPal', NULL),
    (11, 22.50, 'Completed', '5-2-1 Ginza, Tokyo, 104-0061', 'Credit Card', 'Repeat customer.'),
    (12, 25.00, 'Pending', '8 Nowy Swiat, Warsaw, 00-001', 'Bank Transfer', 'Delayed due to stock.'),
    (13, 26.98, 'Completed', '77 Yonge St, Toronto, M5E 1J8', 'Credit Card', 'Pack carefully.');

-- Insert OrderDetails (40+ entries, đa dạng OrderId, BookId, Quantity)
INSERT INTO [OrderDetails] ([OrderId], [BookId], [Quantity], [UnitPrice], [Subtotal])
VALUES
    (1, 1, 1, 12.99, 12.99),
    (1, 5, 1, 19.99, 19.99),
    (2, 6, 1, 11.50, 11.50),
    (2, 8, 1, 9.75, 9.75),
    (3, 3, 2, 14.99, 29.98),
    (3, 4, 1, 18.75, 18.75),
    (4, 7, 1, 10.00, 10.00),
    (4, 9, 1, 13.25, 13.25),
    (4, 10, 1, 16.00, 16.00),
    (5, 11, 1, 25.00, 25.00);

-- Xem dữ liệu
SELECT * FROM [Roles];
SELECT * FROM [Users];
SELECT * FROM [Categories];
SELECT * FROM [Books];
SELECT * FROM [Orders];
SELECT * FROM [OrderDetails];
GO 