using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Models.Responses;
using Prod.Services;

namespace Prod.Controllers;

[ApiController]
[Route("zone/office")]
[Authorize]
public class OfficeZoneController(IOfficeZoneSeatsService officeZoneSeatsService)
{
    [HttpPost("{zoneId:guid}/seat")]
    [Authorize(Policy = "Admin")]
    public Task<OfficeSeatResponse> AddSeat(Guid zoneId, [FromBody] OfficeSeatCreateRequest seat) =>
        officeZoneSeatsService.AddSeat(zoneId, seat);

    [HttpPost("{zoneId:guid}/seats")]
    [Authorize(Policy = "Admin")]
    public Task<List<OfficeSeatResponse>> AddSeats(Guid zoneId, [FromBody] List<OfficeSeatCreateRequest> seats) =>
        officeZoneSeatsService.AddSeats(zoneId, seats);

    [HttpGet("{zoneId:guid}/seats")]
    public Task<List<OfficeSeatResponse>> GetSeats(Guid zoneId) =>
        officeZoneSeatsService.GetSeats(zoneId);

    [HttpDelete("{zoneId:guid}/seat/{seatId:guid}")]
    [Authorize(Policy = "Admin")]
    public Task RemoveSeat(Guid zoneId, Guid seatId) =>
        officeZoneSeatsService.RemoveSeat(zoneId, seatId);
}
