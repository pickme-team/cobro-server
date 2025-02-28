using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Prod.Models.Database;
using Prod.Services;

namespace Prod.Controllers;

[ApiController]
[Route("user")]
public class UserController(UserService userService) : ControllerBase
{
    [HttpGet("{id}")]
    public ActionResult GetUserById([FromRoute] Guid id)
    {
        var user = userService.UserById(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("all")]
    public ActionResult GetAllUsers()
    {
        var users = userService.AllUsers();
        return Ok(users);
    }

    [HttpPut("{id}")]
    public ActionResult UpdateUser([FromRoute] Guid id, [FromBody] User user)
    {
        if (id != user.Id)
        {
            return BadRequest(new { Message = "id не совпадает" });
        }

        userService.Update(user);
        return Ok();
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteUser([FromRoute] Guid id)
    {
        var user = userService.UserById(id);
        if (user is null)
        {
            return NotFound();
        }

        userService.Delete(user);
        return Ok();
    }

    [HttpPost]
    public ActionResult CreateUser([FromBody] User user)
    {
        userService.Create(user);
        return Ok();
    }

    [HttpGet("email")]
    public ActionResult GetUserByEmail([FromQuery] string email)
    {
        string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        if (!Regex.IsMatch(email, emailPattern))
        {
            return BadRequest(new { Message = "Неправильный формат почты" });
        }

        var user = userService.UserByEmail(email);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPut("city/{id}")]
    public ActionResult UpdateCity([FromRoute] Guid id, [FromBody] string city)
    {
        var user = userService.UserById(id);
        if (user is null)
        {
            return NotFound();
        }

        user.City = city;
        userService.Update(user);
        return Ok(user);
    }
}
