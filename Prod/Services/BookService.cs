using Microsoft.EntityFrameworkCore;
using Prod.Exceptions;
using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Models.Responses;

namespace Prod.Services;

public class BookService(ProdContext context, IQrCodeService qrCodeService) : IBookService
{
    public async Task Book(Guid zoneId, Guid? seatId, Guid userId, BookRequest bookRequest)
    {
        var zone = await context.Zones
            .SingleAsync(z => z.Id == zoneId);
        switch (zone)
        {
            case OfficeZone officeZone:
                await BookSeat(officeZone, seatId!.Value, userId, bookRequest);
                break;
            case TalkroomZone talkroomZone:
                await BookTalkroomZone(talkroomZone, userId, bookRequest);
                break;
            case OpenZone openZone:
                await BookOpenZone(openZone, userId, bookRequest);
                break;
            default:
                throw new ForbiddenException("Not a bookable zone");
        }
    }

    private async Task BookSeat(OfficeZone zone, Guid seatId, Guid userId, BookRequest req)
    {
        var seat = await context.Entry(zone)
            .Collection(x => x.Seats)
            .Query()
            .SingleAsync(x => x.Id == seatId);

        var overlaps = await context.Entry(seat)
            .Collection(s => s.Books)
            .Query()
            .Where(b => b.Status == Status.Active || b.Status == Status.Pending)
            .AnyAsync(b => b.Start < req.To && req.From < b.End);

        if (overlaps) throw new ForbiddenException("Time not available");

        context.Books.Add(new OfficeBook
        {
            Start = req.From.ToUniversalTime(),
            End = req.To.ToUniversalTime(),
            UserId = userId,
            Description = req.Description,
            Status = Status.Pending,
            OfficeSeat = seat
        });
        await context.SaveChangesAsync();
    }

    private async Task BookTalkroomZone(TalkroomZone zone, Guid userId, BookRequest req)
    {
        var overlaps = await context.Entry(zone)
            .Collection(z => z.Books)
            .Query()
            .Where(b => b.Status == Status.Active || b.Status == Status.Pending)
            .AnyAsync(b => b.Start < req.To && req.From < b.End);

        if (overlaps) throw new ForbiddenException("Time not available");

        context.Books.Add(new TalkroomBook
        {
            Start = req.From.ToUniversalTime(),
            End = req.To.ToUniversalTime(),
            UserId = userId,
            Description = req.Description,
            Status = Status.Pending,
            TalkroomZone = zone
        });
        await context.SaveChangesAsync();
    }

    private async Task BookOpenZone(OpenZone zone, Guid userId, BookRequest req)
    {
        var timespans = await context.Entry(zone)
            .Collection(z => z.Books)
            .Query()
            .Where(b => b.Status == Status.Active || b.Status == Status.Pending)
            .Select(b => new { b.Start, b.End })
            .ToListAsync();
        var capacity = zone.Capacity;

        // true - start
        var events = timespans
            .SelectMany(t => new List<(DateTime, bool)> { (t.Start, true), (t.End, false) })
            .OrderBy(t => t.Item1);

        var overlapCount = 0;

        foreach (var (time, type) in events)
        {
            if (type)
            {
                overlapCount++;
                if (overlapCount == capacity && time >= req.From && time <= req.To)
                    throw new ForbiddenException("Time not available");
            }
            else
            {
                overlapCount--;
            }
        }

        context.Books.Add(new OpenBook
        {
            Start = req.From.ToUniversalTime(),
            End = req.To.ToUniversalTime(),
            UserId = userId,
            Description = req.Description,
            Status = Status.Pending,
            OpenZone = zone
        });
        await context.SaveChangesAsync();
    }

    public async Task<BookResponse> Delete(Guid id)
    {
        var book = await GetById(id);
        context.Books.Remove(book);
        await context.SaveChangesAsync();
        return BookResponse.From(book);
    }

    private Task<Book> GetById(Guid id) =>
        context.Books
            .Include(b => ((OpenBook)b).OpenZone)
            .Include(b => ((TalkroomBook)b).TalkroomZone)
            .Include(b => ((OfficeBook)b).OfficeSeat)
            .ThenInclude(s => s.OfficeZone)
            .SingleAsync(b => b.Id == id);

    public async Task<List<BookResponse>> GetBooks(Guid id, Guid? seatId)
    {
        var zone = await context.Zones.SingleAsync(z => z.Id == id);
        switch (zone)
        {
            case OfficeZone officeZone:
                var seat = await context.Entry(officeZone)
                    .Collection(z => z.Seats)
                    .Query()
                    .Include(s => s.Books)
                    .ThenInclude(b => b.OfficeSeat)
                    .SingleAsync(s => s.Id == seatId);
                return seat.Books.Select(BookResponse.From).ToList();
            case TalkroomZone talkroomZone:
                await context.Entry(talkroomZone)
                    .Collection(z => z.Books)
                    .Query()
                    .Include(b => b.TalkroomZone)
                    .LoadAsync();
                return talkroomZone.Books.Select(BookResponse.From).ToList();
            case OpenZone openZone:
                await context.Entry(openZone)
                    .Collection(z => z.Books)
                    .Query()
                    .Include(b => b.OpenZone)
                    .LoadAsync();
                return openZone.Books.Select(BookResponse.From).ToList();
            default:
                return [];
        }
    }

    public async Task<QrResponse> Qr(Guid bookId, Guid userId)
    {
        Tuple<long?, int?> t = qrCodeService.GetByValue(bookId);
        if (t.Item1 != null) return new QrResponse { Code = t.Item1!.ToString(), Ttl = t.Item2!.Value};
        var book = await context.Books.SingleAsync(b => b.Id == bookId);
        if (book.UserId != userId)
            throw new ForbiddenException("You did not book this");
        if (book.Status != Status.Pending)
            throw new ForbiddenException("This book is not pending");

        var code = Random.Shared.NextInt64(0, 9999999999);
        qrCodeService[code] = bookId;

        return new QrResponse
        {
            Code = code.ToString().PadLeft(10, '0'),
            Ttl = QrCodeService.Ttl
        };
    }

    public async Task<List<Book>> ActiveBooks(Guid id) =>
        await context.Books
            .Include(b => ((OpenBook)b).OpenZone)
            .Include(b => ((TalkroomBook)b).TalkroomZone)
            .Include(b => ((OfficeBook)b).OfficeSeat)
            .Where(b => b.UserId == id && b.Status == Status.Active)
            .ToListAsync();

    public async Task<List<Book>> UserHistory(Guid id) =>
        await context.Books
            .Include(b => ((OpenBook)b).OpenZone)
            .Include(b => ((TalkroomBook)b).TalkroomZone)
            .Include(b => ((OfficeBook)b).OfficeSeat)
            .Where(b => b.UserId == id && b.Status != Status.Active)
            .ToListAsync();

    public async Task<Book?> LastBook(Guid id) =>
        await context.Books
            .Include(b => ((OpenBook)b).OpenZone)
            .Include(b => ((TalkroomBook)b).TalkroomZone)
            .Include(b => ((OfficeBook)b).OfficeSeat)
            .Where(b => b.UserId == id && b.Status == Status.Ended)
            .OrderByDescending(b => b.End)
            .LastOrDefaultAsync();

    public async Task ConfirmQr(ConfirmQrRequest req)
    {
        var expectedCode = qrCodeService[long.Parse(req.Code)];
        if (expectedCode == null)
            throw new ForbiddenException("Qr code not found or expired");
        var book = await context.Books.SingleAsync(b => b.Id == expectedCode);
        if (book.Status != Status.Pending)
            throw new ForbiddenException("This book is not pending");

        book.Status = Status.Active;
        await context.SaveChangesAsync();
    }
}
