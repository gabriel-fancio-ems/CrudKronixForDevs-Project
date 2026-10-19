using System.ComponentModel.DataAnnotations;

namespace CrudKronixForDevs.DTOs;

/// <summary>
/// Dados aceitos para criação ou alteração de um medicamento.
/// </summary>
public class MedicamentoRequestDto
{
    /// <summary>
    /// Nome comercial do medicamento.
    /// </summary>
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O nome deve possuir entre 2 e 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Princípio ativo do medicamento.
    /// </summary>
    [Required(ErrorMessage = "O princípio ativo é obrigatório.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O princípio ativo deve possuir entre 2 e 150 caracteres.")]
    public string PrincipioAtivo { get; set; } = string.Empty;

    /// <summary>
    /// Fabricante do medicamento.
    /// </summary>
    [Required(ErrorMessage = "O fabricante é obrigatório.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "O fabricante deve possuir entre 2 e 150 caracteres.")]
    public string Fabricante { get; set; } = string.Empty;

    /// <summary>
    /// Preço do medicamento em reais.
    /// </summary>
    [Range(0.01, 999999999.99, ErrorMessage = "O preço deve ser maior que zero.")]
    public decimal Preco { get; set; }

    /// <summary>
    /// Quantidade disponível no estoque.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "A quantidade em estoque não pode ser negativa.")]
    public int QuantidadeEstoque { get; set; }

    /// <summary>
    /// Data de validade do medicamento.
    /// </summary>
    [Required(ErrorMessage = "A data de validade é obrigatória.")]
    public DateOnly? DataValidade { get; set; }
}
