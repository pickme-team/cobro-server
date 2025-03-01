using Microsoft.EntityFrameworkCore;
using Prod.Models.Database;

namespace Prod.Services;

public class ZoneService(ProdContext context) : IZoneService
{
    public Task<List<Zone>> GetAll() => context.Zones.ToListAsync();
    public Task<Zone> Get(Guid id) => context.Zones.SingleAsync(z => z.Id == id);

    public async Task<Zone> Create(string type, Zone zone)
    {
        switch (type)
        {
            case "Office":
                context.Zones.Add(new OfficeZone
                {
                    Name = zone.Name,
                    Description = zone.Description,
                    Capacity = zone.Capacity,
                    Class = zone.Class,
                    XCoordinate = zone.XCoordinate,
                    YCoordinate = zone.YCoordinate,
                    Width = zone.Width,
                    Height = zone.Height,
                });
                break;
            case "Open":
                context.Zones.Add(new OpenZone
                {
                    Name = zone.Name,
                    Description = zone.Description,
                    Capacity = zone.Capacity,
                    Class = zone.Class,
                    XCoordinate = zone.XCoordinate,
                    YCoordinate = zone.YCoordinate,
                    Width = zone.Width,
                    Height = zone.Height,
                });
                break;
            case "Talkroom":
                context.Zones.Add(new TalkroomZone
                {
                    Name = zone.Name,
                    Description = zone.Description,
                    Capacity = zone.Capacity,
                    Class = zone.Class,
                    XCoordinate = zone.XCoordinate,
                    YCoordinate = zone.YCoordinate,
                    Width = zone.Width,
                    Height = zone.Height,
                });
                break;
        }

        context.Zones.Add(zone);
        await context.SaveChangesAsync();
        return zone;
    }
}
