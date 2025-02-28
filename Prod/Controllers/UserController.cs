using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prod.Models;
using Prod.Models.Database;
using Prod.Services;

namespace Prod.Controllers;

[ApiController]
[Route("user")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUserById([FromRoute] Guid id)
    {
        var user = await userService.UserById(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("all")]
    public Task<List<User>> GetAllUsers() => userService.AllUsers();

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser([FromRoute] Guid id, [FromBody] User user)
    {
        if (id != user.Id)
        {
            return BadRequest(new { Message = "id не совпадает" });
        }

        await userService.Update(user);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser([FromRoute] Guid id)
    {
        var user = await userService.UserById(id);
        if (user is null)
        {
            return NotFound();
        }

        await userService.Delete(user);
        return Ok();
    }

    [HttpPost]
    public Task CreateUser([FromBody] User user) => userService.Create(user);

    [HttpGet("email")]
    public async Task<ActionResult<User>> GetUserByEmail([FromQuery] [Email] string email)
    {
        var user = await userService.UserByEmail(email);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPut("city/{id:guid}")]
    public async Task<ActionResult<User>> UpdateCity([FromRoute] Guid id, [FromBody] string city)
    {
        var user = await userService.UserById(id);
        if (user is null)
        {
            return NotFound();
        }

        user.City = city;
        await userService.Update(user);
        return Ok(user);
    }

    [HttpGet]
    public async Task<ActionResult<User>> GetCurrentUser()
    {
        var user = await userService.UserById(User.Id());
        return user is null ? NotFound() : Ok(user);
    }
}
