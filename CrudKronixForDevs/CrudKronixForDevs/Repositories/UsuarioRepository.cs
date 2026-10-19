using CrudKronixForDevs.Data;
using CrudKronixForDevs.Models;
using CrudKronixForDevs.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrudKronixForDevs.Repositories;

/// <summary>
/// Implementa o acesso aos usuários usando Entity Framework Core.
/// </summary>
public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Inicializa o repository com o contexto fornecido pela injeção de dependência.
    /// </summary>
    /// <param name="context">Contexto usado para acessar o banco.</param>
    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Busca um usuário pelo e-mail, sem rastreamento pelo EF Core.
    /// </summary>
    /// <param name="email">E-mail usado para localizar o usuário.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Usuário encontrado ou nulo quando não existir.</returns>
    public async Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        // O AsNoTracking informa ao EF Core que o objeto será apenas lido, sem atualização posterior.
        return await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(usuario => usuario.Email == email, cancellationToken);
    }

    /// <summary>
    /// Adiciona um usuário e persiste a alteração no banco de dados.
    /// </summary>
    /// <param name="usuario">Usuário que será adicionado.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    public async Task AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        // AddAsync registra a entidade no contexto e SaveChangesAsync persiste a alteração.
        await _context.Usuarios.AddAsync(usuario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
