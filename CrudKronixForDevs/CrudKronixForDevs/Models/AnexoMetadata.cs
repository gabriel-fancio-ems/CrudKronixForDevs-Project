namespace CrudKronixForDevs.Models;

/// <summary>
/// Metadados persistidos para localizar e apresentar um anexo armazenado em disco.
/// </summary>
public class AnexoMetadata
{
    /// <summary>
    /// Identificador único do anexo.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome seguro usado fisicamente no diretório de armazenamento.
    /// </summary>
    public string NomeArquivoArmazenado { get; set; } = string.Empty;

    /// <summary>
    /// Nome original informado pelo cliente, usado apenas para exibição.
    /// </summary>
    public string NomeOriginal { get; set; } = string.Empty;

    /// <summary>
    /// Extensão original normalizada.
    /// </summary>
    public string Extensao { get; set; } = string.Empty;

    /// <summary>
    /// Tipo MIME usado na resposta HTTP.
    /// </summary>
    public string TipoConteudo { get; set; } = string.Empty;

    /// <summary>
    /// Tamanho do arquivo em bytes.
    /// </summary>
    public long TamanhoEmBytes { get; set; }

    /// <summary>
    /// Data e hora UTC do envio.
    /// </summary>
    public DateTime EnviadoEmUtc { get; set; }
}
