namespace BIblioApi.models;

public class Libro
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime? PublicationDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int Amount {  get; set; }
    public int CategoriaId { get; set; }
    
    public Categoria? Categoria { get; set; }
}