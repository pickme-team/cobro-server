using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prod.Exceptions;
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

    [HttpGet("{id:guid}/qr")]
    public Task<QrResponse> Qr(Guid id) => bookService.Qr(id, User.Id());

    [HttpPatch("/confirm-qr")]
    [Authorize(Policy = "Admin")]
    public Task ConfirmQr([FromBody] ConfirmQrRequest req) =>
        bookService.ConfirmQr(req);

    [HttpGet("last")]
    public async Task<BookResponse?> Last() =>
        await bookService.LastBook(User.Id()) is { } book ? BookResponse.From(book) : null;

    [HttpGet("history")]
    public async Task<List<BookResponse>> History()
    {
        var books = await bookService.UserHistory(User.Id());
        return books.Select(BookResponse.From).ToList();
    }

    [HttpGet]
    [Authorize(Policy = "Admin")]
    public Task<List<BookWithUserResponse>> GetAll() => bookService.GetAll();

    [HttpGet("active")]
    public async Task<List<BookResponse>> ActiveBooks()
    {
        var books = await bookService.ActiveBooks(User.Id());
        return books.Select(BookResponse.From).ToList();
    }

    [HttpGet("{id:guid}/validate")]
    public async Task<ActionResult> Validate(Guid id, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        try
        {
            var isAvailable = await bookService.Validate(id, from, to, User.Id());
            return isAvailable
                ? Ok()
                : StatusCode(StatusCodes.Status409Conflict, "Time not available");
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
    }
}
