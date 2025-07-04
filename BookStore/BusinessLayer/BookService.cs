using Entities;
using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync(int pageIndex, int pageSize, string? searchKeyword = null)
        {
            if (pageIndex < 1) throw new ArgumentException("Page index must be greater than or equal to 1.", nameof(pageIndex));
            if (pageSize < 1) throw new ArgumentException("Page size must be greater than or equal to 1.", nameof(pageSize));

            Expression<Func<Book, bool>>? predicate = null;
            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                searchKeyword = searchKeyword.Trim().ToLower();
                predicate = b => b.Title.ToLower().Contains(searchKeyword) ||
                                (b.Author != null && b.Author.ToLower().Contains(searchKeyword)) ||
                                b.ISBN.ToLower().Contains(searchKeyword) ||
                                (b.Genre != null && b.Genre.ToLower().Contains(searchKeyword));
            }

            return await _bookRepository.GetAllAsync(
                pageIndex,
                pageSize,
                predicate,
                orderBy: q => q.OrderBy(b => b.Title),
                includeProperties: "OrderDetails");
        }

        public async Task<int> CountBooksAsync(string? searchKeyword = null)
        {
            Expression<Func<Book, bool>>? predicate = null;
            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                searchKeyword = searchKeyword.Trim().ToLower();
                predicate = b => b.Title.ToLower().Contains(searchKeyword) ||
                                (b.Author != null && b.Author.ToLower().Contains(searchKeyword)) ||
                                b.ISBN.ToLower().Contains(searchKeyword) ||
                                (b.Genre != null && b.Genre.ToLower().Contains(searchKeyword));
            }

            return await _bookRepository.CountAsync(predicate);
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("ID must be greater than zero.", nameof(id));
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task<Book?> GetBookByISBNAsync(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn)) throw new ArgumentException("ISBN cannot be empty.", nameof(isbn));
            return await _bookRepository.GetBookByISBNAsync(isbn.Trim());
        }

        public async Task<bool> AddBookAsync(Book book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            if (string.IsNullOrWhiteSpace(book.ISBN)) throw new ArgumentException("ISBN is required.", nameof(book.ISBN));
            if (book.Price < 0) throw new ArgumentException("Price cannot be negative.", nameof(book.Price));
            if (book.Stock < 0) throw new ArgumentException("Stock cannot be negative.", nameof(book.Stock));
            if (await _bookRepository.GetBookByISBNAsync(book.ISBN) != null)
            {
                throw new InvalidOperationException($"Book with ISBN {book.ISBN} already exists.");
            }

            book.CreatedAt = DateTime.UtcNow;
            book.UpdatedAt = DateTime.UtcNow;
            await _bookRepository.AddAsync(book);
            return await _bookRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateBookAsync(Book book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            if (book.Price < 0) throw new ArgumentException("Price cannot be negative.", nameof(book.Price));
            if (book.Stock < 0) throw new ArgumentException("Stock cannot be negative.", nameof(book.Stock));

            var existingBook = await _bookRepository.GetByIdAsync(book.Id)
                ?? throw new KeyNotFoundException($"Book with ID {book.Id} not found.");

            var bookWithSameIsbn = await _bookRepository.GetBookByISBNAsync(book.ISBN);
            if (bookWithSameIsbn != null && bookWithSameIsbn.Id != book.Id)
            {
                throw new InvalidOperationException($"Another book with ISBN {book.ISBN} already exists.");
            }

            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.Price = book.Price;
            existingBook.PublishedDate = book.PublishedDate;
            existingBook.ISBN = book.ISBN;
            existingBook.Genre = book.Genre;
            existingBook.Description = book.Description;
            existingBook.Stock = book.Stock;
            existingBook.Publisher = book.Publisher;
            existingBook.Language = book.Language;
            existingBook.UpdatedAt = DateTime.UtcNow;

            await _bookRepository.UpdateAsync(existingBook);
            return await _bookRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            // First check if the book exists
            var book = await _bookRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Book with ID {id} not found.");

            // Check if the book has any order details by querying OrderDetails directly
            // This avoids the DataReader conflict issue
            var orderDetails = await _bookRepository.GetAllAsync(
                filter: b => b.Id == id,
                includeProperties: "OrderDetails");
            
            var bookWithDetails = orderDetails.FirstOrDefault();
            if (bookWithDetails?.OrderDetails.Any() == true)
            {
                throw new InvalidOperationException("Cannot delete book with existing orders.");
            }

            await _bookRepository.DeleteAsync(id);
            return await _bookRepository.SaveChangesAsync() > 0;
        }

        public async Task<int> CountBooksInStockAsync()
        {
            return await _bookRepository.CountAsync(b => b.Stock > 0);
        }

        public async Task<IEnumerable<Book>> GetLowStockBooksAsync(int threshold)
        {
            if (threshold < 0) throw new ArgumentException("Threshold must be non-negative.", nameof(threshold));
            return await _bookRepository.GetAllAsync(
                1,
                int.MaxValue,
                b => b.Stock <= threshold && b.Stock >= 0,
                orderBy: q => q.OrderBy(b => b.Title));
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _bookRepository.GetAllAsync(1, int.MaxValue, null, null, "OrderDetails");
        }
    }
} 