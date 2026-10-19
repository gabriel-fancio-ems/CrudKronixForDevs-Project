using System.ComponentModel.DataAnnotations;
using CrudKronixForDevs.DTOs;
using CrudKronixForDevs.Models;
using CrudKronixForDevs.Repositories.Interfaces;
using CrudKronixForDevs.Services.Interfaces;

namespace CrudKronixForDevs.Services;

/// <summary>
/// Implementa as regras de negócio relacionadas aos medicamentos.
/// </summary>
public class MedicamentoService : IMedicamentoService
{
    private readonly IMedicamentoRepository _medicamentoRepository;

    /// <summary>
    /// Inicializa o serviço com o repository de medicamentos.
    /// </summary>
    /// <param name="medicamentoRepository">Abstração usada para persistência.</param>
    public MedicamentoService(IMedicamentoRepository medicamentoRepository)
    {
        _medicamentoRepository = medicamentoRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<MedicamentoResponseDto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var medicamentos = await _medicamentoRepository.ListarAsync(cancellationToken);
        return medicamentos.Select(MapearResposta).ToList();
    }

    /// <inheritdoc />
    public async Task<MedicamentoResponseDto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ValidarId(id);
        var medicamento = await _medicamentoRepository.ObterPorIdAsync(id, cancellationToken);
        return medicamento is null ? null : MapearResposta(medicamento);
    }

    /// <inheritdoc />
    public async Task<MedicamentoResponseDto> CriarAsync(MedicamentoRequestDto request, CancellationToken cancellationToken = default)
    {
        ValidarRequest(request);

        var medicamento = MapearEntidade(request);
        await _medicamentoRepository.AdicionarAsync(medicamento, cancellationToken);
        return MapearResposta(medicamento);
    }

    /// <inheritdoc />
    public async Task<MedicamentoResponseDto?> AtualizarAsync(
        Guid id,
        MedicamentoRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidarId(id);
        ValidarRequest(request);

        var medicamento = await _medicamentoRepository.ObterPorIdAsync(id, cancellationToken);
        if (medicamento is null)
        {
            return null;
        }

        // O identificador vem da rota e não do corpo, evitando que o cliente altere o registro errado.
        medicamento.Nome = request.Nome.Trim();
        medicamento.PrincipioAtivo = request.PrincipioAtivo.Trim();
        medicamento.Fabricante = request.Fabricante.Trim();
        medicamento.Preco = request.Preco;
        medicamento.QuantidadeEstoque = request.QuantidadeEstoque;
        medicamento.DataValidade = request.DataValidade!.Value;

        await _medicamentoRepository.AtualizarAsync(medicamento, cancellationToken);
        return MapearResposta(medicamento);
    }

    /// <inheritdoc />
    public async Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ValidarId(id);

        var medicamento = await _medicamentoRepository.ObterPorIdAsync(id, cancellationToken);
        if (medicamento is null)
        {
            return false;
        }

        await _medicamentoRepository.RemoverAsync(medicamento, cancellationToken);
        return true;
    }

    /// <summary>
    /// Valida as regras que não dependem apenas do formato do DTO.
    /// </summary>
    /// <param name="request">Dados recebidos para criação ou atualização.</param>
    private static void ValidarRequest(MedicamentoRequestDto request)
    {
        if (request is null)
        {
            throw new ValidationException("O corpo da requisição é obrigatório.");
        }

        if (request.DataValidade is null || request.DataValidade <= DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new ValidationException("A data de validade deve ser futura.");
        }

        if (request.Preco <= 0)
        {
            throw new ValidationException("O preço deve ser maior que zero.");
        }

        if (request.QuantidadeEstoque < 0)
        {
            throw new ValidationException("A quantidade em estoque não pode ser negativa.");
        }
    }

    /// <summary>
    /// Impede que um identificador vazio chegue ao repository.
    /// </summary>
    /// <param name="id">Identificador recebido pela rota.</param>
    private static void ValidarId(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException("O identificador informado é inválido.");
        }
    }

    /// <summary>
    /// Converte o DTO recebido para a entidade persistida.
    /// </summary>
    /// <param name="request">Dados de entrada já validados.</param>
    /// <returns>Nova entidade de medicamento.</returns>
    private static Medicamento MapearEntidade(MedicamentoRequestDto request)
    {
        return new Medicamento
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome.Trim(),
            PrincipioAtivo = request.PrincipioAtivo.Trim(),
            Fabricante = request.Fabricante.Trim(),
            Preco = request.Preco,
            QuantidadeEstoque = request.QuantidadeEstoque,
            DataValidade = request.DataValidade!.Value
        };
    }

    /// <summary>
    /// Converte a entidade interna para o contrato público da API.
    /// </summary>
    /// <param name="medicamento">Entidade persistida.</param>
    /// <returns>DTO que pode ser retornado ao cliente.</returns>
    private static MedicamentoResponseDto MapearResposta(Medicamento medicamento)
    {
        return new MedicamentoResponseDto
        {
            Id = medicamento.Id,
            Nome = medicamento.Nome,
            PrincipioAtivo = medicamento.PrincipioAtivo,
            Fabricante = medicamento.Fabricante,
            Preco = medicamento.Preco,
            QuantidadeEstoque = medicamento.QuantidadeEstoque,
            DataValidade = medicamento.DataValidade
        };
    }
}
