using Microsoft.EntityFrameworkCore;
using Prod.Exceptions;
using Prod.Models.Database;
using Prod.Models.Requests;

namespace Prod.Services;

public class BookService(ProdContext context) : IBookService
{
    public async Task SetPlaceCount(int count)
    {
        await context.Count
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Count, count));
    }

    public async Task<int> PlaceCount() => (await context.Count.SingleAsync()).Count;

    public async Task BookRoom(Guid roomId, Guid userId, BookRequest req)
    {
        var room = await context.Rooms.Include(r => r.Books).SingleAsync(r => r.Id == roomId);
        var overlaps = room.Books.Any(b => req.From < b.End && b.Start < req.To);
        if (overlaps) throw new ForbiddenException("Not available for this time");

        room.Books.Add(new RoomBook
        {
            Start = req.From,
            End = req.To,
            UserId = userId,
            Description = req.Description,
            Status = Status.Inactive
        });
        await context.SaveChangesAsync();
    }

    public Task BookPlace(Guid placeId, Guid userId, BookRequest req)
    {
        throw new NotImplementedException();
    }

    public Task BookSpace(Guid userId, BookRequest req)
    {
        throw new NotImplementedException();
    }
}
