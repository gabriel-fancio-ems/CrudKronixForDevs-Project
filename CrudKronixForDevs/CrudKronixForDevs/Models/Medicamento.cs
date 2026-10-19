using System.ComponentModel.DataAnnotations;

namespace CrudKronixForDevs.Models;

/// <summary>
/// Representa um medicamento armazenado pela API.
/// </summary>
public class Medicamento
{
    /// <summary>
    /// Identificador único do medicamento.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Nome comercial do medicamento.
    /// </summary>
    [Required]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Substância responsável pelo efeito terapêutico.
    /// </summary>
    [Required]
    [MaxLength(150)]
    public string PrincipioAtivo { get; set; } = string.Empty;

    /// <summary>
    /// Fabricante responsável pela produção do medicamento.
    /// </summary>
    [Required]
    [MaxLength(150)]
    public string Fabricante { get; set; } = string.Empty;

    public Guid? FornecedorId { get; set; }

    public Fornecedor? Fornecedor { get; set; }

    /// <summary>
    /// Preço atual do medicamento em reais.
    /// </summary>
    [Range(0.01, 999999999.99)]
    public decimal Preco { get; set; }

    /// <summary>
    /// Quantidade disponível no estoque.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int QuantidadeEstoque { get; set; }

    /// <summary>
    /// Data após a qual o medicamento não deve ser comercializado.
    /// </summary>
    public DateOnly DataValidade { get; set; }
}
