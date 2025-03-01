using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prod.Models.Requests;
using Prod.Services;

namespace Prod.Controllers;

[ApiController]
[Route("book")]
[Authorize]
public class BookController(IBookService bookService) : ControllerBase
{
    [HttpPost("{id:guid}")]
    public Task Book(Guid id, [FromQuery(Name = "seat-id")] Guid? seatId, [FromBody] BookRequest req) =>
        bookService.Book(id, seatId, User.Id(), req);
}
