using Microsoft.EntityFrameworkCore;
using Prod.Models.Database;
using Prod.Models.Requests;

namespace Prod.Services;

public class ZoneService(ProdContext context) : IZoneService
{
    public Task<List<Zone>> GetAll() => context.Zones.ToListAsync();
    public Task<Zone> Get(Guid id) => context.Zones.SingleAsync(z => z.Id == id);

    public async Task<Zone> Create(string type, ZoneCreateRequest zone)
    {
        var baseZone = new Zone
        {
            Name = zone.Name,
            Description = zone.Description,
            Capacity = zone.Capacity,
            Class = zone.Class,
            XCoordinate = zone.XCoordinate,
            YCoordinate = zone.YCoordinate,
            Width = zone.Width,
            Height = zone.Height,
            ZoneTags = zone.Tags.Select(t => new ZoneTag { Tag = t }).ToList()
        };

        var entity = type switch
        {
            "office" => (OfficeZone)baseZone,
            "open" => (OpenZone)baseZone,
            "talkroom" => (TalkroomZone)baseZone,
            "misc" => baseZone,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        context.Zones.Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }
}
