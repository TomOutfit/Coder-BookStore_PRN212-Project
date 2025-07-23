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
    public async Task GetUserByUsernameAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        var mockRoleRepo = new Mock<IRoleRepository>();
        mockRepo.Setup(r => r.GetUserByUsernameAsync("john.smith"))
                .ReturnsAsync(new User { Username = "john.smith", FirstName = "John" });
        var service = new UserService(mockRepo.Object, mockRoleRepo.Object);

        // Act
        var user = await service.GetUserByUsernameAsync("john.smith");

        // Assert
        Assert.NotNull(user);
        Assert.Equal("John", user.FirstName);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        var mockRoleRepo = new Mock<IRoleRepository>();
        mockRepo.Setup(r => r.GetUserByUsernameAsync("notfound")).ReturnsAsync((User)null);
        var service = new UserService(mockRepo.Object, mockRoleRepo.Object);

        // Act
        var user = await service.GetUserByUsernameAsync("notfound");

        // Assert
        Assert.Null(user);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetUserByUsernameAsync_ThrowsArgumentException_WhenUsernameIsNullOrEmpty(string username)
    {
        // Arrange
        var mockRepo = new Mock<IUserRepository>();
        var mockRoleRepo = new Mock<IRoleRepository>();
        var service = new UserService(mockRepo.Object, mockRoleRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetUserByUsernameAsync(username));
    }
} 