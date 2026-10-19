using System.ComponentModel.DataAnnotations;
using CrudKronixForDevs.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;

namespace CrudKronixForDevs.Tests.Services;

/// <summary>
/// Testa as regras de armazenamento local de anexos.
/// </summary>
public class AnexoServiceTests : IDisposable
{
    private readonly string _diretorioTemporario = Path.Combine(Path.GetTempPath(), $"CrudKronixForDevs-{Guid.NewGuid():N}");
    private readonly AnexoService _service;

    /// <summary>
    /// Inicializa o serviço apontando para uma pasta temporária isolada.
    /// </summary>
    public AnexoServiceTests()
    {
        var environment = new Mock<IWebHostEnvironment>();
        environment.SetupGet(item => item.ContentRootPath).Returns(_diretorioTemporario);
        _service = new AnexoService(environment.Object);
    }

    /// <summary>
    /// Confirma que um PDF válido é salvo com identificador próprio e metadados.
    /// </summary>
    [Fact]
    public async Task EnviarAsync_DeveSalvarArquivoValido()
    {
        var arquivo = CriarArquivo("documento.pdf", "application/pdf", "conteudo pdf");

        var resultado = await _service.EnviarAsync(arquivo);

        Assert.Equal("documento.pdf", resultado.NomeOriginal);
        Assert.Equal(".pdf", resultado.Extensao);
        Assert.True(File.Exists(Path.Combine(_diretorioTemporario, "Storage", "Anexos", resultado.NomeArquivoArmazenado)));
    }

    /// <summary>
    /// Confirma que arquivos maiores que 10 MB são rejeitados.
    /// </summary>
    [Fact]
    public async Task EnviarAsync_DeveRejeitarArquivoMaiorQueDezMegabytes()
    {
        await using var stream = new MemoryStream(new byte[(10 * 1024 * 1024) + 1]);
        var arquivo = new FormFile(stream, 0, stream.Length, "arquivo", "grande.bin")
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/octet-stream"
        };

        await Assert.ThrowsAsync<ValidationException>(() => _service.EnviarAsync(arquivo));
    }

    /// <summary>
    /// Confirma que extensões executáveis são rejeitadas.
    /// </summary>
    [Fact]
    public async Task EnviarAsync_DeveRejeitarExtensaoExecutavel()
    {
        var arquivo = CriarArquivo("programa.exe", "application/octet-stream", "executavel");

        await Assert.ThrowsAsync<ValidationException>(() => _service.EnviarAsync(arquivo));
    }

    /// <summary>
    /// Confirma que a listagem recupera os metadados dos arquivos salvos.
    /// </summary>
    [Fact]
    public async Task ListarAsync_DeveRetornarArquivosSalvos()
    {
        await _service.EnviarAsync(CriarArquivo("imagem.png", "image/png", "imagem"));

        var resultado = await _service.ListarAsync();

        var anexo = Assert.Single(resultado);
        Assert.Equal("imagem.png", anexo.NomeOriginal);
    }

    /// <summary>
    /// Confirma que a consulta de identificador inexistente retorna nulo.
    /// </summary>
    [Fact]
    public async Task ObterArquivoAsync_DeveRetornarNulo_QuandoAnexoNaoExistir()
    {
        var resultado = await _service.ObterArquivoAsync(Guid.NewGuid());

        Assert.Null(resultado);
    }

    /// <summary>
    /// Confirma que a exclusão remove o arquivo e seus metadados.
    /// </summary>
    [Fact]
    public async Task ExcluirAsync_DeveRemoverAnexoExistente()
    {
        var criado = await _service.EnviarAsync(CriarArquivo("arquivo.txt", "text/plain", "texto"));

        var removido = await _service.ExcluirAsync(criado.Id);
        var resultado = await _service.ObterArquivoAsync(criado.Id);

        Assert.True(removido);
        Assert.Null(resultado);
    }

    /// <summary>
    /// Cria um IFormFile em memória para os testes não dependerem de arquivos reais.
    /// </summary>
    private static IFormFile CriarArquivo(string nome, string contentType, string conteudo)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(conteudo);
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, stream.Length, "arquivo", nome)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    /// <summary>
    /// Remove a pasta temporária e os arquivos criados pelo teste.
    /// </summary>
    public void Dispose()
    {
        if (Directory.Exists(_diretorioTemporario))
        {
            Directory.Delete(_diretorioTemporario, recursive: true);
        }
    }
}
