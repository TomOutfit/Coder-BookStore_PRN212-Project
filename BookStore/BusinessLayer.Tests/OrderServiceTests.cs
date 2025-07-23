using Xunit;
using Moq;
using BusinessLayer.Services;
using DataLayer;
using Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class OrderServiceTests
{
    [Fact]
    public async Task AddOrderAsync_ShouldReturnTrue_WhenOrderIsValid()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockOrderDetailRepo = new Mock<IOrderDetailRepository>();
        mockOrderRepo.Setup(r => r.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
        mockOrderRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        var service = new OrderService(mockOrderRepo.Object, mockOrderDetailRepo.Object);
        var order = new Order
        {
            OrderDetails = new List<OrderDetail>
            {
                new OrderDetail { Quantity = 2, Book = new Book { Price = 10 } },
                new OrderDetail { Quantity = 1, Book = new Book { Price = 20 } }
            }
        };
        var result = await service.AddOrderAsync(order);
        Assert.True(result);
        Assert.Equal(40, order.TotalAmount);
    }

    [Fact]
    public async Task AddOrderAsync_ShouldThrowArgumentNullException_WhenOrderIsNull()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockOrderDetailRepo = new Mock<IOrderDetailRepository>();
        var service = new OrderService(mockOrderRepo.Object, mockOrderDetailRepo.Object);
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddOrderAsync((Order?)null!));
    }

    [Fact]
    public async Task AddOrderAsync_ShouldThrowArgumentException_WhenOrderDetailsIsNullOrEmpty()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockOrderDetailRepo = new Mock<IOrderDetailRepository>();
        var service = new OrderService(mockOrderRepo.Object, mockOrderDetailRepo.Object);
        var order1 = new Order { OrderDetails = null };
        var order2 = new Order { OrderDetails = new List<OrderDetail>() };
        await Assert.ThrowsAsync<ArgumentException>(() => service.AddOrderAsync(order1));
        await Assert.ThrowsAsync<ArgumentException>(() => service.AddOrderAsync(order2));
    }

    [Fact]
    public async Task GetOrderByIdAsync_ShouldReturnOrder_WhenOrderExists()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockOrderDetailRepo = new Mock<IOrderDetailRepository>();
        mockOrderRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<string>())).ReturnsAsync((Order?)new Order { Id = 1 });
        var service = new OrderService(mockOrderRepo.Object, mockOrderDetailRepo.Object);
        var order = await service.GetOrderByIdAsync(1);
        Assert.NotNull(order);
        Assert.Equal(1, order.Id);
    }

    [Fact]
    public async Task GetOrderByIdAsync_ShouldThrowArgumentException_WhenIdIsZeroOrNegative()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockOrderDetailRepo = new Mock<IOrderDetailRepository>();
        var service = new OrderService(mockOrderRepo.Object, mockOrderDetailRepo.Object);
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetOrderByIdAsync(0));
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetOrderByIdAsync(-1));
    }
} 