using Microsoft.EntityFrameworkCore;
using Prod.Models.Database;

namespace Prod.Services;

public class ProdContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }
}
