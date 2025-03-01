using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prod.Models.Database;
using Prod.Services;

namespace Prod.Controllers;

[ApiController]
[Route("zone")]
[Authorize(Policy = "Admin")]
public class OfficeZoneController(IOfficeZoneSeatsService officeZoneSeatsService)
{
    [HttpPost("{id:Guid}/seat")]
    public async Task<OfficeSeat> AddSeat(Guid zoneId, [FromBody] OfficeSeat seat)
    {
        return await officeZoneSeatsService.AddSeat(zoneId, seat);
    }

    [HttpGet("{id:Guid}/seat")]
    public async Task<IEnumerable<OfficeSeat>> GetSeats(Guid zoneId)
    {
        return await officeZoneSeatsService.GetSeats(zoneId);
    }
    
    [HttpDelete("{id:Guid}/seat/{seatId:Guid}")]
    public async Task RemoveSeat(Guid zoneId, Guid seatId)
    {
        await officeZoneSeatsService.RemoveSeat(zoneId, seatId);
    }
}