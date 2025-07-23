using Xunit;
using Moq;
using BusinessLayer.Services;
using DataLayer;
using Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public class OrderServiceTests
{
    [Fact]
    public async Task GetOrderByIdAsync_ThrowsArgumentException_WhenIdIsZeroOrNegative()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockOrderDetailRepo = new Mock<IOrderDetailRepository>();
        var service = new OrderService(mockOrderRepo.Object, mockOrderDetailRepo.Object);
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetOrderByIdAsync(0));
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetOrderByIdAsync(-1));
    }

    [Fact]
    public async Task AddOrderAsync_ThrowsArgumentNullException_WhenOrderIsNull()
    {
        var mockOrderRepo = new Mock<IOrderRepository>();
        var mockOrderDetailRepo = new Mock<IOrderDetailRepository>();
        var service = new OrderService(mockOrderRepo.Object, mockOrderDetailRepo.Object);
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.AddOrderAsync(null));
    }

    [Fact]
    public async Task AddOrderAsync_ThrowsArgumentException_WhenOrderDetailsIsNullOrEmpty()
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
    public async Task AddOrderAsync_Success_WhenValidOrder()
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
} 