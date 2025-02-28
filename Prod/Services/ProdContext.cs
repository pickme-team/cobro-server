using Microsoft.EntityFrameworkCore;
using Prod.Models.Database;

namespace Prod.Services;

public class ProdContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<UserDto> Users { get; set; }
}
