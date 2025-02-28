using Microsoft.EntityFrameworkCore;
using Prod.Models.Database;

namespace Prod.Services;

public class ProdContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<UserDto> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserDto>().HasIndex(u => u.Email).IsUnique();
    }
}
