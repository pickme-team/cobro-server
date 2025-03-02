using Microsoft.EntityFrameworkCore;
using Prod.Exceptions;
using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Models.Responses;

namespace Prod.Services;

public class OfficeZoneSeatsService(ProdContext context) : IOfficeZoneSeatsService
{
    public async Task<OfficeSeatResponse> AddSeat(Guid zoneId, OfficeSeatCreateRequest req)
    {
        var zone = await context.Zones.FindAsync(zoneId);

        if (zone is not OfficeZone officeZone)
            throw new NotFoundException("Офисной зоны с таким id не существует");

        var seat = new OfficeSeat
        {
            OfficeZoneId = zoneId,
            X = req.X,
            Y = req.Y,
            InnerNumber = req.InnerNumber,
        };
        context.OfficeSeats.Add(seat);
        await context.SaveChangesAsync();

        return OfficeSeatResponse.From(seat);
    }

    public async Task<List<OfficeSeatResponse>> AddSeats(Guid zoneId, List<OfficeSeatCreateRequest> req)
    {
        var zone = await context.Zones.FindAsync(zoneId);

        if (zone is not OfficeZone officeZone)
            throw new NotFoundException("Офисной зоны с таким id не существует");

        var seats = req.Select(r => new OfficeSeat
        {
            OfficeZoneId = zoneId,
            X = r.X,
            Y = r.Y,
            InnerNumber = r.InnerNumber,
        }).ToList();

        context.OfficeSeats.AddRange(seats);
        await context.SaveChangesAsync();

        return seats.Select(OfficeSeatResponse.From).ToList();
    }

    public async Task RemoveSeat(Guid zoneId, Guid seatId)
    {
        var zone = await context.Zones.FindAsync(zoneId);

        if (zone is not OfficeZone officeZone)
            throw new NotFoundException("Офисной зоны с таким id не существует");

        var seat = context.Entry(officeZone)
            .Collection(z => z.Seats)
            .Query()
            .Include(s => s.Books)
            .FirstOrDefault(s => s.Id == seatId);
        if (seat is null)
            throw new NotFoundException("Зона не имеет такого места");

        foreach (var book in seat.Books)
        {
            if (book.Status is Status.Active or Status.Pending)
                book.Status = Status.Ended;
        }

        context.OfficeSeats.Remove(seat);

        await context.SaveChangesAsync();
    }

    public async Task<List<OfficeSeatResponse>> GetSeats(Guid zoneId)
    {
        var zone = await context.Zones.FindAsync(zoneId);

        if (zone is not OfficeZone officeZone)
            throw new NotFoundException("Офисной зоны с таким id не существует");

        var entities = await context.Entry(officeZone)
            .Collection(z => z.Seats)
            .Query()
            .ToListAsync();

        return entities.Select(OfficeSeatResponse.From).ToList();
    }
}
