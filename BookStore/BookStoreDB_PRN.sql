USE master
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

-- Insert Users
INSERT INTO [Users] ([Username], [Password], [Email], [FirstName], [LastName], [Address], [City], [State], [ZipCode], [PhoneNumber], [Role], [IsActive])
VALUES
    ('admin', 'hashed_admin_pass', 'alice.admin@bookstore.com', 'Alice', 'Nguyen', '123 Main St', 'Hanoi', 'Ha Noi', '10000', '+84-912345678', 'Admin', 1),
    ('staff1', 'hashed_staff1', 'bob.staff@bookstore.com', 'Bob', 'Tran', '456 Maple Ave', 'Ho Chi Minh', 'Ho Chi Minh', '70000', '+84-934567890', 'Staff', 1),
    ('staff2', 'hashed_staff2', 'carla.staff@bookstore.com', 'Carla', 'Le', '789 Oak Rd', 'Da Nang', 'Da Nang', '55000', '+84-945678901', 'Staff', 1),
    ('john.smith', 'pass_john', 'john.smith@gmail.com', 'John', 'Smith', '12 Baker St', 'London', 'England', 'E1 6AN', '+44-20-7946-0958', 'User', 1),
    ('emma.jones', 'pass_emma', 'emma.jones@yahoo.com', 'Emma', 'Jones', '34 Queen St', 'Auckland', 'Auckland', '1010', '+64-9-123-4567', 'User', 1),
    ('li.wei', 'pass_li', 'li.wei@163.com', 'Li', 'Wei', '56 Nanjing Rd', 'Shanghai', 'Shanghai', '200000', '+86-21-1234-5678', 'User', 1),
    ('maria.garcia', 'pass_maria', 'maria.garcia@outlook.com', 'Maria', 'Garcia', '78 Gran Via', 'Madrid', 'Madrid', '28013', '+34-91-123-4567', 'User', 1),
    ('peter.mueller', 'pass_peter', 'peter.mueller@mail.de', 'Peter', 'Mueller', '90 Hauptstrasse', 'Berlin', 'Berlin', '10115', '+49-30-123456', 'User', 1),
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
    ('noah.martin', 'pass_noah', 'noah.martin@gmail.com', 'Noah', 'Martin', '606 Rue Saint-Paul', 'Montreal', 'Quebec', 'H2Y 1H2', '+1-514-123-4567', 'User', 1);

-- Insert Categories
INSERT INTO [Categories] ([Name], [Description])
VALUES
    ('Fiction', 'Books that tell stories about imaginary events and characters'),
    ('Non-Fiction', 'Books that provide factual information and real-world knowledge'),
    ('Children', 'Books designed for young readers'),
    ('Mystery', 'Books that focus on solving puzzles and uncovering secrets'),
    ('Science Fiction', 'Books that explore future worlds and advanced technologies'),
    ('Biography', 'Books that tell the life stories of real people'),
    ('History', 'Books that cover past events and historical periods'),
    ('Travel', 'Books that guide readers to explore different places and cultures'),
    ('Cooking', 'Books that teach readers how to prepare delicious meals'),
    ('Art', 'Books that focus on visual arts and creative expression'),
    ('Self-Help', 'Books that provide guidance and motivation for personal growth'),
    ('Technology', 'Books that cover advancements in science and technology'),
    ('Health', 'Books that focus on physical and mental well-being'),
    ('Business', 'Books that provide insights into business management and entrepreneurship'),
    ('Religion', 'Books that explore spiritual beliefs and practices'),
    ('Science', 'Books that cover scientific discoveries and theories'),
    ('Fantasy', 'Books that create imaginary worlds and magical elements'),
    ('Drama', 'Books that explore human emotions and relationships'),
    ('Poetry', 'Books that use words to express emotions and ideas'),
    ('Classic', 'Books that are considered classics and have stood the test of time'),
    ('Young Adult', 'Books designed for teenagers');

-- Insert Books (removed duplicate ISBN entry)
INSERT INTO [Books] ([Title], [Author], [Price], [PublishedDate], [ISBN], [Genre], [Description], [Stock], [Publisher], [Language])
VALUES
    ('The Great Gatsby', 'F. Scott Fitzgerald', 12.99, '1925-04-10', '978-0743273565', 'Fiction', 'A story of love, greed, and the American Dream', 100, 'Scribner', 'English'),
    ('1984', 'George Orwell', 14.99, '1949-06-08', '978-0451524935', 'Dystopian', 'A powerful critique of totalitarianism', 80, 'Signet Classics', 'English'),
    ('To Kill a Mockingbird', 'Harper Lee', 11.99, '1960-07-11', '978-0743273566', 'Classic', 'A story of racial injustice and moral growth', 120, 'J.B. Lippincott & Co.', 'English'),
    ('The Catcher in the Rye', 'J.D. Salinger', 13.99, '1951-07-16', '978-0316769488', 'Classic', 'A story of teenage angst and rebellion', 90, 'Little, Brown and Company', 'English'),
    ('The Hobbit', 'J.R.R. Tolkien', 15.99, '1937-09-21', '978-0618968689', 'Fantasy', 'A journey through a magical world', 110, 'Houghton Mifflin Harcourt', 'English'),
    ('The Lord of the Rings', 'J.R.R. Tolkien', 24.99, '1954-10-20', '978-0618640158', 'Fantasy', 'A journey through a magical world', 110, 'Houghton Mifflin Harcourt', 'English'),
    ('The Brothers Karamazov', 'Fyodor Dostoevsky', 13.99, '1880-01-01', '978-0374528379', 'Fiction', 'A philosophical novel set in 19th-century Russia.', 20, 'Farrar, Straus and Giroux', 'English'),
    ('Don Quixote', 'Miguel de Cervantes', 15.50, '1605-01-16', '978-0060934347', 'Fiction', 'The adventures of a self-styled knight-errant.', 18, 'Harper Perennial', 'English'),
    ('The Divine Comedy', 'Dante Alighieri', 17.99, '1320-01-01', '978-0142437223', 'Poetry', 'Epic poem describing Dante''s journey through Hell, Purgatory, and Paradise.', 12, 'Penguin Classics', 'English'),
    ('War and Peace', 'Leo Tolstoy', 19.99, '1869-01-01', '978-0199232765', 'Fiction', 'A sweeping narrative of Russian society during the Napoleonic era.', 15, 'Oxford University Press', 'English'),
    ('Madame Bovary', 'Gustave Flaubert', 12.50, '1856-01-01', '978-0140449129', 'Fiction', 'A story of a doctor''s wife who seeks escape from the boredom of provincial life.', 22, 'Penguin Classics', 'English'),
    ('The Count of Monte Cristo', 'Alexandre Dumas', 14.99, '1844-01-01', '978-0140449266', 'Fiction', 'A tale of betrayal, revenge, and redemption.', 19, 'Penguin Classics', 'English'),
    ('Anna Karenina', 'Leo Tolstoy', 13.75, '1877-01-01', '978-0143035008', 'Fiction', 'A tragic story of love and infidelity in Imperial Russia.', 17, 'Penguin Classics', 'English'),
    ('Ulysses', 'James Joyce', 16.50, '1922-02-02', '978-0199535675', 'Fiction', 'A modernist novel chronicling a day in the life of Leopold Bloom.', 10, 'Oxford University Press', 'English'),
    ('The Odyssey', 'Homer', 11.99, '0800-01-01', '978-0140268867', 'Fiction', 'The epic journey of Odysseus returning home from the Trojan War.', 25, 'Penguin Classics', 'English'),
    ('Les Misérables', 'Victor Hugo', 15.99, '1862-01-01', '978-0451419439', 'Fiction', 'A story of redemption and revolution in 19th-century France.', 20, 'Signet', 'English'),
    ('The Stranger', 'Albert Camus', 10.99, '1942-01-01', '978-0679720201', 'Fiction', 'A philosophical novel about absurdism and alienation.', 18, 'Vintage', 'English'),
    ('The Old Man and the Sea', 'Ernest Hemingway', 9.99, '1952-09-01', '978-0684801223', 'Fiction', 'A Cuban fisherman''s epic struggle with a giant marlin.', 30, 'Scribner', 'English'),
    ('Lolita', 'Vladimir Nabokov', 12.99, '1955-09-15', '978-0679723165', 'Fiction', 'A controversial novel about obsession and manipulation.', 14, 'Vintage', 'English'),
    ('The Trial', 'Franz Kafka', 11.50, '1925-01-01', '978-0805209990', 'Fiction', 'A man is arrested and prosecuted by a remote, inaccessible authority.', 16, 'Schocken', 'English'),
    ('The Sun Also Rises', 'Ernest Hemingway', 13.25, '1926-10-22', '978-0743297332', 'Fiction', 'A story of American and British expatriates in 1920s Europe.', 21, 'Scribner', 'English'),
    ('The Sound and the Fury', 'William Faulkner', 14.00, '1929-10-07', '978-0679732242', 'Fiction', 'A Southern family''s decline told through multiple perspectives.', 13, 'Vintage', 'English'),
    ('The Grapes of Wrath', 'John Steinbeck', 15.00, '1939-04-14', '978-0143039433', 'Fiction', 'A family migrates west during the Great Depression.', 19, 'Penguin Classics', 'English'),
    ('A Passage to India', 'E.M. Forster', 12.75, '1924-06-04', '978-0156711425', 'Fiction', 'A novel about colonialism and misunderstanding in British India.', 17, 'Mariner Books', 'English'),
    ('The Magic Mountain', 'Thomas Mann', 16.99, '1924-11-01', '978-0679772873', 'Fiction', 'A philosophical novel set in a Swiss sanatorium.', 11, 'Vintage', 'English'),
    ('The Master and Margarita', 'Mikhail Bulgakov', 13.50, '1967-01-01', '978-0140455465', 'Fiction', 'A satirical fantasy set in Soviet Russia.', 15, 'Penguin Classics', 'English'),
    ('The Remains of the Day', 'Kazuo Ishiguro', 12.99, '1989-05-01', '978-0679731726', 'Fiction', 'A butler reflects on his life and service in England.', 18, 'Vintage', 'English'),
    ('The Unbearable Lightness of Being', 'Milan Kundera', 13.99, '1984-01-01', '978-0061148521', 'Fiction', 'A philosophical novel set in Czechoslovakia.', 16, 'Harper Perennial', 'English'),
    ('The Metamorphosis', 'Franz Kafka', 9.50, '1915-01-01', '978-0553213690', 'Fiction', 'A man wakes up transformed into a giant insect.', 22, 'Bantam Classics', 'English'),
    ('The Idiot', 'Fyodor Dostoevsky', 14.25, '1869-01-01', '978-0140447927', 'Fiction', 'A Russian prince returns to society after years in a sanatorium.', 13, 'Penguin Classics', 'English'),
    ('The Death of Ivan Ilyich', 'Leo Tolstoy', 10.75, '1886-01-01', '978-0553210354', 'Fiction', 'A judge faces his own mortality.', 20, 'Bantam Classics', 'English'),
    ('The Plague', 'Albert Camus', 11.99, '1947-01-01', '978-0679720218', 'Fiction', 'A city is swept by a deadly epidemic.', 17, 'Vintage', 'English'),
    ('The House of the Spirits', 'Isabel Allende', 13.50, '1982-01-01', '978-0553383805', 'Fiction', 'A family saga set in postcolonial Chile.', 15, 'Dial Press', 'English'),
    ('The Name of the Wind', 'Patrick Rothfuss', 14.99, '2007-03-27', '978-0756404741', 'Fantasy', 'A gifted young man grows up to be a legendary figure.', 18, 'DAW Books', 'English'),
    ('The Road to Wigan Pier', 'George Orwell', 12.00, '1937-03-08', '978-0156767507', 'Non-Fiction', 'A report on working-class life in northern England.', 14, 'Mariner Books', 'English'),
    ('The Wind in the Willows', 'Kenneth Grahame', 10.99, '1908-10-08', '978-0143039099', 'Children', 'Adventures of Mole, Rat, Toad, and Badger.', 20, 'Penguin Classics', 'English'),
    ('The Secret History', 'Donna Tartt', 13.75, '1992-09-01', '978-1400031702', 'Fiction', 'A group of classics students at an elite college commit murder.', 16, 'Vintage', 'English'),
    ('The Bell Jar', 'Sylvia Plath', 12.50, '1963-01-14', '978-0060837020', 'Fiction', 'A young woman''s descent into mental illness.', 19, 'Harper Perennial', 'English'),
    ('The Little House on the Prairie', 'Laura Ingalls Wilder', 9.99, '1935-03-01', '978-0064400022', 'Children', 'A family moves to the American West.', 22, 'HarperCollins', 'English'),
    ('The Tale of Peter Rabbit', 'Beatrix Potter', 8.99, '1902-10-01', '978-0723247708', 'Children', 'A mischievous rabbit gets into trouble in Mr. McGregor''s garden.', 25, 'Warne', 'English'),
    ('The Cat in the Hat', 'Dr. Seuss', 7.99, '1957-03-12', '978-0394800011', 'Children', 'A mischievous cat turns a rainy day into an adventure.', 30, 'Random House', 'English'),
    ('The Giving Tree', 'Shel Silverstein', 9.50, '1964-10-07', '978-0060256654', 'Children', 'A story about selfless love between a tree and a boy.', 18, 'Harper & Row', 'English'),
    ('The Very Busy Spider', 'Eric Carle', 8.99, '1984-09-15', '978-0399229191', 'Children', 'A spider busily spins her web.', 20, 'Philomel Books', 'English'),
    ('The Polar Express', 'Chris Van Allsburg', 10.50, '1985-10-28', '978-0544580145', 'Children', 'A magical train ride to the North Pole.', 17, 'Houghton Mifflin', 'English'),
    ('The Rainbow Fish', 'Marcus Pfister', 8.75, '1992-01-01', '978-1558580091', 'Children', 'A beautiful fish learns to share.', 19, 'NorthSouth Books', 'English'),
    ('The Gruffalo', 'Julia Donaldson', 9.25, '1999-03-23', '978-0333710937', 'Children', 'A mouse invents a monster to scare off predators.', 21, 'Macmillan', 'English'),
    ('Charlotte''s Web', 'E.B. White', 10.99, '1952-10-15', '978-0064400558', 'Children', 'A pig named Wilbur befriends a spider named Charlotte.', 24, 'HarperCollins', 'English'),
    ('Matilda', 'Roald Dahl', 11.50, '1988-10-01', '978-0142410370', 'Children', 'A young girl with extraordinary powers.', 22, 'Puffin Books', 'English'),
    ('Where the Wild Things Are', 'Maurice Sendak', 9.99, '1963-04-09', '978-0060254926', 'Children', 'A boy named Max sails to an island of monsters.', 20, 'Harper & Row', 'English'),
    ('The Secret Garden', 'Frances Hodgson Burnett', 8.99, '1911-08-01', '978-0142437056', 'Children', 'A classic children''s novel.', 32, 'Frederick A. Stokes', 'English'),
    ('The Little Prince', 'Antoine de Saint-Exupéry', 8.99, '1943-04-06', '978-0156012195', 'Classic', 'A poetic tale for all ages.', 50, 'Gallimard', 'French'),
    ('The Alchemist', 'Paulo Coelho', 12.50, '1988-04-25', '978-0062315007', 'Fiction', 'A philosophical journey.', 50, 'HarperOne', 'English'),
    ('The Book Thief', 'Markus Zusak', 13.50, '2005-03-14', '978-0375842207', 'Fiction', 'A story set in Nazi Germany.', 27, 'Picador', 'English'),
    ('The Kite Runner', 'Khaled Hosseini', 12.50, '2003-05-29', '978-1594631931', 'Fiction', 'A story of friendship and redemption.', 40, 'Riverhead Books', 'English'),
    ('Life of Pi', 'Yann Martel', 14.99, '2001-09-11', '978-0156027328', 'Fiction', 'A boy survives a shipwreck with a Bengal tiger.', 18, 'Harcourt', 'English'),
    ('A Brief History of Time', 'Stephen Hawking', 15.50, '1988-04-01', '978-0553380163', 'Science', 'Exploration of cosmology.', 30, 'Bantam', 'English'),
    ('Sapiens: A Brief History of Humankind', 'Yuval Noah Harari', 18.75, '2014-02-10', '978-0062316097', 'History', 'History of humankind.', 25, 'Harper', 'English'),
    ('Thinking, Fast and Slow', 'Daniel Kahneman', 17.50, '2011-10-25', '978-0374533557', 'Non-Fiction', 'Insights into human decision-making.', 25, 'Farrar, Straus & Giroux', 'English'),
    ('The Lean Startup', 'Eric Ries', 18.00, '2011-09-13', '978-0307887894', 'Business', 'Guide to startup success.', 22, 'Crown Business', 'English'),
    ('The Art of War', 'Sun Tzu', 9.99, '0500-01-01', '978-1599869773', 'Classic', 'Ancient Chinese military treatise.', 60, 'Shambhala', 'Chinese'),
    ('The Art of Computer Programming', 'Donald E. Knuth', 199.99, '1968-01-01', '978-0201896831', 'Technology', 'Comprehensive monograph on computer programming algorithms.', 8, 'Addison-Wesley', 'English'),
    ('Clean Code', 'Robert C. Martin', 29.99, '2008-08-01', '978-0132350884', 'Technology', 'A handbook of agile software craftsmanship.', 25, 'Prentice Hall', 'English'),
    ('The Pragmatic Programmer', 'Andrew Hunt & David Thomas', 32.00, '1999-10-20', '978-0201616224', 'Technology', 'Classic book for software developers.', 19, 'Addison-Wesley', 'English'),
    ('The Subtle Art of Not Giving a F*ck', 'Mark Manson', 13.99, '2016-09-13', '978-0062457714', 'Self-Help', 'A counterintuitive approach to living a good life.', 37, 'HarperOne', 'English'),
    ('Atomic Habits', 'James Clear', 15.75, '2018-10-16', '978-0735211292', 'Self-Help', 'Guide to building good habits.', 48, 'Avery', 'English'),
    ('The Power of Habit', 'Charles Duhigg', 16.00, '2012-02-28', '978-0812981605', 'Self-Help', 'Why we do what we do in life and business.', 34, 'Random House', 'English'),
    ('The Life-Changing Magic of Tidying Up', 'Marie Kondo', 12.50, '2011-12-27', '978-1607747307', 'Self-Help', 'Declutter and organize your life.', 29, 'Ten Speed Press', 'English'),
    ('Educated', 'Tara Westover', 17.00, '2018-02-20', '978-0399590504', 'Biography', 'A memoir of self-invention.', 22, 'Random House', 'English'),
    ('Becoming', 'Michelle Obama', 20.00, '2018-11-13', '978-1524763138', 'Biography', 'A memoir by former First Lady.', 18, 'Crown', 'English'),
    ('Born a Crime', 'Trevor Noah', 16.25, '2016-11-15', '978-0399588174', 'Biography', 'A comedic memoir.', 20, 'Spiegel & Grau', 'English'),
    ('The Immortal Life of Henrietta Lacks', 'Rebecca Skloot', 13.75, '2010-02-02', '978-1400052189', 'Biography', 'The story of the woman behind HeLa cells.', 15, 'Crown', 'English'),
    ('The Diary of a Young Girl', 'Anne Frank', 12.99, '1947-06-25', '978-0553296983', 'Biography', 'The diary of Anne Frank during WWII.', 18, 'Bantam', 'English'),
    ('Long Walk to Freedom', 'Nelson Mandela', 18.99, '1994-01-01', '978-0316548182', 'Biography', 'The autobiography of Nelson Mandela.', 16, 'Little, Brown', 'English'),
    ('Steve Jobs', 'Walter Isaacson', 19.99, '2011-10-24', '978-1451648539', 'Biography', 'The biography of Apple co-founder Steve Jobs.', 20, 'Simon & Schuster', 'English'),
    ('The Wright Brothers', 'David McCullough', 15.99, '2015-05-05', '978-1476728742', 'Biography', 'The story of the inventors of the airplane.', 14, 'Simon & Schuster', 'English'),
    ('Into the Wild', 'Jon Krakauer', 13.50, '1996-01-20', '978-0385486804', 'Biography', 'The story of Christopher McCandless.', 17, 'Anchor Books', 'English'),
    ('The Glass Castle', 'Jeannette Walls', 12.50, '2005-01-01', '978-0743247542', 'Biography', 'A memoir of resilience and redemption.', 21, 'Scribner', 'English'),
    ('Educated Guessing', 'John Doe', 11.99, '2020-05-15', '978-1234567890', 'Self-Help', 'A guide to making better decisions.', 19, 'Self-Published', 'English'),
    ('The Art Spirit', 'Robert Henri', 15.00, '1923-01-01', '978-0465002634', 'Art', 'Insights on art and creativity.', 10, 'Basic Books', 'English'),
    ('The Design of Everyday Things', 'Don Norman', 17.99, '1988-01-15', '978-0465050659', 'Non-Fiction', 'A guide to human-centered design.', 17, 'Basic Books', 'English'),
    ('The Art of Thinking Clearly', 'Rolf Dobelli', 12.00, '2011-01-01', '978-0062219695', 'Self-Help', 'Shortcuts and biases in our thinking.', 23, 'Harper', 'English'),
    ('Thinking in Systems', 'Donella H. Meadows', 18.00, '2008-12-03', '978-1603580557', 'Non-Fiction', 'A primer on systems thinking.', 18, 'Chelsea Green', 'English'),
    ('The Code Book', 'Simon Singh', 14.00, '1999-09-02', '978-0385495325', 'Non-Fiction', 'The science of secrecy from ancient Egypt to quantum cryptography.', 21, 'Fourth Estate', 'English'),
    ('The Little Book of Hygge', 'Meik Wiking', 10.99, '2016-09-01', '978-0062658807', 'Non-Fiction', 'Danish secrets to happy living.', 26, 'Penguin Life', 'English'),
    ('Salt, Fat, Acid, Heat', 'Samin Nosrat', 28.00, '2017-04-25', '978-1476776535', 'Cooking', 'Mastering cooking elements.', 15, 'Simon & Schuster', 'English'),
    ('The Joy of Cooking', 'Irma S. Rombauer', 24.99, '1931-01-01', '978-0743246262', 'Cooking', 'A classic American cookbook.', 12, 'Scribner', 'English'),
    ('Mastering the Art of French Cooking', 'Julia Child', 29.99, '1961-10-16', '978-0375413407', 'Cooking', 'A classic French cookbook.', 10, 'Knopf', 'English'),
    ('On Food and Cooking', 'Harold McGee', 22.00, '1984-01-01', '978-0684800011', 'Cooking', 'The science and lore of the kitchen.', 8, 'Scribner', 'English'),
    ('The Science Book', 'DK', 19.99, '2014-07-01', '978-1465419651', 'Science', 'Big ideas simply explained.', 15, 'DK', 'English'),
    ('A Short History of Nearly Everything', 'Bill Bryson', 16.99, '2003-05-06', '978-0767908184', 'Science', 'A journey through science and history.', 18, 'Broadway Books', 'English'),
    ('The Selfish Gene', 'Richard Dawkins', 14.99, '1976-03-13', '978-0199291151', 'Science', 'A gene-centered view of evolution.', 12, 'Oxford University Press', 'English'),
    ('The Origin of Species', 'Charles Darwin', 13.99, '1859-11-24', '978-1509827695', 'Science', 'The foundation of evolutionary biology.', 10, 'Macmillan', 'English'),
    ('The Double Helix', 'James D. Watson', 12.50, '1968-02-01', '978-0743216302', 'Science', 'The discovery of DNA structure.', 9, 'Scribner', 'English'),
    ('The Structure of Scientific Revolutions', 'Thomas S. Kuhn', 15.00, '1962-01-01', '978-0226458120', 'Science', 'Paradigm shifts in science.', 8, 'University of Chicago Press', 'English'),
    ('The Art of Happiness', 'Dalai Lama & Howard Cutler', 13.25, '1998-10-01', '978-1573221115', 'Religion', 'A handbook for living.', 22, 'Riverhead Books', 'English'),
    ('The Power of Now', 'Eckhart Tolle', 14.99, '1997-08-01', '978-1577314806', 'Religion', 'A guide to spiritual enlightenment.', 30, 'New World Library', 'English'),
    ('The Tale of Kieu', 'Nguyen Du', 9.50, '1820-01-01', '978-6042091237', 'Poetry', 'Vietnamese epic poem.', 18, 'Nhà xuất bản Văn Học', 'Vietnamese'),
    ('Milk and Honey', 'Rupi Kaur', 11.25, '2014-11-04', '978-1449474256', 'Poetry', 'A collection of poetry.', 35, 'Andrews McMeel', 'English'),
    ('Leaves of Grass', 'Walt Whitman', 13.00, '1855-01-01', '978-0143039273', 'Poetry', 'A poetry collection by Walt Whitman.', 12, 'Penguin Classics', 'English'),
    ('The Waste Land', 'T.S. Eliot', 10.99, '1922-10-01', '978-0156948777', 'Poetry', 'A landmark modernist poem.', 10, 'Mariner Books', 'English'),
    ('Inferno', 'Dan Brown', 15.99, '2013-05-14', '978-1400079155', 'Mystery', 'A thriller set in Italy.', 20, 'Doubleday', 'English'),
    ('Gone Girl', 'Gillian Flynn', 14.99, '2012-06-05', '978-0307588371', 'Mystery', 'A psychological thriller.', 18, 'Crown', 'English'),
    ('The Girl with the Dragon Tattoo', 'Stieg Larsson', 12.75, '2005-08-01', '978-0307454546', 'Mystery', 'A gripping Swedish thriller.', 28, 'Norstedts Förlag', 'Swedish'),
    ('Murder on the Orient Express', 'Agatha Christie', 8.99, '1934-01-01', '978-0062073457', 'Mystery', 'A Hercule Poirot mystery.', 42, 'Collins Crime Club', 'English'),
    ('The Da Vinci Code', 'Dan Brown', 15.00, '2003-03-18', '978-0307474278', 'Mystery', 'A mystery thriller.', 40, 'Doubleday', 'English'),
    ('The Silent Patient', 'Alex Michaelides', 14.50, '2019-02-05', '978-1250301697', 'Mystery', 'A psychological thriller.', 35, 'Celadon Books', 'English'),
    ('The Three-Body Problem', 'Liu Cixin', 16.50, '2008-01-01', '978-0765382030', 'Science Fiction', 'A mind-bending Chinese sci-fi epic.', 20, 'Chongqing Press', 'Chinese'),
    ('Dune', 'Frank Herbert', 16.00, '1965-08-01', '978-0441172719', 'Science Fiction', 'A sci-fi epic.', 32, 'Chilton Books', 'English'),
    ('Foundation', 'Isaac Asimov', 14.99, '1951-06-01', '978-0553293357', 'Science Fiction', 'A science fiction classic.', 18, 'Spectra', 'English'),
    ('Neuromancer', 'William Gibson', 13.99, '1984-07-01', '978-0441569595', 'Science Fiction', 'A cyberpunk classic.', 15, 'Ace', 'English'),
    ('Snow Crash', 'Neal Stephenson', 12.99, '1992-06-01', '978-0553380958', 'Science Fiction', 'A postcyberpunk novel.', 14, 'Bantam', 'English'),
    ('Brave New World', 'Aldous Huxley', 13.50, '1932-01-01', '978-0060850524', 'Dystopian', 'A dystopian future society.', 20, 'Harper Perennial', 'English'),
    ('Fahrenheit 451', 'Ray Bradbury', 12.99, '1953-10-19', '978-1451673319', 'Dystopian', 'A future where books are outlawed.', 18, 'Simon & Schuster', 'English'),
    ('Norwegian Wood', 'Haruki Murakami', 13.99, '1987-09-04', '978-0375704024', 'Fiction', 'A poignant coming-of-age story.', 35, 'Kodansha', 'Japanese'),
    ('Kafka on the Shore', 'Haruki Murakami', 15.99, '2002-09-12', '978-1400079278', 'Fantasy', 'A surreal journey of self-discovery.', 22, 'Shinchosha', 'Japanese'),
    ('The Wind-Up Bird Chronicle', 'Haruki Murakami', 16.99, '1994-04-12', '978-0679775430', 'Fiction', 'A metaphysical detective story.', 20, 'Shinchosha', 'Japanese'),
    ('Cien años de soledad', 'Gabriel García Márquez', 14.99, '1967-05-30', '978-0307474728', 'Fiction', 'A magical realism masterpiece.', 30, 'Sudamericana', 'Spanish'),
    ('The Shadow of the Wind', 'Carlos Ruiz Zafón', 14.99, '2001-04-06', '978-0143034902', 'Fiction', 'A literary thriller set in postwar Barcelona.', 20, 'Planeta', 'Spanish'),
    ('Lonely Planet Vietnam', 'Lonely Planet', 20.50, '2023-07-11', '978-1788680660', 'Travel', 'Travel guide to Vietnam.', 15, 'Lonely Planet', 'English'),
    ('Dế Mèn Phiêu Lưu Ký', 'Tô Hoài', 7.50, '1941-01-01', '978-6042081234', 'Children', 'Vietnamese children''s classic.', 40, 'Kim Đồng', 'Vietnamese'),
    ('Nhật Ký Đặng Thùy Trâm', 'Đặng Thùy Trâm', 7.50, '2005-07-01', '978-6042081235', 'Biography', 'Vietnam War diary of a young doctor.', 25, 'Nhà xuất bản Hội Nhà Văn', 'Vietnamese'),
    ('Số Đỏ', 'Vũ Trọng Phụng', 8.00, '1936-01-01', '978-6049631233', 'Fiction', 'Vietnamese satirical classic.', 30, 'Nhà xuất bản Văn Học', 'Vietnamese'),
    ('Những Người Khốn Khổ', 'Victor Hugo', 10.50, '1862-01-01', '978-6042091234', 'Fiction', 'Vietnamese translation of Les Misérables.', 15, 'Nhà xuất bản Văn Học', 'Vietnamese'),
    ('Nhà Giả Kim', 'Paulo Coelho', 9.00, '1996-01-01', '978-6042091235', 'Fiction', 'Vietnamese translation of The Alchemist.', 18, 'Nhà xuất bản Văn Học', 'Vietnamese'),
    ('Đi Tìm Lẽ Sống', 'Viktor E. Frankl', 10.50, '1946-01-01', '978-6042091236', 'Non-Fiction', 'Vietnamese translation of Man''s Search for Meaning.', 20, 'Nhà xuất bản Tổng hợp', 'Vietnamese');

-- Insert Orders
INSERT INTO [Orders] ([UserId], [TotalAmount], [Status], [ShippingAddress], [PaymentMethod], [Notes])
VALUES
    (4, 28.98, 'Completed', '12 Baker St, London, England, E1 6AN', 'Credit Card', 'Deliver by 2025-07-01'),
    (5, 21.98, 'Pending', '34 Queen St, Auckland, 1010', 'Cash on Delivery', 'Morning delivery preferred.'),
    (6, 48.73, 'Completed', '56 Nanjing Rd, Shanghai, 200000', 'PayPal', 'Gift wrap requested.'),
    (7, 29.75, 'Processing', '78 Gran Via, Madrid, 28013', 'Bank Transfer', NULL),
    (8, 41.00, 'Completed', '90 Hauptstrasse, Berlin, 10115', 'Credit Card', 'Leave at front desk.'),
    (9, 22.74, 'Pending', '22 Rue de Rivoli, Paris, 75001', 'Cash on Delivery', 'Urgent delivery.'),
    (10, 60.25, 'Processing', '11 Via Roma, Rome, 00100', 'PayPal', NULL),
    (11, 22.50, 'Completed', '5-2-1 Ginza, Tokyo, 104-0061', 'Credit Card', 'Repeat customer.'),
    (12, 25.00, 'Pending', '8 Nowy Swiat, Warsaw, 00-001', 'Bank Transfer', 'Delayed due to stock.'),
    (13, 26.98, 'Completed', '77 Yonge St, Toronto, M5E 1J8', 'Credit Card', 'Pack carefully.'),
    (14, 35.50, 'Processing', '15 Nevsky Ave, Saint Petersburg, 191186', 'PayPal', 'Include receipt.'),
    (15, 18.99, 'Completed', '101 King St, Sydney, NSW, 2000', 'Credit Card', NULL),
    (16, 27.75, 'Pending', '202 Nile St, Cairo, 11511', 'Cash on Delivery', 'Deliver to neighbor if not home.'),
    (17, 39.99, 'Completed', '303 Hassan II Ave, Casablanca, 20000', 'Bank Transfer', 'Gift order.'),
    (18, 20.50, 'Processing', '404 Libertad, Buenos Aires, C1002', 'PayPal', NULL),
    (19, 24.75, 'Pending', '505 Drottninggatan, Stockholm, 111 60', 'Credit Card', 'Eco-friendly packaging.'),
    (20, 33.25, 'Completed', '606 Rue Saint-Paul, Montreal, H2Y 1H2', 'Credit Card', 'Urgent delivery.'),
    (4, 15.99, 'Pending', '12 Baker St, London, England, E1 6AN', 'Cash on Delivery', NULL),
    (5, 29.98, 'Completed', '34 Queen St, Auckland, 1010', 'PayPal', 'Gift wrap requested.'),
    (6, 22.50, 'Processing', '56 Nanjing Rd, Shanghai, 200000', 'Bank Transfer', 'Include invoice.');

-- Insert OrderDetails (using valid BookId values)
INSERT INTO [OrderDetails] ([OrderId], [BookId], [Quantity], [UnitPrice], [Subtotal])
VALUES
    (1, 1, 1, 12.99, 12.99), -- The Great Gatsby
    (1, 5, 1, 15.99, 15.99), -- The Hobbit
    (2, 3, 1, 11.99, 11.99), -- To Kill a Mockingbird
    (2, 4, 1, 13.99, 13.99), -- The Catcher in the Rye
    (3, 2, 2, 14.99, 29.98), -- 1984
    (3, 7, 1, 13.99, 13.99), -- The Brothers Karamazov
    (4, 8, 1, 15.50, 15.50), -- Don Quixote
    (4, 9, 1, 17.99, 17.99), -- The Divine Comedy
    (5, 10, 1, 19.99, 19.99), -- War and Peace
    (5, 11, 1, 12.50, 12.50), -- Madame Bovary
    (6, 12, 1, 14.99, 14.99), -- The Count of Monte Cristo
    (6, 13, 1, 13.75, 13.75), -- Anna Karenina
    (7, 14, 2, 16.50, 33.00), -- Ulysses
    (7, 15, 1, 11.99, 11.99), -- The Odyssey
    (8, 16, 1, 15.99, 15.99), -- Les Misérables
    (8, 17, 1, 10.99, 10.99), -- The Stranger
    (9, 18, 1, 9.99, 9.99), -- The Old Man and the Sea
    (9, 19, 1, 12.99, 12.99), -- Lolita
    (10, 20, 1, 11.50, 11.50), -- The Trial
    (10, 21, 1, 13.25, 13.25), -- The Sun Also Rises
    (11, 22, 1, 14.00, 14.00), -- The Sound and the Fury
    (11, 23, 1, 15.00, 15.00), -- The Grapes of Wrath
    (12, 24, 1, 12.75, 12.75), -- A Passage to India
    (12, 25, 1, 16.99, 16.99), -- The Magic Mountain
    (13, 26, 1, 13.50, 13.50), -- The Master and Margarita
    (13, 27, 1, 12.99, 12.99), -- The Remains of the Day
    (14, 28, 1, 13.99, 13.99), -- The Unbearable Lightness of Being
    (14, 29, 1, 9.50, 9.50), -- The Metamorphosis
    (15, 30, 1, 14.25, 14.25), -- The Idiot
    (15, 31, 1, 10.75, 10.75), -- The Death of Ivan Ilyich
    (16, 32, 1, 11.99, 11.99), -- The Plague
    (16, 33, 1, 13.50, 13.50), -- The House of the Spirits
    (17, 34, 1, 14.99, 14.99), -- The Name of the Wind
    (17, 35, 1, 12.00, 12.00), -- The Road to Wigan Pier
    (18, 36, 1, 10.99, 10.99), -- The Wind in the Willows
    (18, 37, 1, 13.75, 13.75), -- The Secret History
    (19, 38, 1, 12.50, 12.50), -- The Bell Jar
    (19, 39, 1, 9.99, 9.99), -- The Little House on the Prairie
    (20, 40, 1, 8.99, 8.99), -- The Tale of Peter Rabbit
    (20, 41, 1, 7.99, 7.99), -- The Cat in the Hat
    (20, 42, 1, 9.50, 9.50); -- The Giving Tree
    --CHECK DATA
    select * from [Books];
    select * from [Orders];
    select * from [OrderDetails];
    select * from [Users];
    select * from [Roles];
    select * from [Categories];
