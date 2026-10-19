using CrudKronixForDevs.Controllers;
using CrudKronixForDevs.DTOs;
using CrudKronixForDevs.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CrudKronixForDevs.Tests.Controllers;

/// <summary>
/// Testa as respostas HTTP da Controller de medicamentos.
/// </summary>
public class MedicamentosControllerTests
{
    /// <summary>
    /// Confirma que uma criação válida retorna 201 e a localização do recurso.
    /// </summary>
    [Fact]
    public async Task Criar_DeveRetornarCreated_QuandoServiceCriarRecurso()
    {
        var medicamento = new MedicamentoResponseDto { Id = Guid.NewGuid(), Nome = "Paracetamol" };
        var service = new Mock<IMedicamentoService>();
        service
            .Setup(item => item.CriarAsync(It.IsAny<MedicamentoRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(medicamento);

        var controller = new MedicamentosController(service.Object);
        var resultado = await controller.Criar(new MedicamentoRequestDto(), CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(resultado.Result);
        Assert.Equal(nameof(MedicamentosController.ObterPorId), created.ActionName);
        Assert.Same(medicamento, created.Value);
    }

    /// <summary>
    /// Confirma que a consulta de registro ausente retorna 404.
    /// </summary>
    [Fact]
    public async Task ObterPorId_DeveRetornarNotFound_QuandoMedicamentoNaoExistir()
    {
        var service = new Mock<IMedicamentoService>();
        service
            .Setup(item => item.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MedicamentoResponseDto?)null);

        var controller = new MedicamentosController(service.Object);
        var resultado = await controller.ObterPorId(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(resultado.Result);
    }
}
