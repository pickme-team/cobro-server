using Microsoft.EntityFrameworkCore;
using Prod.Models.Database;
using Prod.Models.Requests;

namespace Prod.Services;

public class ZoneService(ProdContext context, IBookService bookService) : IZoneService
{
    public Task<List<Zone>> GetAll() => context.Zones.ToListAsync();
    public Task<Zone> Get(Guid id) => context.Zones.SingleAsync(z => z.Id == id);

    public async Task<Zone> Create(string type, ZoneCreateRequest req)
    {
        var entity = type switch
        {
            "office" => new OfficeZone
            {
                Name = req.Name,
                Description = req.Description,
                Capacity = req.Capacity,
                Class = req.Class,
                XCoordinate = req.XCoordinate,
                YCoordinate = req.YCoordinate,
                Width = req.Width,
                Height = req.Height,
                ZoneTags = req.Tags.Select(t => new ZoneTag { Tag = t }).ToList(),
                IsPublic = req.IsPublic,
            },
            "open" => new OpenZone
            {
                Name = req.Name,
                Description = req.Description,
                Capacity = req.Capacity,
                Class = req.Class,
                XCoordinate = req.XCoordinate,
                YCoordinate = req.YCoordinate,
                Width = req.Width,
                Height = req.Height,
                ZoneTags = req.Tags.Select(t => new ZoneTag { Tag = t }).ToList(),
                IsPublic = req.IsPublic,
            },
            "talkroom" => new TalkroomZone
            {
                Name = req.Name,
                Description = req.Description,
                Capacity = req.Capacity,
                Class = req.Class,
                XCoordinate = req.XCoordinate,
                YCoordinate = req.YCoordinate,
                Width = req.Width,
                Height = req.Height,
                ZoneTags = req.Tags.Select(t => new ZoneTag { Tag = t }).ToList(),
                IsPublic = req.IsPublic,
            },
            "misc" => new Zone
            {
                Name = req.Name,
                Description = req.Description,
                Capacity = req.Capacity,
                Class = req.Class,
                XCoordinate = req.XCoordinate,
                YCoordinate = req.YCoordinate,
                Width = req.Width,
                Height = req.Height,
                ZoneTags = req.Tags.Select(t => new ZoneTag { Tag = t }).ToList(),
                IsPublic = req.IsPublic,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        context.Zones.Add(entity);
        await context.SaveChangesAsync();
        return entity;
    }

    public async Task<int> GetCurrentUsersCount(Guid id)
    {
        var zone = await Get(id);
        var res = 0;
        bookService.GetBooks(zone.Id, null).Result.ForEach(book =>
        {
            if (book.Status == Status.Active && book.Start < DateTime.UtcNow && book.End > DateTime.UtcNow)
            {
                res++;
            }
        });
        return res;
    }
}
