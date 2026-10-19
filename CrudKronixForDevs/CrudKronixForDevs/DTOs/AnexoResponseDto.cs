namespace CrudKronixForDevs.DTOs;

/// <summary>
/// Metadados públicos de um arquivo armazenado pela API.
/// </summary>
public class AnexoResponseDto
{
    /// <summary>
    /// Identificador usado nas rotas de consulta e exclusão.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome original enviado pelo cliente.
    /// </summary>
    public string NomeOriginal { get; set; } = string.Empty;

    /// <summary>
    /// Extensão original do arquivo.
    /// </summary>
    public string Extensao { get; set; } = string.Empty;

    /// <summary>
    /// Tipo MIME informado pelo cliente ou inferido pela extensão.
    /// </summary>
    public string TipoConteudo { get; set; } = string.Empty;

    /// <summary>
    /// Tamanho do arquivo em bytes.
    /// </summary>
    public long TamanhoEmBytes { get; set; }

    /// <summary>
    /// Data e hora UTC em que o arquivo foi salvo.
    /// </summary>
    public DateTime EnviadoEmUtc { get; set; }

    /// <summary>
    /// URL que pode ser usada pelo front-end para visualizar ou baixar o arquivo.
    /// </summary>
    public string Url { get; set; } = string.Empty;
}
