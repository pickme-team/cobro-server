using Microsoft.AspNetCore.Mvc;
using Prod.Models.Database;
using Prod.Services;

namespace Prod.Controllers;

[ApiController]
[Route("decorations")]
public class DecorationsController(IDecorationService decorationService)
{
    [HttpGet]
    public Task<List<Decoration>> GetAll() => decorationService.GetAll();
}
