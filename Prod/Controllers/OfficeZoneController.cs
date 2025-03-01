using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prod.Models.Database;
using Prod.Services;

namespace Prod.Controllers;

[ApiController]
[Route("zone/office")]
[Authorize(Policy = "Admin")]
public class OfficeZoneController(IOfficeZoneSeatsService officeZoneSeatsService)
{
    [HttpPost("{zoneId:guid}/seat")]
    public Task<OfficeSeat> AddSeat(Guid zoneId, [FromBody] OfficeSeat seat) =>
        officeZoneSeatsService.AddSeat(zoneId, seat);

    [HttpPost("{zoneId:guid}/seats")]
    public Task<List<OfficeSeat>> AddSeats(Guid zoneId, [FromBody] List<OfficeSeat> seats) =>
        officeZoneSeatsService.AddSeats(zoneId, seats);

    [HttpGet("{zoneId:guid}/seats")]
    public Task<List<OfficeSeat>> GetSeats(Guid zoneId) =>
        officeZoneSeatsService.GetSeats(zoneId);

    [HttpDelete("{zoneId:guid}/seat/{seatId:guid}")]
    public Task RemoveSeat(Guid zoneId, Guid seatId) =>
        officeZoneSeatsService.RemoveSeat(zoneId, seatId);
}
