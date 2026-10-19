using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using CrudKronixForDevs.Models;
using CrudKronixForDevs.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CrudKronixForDevs.Services;

/// <summary>
/// Implementa o armazenamento local de anexos e seus metadados.
/// </summary>
public class AnexoService : IAnexoService
{
    private const long TamanhoMaximoEmBytes = 10 * 1024 * 1024;
    private const string ExtensaoMetadados = ".metadata.json";
    private static readonly HashSet<string> ExtensoesBloqueadas = new(StringComparer.OrdinalIgnoreCase)
    {
        ".apk", ".app", ".bat", ".cmd", ".com", ".dll", ".dmg", ".exe", ".hta", ".jar", ".js", ".msi", ".ps1", ".scr", ".sh", ".so", ".vbe", ".vbs"
    };

    private readonly string _diretorioAnexos;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    /// <summary>
    /// Inicializa o serviço e garante que o diretório local exista.
    /// </summary>
    /// <param name="environment">Ambiente usado para obter a raiz do projeto.</param>
    public AnexoService(IWebHostEnvironment environment)
    {
        _diretorioAnexos = Path.Combine(environment.ContentRootPath, "Storage", "Anexos");
        Directory.CreateDirectory(_diretorioAnexos);
    }

    /// <inheritdoc />
    public async Task<AnexoMetadata> EnviarAsync(IFormFile arquivo, CancellationToken cancellationToken = default)
    {
        ValidarArquivo(arquivo);

        var nomeOriginal = Path.GetFileName(arquivo.FileName);
        var extensao = Path.GetExtension(nomeOriginal).ToLowerInvariant();
        var id = Guid.NewGuid();
        var nomeArquivoArmazenado = $"{id:N}{extensao}";
        var caminhoArquivo = ObterCaminhoSeguro(nomeArquivoArmazenado);
        var caminhoMetadados = ObterCaminhoMetadados(id);
        var metadados = new AnexoMetadata
        {
            Id = id,
            NomeArquivoArmazenado = nomeArquivoArmazenado,
            NomeOriginal = nomeOriginal,
            Extensao = extensao,
            TipoConteudo = string.IsNullOrWhiteSpace(arquivo.ContentType) ? "application/octet-stream" : arquivo.ContentType,
            TamanhoEmBytes = arquivo.Length,
            EnviadoEmUtc = DateTime.UtcNow
        };

        try
        {
            await using (var destino = new FileStream(caminhoArquivo, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, FileOptions.Asynchronous))
            {
                await arquivo.CopyToAsync(destino, cancellationToken);
            }

            await using (var arquivoMetadados = new FileStream(caminhoMetadados, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, FileOptions.Asynchronous))
            {
                await JsonSerializer.SerializeAsync(arquivoMetadados, metadados, _jsonOptions, cancellationToken);
            }

            return metadados;
        }
        catch
        {
            ExcluirSeExistir(caminhoArquivo);
            ExcluirSeExistir(caminhoMetadados);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AnexoMetadata>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var anexos = new List<AnexoMetadata>();
        var arquivosMetadados = Directory.EnumerateFiles(_diretorioAnexos, $"*{ExtensaoMetadados}");

        foreach (var caminhoMetadados in arquivosMetadados)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await using var arquivoMetadados = new FileStream(caminhoMetadados, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, FileOptions.Asynchronous);
            var metadados = await JsonSerializer.DeserializeAsync<AnexoMetadata>(arquivoMetadados, _jsonOptions, cancellationToken);

            if (metadados is not null && File.Exists(ObterCaminhoSeguro(metadados.NomeArquivoArmazenado)))
            {
                anexos.Add(metadados);
            }
        }

        return anexos.OrderByDescending(anexo => anexo.EnviadoEmUtc).ToList();
    }

    /// <inheritdoc />
    public async Task<(Stream Stream, AnexoMetadata Metadata)?> ObterArquivoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ValidarId(id);
        var metadados = await LerMetadadosAsync(id, cancellationToken);

        if (metadados is null)
        {
            return null;
        }

        var caminhoArquivo = ObterCaminhoSeguro(metadados.NomeArquivoArmazenado);
        if (!File.Exists(caminhoArquivo))
        {
            return null;
        }

        var stream = new FileStream(caminhoArquivo, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, FileOptions.Asynchronous | FileOptions.SequentialScan);
        return (stream, metadados);
    }

    /// <inheritdoc />
    public async Task<bool> ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ValidarId(id);
        var metadados = await LerMetadadosAsync(id, cancellationToken);

        if (metadados is null)
        {
            return false;
        }

        ExcluirSeExistir(ObterCaminhoSeguro(metadados.NomeArquivoArmazenado));
        ExcluirSeExistir(ObterCaminhoMetadados(id));
        return true;
    }

    /// <summary>
    /// Valida tamanho, nome e extensão antes de gravar o arquivo.
    /// </summary>
    /// <param name="arquivo">Arquivo recebido do cliente.</param>
    private static void ValidarArquivo(IFormFile? arquivo)
    {
        if (arquivo is null || arquivo.Length == 0)
        {
            throw new ValidationException("Envie um arquivo não vazio.");
        }

        if (arquivo.Length > TamanhoMaximoEmBytes)
        {
            throw new ValidationException("O arquivo não pode ultrapassar 10 MB.");
        }

        var nomeOriginal = Path.GetFileName(arquivo.FileName);
        if (string.IsNullOrWhiteSpace(nomeOriginal))
        {
            throw new ValidationException("O arquivo precisa possuir um nome válido.");
        }

        var extensao = Path.GetExtension(nomeOriginal);
        if (ExtensoesBloqueadas.Contains(extensao))
        {
            throw new ValidationException("Esse tipo de arquivo não é permitido por segurança.");
        }
    }

    /// <summary>
    /// Lê os metadados de um anexo usando somente o identificador validado.
    /// </summary>
    /// <param name="id">Identificador do anexo.</param>
    /// <param name="cancellationToken">Token para cancelar a leitura.</param>
    /// <returns>Metadados encontrados ou nulo.</returns>
    private async Task<AnexoMetadata?> LerMetadadosAsync(Guid id, CancellationToken cancellationToken)
    {
        var caminhoMetadados = ObterCaminhoMetadados(id);
        if (!File.Exists(caminhoMetadados))
        {
            return null;
        }

        await using var arquivoMetadados = new FileStream(caminhoMetadados, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, FileOptions.Asynchronous);
        return await JsonSerializer.DeserializeAsync<AnexoMetadata>(arquivoMetadados, _jsonOptions, cancellationToken);
    }

    /// <summary>
    /// Gera o caminho de metadados a partir de um identificador confiável.
    /// </summary>
    /// <param name="id">Identificador do anexo.</param>
    /// <returns>Caminho completo do arquivo JSON de metadados.</returns>
    private string ObterCaminhoMetadados(Guid id)
    {
        return Path.Combine(_diretorioAnexos, $"{id:N}{ExtensaoMetadados}");
    }

    /// <summary>
    /// Monta um caminho seguro e rejeita nomes que tentem escapar do diretório configurado.
    /// </summary>
    /// <param name="nomeArquivo">Nome físico gerado pela aplicação.</param>
    /// <returns>Caminho completo do arquivo.</returns>
    private string ObterCaminhoSeguro(string nomeArquivo)
    {
        var nomeNormalizado = Path.GetFileName(nomeArquivo);
        if (!string.Equals(nomeNormalizado, nomeArquivo, StringComparison.Ordinal))
        {
            throw new ValidationException("O nome físico do arquivo é inválido.");
        }

        return Path.Combine(_diretorioAnexos, nomeNormalizado);
    }

    /// <summary>
    /// Garante que a rota não consulte o arquivo de outro identificador.
    /// </summary>
    /// <param name="id">Identificador informado pela rota.</param>
    private static void ValidarId(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException("O identificador do anexo é inválido.");
        }
    }

    /// <summary>
    /// Remove um arquivo sem falhar quando ele já não estiver presente.
    /// </summary>
    /// <param name="caminho">Caminho do arquivo que será removido.</param>
    private static void ExcluirSeExistir(string caminho)
    {
        if (File.Exists(caminho))
        {
            File.Delete(caminho);
        }
    }
}
