using Microsoft.AspNetCore.Mvc;
using Prod.Models.Database;
using Prod.Services;

namespace Prod.Controllers;

[ApiController]
public class OfficeZoneController(IOfficeZoneSeatsService officeZoneSeatsService)
{
    [HttpPost("{id:Guid}")]
    public async Task AddSeat(Guid zoneId, [FromBody] OfficeSeat seat)
    {
        officeZoneSeatsService.AddSeat(zoneId, seat);
    }
}