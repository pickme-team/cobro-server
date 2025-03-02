using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Models.Responses;
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

    [HttpGet("{id:guid}")]
    public Task<List<BookResponse>> GetBooks(Guid id, [FromQuery(Name = "seat-id")] Guid? seatId) =>
        bookService.GetBooks(id, seatId);

    [HttpDelete("{id:guid}")]
    public Task<BookResponse> Delete(Guid id) => bookService.Delete(id);
}
