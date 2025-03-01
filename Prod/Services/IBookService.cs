using Prod.Models.Requests;

namespace Prod.Services;

public interface IBookService
{
    Task SetPlaceCount(int count);
    Task<int> PlaceCount();
    Task BookRoom(Guid roomId, Guid userId, BookRequest req);
    Task BookPlace(Guid placeId, Guid userId, BookRequest req);
    Task BookSpace(Guid spaceId, Guid userId, BookRequest req);
}
