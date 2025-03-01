using Prod.Models.Database;

namespace Prod.Services;

public interface IPlaceService
{
    Task Update(Place place);
    Task Create(Place place);
    Task<Place?> PlaceById(Guid id);
}