using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prod.Models.Database;
using Prod.Services;

namespace Prod.Controllers;

[ApiController]
[Route("request")]
[Authorize]
public class RequestController(IRequestService requestService) : ControllerBase
{
    [HttpPost]
    public Task Add([FromBody] Request request) => requestService.Add(request);

    [HttpGet("today")]
    [Authorize(Policy = "Admin")]
    public Task<List<Request>> Today() => requestService.Today();

    [HttpPatch("{id:guid}/mark")]
    [Authorize(Policy = "Admin")]
    public Task Mark(Guid id, [FromQuery] RequestStatus status) => requestService.Mark(id, status);
}
