using Prod.Exceptions;
using Prod.Models.Database;

namespace Prod.Services;

class OfficeZoneSeatsService(ProdContext context)
{
    public async Task AddSeat(Guid zoneId, OfficeSeat seat)
    {
        var zone = await context.Zones.FindAsync(zoneId);

        if (zone is not OfficeZone officeZone)
            throw new NotFoundException("Офисной зоны с таким id не существует");
        
        officeZone.Seats.Add(seat);
        await context.SaveChangesAsync();
    }

    public async Task RemoveSeat(Guid zoneId, OfficeSeat seat)
    {
        var zone = await context.Zones.FindAsync(zoneId);

        if (zone is not OfficeZone officeZone)
            throw new NotFoundException("Офисной зоны с таким id не существует");
        
        officeZone.Seats.Remove(seat);
        await context.SaveChangesAsync();
    }
    
    public async Task<List<OfficeSeat>> GetSeats(Guid zoneId)
    {
        var zone = await context.Zones.FindAsync(zoneId);

        if (zone is not OfficeZone officeZone)
            throw new NotFoundException("Офисной зоны с таким id не существует");

        return officeZone.Seats;
    }
}