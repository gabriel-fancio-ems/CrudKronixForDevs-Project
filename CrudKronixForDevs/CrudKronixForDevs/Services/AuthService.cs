using System.IdentityModel.Tokens.Jwt;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using CrudKronixForDevs.Configurations;
using CrudKronixForDevs.DTOs;
using CrudKronixForDevs.Models;
using CrudKronixForDevs.Repositories.Interfaces;
using CrudKronixForDevs.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CrudKronixForDevs.Services;

/// <summary>
/// Implementa a autenticação e a criação de tokens JWT.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    /// Inicializa o serviço com suas dependências.
    /// </summary>
    /// <param name="usuarioRepository">Repository usado para consultar usuários.</param>
    /// <param name="passwordHasher">Componente que compara senhas com seus hashes.</param>
    /// <param name="jwtOptions">Configurações usadas para assinar o token.</param>
    public AuthService(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher<Usuario> passwordHasher,
        IOptions<JwtSettings> jwtOptions)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _jwtSettings = jwtOptions.Value;
    }

    /// <inheritdoc />
    public async Task<LoginResponseDto?> AutenticarAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        // Normalizar o e-mail evita que diferenças de maiúsculas e minúsculas impeçam o login.
        var email = request.Email.Trim().ToLowerInvariant();
        var usuario = await _usuarioRepository.ObterPorEmailAsync(email, cancellationToken);

        // A mesma resposta é usada para usuário inexistente, inativo ou senha incorreta.
        // Isso evita revelar qual credencial falhou.
        if (usuario is null || !usuario.Ativo)
        {
            return null;
        }

        var resultadoSenha = _passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.PasswordHash,
            request.Password);

        if (resultadoSenha == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return GerarToken(usuario);
    }

    /// <summary>
    /// Cria o token com os dados mínimos necessários para identificar o usuário.
    /// </summary>
    /// <param name="usuario">Usuário autenticado.</param>
    /// <returns>Resposta contendo o token e sua validade.</returns>
    private LoginResponseDto GerarToken(Usuario usuario)
    {
        // Uma chave vazia ou curta torna a emissão insegura e deve interromper a inicialização do fluxo.
        if (string.IsNullOrWhiteSpace(_jwtSettings.Key) || _jwtSettings.Key.Length < 32)
        {
            throw new InvalidOperationException("A chave JWT deve possuir pelo menos 32 caracteres.");
        }

        var agora = DateTime.UtcNow;
        var expiracao = agora.AddMinutes(_jwtSettings.ExpirationMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: agora,
            expires: expiracao,
            signingCredentials: credenciais);

        return new LoginResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expiracao.ToString("dd/MM/yyyy HH:mm:ss 'UTC'", CultureInfo.InvariantCulture)
        };
    }
}
