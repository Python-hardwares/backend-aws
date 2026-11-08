using Microsoft.EntityFrameworkCore;
using TB2_aws.Models;

namespace TB2_aws.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
}