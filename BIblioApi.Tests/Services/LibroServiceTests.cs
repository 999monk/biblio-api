
using BIblioApi.data;
using BIblioApi.models;
using BIblioApi.models.DTOs;
using BIblioApi.services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BIblioApi.Tests.Services;

public class LibroServiceTests
{
    private readonly DbContextOptions<DataContext> _dbContextOptions;

    public LibroServiceTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task CreateLibroAsync_ShouldCreateAndReturnLibro()
    {
        // Arrange
        var createLibroDto = new CreateLibroDTO
        {
            Title = "Nuevo Libro",
            Author = "Autor de Prueba",
            Code = "L001",
            PublicationDate = new DateTime(2023, 1, 1),
            Amount = 10,
            CategoriaId = 1
        };

        await using (var context = new DataContext(_dbContextOptions))
        {
            // Ensure the category exists
            context.Categorias.Add(new Categoria { Id = 1, Name = "Ficción" });
            await context.SaveChangesAsync();
        }

        await using (var context = new DataContext(_dbContextOptions))
        {
            var libroService = new LibroService(context);

            // Act
            var result = await libroService.CreateLibroAsync(createLibroDto);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be(createLibroDto.Title);
            result.Author.Should().Be(createLibroDto.Author);
        }

        // Verify in a separate context
        await using (var context = new DataContext(_dbContextOptions))
        {
            var libroInDb = await context.Libros.FirstOrDefaultAsync(l => l.Title == "Nuevo Libro");
            libroInDb.Should().NotBeNull();
            libroInDb.Amount.Should().Be(10);
        }
    }

    [Fact]
    public async Task SearchLibrosAsync_ShouldReturnMatchingLibros()
    {
        // Arrange
        await using (var context = new DataContext(_dbContextOptions))
        {
            await context.Libros.AddRangeAsync(
                new Libro { Id = 1, Title = "El Señor de los Anillos", Author = "J.R.R. Tolkien", Amount = 5, CategoriaId = 1 },
                new Libro { Id = 2, Title = "Cien Años de Soledad", Author = "Gabriel García Márquez", Amount = 3, CategoriaId = 1 },
                new Libro { Id = 3, Title = "El Código Da Vinci", Author = "Dan Brown", Amount = 7, CategoriaId = 1 }
            );
            await context.SaveChangesAsync();
        }

        await using (var context = new DataContext(_dbContextOptions))
        {
            var libroService = new LibroService(context);

            // Act
            var result = await libroService.SearchLibrosAsync("Señor");

            // Assert
            var libroDtos = result.ToList();
            libroDtos.Should().NotBeNull();
            libroDtos.Should().HaveCount(1);
            libroDtos.First().Title.Should().Be("El Señor de los Anillos");
        }
    }
}
