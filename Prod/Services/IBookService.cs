namespace Prod.Services;

public interface IBookService
{
    Task SetPlaceCount(int count);
    Task<int> PlaceCount();
    Task BookRoom(Guid roomId, Guid userId, DateTime from, DateTime to);
    Task BookPlace(Guid placeId, Guid userId, DateTime from, DateTime to);
    Task BookSpace(Guid userId, DateTime from, DateTime to);
}
