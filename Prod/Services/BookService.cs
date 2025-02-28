using Microsoft.EntityFrameworkCore;

namespace Prod.Services;

public class BookService(ProdContext context) : IBookService
{
    public async Task SetPlaceCount(int count)
    {
        await context.Count
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Count, count));
    }

    public async Task<int> PlaceCount() => (await context.Count.SingleAsync()).Count;

    public Task BookRoom(Guid roomId, Guid userId, DateTime from, DateTime to)
    {
        throw new NotImplementedException();
    }

    public Task BookPlace(Guid placeId, Guid userId, DateTime from, DateTime to)
    {
        throw new NotImplementedException();
    }

    public Task BookSpace(Guid userId, DateTime from, DateTime to)
    {
        throw new NotImplementedException();
    }
}
