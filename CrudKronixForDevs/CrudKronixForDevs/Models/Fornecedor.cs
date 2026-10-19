using System.ComponentModel.DataAnnotations;

namespace CrudKronixForDevs.Models;

public class Fornecedor
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;
}
