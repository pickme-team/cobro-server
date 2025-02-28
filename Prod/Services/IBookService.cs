namespace Prod.Services;

public interface IBookService
{
    Task SetPlaceCount(int count);
    Task<int> PlacesCount();
}