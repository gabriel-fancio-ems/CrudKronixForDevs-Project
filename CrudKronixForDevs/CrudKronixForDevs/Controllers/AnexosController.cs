using CrudKronixForDevs.DTOs;
using CrudKronixForDevs.Models;
using CrudKronixForDevs.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrudKronixForDevs.Controllers;

/// <summary>
/// Expõe operações de armazenamento e consulta de anexos.
/// </summary>
[ApiController]
[Authorize]
[Route("api/anexos")]
public class AnexosController : ControllerBase
{
    private readonly IAnexoService _anexoService;

    /// <summary>
    /// Inicializa a Controller com o serviço de anexos.
    /// </summary>
    /// <param name="anexoService">Serviço responsável pelo armazenamento local.</param>
    public AnexosController(IAnexoService anexoService)
    {
        _anexoService = anexoService;
    }

    /// <summary>
    /// Recebe e salva um arquivo enviado como multipart/form-data.
    /// </summary>
    /// <param name="arquivo">Arquivo enviado no campo arquivo.</param>
    /// <param name="cancellationToken">Token para cancelar o upload.</param>
    /// <returns>Metadados do arquivo criado.</returns>
    [HttpPost]
    [RequestSizeLimit(10 * 1024 * 1024)]
    [ProducesResponseType(typeof(AnexoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AnexoResponseDto>> Enviar([FromForm] IFormFile arquivo, CancellationToken cancellationToken)
    {
        var anexo = await _anexoService.EnviarAsync(arquivo, cancellationToken);
        var resposta = MapearResposta(anexo);
        return CreatedAtAction(nameof(Obter), new { id = anexo.Id }, resposta);
    }

    /// <summary>
    /// Lista os metadados dos anexos disponíveis para o front-end.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar a consulta.</param>
    /// <returns>Lista com nomes, tamanhos, tipos e URLs dos arquivos.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AnexoResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AnexoResponseDto>>> Listar(CancellationToken cancellationToken)
    {
        var anexos = await _anexoService.ListarAsync(cancellationToken);
        return Ok(anexos.Select(MapearResposta).ToList());
    }

    /// <summary>
    /// Abre PDFs e imagens no navegador ou baixa outros tipos de arquivo.
    /// </summary>
    /// <param name="id">Identificador do anexo.</param>
    /// <param name="cancellationToken">Token para cancelar a leitura.</param>
    /// <returns>Conteúdo do arquivo ou 404 quando não encontrado.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _anexoService.ObterArquivoAsync(id, cancellationToken);
        if (resultado is null)
        {
            return NotFound();
        }

        var (stream, metadados) = resultado.Value;
        var podeVisualizarNoNavegador = metadados.TipoConteudo.Equals("application/pdf", StringComparison.OrdinalIgnoreCase)
            || metadados.TipoConteudo.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase)
            || metadados.TipoConteudo.Equals("image/png", StringComparison.OrdinalIgnoreCase);

        return podeVisualizarNoNavegador
            ? File(stream, metadados.TipoConteudo, enableRangeProcessing: true)
            : File(stream, metadados.TipoConteudo, metadados.NomeOriginal, enableRangeProcessing: true);
    }

    /// <summary>
    /// Exclui um anexo armazenado localmente.
    /// </summary>
    /// <param name="id">Identificador do anexo.</param>
    /// <param name="cancellationToken">Token para cancelar a exclusão.</param>
    /// <returns>204 quando removido ou 404 quando não encontrado.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken cancellationToken)
    {
        var removido = await _anexoService.ExcluirAsync(id, cancellationToken);
        return removido ? NoContent() : NotFound();
    }

    /// <summary>
    /// Converte os metadados internos para o contrato consumido pelo front-end.
    /// </summary>
    /// <param name="anexo">Metadados armazenados.</param>
    /// <returns>DTO com URL relativa da API.</returns>
    private AnexoResponseDto MapearResposta(AnexoMetadata anexo)
    {
        return new AnexoResponseDto
        {
            Id = anexo.Id,
            NomeOriginal = anexo.NomeOriginal,
            Extensao = anexo.Extensao,
            TipoConteudo = anexo.TipoConteudo,
            TamanhoEmBytes = anexo.TamanhoEmBytes,
            EnviadoEmUtc = anexo.EnviadoEmUtc,
            Url = Url.Action(nameof(Obter), "Anexos", new { id = anexo.Id }) ?? $"/api/anexos/{anexo.Id}"
        };
    }
}
