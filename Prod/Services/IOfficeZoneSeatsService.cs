using Prod.Models.Database;

namespace Prod.Services;

public interface IOfficeZoneSeatsService
{
    Task AddSeat(Guid zoneId, OfficeSeat seat);
    Task RemoveSeat(Guid zoneId, OfficeSeat seat);
    Task<List<OfficeSeat>> GetSeats(Guid zoneId);
}