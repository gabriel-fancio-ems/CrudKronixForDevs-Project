using CrudKronixForDevs.Controllers;
using CrudKronixForDevs.DTOs;
using CrudKronixForDevs.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CrudKronixForDevs.Tests.Controllers;

/// <summary>
/// Testa as respostas HTTP da Controller de autenticação.
/// </summary>
public class AuthControllerTests
{
    /// <summary>
    /// Confirma que credenciais inválidas resultam em 401 sem expor detalhes.
    /// </summary>
    [Fact]
    public async Task Login_DeveRetornarUnauthorized_QuandoCredenciaisForemInvalidas()
    {
        var service = new Mock<IAuthService>();
        service
            .Setup(item => item.AutenticarAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LoginResponseDto?)null);

        var controller = new AuthController(service.Object);
        var resultado = await controller.Login(new LoginRequestDto(), CancellationToken.None);

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(resultado.Result);
        Assert.Contains("inválidos", unauthorized.Value?.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Confirma que um login válido retorna o token produzido pelo Service.
    /// </summary>
    [Fact]
    public async Task Login_DeveRetornarOk_QuandoCredenciaisForemValidas()
    {
        var resposta = new LoginResponseDto { Token = "token-de-teste" };
        var service = new Mock<IAuthService>();
        service
            .Setup(item => item.AutenticarAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resposta);

        var controller = new AuthController(service.Object);
        var resultado = await controller.Login(new LoginRequestDto(), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(resultado.Result);
        Assert.Same(resposta, ok.Value);
    }
}
