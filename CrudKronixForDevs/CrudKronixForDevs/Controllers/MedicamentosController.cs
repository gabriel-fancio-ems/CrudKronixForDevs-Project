using CrudKronixForDevs.DTOs;
using CrudKronixForDevs.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrudKronixForDevs.Controllers;

/// <summary>
/// Expõe o CRUD de medicamentos para consumidores autenticados.
/// </summary>
[ApiController]
[Authorize]
[Route("api/medicamentos")]
public class MedicamentosController : ControllerBase
{
    private readonly IMedicamentoService _medicamentoService;

    /// <summary>
    /// Inicializa a Controller com o serviço de medicamentos.
    /// </summary>
    /// <param name="medicamentoService">Serviço que contém as regras do CRUD.</param>
    public MedicamentosController(IMedicamentoService medicamentoService)
    {
        _medicamentoService = medicamentoService;
    }

    /// <summary>
    /// Lista todos os medicamentos cadastrados.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar a consulta.</param>
    /// <returns>Lista de medicamentos.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MedicamentoResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MedicamentoResponseDto>>> Listar(CancellationToken cancellationToken)
    {
        // A Controller coordena HTTP; a busca e o mapeamento ficam no Service.
        var medicamentos = await _medicamentoService.ListarAsync(cancellationToken);
        return Ok(medicamentos);
    }

    /// <summary>
    /// Busca um medicamento pelo identificador.
    /// </summary>
    /// <param name="id">Identificador informado na rota.</param>
    /// <param name="cancellationToken">Token para cancelar a consulta.</param>
    /// <returns>Medicamento encontrado ou 404.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MedicamentoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MedicamentoResponseDto>> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        var medicamento = await _medicamentoService.ObterPorIdAsync(id, cancellationToken);
        return medicamento is null ? NotFound() : Ok(medicamento);
    }

    /// <summary>
    /// Cria um novo medicamento.
    /// </summary>
    /// <param name="request">Dados do medicamento enviados no corpo.</param>
    /// <param name="cancellationToken">Token para cancelar a operação.</param>
    /// <returns>Medicamento criado com sua localização.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(MedicamentoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MedicamentoResponseDto>> Criar([FromBody] MedicamentoRequestDto request, CancellationToken cancellationToken)
    {
        // O [ApiController] rejeita automaticamente o corpo que não atende às validações dos DTOs.
        var medicamento = await _medicamentoService.CriarAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id = medicamento.Id }, medicamento);
    }

    /// <summary>
    /// Atualiza todos os dados de um medicamento existente.
    /// </summary>
    /// <param name="id">Identificador informado na rota.</param>
    /// <param name="request">Novos dados enviados no corpo.</param>
    /// <param name="cancellationToken">Token para cancelar a operação.</param>
    /// <returns>Medicamento atualizado ou 404.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(MedicamentoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MedicamentoResponseDto>> Atualizar(Guid id, [FromBody] MedicamentoRequestDto request, CancellationToken cancellationToken)
    {
        var medicamento = await _medicamentoService.AtualizarAsync(id, request, cancellationToken);
        return medicamento is null ? NotFound() : Ok(medicamento);
    }

    /// <summary>
    /// Exclui um medicamento existente.
    /// </summary>
    /// <param name="id">Identificador informado na rota.</param>
    /// <param name="cancellationToken">Token para cancelar a operação.</param>
    /// <returns>204 quando removido ou 404 quando não encontrado.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(Guid id, CancellationToken cancellationToken)
    {
        var removido = await _medicamentoService.RemoverAsync(id, cancellationToken);
        return removido ? NoContent() : NotFound();
    }
}
