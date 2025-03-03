using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using Prod.Models.Database;
using Prod.Models.Requests;

namespace Prod.Services;

public class RequestService(ProdContext context) : IRequestService
{
    public async Task<Request> Add(RequestRequest req)
    {
        var entity = new Request
        {
            Text = req.Text,
            ActionNumber = req.ActionNumber,
            Status = req.Status,
            AdditionalInfo = req.AdditionalInfo,
            BookId = req.BookId
        };

        context.Requests.Add(entity);
        await context.SaveChangesAsync();

        return entity;
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
