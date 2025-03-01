namespace Prod.Services;

public interface IBookService
{
    Task SetPlaceCount(int count);
    Task<int> PlaceCount();
    Task BookRoom(Guid roomId, Guid userId, DateTime from, DateTime to, string? description);
    Task BookPlace(Guid placeId, Guid userId, DateTime from, DateTime to, string? description);
    Task BookSpace(Guid userId, DateTime from, DateTime to);
}
