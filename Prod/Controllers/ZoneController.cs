using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Services;

namespace Prod.Controllers;

[ApiController]
[Authorize]
[Route("zone")]
public class ZoneController(IZoneService zoneService) : ControllerBase
{
    [HttpGet]
    public Task<List<Zone>> GetAll() => zoneService.GetAll();

    [HttpGet("{id:guid}")]
    public Task<Zone> Get(Guid id) => zoneService.Get(id);

    [HttpPost]
    public Task<Zone> Add(
        [FromQuery] [Required] [AllowedValues("office", "open", "talkroom", "misc")]
        string type,
        [FromBody] ZoneCreateRequest req) =>
        zoneService.Create(type, req);

    [HttpGet("{id:guid}/current")]
    public Task<int> Update([Required] [FromRoute] Guid id) =>
        zoneService.GetCurrentUsersCount(id);
}
