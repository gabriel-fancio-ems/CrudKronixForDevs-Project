using CrudKronixForDevs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CrudKronixForDevs.Data;

/// <summary>
/// Inicializa dados mínimos necessários para executar a API localmente.
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Cria o usuário de desenvolvimento caso ele ainda não exista.
    /// </summary>
    /// <param name="services">Provedor de serviços da aplicação.</param>
    /// <param name="configuration">Fonte das configurações seguras.</param>
    public static async Task InitializeAsync(
        IServiceProvider services,
        IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();

        var email = configuration["SeedUser:Email"]?.Trim().ToLowerInvariant();
        var password = configuration["SeedUser:Password"];
        var nome = configuration["SeedUser:Name"]?.Trim();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(nome))
        {
            throw new InvalidOperationException(
                "Configure SeedUser:Email, SeedUser:Name e SeedUser:Password usando User Secrets ou variáveis de ambiente.");
        }

        // O método é idempotente: executar a aplicação novamente não duplica o usuário.
        if (await context.Usuarios.AnyAsync(usuario => usuario.Email == email))
        {
            return;
        }

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Email = email,
            Nome = nome,
            Ativo = true
        };

        // Somente o hash é persistido; a senha recebida deixa de ser necessária após esta operação.
        usuario.PasswordHash = passwordHasher.HashPassword(usuario, password);
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
    }
}
