using Microsoft.EntityFrameworkCore;
using Prod.Exceptions;
using Prod.Models.Database;
using Prod.Models.Requests;

namespace Prod.Services;

public class BookService(ProdContext context) : IBookService
{
    public async Task Book(Guid zoneId, Guid? seatId, Guid userId, BookRequest bookRequest)
    {
        var zone = await context.Zones.SingleAsync(z => z.Id == zoneId);

        switch (zone)
        {
            case OfficeZone officeZone:
                await BookSeat(officeZone, seatId!.Value, userId, bookRequest);
                break;
            case TalkroomZone talkroomZone:
                await BookTalkroomZone(talkroomZone, userId, bookRequest);
                break;
            case OpenZone openZone:
                await BookOpenZone(openZone, userId, bookRequest);
                break;
        }
    }

    private async Task BookSeat(OfficeZone zone, Guid seatId, Guid userId, BookRequest req)
    {
    }

    private async Task BookTalkroomZone(TalkroomZone zone, Guid userId, BookRequest req)
    {
        var overlaps = await context.Entry(zone)
            .Collection(z => z.Books)
            .Query()
            .AnyAsync(b => b.Start < req.To && req.From < b.End);

        if (overlaps) throw new ForbiddenException("Time not available");

        context.Books.Add(new TalkroomBook
        {
            Start = req.From,
            End = req.To,
            UserId = userId,
            Description = req.Description,
            Status = Status.Active,
            TalkroomZone = zone
        });
        await context.SaveChangesAsync();
    }

    private async Task BookOpenZone(OpenZone zone, Guid userId, BookRequest req)
    {
        var timespans = context.Entry(zone)
            .Collection(z => z.Books)
            .Query()
            .Select(b => new { b.Start, b.End });
        var capacity = zone.Capacity;
    }
}
