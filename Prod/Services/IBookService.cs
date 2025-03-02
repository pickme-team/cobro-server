using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Models.Responses;

namespace Prod.Services;

public interface IBookService
{
    Task<List<Book>> GetAllActiveBooks();
    Task CancelBook(Guid bookId);
    Task EditDateBook(Guid bookId, DateTime start, DateTime end);
    Task<Book?> GetBookById(Guid bookId);
    Task Book(Guid zoneId, Guid? seatId, Guid userId, BookRequest bookRequest);
    Task<BookResponse> Delete(Guid guid);
    Task<List<BookResponse>> GetBooks(Guid id, Guid? seatId);
    Task<QrResponse> Qr(Guid bookId, Guid userId);
    Task<List<Book>> ActiveBooks(Guid id);
    Task<List<Book>> UserHistory(Guid id);
    Task<Book?> LastBook(Guid id);
    Task ConfirmQr(ConfirmQrRequest req);
    Task<bool> Validate(Guid zoneId, DateTime from, DateTime to, Guid guid);
    Task<List<BookWithUserResponse>> GetAll();
}
