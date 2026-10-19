namespace CrudKronixForDevs.DTOs;

/// <summary>
/// Resposta devolvida após uma autenticação bem-sucedida.
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// Token que deve ser enviado no header Authorization como Bearer token.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Instante em que o token deixará de ser aceito pela API.
    /// </summary>
    public string ExpiresAtUtc { get; set; }
}
