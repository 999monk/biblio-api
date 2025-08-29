namespace BIblioApi.models.DTOs;

public class LibroDTO
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime? PublicationDate { get; set; }
    public int Amount { get; set; }
    public int CategoriaId { get; set; }
}