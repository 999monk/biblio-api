namespace BIblioApi.models.DTOs;

public class DevolverPrestamoDTO
{
    public int PrestamoId { get; set; }
    public DateTime ReturnDate { get; set; } = DateTime.Now;
}