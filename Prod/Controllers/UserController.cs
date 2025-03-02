using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prod.Models;
using Prod.Models.Database;
using Prod.Models.Responses;
using Prod.Services;

namespace Prod.Controllers;

[ApiController]
[Route("user")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("{id}")]
    public Task<UserResponse> GetUserById([FromRoute] Guid id) => userService.UserById(id);

    [HttpGet("all")]
    public Task<List<UserResponse>> GetAllUsers() => userService.AllUsers();

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
    public Task DeleteUser([FromRoute] Guid id) =>
        userService.Delete(id);

    [HttpGet("email")]
    public Task<UserResponse> GetUserByEmail([FromQuery] [Email] string email) =>
        userService.UserByEmail(email);

    [HttpPatch("city/{id:guid}")]
    public Task UpdateCity([FromRoute] Guid id, [FromBody] string city) =>
        userService.UpdateCity(id, city);

    [HttpGet]
    public Task<UserResponse> GetCurrentUser() =>
        userService.UserById(User.Id());

    [HttpPost("upload")]
    public async Task<ActionResult<string>> UploadMedia(IFormFile file)
    {
        try
        {
            return Ok(await userService.UploadMedia(file));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
    }
}
