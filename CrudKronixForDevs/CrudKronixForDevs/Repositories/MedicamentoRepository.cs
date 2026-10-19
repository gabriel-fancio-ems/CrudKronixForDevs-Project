using CrudKronixForDevs.Data;
using CrudKronixForDevs.DTOs;
using CrudKronixForDevs.Models;
using CrudKronixForDevs.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrudKronixForDevs.Repositories;

/// <summary>
/// Implementa o acesso aos medicamentos usando Entity Framework Core.
/// </summary>
public class MedicamentoRepository : IMedicamentoRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa o repository com o contexto fornecido pela injeção de dependência.
    /// </summary>
    /// <param name="context">Contexto usado para acessar o banco.</param>
    public MedicamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna todos os medicamentos ordenados pelo nome, sem rastreamento pelo EF Core.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Lista de medicamentos cadastrados.</returns>
    public async Task<IReadOnlyList<Medicamento>> ListarAsync(CancellationToken cancellationToken = default)
    {
        // A ordenação deixa o resultado previsível para consumidores e testes.
        return await _context.Medicamentos
            .AsNoTracking()
            .OrderBy(medicamento => medicamento.Nome)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retorna medicamentos que possuem fornecedor associado, usando um INNER JOIN.
    /// </summary>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Lista com os dados dos medicamentos e de seus fornecedores.</returns>
    public async Task<IReadOnlyList<MedicamentoFornecedorDto>> ListarComFornecedoresAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Medicamentos
            .AsNoTracking()
            .Join(_context.Fornecedores.AsNoTracking(), medicamento => medicamento.FornecedorId, fornecedor => fornecedor.Id, (medicamento, fornecedor) => new MedicamentoFornecedorDto
            {
                MedicamentoId = medicamento.Id,
                NomeMedicamento = medicamento.Nome,
                FornecedorId = fornecedor.Id,
                NomeFornecedor = fornecedor.Nome
            })
            .OrderBy(resultado => resultado.NomeMedicamento)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Exemplo de chamada a uma procedure SQL Server que retorna as colunas de Medicamento.
    /// Requer um provider relacional e a procedure dbo.ListarMedicamentosPorFornecedor existente.
    /// </summary>
    /// <param name="fornecedorId">Identificador do fornecedor usado como parâmetro da procedure.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Medicamentos retornados pela procedure.</returns>
    public async Task<IReadOnlyList<Medicamento>> ExemploListarPorFornecedorComProcedureAsync(Guid fornecedorId, CancellationToken cancellationToken = default)
    {
        return await _context.Medicamentos
            .FromSqlInterpolated($"EXEC dbo.ListarMedicamentosPorFornecedor {fornecedorId}")
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca um medicamento pelo identificador, sem rastreamento pelo EF Core.
    /// </summary>
    /// <param name="id">Identificador do medicamento.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Medicamento encontrado ou nulo quando não existir.</returns>
    public async Task<Medicamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // A leitura sem tracking é suficiente porque a atualização e a exclusão recebem a entidade depois.
        return await _context.Medicamentos
            .AsNoTracking()
            .FirstOrDefaultAsync(medicamento => medicamento.Id == id, cancellationToken);
    }

    /// <summary>
    /// Adiciona um medicamento e persiste a alteração no banco de dados.
    /// </summary>
    /// <param name="medicamento">Medicamento que será adicionado.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public async Task AdicionarAsync(Medicamento medicamento, CancellationToken cancellationToken = default)
    {
        // O repository confirma a alteração para que o Service não precise conhecer o DbContext.
        await _context.Medicamentos.AddAsync(medicamento, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Marca o medicamento como alterado e persiste as mudanças no banco de dados.
    /// </summary>
    /// <param name="medicamento">Medicamento com os valores atualizados.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public async Task AtualizarAsync(Medicamento medicamento, CancellationToken cancellationToken = default)
    {
        // Update informa ao EF Core que a entidade deve ser persistida como alterada.
        _context.Medicamentos.Update(medicamento);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Remove o medicamento e persiste a exclusão no banco de dados.
    /// </summary>
    /// <param name="medicamento">Medicamento que será removido.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public async Task RemoverAsync(Medicamento medicamento, CancellationToken cancellationToken = default)
    {
        // Remove marca a entidade para exclusão antes de confirmar a transação.
        _context.Medicamentos.Remove(medicamento);
        await _context.SaveChangesAsync(cancellationToken);
    }

    
}
