using System.ComponentModel.DataAnnotations;
using CrudKronixForDevs.DTOs;
using CrudKronixForDevs.Models;
using CrudKronixForDevs.Repositories.Interfaces;
using CrudKronixForDevs.Services;
using Moq;

namespace CrudKronixForDevs.Tests.Services;

/// <summary>
/// Testa as regras de negócio do CRUD de medicamentos.
/// </summary>
public class MedicamentoServiceTests
{
    /// <summary>
    /// Confirma que a listagem converte as entidades para DTOs.
    /// </summary>
    [Fact]
    public async Task ListarAsync_DeveRetornarMedicamentosConvertidos()
    {
        var medicamento = CriarMedicamento();
        var repository = new Mock<IMedicamentoRepository>();
        repository
            .Setup(item => item.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Medicamento> { medicamento });

        var service = new MedicamentoService(repository.Object);
        var resultado = await service.ListarAsync();

        var item = Assert.Single(resultado);
        Assert.Equal(medicamento.Id, item.Id);
        Assert.Equal(medicamento.Nome, item.Nome);
    }

    /// <summary>
    /// Confirma que um medicamento válido é normalizado, persistido e retornado.
    /// </summary>
    [Fact]
    public async Task CriarAsync_DevePersistirMedicamentoValido()
    {
        var repository = new Mock<IMedicamentoRepository>();
        var service = new MedicamentoService(repository.Object);
        var request = CriarRequest();

        var resultado = await service.CriarAsync(request);

        Assert.NotEqual(Guid.Empty, resultado.Id);
        Assert.Equal("Paracetamol", resultado.Nome);
        repository.Verify(
            item => item.AdicionarAsync(It.Is<Medicamento>(medicamento => medicamento.Nome == "Paracetamol"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Confirma que uma atualização de registro existente chama o repository.
    /// </summary>
    [Fact]
    public async Task AtualizarAsync_DeveAtualizarMedicamentoExistente()
    {
        var medicamento = CriarMedicamento();
        var repository = new Mock<IMedicamentoRepository>();
        repository
            .Setup(item => item.ObterPorIdAsync(medicamento.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(medicamento);

        var service = new MedicamentoService(repository.Object);
        var resultado = await service.AtualizarAsync(medicamento.Id, CriarRequest("Novo nome"));

        Assert.NotNull(resultado);
        Assert.Equal("Novo nome", resultado.Nome);
        repository.Verify(item => item.AtualizarAsync(medicamento, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Confirma que a exclusão de um registro existente retorna verdadeiro.
    /// </summary>
    [Fact]
    public async Task RemoverAsync_DeveRemoverMedicamentoExistente()
    {
        var medicamento = CriarMedicamento();
        var repository = new Mock<IMedicamentoRepository>();
        repository
            .Setup(item => item.ObterPorIdAsync(medicamento.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(medicamento);

        var service = new MedicamentoService(repository.Object);
        var removido = await service.RemoverAsync(medicamento.Id);

        Assert.True(removido);
        repository.Verify(item => item.RemoverAsync(medicamento, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Confirma que a exclusão de um registro ausente não tenta persistir uma remoção.
    /// </summary>
    [Fact]
    public async Task RemoverAsync_DeveRetornarFalso_QuandoMedicamentoNaoExistir()
    {
        var repository = new Mock<IMedicamentoRepository>();
        repository
            .Setup(item => item.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Medicamento?)null);

        var service = new MedicamentoService(repository.Object);
        var removido = await service.RemoverAsync(Guid.NewGuid());

        Assert.False(removido);
        repository.Verify(item => item.RemoverAsync(It.IsAny<Medicamento>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Confirma que a regra de validade futura rejeita medicamentos vencidos.
    /// </summary>
    [Fact]
    public async Task CriarAsync_DeveRejeitarDataDeValidadeVencida()
    {
        var service = new MedicamentoService(new Mock<IMedicamentoRepository>().Object);
        var request = CriarRequest();
        request.DataValidade = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

        await Assert.ThrowsAsync<ValidationException>(() => service.CriarAsync(request));
    }

    /// <summary>
    /// Cria uma requisição válida para reutilização nos cenários.
    /// </summary>
    private static MedicamentoRequestDto CriarRequest(string nome = "Paracetamol")
    {
        return new MedicamentoRequestDto
        {
            Nome = nome,
            PrincipioAtivo = "Paracetamol",
            Fabricante = "Fabricante Exemplo",
            Preco = 12.50m,
            QuantidadeEstoque = 10,
            DataValidade = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1))
        };
    }

    /// <summary>
    /// Cria uma entidade válida para os cenários que consultam um registro existente.
    /// </summary>
    private static Medicamento CriarMedicamento()
    {
        return new Medicamento
        {
            Id = Guid.NewGuid(),
            Nome = "Paracetamol",
            PrincipioAtivo = "Paracetamol",
            Fabricante = "Fabricante Exemplo",
            Preco = 12.50m,
            QuantidadeEstoque = 10,
            DataValidade = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1))
        };
    }
}
