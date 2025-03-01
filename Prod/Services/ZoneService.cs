using Prod.Models.Database;

namespace Prod.Services;

public class ZoneService(ProdContext context)
{
    public List<Zone> GetAll() => context.Zones.ToList();
    public async Task<Zone?> Get(Guid id) => await context.Zones.FindAsync(id);

    public async Task<Zone> Create(Zone zone)
    {
        await context.Zones.AddAsync(zone);
        return zone;
    }
}
