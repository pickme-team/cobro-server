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
    public Task<BookResponse> Delete(Guid id) =>
        bookService.Delete(id);

    [HttpPatch("{id:guid}/cancel")]
    public Task Cancel(Guid id) => bookService.CancelBook(id);

    [HttpGet("{id:guid}/qr")]
    public Task<QrResponse> Qr(Guid id) =>
        bookService.Qr(id, User.Id());

    [HttpPatch("/confirm-qr")]
    [Authorize(Policy = "Admin")]
    public Task<ConfirmQrResponse> ConfirmQr([FromBody] ConfirmQrRequest req) =>
        bookService.ConfirmQr(req);

    [HttpGet("last")]
    public async Task<BookResponse?> Last() =>
        await bookService.LastBook(User.Id()) is { } book ? BookResponse.From(book) : null;

    [HttpGet("history")]
    public async Task<List<BookResponse>> History() =>
        (await bookService.UserHistory(User.Id())).Select(BookResponse.From).ToList();

    [HttpGet]
    [Authorize(Policy = "Admin")]
    public Task<List<BookWithUserResponse>> GetAll() =>
        bookService.GetAll();

    [HttpGet("/admin/active")]
    [Authorize(Policy = "Admin")]
    public async Task<List<BookWithUserResponse>> GetAllAdmin() =>
        (await bookService.GetAllActiveBooks()).Select(BookWithUserResponse.From).ToList();

    [HttpGet("active")]
    public async Task<List<BookResponse>> ActiveBooks() =>
        (await bookService.ActiveBooks(User.Id())).Select(BookResponse.From).ToList();

    [HttpPatch("{id:guid}/reschedule")]
    public Task Reschedule(Guid id, [FromBody] RescheduleRequest req) =>
        bookService.EditDateBook(id, req.From, req.To, User.Id());

    [HttpGet("{id:guid}/validate")]
    public async Task<ActionResult> Validate(Guid id, [FromQuery] DateTime from, [FromQuery] DateTime to,
        [FromQuery] Guid? seatId) =>
        await bookService.Validate(id, from, to, User.Id(), seatId)
            ? Ok()
            : Conflict("Time not available");

    [HttpGet("qr")]
    [Authorize(Policy = "Admin")]
    public Task<Book> GetBookByQr([FromQuery] long id) =>
        bookService.GetBookByQr(id);

    [HttpPost("massBook")]
    public async Task<ActionResult<List<BookResponse>>> MassBook([FromQuery] Guid zoneId, [FromQuery] DateOnly from,
        [FromQuery] DateOnly to, [FromQuery] TimeOnly fromTime, [FromQuery] TimeOnly toTime,
        [FromQuery] string description)
    {
        if (from > to)
            return BadRequest("Start date cannot be greater than end date");

        return await bookService.MassBook(zoneId, from, to, fromTime, toTime, description, User.Id());
    }
}
