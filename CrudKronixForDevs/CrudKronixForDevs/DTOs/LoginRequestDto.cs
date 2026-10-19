using System.ComponentModel.DataAnnotations;

namespace CrudKronixForDevs.DTOs;

/// <summary>
/// Dados necessários para solicitar um token JWT.
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// E-mail cadastrado do usuário.
    /// </summary>
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha informada pelo usuário durante o login.
    /// </summary>
    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "A senha deve possuir pelo menos 6 caracteres.")]
    public string Password { get; set; } = string.Empty;
}
