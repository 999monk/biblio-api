using BIblioApi.models;
using BIblioApi.models.DTOs;

namespace BIblioApi.services;

public interface ILibroService
{
    Task<IEnumerable<LibroDTO>> GetAllLibrosAsync();
    Task<LibroDTO?> GetLibroByIdAsync(int id);
    Task<LibroDTO> CreateLibroAsync(CreateLibroDTO createLibroDto);
    Task<LibroDTO?> UpdateLibroAsync(int id, UpdateLibroDTO updateLibroDto);
    Task<bool> DeleteLibroAsync(int id);
    Task<IEnumerable<LibroDTO>> SearchLibrosAsync(string searchTerm);
}
