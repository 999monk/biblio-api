using BIblioApi.models.DTOs;

namespace BIblioApi.services;

public interface IPrestamoService
{
    Task<PrestamoDTO> CreatePrestamoAsync(RegistrarPrestamoDTO nuevoPrestamo);
    Task<bool> DevolverPrestamoAsync(int prestamoId);
    Task<PrestamoDTO?> GetPrestamoByIdAsync(int id);
    Task<IEnumerable<PrestamoDTO>> GetAllPrestamosAsync();
    Task<IEnumerable<PrestamoDTO>> GetPrestamosBySocioAsync(int socioId);
    Task<IEnumerable<PrestamoDTO>> GetPrestamosActivosAsync();
    Task<bool> RenovarPrestamoAsync(int prestamoId);
}