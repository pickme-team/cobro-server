using Microsoft.EntityFrameworkCore;
using Prod.Exceptions;
using Prod.Models.Database;

namespace Prod.Services;

class OfficeZoneSeatsService(ProdContext context) : IOfficeZoneSeatsService
{
    public async Task<OfficeSeat> AddSeat(Guid zoneId, OfficeSeat seat)
    {
        var zone = await context.Zones.FindAsync(zoneId);

        if (zone is not OfficeZone officeZone)
            throw new NotFoundException("Офисной зоны с таким id не существует");

        seat.OfficeZone = officeZone;
        context.OfficeSeats.Add(seat);
        await context.SaveChangesAsync();

        return seat;
    }

    public async Task<List<OfficeSeat>> AddSeats(Guid zoneId, List<OfficeSeat> seats)
    {
        var zone = await context.Zones.FindAsync(zoneId);

        if (zone is not OfficeZone officeZone)
            throw new NotFoundException("Офисной зоны с таким id не существует");

        foreach (var officeSeat in seats)
        {
            officeSeat.OfficeZone = officeZone;
        }

        context.OfficeSeats.AddRange(seats);
        await context.SaveChangesAsync();

        return seats;
    }

    public async Task RemoveSeat(Guid zoneId, Guid seatId)
    {
        var zone = await context.Zones.FindAsync(zoneId);

        if (zone is not OfficeZone officeZone)
            throw new NotFoundException("Офисной зоны с таким id не существует");

        var seat = officeZone.Seats.FirstOrDefault(s => s.Id == seatId);
        if (seat is null)
            throw new NotFoundException("Зона не имеет такого места");

        var book = await context.Books.FirstOrDefaultAsync(b => seat.Books.Contains(b));
        if (book != null)
        {
            book.Status = Status.Ended;
            officeZone.Seats.Remove(seat);
        }

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
