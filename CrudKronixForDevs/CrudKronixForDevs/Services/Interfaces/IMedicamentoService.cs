using CrudKronixForDevs.DTOs;

namespace CrudKronixForDevs.Services.Interfaces;

/// <summary>
/// Define os casos de uso disponíveis para medicamentos.
/// </summary>
public interface IMedicamentoService
{
    /// <summary>
    /// Lista os medicamentos cadastrados.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Medicamentos convertidos para o contrato de resposta.</returns>
    Task<IReadOnlyList<MedicamentoResponseDto>> ListarAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca um medicamento pelo identificador.
    /// </summary>
    /// <param name="id">Identificador recebido pela rota.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Medicamento encontrado ou nulo.</returns>
    Task<MedicamentoResponseDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida e cria um novo medicamento.
    /// </summary>
    /// <param name="request">Dados recebidos do cliente.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Medicamento criado.</returns>
    Task<MedicamentoResponseDto> CriarAsync(MedicamentoRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um medicamento existente.
    /// </summary>
    /// <param name="id">Identificador recebido pela rota.</param>
    /// <param name="request">Novos dados do medicamento.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Medicamento atualizado ou nulo se não existir.</returns>
    Task<MedicamentoResponseDto?> AtualizarAsync(Guid id, MedicamentoRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um medicamento existente.
    /// </summary>
    /// <param name="id">Identificador recebido pela rota.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Verdadeiro quando o registro foi removido.</returns>
    Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default);
}
