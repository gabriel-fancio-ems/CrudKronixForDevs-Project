using CrudKronixForDevs.DTOs;

namespace CrudKronixForDevs.Services.Interfaces;

/// <summary>
/// Define os casos de uso relacionados à autenticação da API.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Valida as credenciais e cria um token JWT quando o acesso é permitido.
    /// </summary>
    /// <param name="request">E-mail e senha recebidos do cliente.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Dados do token ou nulo quando as credenciais forem inválidas.</returns>
    Task<LoginResponseDto?> AutenticarAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
