using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Prod.Exceptions;
using Prod.Models;
using Prod.Models.Database;
using Prod.Models.Requests;
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
            return BadRequest(new { e.Message });
        }
    }

    [HttpPost("{id:guid}/passport")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> SetPassport([FromBody] PassportCreateRequest req, Guid id)
    {
        try
        {
            await userService.SetPassport(id, req);
            return Ok();
        }
        catch (ArgumentException e)
        {
            return Conflict(new { e.Message });
        }
    }

    [HttpPost("{id:guid}/verification-photo")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> SetVerificationPhoto(IFormFile file, Guid id)
    {
        try
        {
            await userService.SetVerificationPhoto(file.OpenReadStream(), file.FileName, id);
            return Ok();
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { e.Message });
        }
    }

    [HttpGet("{id:guid}/verification-photo")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> GetVerificationPhoto(Guid id) =>
        Ok(new { Link = await userService.GetVerificationPhoto(id) });

    [HttpGet("{id:guid}/passport")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<Passport>> GetPassport(Guid id)
    {
        var user = await userService.Get(id);
        if (user.Passport is null) return NotFound();
        try
        {
            var passport = new Passport
            {
                Id = user.Passport.Id,
                Number = user.Passport.Number.Substring(0, 2) + new string('*', user.Passport.Number.Length - 4) +
                         user.Passport.Number.Substring(user.Passport.Number.Length - 2),
                Serial = user.Passport.Serial.Substring(0, 2) + new string('*', user.Passport.Serial.Length - 2),
                Firstname = user.Passport.Firstname,
                Lastname = user.Passport.Lastname,
                Middlename = user.Passport.Middlename,
                Birthday = user.Passport.Birthday
            };
            return Ok(passport);
        }
        catch
        {
            return Ok(user.Passport);
        }
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
