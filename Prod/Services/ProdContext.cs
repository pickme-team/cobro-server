using Microsoft.EntityFrameworkCore;
using Prod.Models.Database;

namespace Prod.Services;

public class ProdContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<Book> Books { get; set; }

    public DbSet<Room> Rooms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Book>().Property(u => u.Status).HasConversion<string>();

        modelBuilder.Entity<Book>()
            .HasDiscriminator<bool>("IsRoom")
            .HasValue<Book>(false)
            .HasValue<RoomBook>(true);
    }
}
