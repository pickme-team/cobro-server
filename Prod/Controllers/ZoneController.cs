using Microsoft.AspNetCore.Mvc;
using Prod.Services;

namespace Prod.Controllers;

[ApiController]
public class ZoneController
{
    [HttpPost("{id:Guid}")]
    public async Task AddSeat(Guid zoneId)
    {
        
    }
}