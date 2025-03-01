using Prod.Models.Database;
using Prod.Models.Requests;

namespace Prod.Services;

public interface IBookService
{
    Task Book(Guid zoneId, Guid? seatId, Guid userId, BookRequest bookRequest);
    
    Task<Book> Delete(Guid guid);
}
