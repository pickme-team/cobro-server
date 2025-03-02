using Microsoft.EntityFrameworkCore;
using Prod.Exceptions;
using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Models.Responses;

namespace Prod.Services;

public class BookService(ProdContext context, IQrCodeService qrCodeService, IUserService userService) : IBookService
{
    public async Task<List<Book>> GetAllActiveBooks() =>
        await context.Books
            .Include(b => ((OpenBook)b).OpenZone)
            .Include(b => ((TalkroomBook)b).TalkroomZone)
            .Include(b => ((OfficeBook)b).OfficeSeat)
            .Include(b => b.User)
            .Where(b => b.Status == Status.Active || b.Status == Status.Pending)
            .ToListAsync();

    public async Task CancelBook(Guid bookId)
    {
        var book = await context.Books.FindAsync(bookId);

        if (book == null)
            return;

        book.Status = Status.Cancelled;
        await context.SaveChangesAsync();
    }

    public async Task EditDateBook(Guid bookId, DateTime start, DateTime end)
    {
        if (start > end)
            throw new ArgumentOutOfRangeException("Start date cannot be greater than end date");
        
        await CancelBook(bookId);

        var book = await context.Books.SingleAsync(b => b.Id == bookId);
        var zoneId = book switch
        {
            OfficeBook officeBook => officeBook.OfficeSeat.OfficeZoneId,
            OpenBook openBook => openBook.OpenZoneId,
            TalkroomBook talkroomBook => talkroomBook.TalkroomZoneId,
            _ => throw new ArgumentOutOfRangeException(nameof(book), book, null)
        };

        var data = new BookRequest()
        {
            From = start,
            To = end,
            Description = book.Description
        };
        await Book(zoneId, (book as OfficeBook)?.OfficeSeat.Id, book.UserId, data);
    }

    public async Task<Book?> GetBookById(Guid bookId) =>
        await context.Books.FindAsync(bookId);

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
            .AnyAsync(b => b.Start < req.To.ToUniversalTime() && req.From.ToUniversalTime() < b.End);

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
            .AnyAsync(b => b.Start < req.To.ToUniversalTime() && req.From.ToUniversalTime() < b.End);

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
        if (t.Item1 != null) return new QrResponse { Code = t.Item1!.ToString(), Ttl = t.Item2!.Value };
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

    public async Task<bool> Validate(Guid zoneId, DateTime from, DateTime to, Guid guid)
    {
        var zone = await context.Zones.SingleAsync(z => z.Id == zoneId);
        return zone switch
        {
            OfficeZone officeZone => await ValidateOfficeZone(officeZone, from, to, guid),
            TalkroomZone talkroomZone => await ValidateTalkroomZone(talkroomZone, from, to, guid),
            OpenZone openZone => await ValidateOpenZone(openZone, from, to, guid),
            _ => throw new ForbiddenException("Not a bookable zone")
        };
    }

    public async Task<List<BookWithUserResponse>> GetAll()
    {
        var entities = await context.Books
            .Include(b => ((OpenBook)b).OpenZone)
            .Include(b => ((TalkroomBook)b).TalkroomZone)
            .Include(b => ((OfficeBook)b).OfficeSeat)
            .ThenInclude(s => s.OfficeZone)
            .Include(b => b.User)
            .ToListAsync();

        foreach (var entity in entities)
        {
            entity.User.Books.Clear();
        }

        return entities.Select(BookWithUserResponse.From).ToList();
    }

    private async Task<bool> ValidateOfficeZone(OfficeZone zone, DateTime from, DateTime to, Guid userId)
    {
        var seats = await context.Entry(zone)
            .Collection(x => x.Seats)
            .Query()
            .ToListAsync();
        if (!zone.IsPublic && userService.UserById(userId).Result.Role == Role.CLIENT) return false;

        return seats.Any(seat => !seat.Books.Any(b =>
            (b.Status == Status.Active || b.Status == Status.Pending) &&
            b.Start < to && from < b.End));
    }

    private async Task<bool> ValidateTalkroomZone(TalkroomZone zone, DateTime from, DateTime to, Guid userId)
    {
        if (!zone.IsPublic && userService.UserById(userId).Result.Role == Role.CLIENT) return false;
        return !await context.Entry(zone)
            .Collection(z => z.Books)
            .Query()
            .Where(b => b.Status == Status.Active || b.Status == Status.Pending)
            .AnyAsync(b => b.Start < to && from < b.End);
    }

    private async Task<bool> ValidateOpenZone(OpenZone zone, DateTime from, DateTime to, Guid userId)
    {
        if (!zone.IsPublic && userService.UserById(userId).Result.Role == Role.CLIENT) return false;
        var books = await context.Entry(zone)
            .Collection(z => z.Books)
            .Query()
            .Where(b => b.Status == Status.Active || b.Status == Status.Pending)
            .ToListAsync();

        var events = books
            .SelectMany(b => new List<(DateTime, bool)> { (b.Start, true), (b.End, false) })
            .OrderBy(t => t.Item1)
            .ToList();

        var overlapCount = 0;
        foreach (var (time, isStart) in events)
        {
            if (isStart)
            {
                overlapCount++;
                if (overlapCount == zone.Capacity && time >= from && time <= to)
                    return false;
            }
            else
            {
                overlapCount--;
            }
        }

        return true;
    }
}
