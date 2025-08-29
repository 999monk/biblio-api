
using BIblioApi.data;
using BIblioApi.models;
using BIblioApi.models.DTOs;
using BIblioApi.services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BIblioApi.Tests.Services;

public class LoginServiceTests
{
    private readonly DbContextOptions<DataContext> _dbContextOptions;
    private readonly Mock<IConfiguration> _mockConfiguration;

    public LoginServiceTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _mockConfiguration = new Mock<IConfiguration>();
        
        // Setup the secret key for JWT
        _mockConfiguration.Setup(c => c["Jwt:SecretKey"]).Returns("a_super_secret_key_that_is_long_enough_to_be_valid");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnJwtToken_WhenCredentialsAreValid()
    {
        // Arrange
        var userPassword = "password123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userPassword);
        var user = new Usuario 
        { 
            Id = 1, 
            Email = "test@test.com", 
            Password = hashedPassword, 
            Perfil = "Admin"
        };

        await using (var context = new DataContext(_dbContextOptions))
        {
            await context.Usuarios.AddAsync(user);
            await context.SaveChangesAsync();
        }

        var loginDto = new LoginDTO { Email = "test@test.com", Password = userPassword };

        await using (var context = new DataContext(_dbContextOptions))
        {
            var loginService = new LoginService(context, _mockConfiguration.Object);

            // Act
            var result = await loginService.LoginAsync(loginDto);

            
            result.Should().NotBeNullOrEmpty();
            
            result.Split('.').Should().HaveCount(3);
        }
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenCredentialsAreInvalid()
    {
        // Arrange
        var userPassword = "password123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userPassword);
        var user = new Usuario 
        { 
            Id = 1, 
            Email = "test@test.com", 
            Password = hashedPassword, 
            Perfil = "Admin"
        };

        await using (var context = new DataContext(_dbContextOptions))
        {
            await context.Usuarios.AddAsync(user);
            await context.SaveChangesAsync();
        }

        var loginDto = new LoginDTO { Email = "test@test.com", Password = "wrongpassword" };

        await using (var context = new DataContext(_dbContextOptions))
        {
            var loginService = new LoginService(context, _mockConfiguration.Object);

            // Act
            var result = await loginService.LoginAsync(loginDto);

            // Assert
            result.Should().BeNull();
        }
    }
}
