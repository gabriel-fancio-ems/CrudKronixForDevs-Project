using CrudKronixForDevs.DTOs;
using CrudKronixForDevs.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrudKronixForDevs.Controllers;

/// <summary>
/// Expõe operações relacionadas à autenticação da API.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Inicializa a Controller com o serviço de autenticação.
    /// </summary>
    /// <param name="authService">Serviço responsável por validar credenciais e emitir tokens.</param>
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Valida as credenciais e retorna um token Bearer para as rotas protegidas.
    /// </summary>
    /// <param name="request">E-mail e senha informados pelo cliente.</param>
    /// <param name="cancellationToken">Token para cancelar o processamento.</param>
    /// <returns>Token JWT ou 401 quando as credenciais não forem aceitas.</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        // O [ApiController] valida o DTO antes de executar o método quando houver campos inválidos.
        var response = await _authService.AutenticarAsync(request, cancellationToken);

        // Não diferenciamos usuário inexistente de senha incorreta para evitar enumeração de contas.
        if (response is null)
        {
            return Unauthorized(new { message = "E-mail ou senha inválidos." });
        }

        return Ok(response);
    }
}
