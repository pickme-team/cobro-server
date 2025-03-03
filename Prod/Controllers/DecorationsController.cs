using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Services;

namespace Prod.Controllers;

[ApiController]
[Route("decorations")]
public class DecorationsController(IDecorationService decorationService) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public Task Add([FromBody] DecorationCreateRequest req) => decorationService.Add(req);

    [HttpGet]
    public Task<List<Decoration>> GetAll() => decorationService.GetAll();
}
