using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prod.Models.Database;
using Prod.Services;

namespace Prod.Controllers;

[ApiController]
[Authorize]
[Route("place")]
public class PlaceController(PlaceService placeService) : ControllerBase
{
    // [HttpGet("{id}")]
    // public async Task<ActionResult<Place>> GetUserById([FromRoute] Guid id)
    // {
    //     var place = await placeService.PlaceById(id);
    //     return place is null ? NotFound() : Ok(place);
    // }
    //
    // [HttpPut("{id}")]
    // public async Task<ActionResult> UpdatePlace([FromBody] Place place)
    // {
    //     await placeService.Update(place);
    //     return Created();
    // }
}
