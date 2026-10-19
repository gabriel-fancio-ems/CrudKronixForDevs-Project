using CrudKronixForDevs.Models;
using Microsoft.AspNetCore.Http;

namespace CrudKronixForDevs.Services.Interfaces;

/// <summary>
/// Define as operações de armazenamento local de anexos.
/// </summary>
public interface IAnexoService
{
    /// <summary>
    /// Valida e salva um arquivo no armazenamento local.
    /// </summary>
    /// <param name="arquivo">Arquivo recebido em multipart/form-data.</param>
    /// <param name="cancellationToken">Token para cancelar a operação.</param>
    /// <returns>Metadados do anexo criado.</returns>
    Task<AnexoMetadata> EnviarAsync(IFormFile arquivo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista os metadados dos anexos armazenados.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar a operação.</param>
    /// <returns>Lista ordenada dos anexos.</returns>
    Task<IReadOnlyList<AnexoMetadata>> ListarAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Abre um anexo existente para leitura.
    /// </summary>
    /// <param name="id">Identificador do anexo.</param>
    /// <param name="cancellationToken">Token para cancelar a operação.</param>
    /// <returns>Stream e metadados, ou nulo quando o anexo não existir.</returns>
    Task<(Stream Stream, AnexoMetadata Metadata)?> ObterArquivoAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui o arquivo e seus metadados do armazenamento local.
    /// </summary>
    /// <param name="id">Identificador do anexo.</param>
    /// <param name="cancellationToken">Token para cancelar a operação.</param>
    /// <returns>Verdadeiro quando o anexo foi removido.</returns>
    Task<bool> ExcluirAsync(Guid id, CancellationToken cancellationToken = default);
}
