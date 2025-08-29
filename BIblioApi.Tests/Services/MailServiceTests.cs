
using BIblioApi.Configuration;
using BIblioApi.data;
using BIblioApi.models;
using BIblioApi.services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using MimeKit;

namespace BIblioApi.Tests.Services;

public class MailServiceTests
{
    private readonly DbContextOptions<DataContext> _dbContextOptions;
    private readonly Mock<IEmailSender> _mockEmailSender;
    private readonly IOptions<MailSettings> _mockMailSettings;

    public MailServiceTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _mockEmailSender = new Mock<IEmailSender>();
        
        var mailSettings = new MailSettings
        {
            SenderName = "Test Sender",
            SenderEmail = "sender@test.com"
        };
        _mockMailSettings = Options.Create(mailSettings);
    }

    private async Task SeedDatabase(Socio socio, Libro libro, Prestamo prestamo)
    {
        await using var context = new DataContext(_dbContextOptions);
        context.Categorias.Add(new Categoria { Id = 1, Name = "Test" });
        await context.Socios.AddAsync(socio);
        await context.Libros.AddAsync(libro);
        await context.Prestamos.AddAsync(prestamo);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task EnviarMailConfirmacionPrestamo_Should_CallEmailSender_WhenPrestamoExists()
    {
        // Arrange
        var socio = new Socio { Id = 1, Name = "Socio de Prueba", Email = "test@test.com" };
        var libro = new Libro { Id = 1, Title = "Libro de Prueba", Amount = 1, CategoriaId = 1 };
        var prestamo = new Prestamo { Id = 1, SocioId = 1, LibroId = 1, ReturnDate = DateTime.Now.AddDays(10), Socio = socio, Libro = libro };
        await SeedDatabase(socio, libro, prestamo);

        await using var context = new DataContext(_dbContextOptions);
        var mailService = new MailService(context, _mockEmailSender.Object, _mockMailSettings);

        // Act
        await mailService.EnviarMailConfirmacionPrestamo(1);

        // Assert
        _mockEmailSender.Verify(sender => sender.SendAsync(It.IsAny<MimeMessage>()), Times.Once);
    }

    [Fact]
    public async Task EnviarMailConfirmacionPrestamo_Should_SendEmailWithCorrectContent_WhenPrestamoExists()
    {
        // Arrange
        var socio = new Socio { Id = 1, Name = "Socio de Prueba", Email = "recipient@test.com" };
        var libro = new Libro { Id = 1, Title = "Libro de Prueba", Amount = 1, CategoriaId = 1 };
        var prestamo = new Prestamo { Id = 1, SocioId = 1, LibroId = 1, ReturnDate = DateTime.Now.AddDays(10), Socio = socio, Libro = libro };
        await SeedDatabase(socio, libro, prestamo);
        
        MimeMessage? capturedMessage = null;
        _mockEmailSender.Setup(s => s.SendAsync(It.IsAny<MimeMessage>()))
            .Callback<MimeMessage>(message => capturedMessage = message);

        await using var context = new DataContext(_dbContextOptions);
        var mailService = new MailService(context, _mockEmailSender.Object, _mockMailSettings);

        // Act
        await mailService.EnviarMailConfirmacionPrestamo(1);

        // Assert
        Assert.NotNull(capturedMessage);
        Assert.Equal("Confirmación de Préstamo de Libro", capturedMessage.Subject);
        Assert.Equal("recipient@test.com", capturedMessage.To.Mailboxes.First().Address);
        Assert.Contains("<h1>Confirmación de Préstamo</h1>", ((TextPart)capturedMessage.Body).Text);
    }

    [Fact]
    public async Task EnviarMailConfirmacionPrestamo_Should_NotCallEmailSender_WhenPrestamoDoesNotExist()
    {
        // Arrange
        await using var context = new DataContext(_dbContextOptions); 
        var mailService = new MailService(context, _mockEmailSender.Object, _mockMailSettings);

        // Act
        await mailService.EnviarMailConfirmacionPrestamo(999); 
        // Assert
        _mockEmailSender.Verify(sender => sender.SendAsync(It.IsAny<MimeMessage>()), Times.Never);
    }
}
