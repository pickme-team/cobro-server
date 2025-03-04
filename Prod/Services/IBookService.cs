using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Models.Responses;

namespace Prod.Services;

public interface IBookService
{
    Task<List<Book>> GetAllActiveBooks();
    Task<List<Book>> GetAllFinishedBooks();
    Task CancelBook(Guid bookId);
    Task EditDateBook(Guid bookId, DateTime start, DateTime end, Guid? userId = null);
    Task<Book?> GetBookById(Guid bookId);
    Task Book(Guid zoneId, Guid? seatId, Guid userId, BookRequest bookRequest);
    Task<BookResponse> Delete(Guid guid);
    Task<List<BookResponse>> GetBooks(Guid id, Guid? seatId);
    Task<QrResponse> Qr(Guid bookId, Guid userId);
    Task<List<Book>> ActiveBooks(Guid id);
    Task<List<Book>> UserHistory(Guid id);
    Task<Book?> LastBook(Guid id);
    Task<ConfirmQrResponse> ConfirmQr(ConfirmQrRequest req);
    Task<bool> Validate(Guid zoneId, DateTime from, DateTime to, Guid guid, Guid? seat);
    Task<List<BookWithUserResponse>> GetAll();

    Task<List<BookResponse>> MassBook(Guid zoneId, DateOnly from, DateOnly to, TimeOnly fromTime, TimeOnly toTime,
        string description, Guid userId);

    Task<Book> GetBookByQr(long id);
}
