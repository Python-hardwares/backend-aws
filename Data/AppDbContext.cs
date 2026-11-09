using Microsoft.EntityFrameworkCore;
using TB2_aws.Models;

namespace TB2_aws.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.Property(usuario => usuario.Nome)
                .HasMaxLength(120);
            entity.Property(usuario => usuario.Email)
                .HasMaxLength(255);
            entity.HasIndex(usuario => usuario.Email)
                .IsUnique();
        });
    }
}