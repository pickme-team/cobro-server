using Prod.Models.Database;

namespace Prod.Services;

public interface IOfficeZoneSeatsService
{
    Task<OfficeSeat> AddSeat(Guid zoneId, OfficeSeat seat);
    Task RemoveSeat(Guid zoneId, Guid seat);
    Task<List<OfficeSeat>> GetSeats(Guid zoneId);
}