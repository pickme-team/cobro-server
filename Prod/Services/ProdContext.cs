using Microsoft.EntityFrameworkCore;
using Prod.Models.Database;

namespace Prod.Services;

public class ProdContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<Book> Books { get; set; }

    public DbSet<Zone> Zones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Book>().Property(u => u.Status).HasConversion<string>();

        modelBuilder.Entity<Book>()
            .HasDiscriminator<string>("Type")
            .HasValue<OfficeBook>("Office")
            .HasValue<OpenBook>("Open")
            .HasValue<TalkroomBook>("Talkroom");

        modelBuilder.Entity<Zone>()
            .HasDiscriminator<string>("Type")
            .HasValue<OfficeZone>("Office")
            .HasValue<OpenZone>("Open")
            .HasValue<TalkroomZone>("Talkroom");
    }
}
