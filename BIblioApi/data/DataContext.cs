using BIblioApi.models;
using Microsoft.EntityFrameworkCore;

namespace BIblioApi.data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Socio> Socios { get; set; }
    public DbSet<Libro> Libros { get; set; }
    public DbSet<Prestamo> Prestamos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
}