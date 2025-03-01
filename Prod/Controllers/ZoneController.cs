using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Prod.Models.Database;
using Prod.Services;

namespace Prod.Controllers;

[ApiController]
[Authorize]
[Route("zone")]
public class ZoneController(IZoneService zoneService) : ControllerBase
{
    [HttpGet]
    public List<Zone> GetAll()
    {
        return zoneService.GetAll();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Zone>> Get(Guid id)
    {
        Zone? zone = await zoneService.Get(id);
        if (zone is null) return NotFound();
        return Ok(zone);
    }

    [HttpPost]
    public async Task<ActionResult<Zone>> Add([FromBody] Zone zone)
    {
        return await zoneService.Create(zone);
    }
}
