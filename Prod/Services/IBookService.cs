using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Models.Responses;

namespace Prod.Services;

public interface IBookService
{
    Task Book(Guid zoneId, Guid? seatId, Guid userId, BookRequest bookRequest);
    Task<BookResponse> Delete(Guid guid);
    Task<List<BookResponse>> GetBooks(Guid id, Guid? seatId);
    Task<QrResponse> Qr(Guid bookId, Guid userId);
}
