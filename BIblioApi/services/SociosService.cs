using BIblioApi.data;
using BIblioApi.models;
using BIblioApi.models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BIblioApi.services;

public class SociosService : ISociosService
{
    private readonly DataContext _context;

    public SociosService(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SocioDTO>> GetAllSociosAsync()
    {
        return await _context.Socios
            .Select(socio => ToSocioDTO(socio))
            .ToListAsync();
    }

    public async Task<SocioDTO?> GetSocioByIdAsync(int id)
    {
        var socio = await  _context.Socios.FindAsync(id);
        return socio == null ? null : ToSocioDTO(socio);
    }

    public async Task<SocioDTO> CreateSocioAsync(CreateSocioDTO createSocioDto)
    {
        var socio = new Socio
        {
            Code = createSocioDto.Code,
            Email = createSocioDto.Email,
            Name = createSocioDto.Name,
        };
        
        await _context.Socios.AddAsync(socio);
        await _context.SaveChangesAsync();
        return ToSocioDTO(socio);
    }

    public async Task<SocioDTO?> UpdateSocioAsync(int id, UpdateSocioDTO updateSocioDto)
    {
        var socio =  await _context.Socios.FindAsync(id);
        if (socio == null) return null;
        
        socio.Code = updateSocioDto.Code;
        socio.Email = updateSocioDto.Email;
        socio.Name = updateSocioDto.Name;
        
        _context.Entry(socio).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return ToSocioDTO(socio);
    }

    public async Task<bool> DeleteSocioAsync(int id)
    {
        var socio = await  _context.Socios.FindAsync(id);
        if (socio == null) return false;
        
        _context.Socios.Remove(socio);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<SocioDTO>> SearchSocioAsync(string searchTerm)
    {
        return await  _context.Socios
            .Where(s => s.Name.ToLower().Contains(searchTerm.ToLower()))
            .Select(socio =>  ToSocioDTO(socio))
            .ToListAsync();
    }
    
    // mapeo
    private static SocioDTO ToSocioDTO(Socio socio)
    {
        return new SocioDTO
        {
            Id = socio.Id,
            Name = socio.Name,
            Code =  socio.Code,
            Email = socio.Email,
        };
    }
}