
using BIblioApi.data;
using BIblioApi.models;
using BIblioApi.models.DTOs;
using BIblioApi.services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BIblioApi.Tests.Services;

public class SociosServiceTests
{
    private readonly DbContextOptions<DataContext> _dbContextOptions;

    public SociosServiceTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task CreateSocioAsync_ShouldCreateAndReturnSocio()
    {
        // Arrange
        var createSocioDto = new CreateSocioDTO
        {
            Name = "Nuevo Socio",
            Email = "nuevo@socio.com",
            Code = "S001"
        };

        await using (var context = new DataContext(_dbContextOptions))
        {
            var sociosService = new SociosService(context);

            // Act
            var result = await sociosService.CreateSocioAsync(createSocioDto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(createSocioDto.Name);
            result.Email.Should().Be(createSocioDto.Email);
            result.Code.Should().Be(createSocioDto.Code);
        }

        // Verify in a separate context
        await using (var context = new DataContext(_dbContextOptions))
        {
            var socioInDb = await context.Socios.FirstOrDefaultAsync(s => s.Name == "Nuevo Socio");
            socioInDb.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetAllSociosAsync_ShouldReturnAllSocios()
    {
        // Arrange
        await using (var context = new DataContext(_dbContextOptions))
        {
            await context.Socios.AddRangeAsync(
                new Socio { Id = 1, Name = "Socio 1", Email = "s1@test.com", Code = "S001" },
                new Socio { Id = 2, Name = "Socio 2", Email = "s2@test.com", Code = "S002" }
            );
            await context.SaveChangesAsync();
        }

        await using (var context = new DataContext(_dbContextOptions))
        {
            var sociosService = new SociosService(context);

            // Act
            var result = await sociosService.GetAllSociosAsync();

            // Assert
            var socioDtos = result.ToList();
            socioDtos.Should().NotBeNull();
            socioDtos.Should().HaveCount(2);
            socioDtos.First().Name.Should().Be("Socio 1");
            socioDtos.Last().Name.Should().Be("Socio 2");
        }
    }
}
