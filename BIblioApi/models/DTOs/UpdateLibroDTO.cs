using System.ComponentModel.DataAnnotations;

namespace BIblioApi.models.DTOs;

public class UpdateLibroDTO
{
    [Required]
    [StringLength(10)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Author { get; set; } = string.Empty;

    public DateTime? PublicationDate { get; set; }

    [Range(1, int.MaxValue)]
    public int Amount { get; set; }

    [Required]
    public int CategoriaId { get; set; }
}
