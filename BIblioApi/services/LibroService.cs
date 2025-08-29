using BIblioApi.data;
using BIblioApi.models;
using BIblioApi.models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BIblioApi.services;

public class LibroService : ILibroService
{
    private readonly DataContext _context;

    public LibroService(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LibroDTO>> GetAllLibrosAsync()
    {
        return await _context.Libros
            .Select(libro => ToLibroDTO(libro))
            .ToListAsync();
    }

    public async Task<LibroDTO?> GetLibroByIdAsync(int id)
    {
        var libro = await _context.Libros.FindAsync(id);
        return libro == null ? null : ToLibroDTO(libro);
    }

    public async Task<LibroDTO> CreateLibroAsync(CreateLibroDTO createLibroDto)
    {
        var libro = new Libro
        {
            Code = createLibroDto.Code,
            Title = createLibroDto.Title,
            Author = createLibroDto.Author,
            PublicationDate = createLibroDto.PublicationDate,
            Amount = createLibroDto.Amount,
            CategoriaId = createLibroDto.CategoriaId
        };

        _context.Libros.Add(libro);
        await _context.SaveChangesAsync();

        return ToLibroDTO(libro);
    }

    public async Task<LibroDTO?> UpdateLibroAsync(int id, UpdateLibroDTO updateLibroDto)
    {
        var libro = await _context.Libros.FindAsync(id);

        if (libro == null) return null;

        libro.Code = updateLibroDto.Code;
        libro.Title = updateLibroDto.Title;
        libro.Author = updateLibroDto.Author;
        libro.PublicationDate = updateLibroDto.PublicationDate;
        libro.Amount = updateLibroDto.Amount;
        libro.CategoriaId = updateLibroDto.CategoriaId;

        _context.Entry(libro).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return ToLibroDTO(libro);
    }

    public async Task<bool> DeleteLibroAsync(int id)
    {
        var libro = await _context.Libros.FindAsync(id);
        if (libro == null) return false;

        _context.Libros.Remove(libro);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<LibroDTO>> SearchLibrosAsync(string searchTerm)
    {
        return await _context.Libros
            .Where(l => l.Title.Contains(searchTerm) || l.Author.Contains(searchTerm))
            .Select(libro => ToLibroDTO(libro))
            .ToListAsync();
    }
    
    // mapeo
    private static LibroDTO ToLibroDTO(Libro libro)
    {
        return new LibroDTO
        {
            Id = libro.Id,
            Code = libro.Code,
            Title = libro.Title,
            Author = libro.Author,
            PublicationDate = libro.PublicationDate,
            Amount = libro.Amount,
            CategoriaId = libro.CategoriaId
        };
    }
}
