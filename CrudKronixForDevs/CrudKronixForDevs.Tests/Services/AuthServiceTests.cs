using System.IdentityModel.Tokens.Jwt;
using System.Globalization;
using CrudKronixForDevs.Configurations;
using CrudKronixForDevs.DTOs;
using CrudKronixForDevs.Models;
using CrudKronixForDevs.Repositories.Interfaces;
using CrudKronixForDevs.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;

namespace CrudKronixForDevs.Tests.Services;

/// <summary>
/// Testa as regras de autenticação e geração de JWT.
/// </summary>
public class AuthServiceTests
{
    private const string JwtKey = "chave-de-testes-com-pelo-menos-32-caracteres";

    /// <summary>
    /// Confirma que credenciais válidas retornam um token com claims esperadas.
    /// </summary>
    [Fact]
    public async Task AutenticarAsync_DeveRetornarToken_QuandoCredenciaisForemValidas()
    {
        var usuario = CriarUsuario();
        var repository = new Mock<IUsuarioRepository>();
        repository
            .Setup(item => item.ObterPorEmailAsync("admin@exemplo.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        var service = CriarService(repository, usuario);

        var resposta = await service.AutenticarAsync(new LoginRequestDto
        {
            Email = " ADMIN@EXEMPLO.COM ",
            Password = "Senha123!"
        });

        Assert.NotNull(resposta);
        Assert.False(string.IsNullOrWhiteSpace(resposta.Token));
        var expiracaoUtc = DateTime.ParseExact(
            resposta.ExpiresAtUtc,
            "dd/MM/yyyy HH:mm:ss 'UTC'",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
        Assert.True(expiracaoUtc > DateTime.UtcNow);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(resposta.Token);
        Assert.Equal(usuario.Id.ToString(), token.Claims.Single(item => item.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal(usuario.Email, token.Claims.Single(item => item.Type == JwtRegisteredClaimNames.Email).Value);
    }

    /// <summary>
    /// Confirma que senha incorreta não gera token.
    /// </summary>
    [Fact]
    public async Task AutenticarAsync_DeveRetornarNulo_QuandoSenhaForIncorreta()
    {
        var usuario = CriarUsuario();
        var repository = new Mock<IUsuarioRepository>();
        repository
            .Setup(item => item.ObterPorEmailAsync("admin@exemplo.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        var service = CriarService(repository, usuario);

        var resposta = await service.AutenticarAsync(new LoginRequestDto
        {
            Email = usuario.Email,
            Password = "senha-incorreta"
        });

        Assert.Null(resposta);
    }

    /// <summary>
    /// Confirma que usuário inexistente não gera token.
    /// </summary>
    [Fact]
    public async Task AutenticarAsync_DeveRetornarNulo_QuandoUsuarioNaoExistir()
    {
        var repository = new Mock<IUsuarioRepository>();
        repository
            .Setup(item => item.ObterPorEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        var service = CriarService(repository, new Usuario());

        var resposta = await service.AutenticarAsync(new LoginRequestDto
        {
            Email = "naoexiste@exemplo.com",
            Password = "Senha123!"
        });

        Assert.Null(resposta);
    }

    /// <summary>
    /// Cria o serviço com dependências isoladas por mocks para o teste unitário.
    /// </summary>
    private static AuthService CriarService(Mock<IUsuarioRepository> repository, Usuario usuario)
    {
        var passwordHasher = new PasswordHasher<Usuario>();
        if (string.IsNullOrWhiteSpace(usuario.PasswordHash))
        {
            usuario.PasswordHash = passwordHasher.HashPassword(usuario, "Senha123!");
        }

        var settings = Options.Create(new JwtSettings
        {
            Key = JwtKey,
            Issuer = "CrudKronixForDevs.Tests",
            Audience = "CrudKronixForDevs.Tests.Client",
            ExpirationMinutes = 30
        });

        return new AuthService(repository.Object, passwordHasher, settings);
    }

    /// <summary>
    /// Cria um usuário ativo usado pelos cenários de autenticação.
    /// </summary>
    private static Usuario CriarUsuario()
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            Email = "admin@exemplo.com",
            Nome = "Administrador",
            Ativo = true
        };
    }
}
