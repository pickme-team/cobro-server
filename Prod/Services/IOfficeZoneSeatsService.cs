using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Models.Responses;

namespace Prod.Services;

public interface IOfficeZoneSeatsService
{
    Task<OfficeSeatResponse> AddSeat(Guid zoneId, OfficeSeatCreateRequest seat);
    Task<List<OfficeSeatResponse>> AddSeats(Guid zoneId, List<OfficeSeatCreateRequest> seats);
    Task RemoveSeat(Guid zoneId, Guid seat);
    Task<List<OfficeSeatResponse>> GetSeats(Guid zoneId);
}
