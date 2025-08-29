
using BIblioApi.data;
using BIblioApi.models;
using BIblioApi.models.DTOs;
using BIblioApi.services;
using FluentAssertions;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BIblioApi.Tests.Services;

public class PrestamoServiceTests
{
    private readonly DbContextOptions<DataContext> _dbContextOptions;

    public PrestamoServiceTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task CreatePrestamoAsync_ShouldThrowException_WhenBookIsNotAvailable()
    {
        // Arrange
        var libroSinStock = new Libro { Id = 1, Title = "Libro de Prueba", Amount = 0 };
        var registrarPrestamoDto = new RegistrarPrestamoDTO { LibroId = 1, SocioId = 1 };
        
        await using (var context = new DataContext(_dbContextOptions))
        {
            await context.Libros.AddAsync(libroSinStock);
            await context.SaveChangesAsync();
        }
        
        await using (var context = new DataContext(_dbContextOptions))
        {
            var mockBackgroundJobClient = new Mock<IBackgroundJobClient>();
            var prestamoService = new PrestamoService(context, mockBackgroundJobClient.Object);

            // Act
            Func<Task> act = async () => await prestamoService.CreatePrestamoAsync(registrarPrestamoDto);

            // Assert 
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("El libro no está disponible.");
        }
    }

    [Fact]
    public async Task CreatePrestamoAsync_ShouldCreatePrestamo_WhenBookIsAvailable()
    {
        // Arrange
        var libroConStock = new Libro { Id = 1, Title = "Libro Disponible", Amount = 5 };
        var socio = new Socio { Id = 1, Name = "Socio de Prueba", Email = "test@test.com" };
        var registrarPrestamoDto = new RegistrarPrestamoDTO { LibroId = 1, SocioId = 1 };
        var mockBackgroundJobClient = new Mock<IBackgroundJobClient>();

        await using (var context = new DataContext(_dbContextOptions))
        {
            await context.Libros.AddAsync(libroConStock);
            await context.Socios.AddAsync(socio);
            await context.SaveChangesAsync();
        }

        PrestamoDTO result;
        await using (var context = new DataContext(_dbContextOptions))
        {
            var prestamoService = new PrestamoService(context, mockBackgroundJobClient.Object);

            // Act
            result = await prestamoService.CreatePrestamoAsync(registrarPrestamoDto);
        }

        // Assert
        result.Should().NotBeNull();
        result.LibroId.Should().Be(libroConStock.Id);
        result.SocioId.Should().Be(socio.Id);
        result.Status.Should().Be("Activo");

        // Verificar el stock
        await using (var context = new DataContext(_dbContextOptions))
        {
            var libroEnDb = await context.Libros.FindAsync(libroConStock.Id);
            libroEnDb.Amount.Should().Be(4);
        }
        
        // Verificar que el trabajo de Hangfire fue encolado
        mockBackgroundJobClient.Verify(client => client.Create(
            It.Is<Job>(job => job.Method.Name == "EnviarMailConfirmacionPrestamo" && job.Type == typeof(IMailService)),
            It.IsAny<EnqueuedState>()),
            Times.Once);
    }
}
