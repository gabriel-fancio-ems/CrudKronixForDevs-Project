namespace CrudKronixForDevs.DTOs;

/// <summary>
/// Representação pública de um medicamento retornada pela API.
/// </summary>
public class MedicamentoResponseDto
{
    /// <summary>
    /// Identificador único do medicamento.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome comercial do medicamento.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Princípio ativo do medicamento.
    /// </summary>
    public string PrincipioAtivo { get; set; } = string.Empty;

    /// <summary>
    /// Fabricante do medicamento.
    /// </summary>
    public string Fabricante { get; set; } = string.Empty;

    /// <summary>
    /// Preço atual do medicamento.
    /// </summary>
    public decimal Preco { get; set; }

    /// <summary>
    /// Quantidade disponível em estoque.
    /// </summary>
    public int QuantidadeEstoque { get; set; }

    /// <summary>
    /// Data de validade do medicamento.
    /// </summary>
    public DateOnly DataValidade { get; set; }
}
