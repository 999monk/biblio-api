namespace BIblioApi.models.DTOs;

public class PrestamoDTO
{
    public int Id { get; set; }
    public int LibroId { get; set; }
    public string? TituloLibro { get; set; }  
    public int SocioId { get; set; }
    public string? NombreSocio { get; set; }   
    public DateTime LoanDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public string Status { get; set; } = "Pendiente";
}