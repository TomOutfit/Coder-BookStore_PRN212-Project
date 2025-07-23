using Xunit;
using BusinessLayer.Services;
using DataLayer;
using Moq;
using Entities;
using System.Threading.Tasks;
using System;

public class UserServiceTests
{
    [Fact]
    public async Task GetUserByUsernameAsync_ShouldReturnUser_WhenUserExists()
    {
        var mockRepo = new Mock<IUserRepository>();
        var mockRoleRepo = new Mock<IRoleRepository>();
        mockRepo.Setup(r => r.GetUserByUsernameAsync("john.smith"))
                .ReturnsAsync(new User { Username = "john.smith", FirstName = "John" });
        var service = new UserService(mockRepo.Object, mockRoleRepo.Object);
        var user = await service.GetUserByUsernameAsync("john.smith");
        Assert.NotNull(user);
        Assert.Equal("John", user.FirstName);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        var mockRepo = new Mock<IUserRepository>();
        var mockRoleRepo = new Mock<IRoleRepository>();
        mockRepo.Setup(r => r.GetUserByUsernameAsync("notfound")).ReturnsAsync((User)null);
        var service = new UserService(mockRepo.Object, mockRoleRepo.Object);
        var user = await service.GetUserByUsernameAsync("notfound");
        Assert.Null(user);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetUserByUsernameAsync_ShouldThrowArgumentException_WhenUsernameIsNullOrEmpty(string username)
    {
        var mockRepo = new Mock<IUserRepository>();
        var mockRoleRepo = new Mock<IRoleRepository>();
        var service = new UserService(mockRepo.Object, mockRoleRepo.Object);
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetUserByUsernameAsync(username));
    }
} 