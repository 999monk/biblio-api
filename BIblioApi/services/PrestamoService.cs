using BIblioApi.data;
using BIblioApi.models;
using BIblioApi.models.DTOs;
using BIblioApi.services;
using Hangfire;
using Microsoft.EntityFrameworkCore;

public class PrestamoService : IPrestamoService
{
    private readonly DataContext _context;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public PrestamoService(DataContext context, IBackgroundJobClient backgroundJobClient)
    {
        _context = context;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<PrestamoDTO> CreatePrestamoAsync(RegistrarPrestamoDTO nuevoPrestamo)
    {
        var libro = await _context.Libros.FindAsync(nuevoPrestamo.LibroId);
        if (libro == null || libro.Amount <= 0)
        {
            throw new Exception("El libro no está disponible.");
        }

        var prestamo = new Prestamo
        {
            LibroId = nuevoPrestamo.LibroId,
            SocioId = nuevoPrestamo.SocioId,
            LoanDate = DateTime.Now,
            ReturnDate = DateTime.Now.AddDays(15),
            Status = "Activo"
        };

        libro.Amount--;
        _context.Prestamos.Add(prestamo);
        await _context.SaveChangesAsync();
        
        _backgroundJobClient.Enqueue<IMailService>(x => x.EnviarMailConfirmacionPrestamo(prestamo.Id));

        var socio = await _context.Socios.FindAsync(prestamo.SocioId);

        return new PrestamoDTO
        {
            Id = prestamo.Id,
            LibroId = prestamo.LibroId,
            TituloLibro = libro.Title,
            SocioId = prestamo.SocioId,
            NombreSocio = socio?.Name,
            LoanDate = prestamo.LoanDate,
            ReturnDate = prestamo.ReturnDate,
            Status = prestamo.Status
        };
    }

    public async Task<bool> DevolverPrestamoAsync(int prestamoId)
    {
        var prestamo = await _context.Prestamos.FindAsync(prestamoId);
        if (prestamo == null) return false;

        prestamo.Status = "Devuelto";
        var libro = await _context.Libros.FindAsync(prestamo.LibroId);
        if (libro != null) libro.Amount++;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PrestamoDTO?> GetPrestamoByIdAsync(int id)
    {
        return await _context.Prestamos
            .Where(p => p.Id == id)
            .Select(p => new PrestamoDTO
            {
                Id = p.Id,
                LibroId = p.LibroId,
                TituloLibro = p.Libro.Title,
                SocioId = p.SocioId,
                NombreSocio = p.Socio.Name,
                LoanDate = p.LoanDate,
                ReturnDate = p.ReturnDate,
                Status = p.Status
            }).FirstOrDefaultAsync();
    }
    
    public async Task<IEnumerable<PrestamoDTO>> GetAllPrestamosAsync()
    {
        return await _context.Prestamos
            .Select(p => new PrestamoDTO
            {
                Id = p.Id,
                LibroId = p.LibroId,
                TituloLibro = p.Libro.Title,
                SocioId = p.SocioId,
                NombreSocio = p.Socio.Name,
                LoanDate = p.LoanDate,
                ReturnDate = p.ReturnDate,
                Status = p.Status
            }).ToListAsync();
    }

    public async Task<IEnumerable<PrestamoDTO>> GetPrestamosBySocioAsync(int socioId)
    {
        return await _context.Prestamos
            .Where(p => p.SocioId == socioId)
            .Select(p => new PrestamoDTO
            {
                Id = p.Id,
                LibroId = p.LibroId,
                TituloLibro = p.Libro.Title,
                SocioId = p.SocioId,
                NombreSocio = p.Socio.Name,
                LoanDate = p.LoanDate,
                ReturnDate = p.ReturnDate,
                Status = p.Status
            }).ToListAsync();
    }

    public async Task<IEnumerable<PrestamoDTO>> GetPrestamosActivosAsync()
    {
        return await _context.Prestamos
            .Where(p => p.Status == "Activo")
            .Select(p => new PrestamoDTO
            {
                Id = p.Id,
                LibroId = p.LibroId,
                TituloLibro = p.Libro.Title,
                SocioId = p.SocioId,
                NombreSocio = p.Socio.Name,
                LoanDate = p.LoanDate,
                ReturnDate = p.ReturnDate,
                Status = p.Status
            }).ToListAsync();
    }

    public async Task<bool> RenovarPrestamoAsync(int prestamoId)
    {
        var prestamo = await _context.Prestamos.FindAsync(prestamoId);
        if (prestamo == null || prestamo.Status != "Activo") return false;

        prestamo.ReturnDate = prestamo.ReturnDate.AddDays(15);
        await _context.SaveChangesAsync();
        return true;
    }
}