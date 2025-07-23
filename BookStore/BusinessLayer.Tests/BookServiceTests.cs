using Xunit;
using Moq;
using BusinessLayer.Services;
using DataLayer;
using Entities;
using System;
using System.Threading.Tasks;

public class BookServiceTests
{
    [Fact]
    public async Task AddBookAsync_ShouldReturnTrue_WhenBookIsValid()
    {
        var mockRepo = new Mock<IBookRepository>();
        mockRepo.Setup(r => r.GetBookByISBNAsync(It.IsAny<string>())).ReturnsAsync((Book?)null);
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);
        mockRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        var service = new BookService(mockRepo.Object);
        var book = new Book { ISBN = "1234567890", Price = 50, Stock = 10, Title = "Test Book" };
        var result = await service.AddBookAsync(book);
        Assert.True(result);
    }

    [Fact]
    public async Task AddBookAsync_ShouldThrowInvalidOperationException_WhenISBNExists()
    {
        var mockRepo = new Mock<IBookRepository>();
        mockRepo.Setup(r => r.GetBookByISBNAsync("1234567890")).ReturnsAsync(new Book { ISBN = "1234567890" });
        var service = new BookService(mockRepo.Object);
        var book = new Book { ISBN = "1234567890", Price = 50, Stock = 10, Title = "Test Book" };
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddBookAsync(book));
    }

    [Theory]
    [InlineData("__NULL__")]
    [InlineData("")]
    public async Task AddBookAsync_ShouldThrowArgumentException_WhenISBNIsNullOrEmpty(string isbn)
    {
        var mockRepo = new Mock<IBookRepository>();
        var service = new BookService(mockRepo.Object);
        var book = new Book { ISBN = isbn == "__NULL__" ? null : isbn, Price = 50, Stock = 10, Title = "Test Book" };
        await Assert.ThrowsAsync<ArgumentException>(() => service.AddBookAsync(book));
    }

    [Fact]
    public async Task GetBookByISBNAsync_ShouldReturnBook_WhenBookExists()
    {
        var mockRepo = new Mock<IBookRepository>();
        mockRepo.Setup(r => r.GetBookByISBNAsync("1234567890")).ReturnsAsync(new Book { ISBN = "1234567890", Title = "Test Book" });
        var service = new BookService(mockRepo.Object);
        var book = await service.GetBookByISBNAsync("1234567890");
        Assert.NotNull(book);
        Assert.Equal("Test Book", book.Title);
    }

    [Fact]
    public async Task GetBookByISBNAsync_ShouldReturnNull_WhenBookDoesNotExist()
    {
        var mockRepo = new Mock<IBookRepository>();
        mockRepo.Setup(r => r.GetBookByISBNAsync("notfound")).ReturnsAsync((Book?)null);
        var service = new BookService(mockRepo.Object);
        var book = await service.GetBookByISBNAsync("notfound");
        Assert.Null(book);
    }
} 