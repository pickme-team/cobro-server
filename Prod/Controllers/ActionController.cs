using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prod.Services;
using Action = Prod.Models.Database.Action;

namespace Prod.Controllers;

[Authorize]
[ApiController]
[Route("action")]
public class ActionController(IActionService actionService) : ControllerBase
{
    [HttpGet]
    public Task<List<Action>> GetAll() => actionService.AllActions();

    [HttpGet("{id:long}")]
    public Task<Action> Get(long id) => actionService.Get(id);

    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<Action> Add([FromBody] Action action) => await actionService.Create(action);

    [HttpPut]
    [Authorize(Policy = "Admin")]
    public async Task<Action> Update([FromBody] Action action) => await actionService.Update(action);

    [HttpDelete]
    [Authorize(Policy = "Admin")]
    public async Task<Action> Delete([FromBody] Action action) => await actionService.Delete(action);
}
