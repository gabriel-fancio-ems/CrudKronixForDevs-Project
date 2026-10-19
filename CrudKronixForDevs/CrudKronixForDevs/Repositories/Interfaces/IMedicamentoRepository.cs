using CrudKronixForDevs.DTOs;
using CrudKronixForDevs.Models;

namespace CrudKronixForDevs.Repositories.Interfaces;

/// <summary>
/// Define as operações de persistência necessárias para medicamentos.
/// </summary>
public interface IMedicamentoRepository
{
    /// <summary>
    /// Retorna todos os medicamentos cadastrados.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Lista de medicamentos.</returns>
    Task<IReadOnlyList<Medicamento>> ListarAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna medicamentos associados a fornecedores usando INNER JOIN.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Medicamentos acompanhados dos dados de seus fornecedores.</returns>
    Task<IReadOnlyList<MedicamentoFornecedorDto>> ListarComFornecedoresAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Exemplo de chamada a uma procedure SQL Server que retorna medicamentos por fornecedor.
    /// </summary>
    /// <param name="fornecedorId">Identificador do fornecedor.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Medicamentos retornados pela procedure.</returns>
    Task<IReadOnlyList<Medicamento>> ExemploListarPorFornecedorComProcedureAsync(Guid fornecedorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca um medicamento pelo identificador.
    /// </summary>
    /// <param name="id">Identificador do medicamento.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Medicamento encontrado ou nulo.</returns>
    Task<Medicamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona um medicamento e confirma a operação no banco.
    /// </summary>
    /// <param name="medicamento">Medicamento que será salvo.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    Task AdicionarAsync(Medicamento medicamento, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um medicamento existente e confirma a operação no banco.
    /// </summary>
    /// <param name="medicamento">Medicamento com os novos valores.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    Task AtualizarAsync(Medicamento medicamento, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um medicamento existente e confirma a operação no banco.
    /// </summary>
    /// <param name="medicamento">Medicamento que será removido.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    Task RemoverAsync(Medicamento medicamento, CancellationToken cancellationToken = default);
}
