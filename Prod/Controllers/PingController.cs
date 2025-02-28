using Microsoft.AspNetCore.Mvc;

namespace Prod.Controllers;

[ApiController]
public class PingController : ControllerBase
{
    [HttpGet("ping")]
    public ActionResult Ping() => Ok(new { Message = "Pong!" });
}
