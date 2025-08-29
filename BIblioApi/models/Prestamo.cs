namespace BIblioApi.models;

public class Prestamo
{
    public int Id { get; set; }
    public int LibroId { get; set; }
    public Libro? Libro { get; set; }
    public int SocioId { get; set; }
    public Socio? Socio { get; set; }
    public DateTime LoanDate { get; set; } = DateTime.Now;
    public DateTime ReturnDate { get; set; } = DateTime.Now.AddDays(15);
    public string Status { get; set; } = "Activo";
}