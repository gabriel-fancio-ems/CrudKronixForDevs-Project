using CrudKronixForDevs.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudKronixForDevs.Data;

/// <summary>
/// Representa a sessão da aplicação com o banco de dados.
/// Mesmo usando InMemory, o DbContext mantém o mesmo papel que teria com um banco real.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Inicializa o contexto recebendo as opções configuradas pela injeção de dependência.
    /// </summary>
    /// <param name="options">Configurações do provedor do Entity Framework Core.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Conjunto que representa os usuários persistidos.
    /// </summary>
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    /// <summary>
    /// Conjunto que representa os medicamentos persistidos.
    /// </summary>
    public DbSet<Medicamento> Medicamentos => Set<Medicamento>();

    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();

    /// <summary>
    /// Configura chaves, índices e tipos das propriedades do modelo.
    /// </summary>
    /// <param name="modelBuilder">Objeto usado pelo EF Core para construir o modelo.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // O índice único evita que dois usuários utilizem o mesmo e-mail.
        modelBuilder.Entity<Usuario>()
            .HasIndex(usuario => usuario.Email)
            .IsUnique();

        // Define precisão suficiente para valores monetários sem usar ponto flutuante.
        modelBuilder.Entity<Medicamento>()
            .Property(medicamento => medicamento.Preco)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Medicamento>()
            .HasOne(medicamento => medicamento.Fornecedor)
            .WithMany()
            .HasForeignKey(medicamento => medicamento.FornecedorId)
            .IsRequired(false);
    }
}
