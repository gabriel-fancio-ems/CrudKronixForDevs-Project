using System.ComponentModel.DataAnnotations;

namespace CrudKronixForDevs.Models;

/// <summary>
/// Representa um usuário que pode se autenticar na API.
/// </summary>
public class Usuario
{
    /// <summary>
    /// Identificador único do usuário.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// E-mail utilizado no login. Ele também identifica o usuário de forma única.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nome exibido do usuário.
    /// </summary>
    [Required]
    [MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Hash da senha produzido pelo PasswordHasher.
    /// A senha original nunca deve ser armazenada no banco de dados.
    /// </summary>
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o usuário está autorizado a realizar login.
    /// </summary>
    public bool Ativo { get; set; } = true;
}
