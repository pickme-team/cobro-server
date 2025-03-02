using Microsoft.EntityFrameworkCore;
using Prod.Models.Database;

namespace Prod.Services;

public class DecorationService(ProdContext context) : IDecorationService
{
    public Task<List<Decoration>> GetAll() => context.Decorations.ToListAsync();
}
