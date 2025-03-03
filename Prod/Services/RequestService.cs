using Microsoft.EntityFrameworkCore;
using Prod.Models.Database;

namespace Prod.Services;

public class RequestService(ProdContext context) : IRequestService
{
    public async Task Add(Request request)
    {
        request.CreatedAt = DateTime.UtcNow;

        context.Requests.Add(request);
        await context.SaveChangesAsync();
    }

    public Task<List<Request>> Today() =>
        context.Requests.Where(r => r.CreatedAt.Date == DateTime.UtcNow.Date).ToListAsync();

    public async Task Mark(Guid id, RequestStatus status)
    {
        var request = await context.Requests.SingleAsync(r => r.Id == id);
        request.Status = status;
        await context.SaveChangesAsync();
    }
}
