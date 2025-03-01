using Prod.Models.Requests;

namespace Prod.Services;

public interface IBookService
{
    Task Book(Guid zoneId, Guid? seatId, Guid userId, BookRequest bookRequest);
}
