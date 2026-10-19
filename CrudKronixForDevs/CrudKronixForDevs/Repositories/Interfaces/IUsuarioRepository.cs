using CrudKronixForDevs.Models;

namespace CrudKronixForDevs.Repositories.Interfaces;

/// <summary>
/// Define as operações de persistência necessárias para usuários.
/// </summary>
public interface IUsuarioRepository
{
    /// <summary>
    /// Busca um usuário pelo e-mail informado.
    /// </summary>
    /// <param name="email">E-mail usado como identificador de login.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    /// <returns>Usuário encontrado ou nulo quando não existir.</returns>
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona um novo usuário ao contexto e persiste a alteração.
    /// </summary>
    /// <param name="usuario">Usuário que será salvo.</param>
    /// <param name="cancellationToken">Token para cancelar a operação assíncrona.</param>
    Task AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default);
}
