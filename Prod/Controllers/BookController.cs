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

    [HttpGet("{id:guid}/qr")]
    public Task<QrResponse> Qr(Guid id) => bookService.Qr(id, User.Id());

    [HttpPost("/confirm-qr")]
    [Authorize(Policy = "Admin")]
    public Task ConfirmQr([FromBody] ConfirmQrRequest req) =>
        bookService.ConfirmQr(req);

    [HttpPost("last")]
    public async Task<BookResponse?> Last() =>
        await bookService.LastBook(User.Id()) is { } book ? BookResponse.From(book) : null;

    [HttpPost("history")]
    public async Task<List<BookResponse>> History()
    {
        var books = await bookService.UserHistory(User.Id());
        return books.Select(BookResponse.From).ToList();
    }

    [HttpPost("active")]
    public async Task<List<BookResponse>> ActiveBooks()
    {
        var books = await bookService.ActiveBooks(User.Id());
        return books.Select(BookResponse.From).ToList();
    }
}
