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
    public async Task GetBookByIdAsync_ThrowsArgumentException_WhenIdIsZeroOrNegative()
    {
        var mockRepo = new Mock<IBookRepository>();
        var service = new BookService(mockRepo.Object);
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetBookByIdAsync(0));
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetBookByIdAsync(-1));
    }

    [Fact]
    public async Task AddBookAsync_ThrowsArgumentException_WhenISBNIsEmpty()
    {
        var mockRepo = new Mock<IBookRepository>();
        var service = new BookService(mockRepo.Object);
        var book = new Book { ISBN = "", Price = 10, Stock = 1 };
        await Assert.ThrowsAsync<ArgumentException>(() => service.AddBookAsync(book));
    }

    [Fact]
    public async Task AddBookAsync_ThrowsArgumentException_WhenPriceIsNegative()
    {
        var mockRepo = new Mock<IBookRepository>();
        var service = new BookService(mockRepo.Object);
        var book = new Book { ISBN = "123", Price = -1, Stock = 1 };
        await Assert.ThrowsAsync<ArgumentException>(() => service.AddBookAsync(book));
    }

    [Fact]
    public async Task AddBookAsync_Success_WhenValidBook()
    {
        var mockRepo = new Mock<IBookRepository>();
        mockRepo.Setup(r => r.GetBookByISBNAsync(It.IsAny<string>())).ReturnsAsync((Book)null);
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);
        mockRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        var service = new BookService(mockRepo.Object);
        var book = new Book { ISBN = "123", Price = 10, Stock = 1 };
        var result = await service.AddBookAsync(book);
        Assert.True(result);
    }
} 