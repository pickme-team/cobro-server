using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prod.Services;

namespace Prod.Controllers;

[ApiController]
[Route("book")]
[Authorize]
public class BookController(IBookService bookService) : ControllerBase
{
//     // TODO Admin-only authorization for this endpoint
//     [HttpPost("place-count")]
//     public Task SetPlaceCount([FromQuery] int count) => bookService.SetPlaceCount(count);
//
//     [HttpGet("place-count")]
//     public async Task<ActionResult> PlacesCount() => Ok(new { Count = await bookService.PlaceCount() });
//
//     [HttpPost("room/{id:guid}")]
//     public Task BookRoom(Guid id, [FromBody] BookRequest req) =>
//         bookService.BookRoom(id, User.Id(), req);
//
//     [HttpPost("place/{id:guid}")]
//     public Task BookPlace(Guid id, [FromBody] BookRequest req) =>
//         bookService.BookPlace(id, User.Id(), req);
//
//     [HttpPost("space/{id:guid}")]
//     public Task BookSpace(Guid id, [FromBody] BookRequest req) =>
//         bookService.BookSpace(id, User.Id(), req);
}
