using Prod.Models.Database;

namespace Prod.Services;

public class PlaceService(ProdContext context) : IPlaceService
{
    public async Task Update(Place place)
    {
        context.Places.Update(place);
        await context.SaveChangesAsync();
    }

    public async Task Create(Place place)
    {
        context.Places.Add(place);
        await context.SaveChangesAsync();
    }

    public async Task<Place?> PlaceById(Guid id) => await context.Places.FindAsync(id);
}
