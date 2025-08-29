using BIblioApi.models.DTOs;

namespace BIblioApi.services;

public interface ISociosService
{
    Task<IEnumerable<SocioDTO>> GetAllSociosAsync();
    Task<SocioDTO?> GetSocioByIdAsync(int id);
    Task<SocioDTO> CreateSocioAsync(CreateSocioDTO createSocioDto);
    Task<SocioDTO?> UpdateSocioAsync(int id, UpdateSocioDTO updateSocioDto);
    Task<bool> DeleteSocioAsync(int id);
    Task<IEnumerable<SocioDTO>> SearchSocioAsync(string searchTerm);
}