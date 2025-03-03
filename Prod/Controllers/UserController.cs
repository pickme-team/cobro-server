using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prod.Exceptions;
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
    [Authorize(Policy = "Admin")]
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

    [HttpGet]
    public Task<UserResponse> GetCurrentUser() =>
        userService.UserById(User.Id());

    [HttpPost("upload")]
    public async Task<ActionResult<string>> UploadMedia(IFormFile file)
    {
        try
        {
            return Ok(await userService.UploadMedia(file, User.Id()));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
    }

    [HttpPost("{id:guid}/passport")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> SetPassport([FromForm] Passport passport, Guid id)
    {
        await userService.SetPassport(User.Id(), passport);
        return Ok();
    }

    [HttpPost("{id:guid}/verification-photo")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> SetVerificationPhoto([FromBody] IFormFile file, Guid id)
    {
        try
        {
            await userService.SetVerificationPhoto(file, id);
            return Ok();
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
    }

    [HttpGet("{id:guid}/passport")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<Passport>> GetPassport(Guid id)
    {
        User user = await userService.Get(id);
        return Ok(user.Passport);
    }

    [HttpGet("{id:guid}/verification")]
    public async Task<ActionResult<bool>> Verification(Guid id)
    {
        try
        {
            var user = await userService.Get(id);
            return Ok(user.Passport != null || user.Role == Role.ADMIN || user.Role == Role.INTERNAL);
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
    }
}
