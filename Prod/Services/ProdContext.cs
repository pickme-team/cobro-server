using Microsoft.EntityFrameworkCore;

namespace Prod.Services;

public class ProdContext(DbContextOptions options) : DbContext(options)
{
    
}
