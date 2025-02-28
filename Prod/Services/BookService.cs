using Microsoft.EntityFrameworkCore;

namespace Prod.Services;

public class BookService(ProdContext context) : IBookService
{
    public async Task SetPlaceCount(int count)
    {
        await context.Count
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Count, count));
    }

    public async Task<int> PlacesCount() => (await context.Count.SingleAsync()).Count;
}
