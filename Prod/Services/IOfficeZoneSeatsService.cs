using Prod.Models.Database;

namespace Prod.Services;

public interface IOfficeZoneSeatsService
{
    Task<OfficeSeat> AddSeat(Guid zoneId, OfficeSeat seat);
    Task<List<OfficeSeat>> AddSeats(Guid zoneId, List<OfficeSeat> seats);
    Task RemoveSeat(Guid zoneId, Guid seat);
    Task<List<OfficeSeat>> GetSeats(Guid zoneId);
}
