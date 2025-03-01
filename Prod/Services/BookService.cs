using Microsoft.EntityFrameworkCore;
using Prod.Exceptions;
using Prod.Models.Database;

namespace Prod.Services;

public class BookService(ProdContext context) : IBookService
{
    public async Task SetPlaceCount(int count)
    {
        await context.Count
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Count, count));
    }

    public async Task<int> PlaceCount() => (await context.Count.SingleAsync()).Count;

    public async Task BookRoom(Guid roomId, Guid userId, DateTime from, DateTime to, string? description = null)
    {
        bool isOverlap = await context.Books
            .AnyAsync(b => b.Start <= from && b.Start >= to && b.End >= from && b.End <= to);
        if (isOverlap)
            throw new AlreadyBookedException(typeof(RoomBook));

        var room = new RoomBook()
        {
            RoomId = roomId,
            UserId = userId,
            Start = from,
            End = to,
            Description = description
        };
        context.Books.Add(room);
        
        await context.SaveChangesAsync();
    }

    public async Task BookPlace(Guid placeId, Guid userId, DateTime from, DateTime to, string? description = null)
    {
        bool isOverlap = await context.Books
            .AnyAsync(b => b.Start <= from && b.Start >= to && b.End >= from && b.End <= to);
        if (isOverlap)
            throw new AlreadyBookedException(typeof(RoomBook));

        var room = new PlaceBook()
        {
            PlaceId = placeId,
            UserId = userId,
            Start = from,
            End = to,
            Description = description
        };
        context.Books.Add(room);
        
        await context.SaveChangesAsync();
    }

    public async Task BookSpace(Guid userId, DateTime from, DateTime to)
    {
        throw new NotImplementedException();
    }
}
