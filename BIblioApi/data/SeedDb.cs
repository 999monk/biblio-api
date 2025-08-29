using System.Net;
using BIblioApi.models;
using Microsoft.EntityFrameworkCore;

namespace BIblioApi.data;

public class SeedDb(DataContext context)
{
    public async Task SeedAsync()
    {
        await context.Database.EnsureCreatedAsync();
        await CheckCategoriasAsync();
        await CheckUsersAsync("newUser999", "mailPrueba@gmail.com", "asd.123", "Admin");
    }

    private async Task<Usuario> CheckUsersAsync(string name, string email, string password, string perfil)
    {
        var existingUser = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        if (existingUser != null)
        {
            return  existingUser;
        }
        
        Usuario user = new()
        {
            Email = email,
            Username = name,
            Perfil = perfil,
            Password = password,
        };
        
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        
        context.Usuarios.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    private async Task CheckCategoriasAsync()
    {
        if (!context.Categorias.Any())
        {
            var categorias = new List<Categoria>
            {
                new Categoria { Name = "Arte" },
            new Categoria { Name = "Autoayuda" },
            new Categoria { Name = "Biografía" },
            new Categoria { Name = "Ciencia" },
            new Categoria { Name = "Ciencia Ficción" },
            new Categoria { Name = "Cine y Teatro" },
            new Categoria { Name = "Clásicos" },
            new Categoria { Name = "Cómics y Novela Gráfica" },
            new Categoria { Name = "Crimen y Misterio" },
            new Categoria { Name = "Cuentos" },
            new Categoria { Name = "Derecho" },
            new Categoria { Name = "Desarrollo Personal" },
            new Categoria { Name = "Diccionarios y Referencia" },
            new Categoria { Name = "Didácticos / Textos Escolares" },
            new Categoria { Name = "Divulgación Científica" },
            new Categoria { Name = "Drama" },
            new Categoria { Name = "Economía" },
            new Categoria { Name = "Ensayo" },
            new Categoria { Name = "Espiritualidad" },
            new Categoria { Name = "Fantasía" },
            new Categoria { Name = "Filosofía" },
            new Categoria { Name = "Historia" },
            new Categoria { Name = "Humor" },
            new Categoria { Name = "Infantil" },
            new Categoria { Name = "Juvenil" },
            new Categoria { Name = "Literatura Contemporánea" },
            new Categoria { Name = "Literatura Universal" },
            new Categoria { Name = "Novela" },
            new Categoria { Name = "Novela Histórica" },
            new Categoria { Name = "Novela Negra" },
            new Categoria { Name = "Poesía" },
            new Categoria { Name = "Política" },
            new Categoria { Name = "Psicología" },
            new Categoria { Name = "Religión" },
            new Categoria { Name = "Romance" },
            new Categoria { Name = "Salud y Bienestar" },
            new Categoria { Name = "Sociología" },
            new Categoria { Name = "Tecnología" },
            new Categoria { Name = "Terror" },
            new Categoria { Name = "Viajes y Aventuras" }
            };
        

        context.Categorias.AddRange(categorias);
        await context.SaveChangesAsync();
        }
    }
}