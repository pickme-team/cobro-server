using Microsoft.EntityFrameworkCore;
using Prod.Exceptions;
using Prod.Models.Database;

namespace Prod.Services;

public class RequestService(ProdContext context, IBookService bookService) : IRequestService
{
    public async Task Add(Request request)
    {
        request.CreatedAt = DateTime.UtcNow;
        request.Book = await bookService.GetBookById(request.Book.Id);
        if (bookService.GetBookById(request.Book.Id).Result == null) throw new NotFoundException("book not found");
        if (request.Book == null) throw new NotFoundException("book not found");
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
