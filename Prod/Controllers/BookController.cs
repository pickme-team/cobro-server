using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prod.Exceptions;
using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Models.Responses;
using Prod.Services;

namespace Prod.Controllers;

[ApiController]
[Route("book")]
[Authorize]
public class BookController(IBookService bookService, IQrCodeService qrCodeService, ProdContext context)
    : ControllerBase
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

    [HttpGet("/admin/active")]
    [Authorize(Policy = "Admin")]
    public List<BookWithUserResponse> GetAllAdmin() => bookService.GetAll().Result
        .FindAll(b => b.Status == Status.Active || b.Status == Status.Pending);

    [HttpGet("active")]
    public async Task<List<BookResponse>> ActiveBooks()
    {
        var books = await bookService.ActiveBooks(User.Id());
        return books.Select(BookResponse.From).ToList();
    }

    [HttpPatch("{id:guid}/reschedule")]
    public Task Reschedule(Guid id, [FromBody] RescheduleRequest req) =>
        bookService.EditDateBook(id, req.From, req.To, User.Id());

    [HttpGet("{id:guid}/validate")]
    public async Task<ActionResult> Validate(Guid id, [FromQuery] DateTime from, [FromQuery] DateTime to,
        [FromQuery] Guid? seatId)
    {
        try
        {
            var isAvailable = await bookService.Validate(id, from, to, User.Id(), seatId);
            return isAvailable
                ? Ok()
                : StatusCode(StatusCodes.Status409Conflict, "Time not available");
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
    }

    [HttpGet("qr")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<Book>> Qr([FromQuery] long id)
    {
        try
        {
            var qr = qrCodeService.Get(id);
            if (qr == null) return NotFound("QR code not found");
            var book = await bookService.GetBookById(qr!.Value);
            if (book == null) return NotFound("Book not found");
            return Ok(book);
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
    }

    [HttpPost("massBook")]
    public async Task<ActionResult<List<BookResponse>>> MassBook([FromQuery] Guid zoneId, [FromQuery] DateOnly from,
        [FromQuery] DateOnly to, [FromQuery] TimeOnly fromTime, [FromQuery] TimeOnly toTime,
        [FromQuery] string description)
    {
        if (from > to)
        {
            return BadRequest("Start date cannot be greater than end date");
        }

        var userId = User.Id();
        List<BookResponse> bookResponses = new List<BookResponse>();

        for (var date = from; date <= to; date = date.AddDays(1))
        {
            DateTime startDateTime = date.ToDateTime(fromTime);
            DateTime endDateTime = date.ToDateTime(toTime);

            try
            {
                var bookRequest = new BookRequest
                {
                    From = startDateTime,
                    To = endDateTime,
                    Description = description
                };

                Guid? seatId = null;
                Zone zone = await context.Zones.FindAsync(zoneId) ?? throw new NotFoundException("Zone not found");

                switch (zone)
                {
                    case OfficeZone officeZone:
                        seatId = await GetAvailableSeat(officeZone, startDateTime, endDateTime);
                        if (seatId == null)
                        {
                            return Forbid("No seats available for the specified date range.");
                        }

                        break;

                    case TalkroomZone talkroomZone:
                        if (!await IsTalkroomZoneAvailable(talkroomZone, startDateTime, endDateTime))
                        {
                            return Forbid("Talkroom is not available for the specified date range.");
                        }

                        break;

                    case OpenZone openZone:
                        if (!await IsOpenZoneAvailable(openZone, startDateTime, endDateTime))
                        {
                            return Forbid("Open zone is not available for the specified date range.");
                        }

                        break;

                    default:
                        return BadRequest("Zone type not supported.");
                }

                // Бронируем место
                await bookService.Book(zoneId, seatId, userId, bookRequest);

                // Получаем созданную запись из таблицы Books
                Book? book = await GetCreatedBook(zone, userId, startDateTime, endDateTime, seatId);

                if (book != null)
                {
                    bookResponses.Add(BookResponse.From(book));
                }
                else
                {
                    return StatusCode(418, "Failed to retrieve the newly created book.");
                }
            }
            catch (ForbiddenException ex)
            {
                return Forbid(ex.Message); // Возвращаем 403 Forbidden
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Возвращаем 400 Bad Request для других ошибок
            }
        }

        return Ok(bookResponses);
    }

    private async Task<Guid?> GetAvailableSeat(OfficeZone officeZone, DateTime start, DateTime end)
    {
        await context.Entry(officeZone)
            .Collection(oz => oz.Seats)
            .LoadAsync();

        foreach (var seat in officeZone.Seats)
        {
            bool isSeatAvailable = !await context.Books
                .OfType<OfficeBook>()
                .AnyAsync(b =>
                    b.OfficeSeatId == seat.Id &&
                    (b.Status == Status.Active || b.Status == Status.Pending) &&
                    b.Start < end.ToUniversalTime() && start.ToUniversalTime() < b.End);

            if (isSeatAvailable)
            {
                return seat.Id;
            }
        }

        return null;
    }

    private async Task<bool> IsTalkroomZoneAvailable(TalkroomZone zone, DateTime from, DateTime to)
    {
        return !await context.Books
            .OfType<TalkroomBook>()
            .AnyAsync(b =>
                b.TalkroomZoneId == zone.Id &&
                (b.Status == Status.Active || b.Status == Status.Pending) &&
                b.Start < to.ToUniversalTime() && from.ToUniversalTime() < b.End);
    }

    private async Task<bool> IsOpenZoneAvailable(OpenZone zone, DateTime from, DateTime to)
    {
        var books = await context.Books
            .OfType<OpenBook>()
            .Where(b => b.OpenZoneId == zone.Id && (b.Status == Status.Active || b.Status == Status.Pending))
            .ToListAsync();

        var events = books
            .SelectMany(b => new List<(DateTime Time, bool IsStart)>
            {
                (b.Start.ToUniversalTime(), true),
                (b.End.ToUniversalTime(), false)
            })
            .OrderBy(e => e.Time);

        int overlapCount = 0;

        foreach (var (time, isStart) in events)
        {
            if (isStart)
            {
                overlapCount++;
                if (overlapCount > zone.Capacity && time >= from.ToUniversalTime() && time <= to.ToUniversalTime())
                    return false;
            }
            else
            {
                overlapCount--;
            }
        }

        return true;
    }

    private async Task<Book?> GetCreatedBook(Zone zone, Guid userId, DateTime startDateTime, DateTime endDateTime,
        Guid? seatId)
    {
        if (zone is OfficeZone officeZone)
        {
            return await context.Books
                .OfType<OfficeBook>()
                .FirstOrDefaultAsync(b =>
                    b.OfficeSeatId == seatId &&
                    b.UserId == userId &&
                    b.Start == startDateTime.ToUniversalTime() &&
                    b.End == endDateTime.ToUniversalTime());
        }

        if (zone is TalkroomZone talkroomZone)
        {
            return await context.Books
                .OfType<TalkroomBook>()
                .FirstOrDefaultAsync(b =>
                    b.TalkroomZoneId == zone.Id &&
                    b.UserId == userId &&
                    b.Start == startDateTime.ToUniversalTime() &&
                    b.End == endDateTime.ToUniversalTime());
        }

        if (zone is OpenZone openZone)
        {
            return await context.Books
                .OfType<OpenBook>()
                .FirstOrDefaultAsync(b =>
                    b.OpenZoneId == zone.Id &&
                    b.UserId == userId &&
                    b.Start == startDateTime.ToUniversalTime() &&
                    b.End == endDateTime.ToUniversalTime());
        }

        return null; // Если зона не поддерживается
    }
}
