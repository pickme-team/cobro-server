using Microsoft.EntityFrameworkCore;
using Prod.Exceptions;
using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Models.Responses;
using Serilog;

namespace Prod.Services;

public class BookService(
    ProdContext context,
    IQrCodeService qrCodeService,
    IUserService userService,
    IEmailService emailService) : IBookService
{
    private IQueryable<Book> BooksQuery => context.Books
        .Include(b => ((OpenBook)b).OpenZone)
        .Include(b => ((TalkroomBook)b).TalkroomZone)
        .Include(b => ((OfficeBook)b).OfficeSeat)
        .ThenInclude(s => s.OfficeZone)
        .Include(b => b.User);

    public async Task<List<Book>> GetAllActiveBooks() =>
        await BooksQuery.Where(b => b.Status == Status.Active || b.Status == Status.Pending).ToListAsync();

    public async Task<List<Book>> GetAllFinishedBooks() =>
        await BooksQuery.Where(b => b.Status == Status.Ended || b.Status == Status.Cancelled).ToListAsync();

    public async Task CancelBook(Guid bookId)
    {
        var book = await context.Books.SingleAsync(b => b.Id == bookId);

        book.Status = Status.Cancelled;
        await context.SaveChangesAsync();
    }

    public async Task EditDateBook(Guid bookId, DateTime start, DateTime end, Guid? userId = null)
    {
        if (start > end)
            throw new ArgumentOutOfRangeException(nameof(start), "Start date cannot be greater than end date");

        await CancelBook(bookId);

        var book = await context.Books.SingleAsync(b => b.Id == bookId);
        if (userId.HasValue && book.UserId != userId.Value)
            throw new ForbiddenException("BRO WHY YOU EDITING SOMEONE ELSE'S BOOKING");

        var zoneId = book switch
        {
            OfficeBook officeBook => officeBook.OfficeSeat.OfficeZoneId,
            OpenBook openBook => openBook.OpenZoneId,
            TalkroomBook talkroomBook => talkroomBook.TalkroomZoneId,
            _ => throw new ArgumentOutOfRangeException(nameof(book), book, null)
        };

        var data = new BookRequest
        {
            From = start,
            To = end,
            Description = book.Description
        };
        await Book(zoneId, (book as OfficeBook)?.OfficeSeat.Id, book.UserId, data);
    }

    public async Task<Book?> GetBookById(Guid bookId) =>
        await context.Books.Include(b => b.User).SingleAsync(b => b.Id == bookId);

    public async Task Book(Guid zoneId, Guid? seatId, Guid userId, BookRequest bookRequest)
    {
        var zone = await context.Zones
            .SingleAsync(z => z.Id == zoneId);
        switch (zone)
        {
            case OfficeZone officeZone:
                await BookOfficeSeat(officeZone, seatId!.Value, userId, bookRequest);
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

        string messageTemplate = @"
<!DOCTYPE html>
<html lang=""ru"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Бронирование коворкинга зафиксировано!</title>
    <style>
        body {{
            font-family: Helvetica, Arial, sans-serif;
            background-color: #000000;
            margin: 0;
            padding: 0;
            color: #ffffff;
        }}
        .container {{
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            border: 1px solid #444444;
            border-radius: 8px;
        }}
        .header {{
            text-align: center;
            padding: 20px 0;
            border-bottom: 1px solid #444444;
        }}
        .header h1 {{
            font-size: 24px;
            margin: 0;
        }}
        .content {{
            padding: 20px 0;
        }}
        .content p {{
            font-size: 16px;
            line-height: 1.5;
            margin-bottom: 20px;
        }}
        .booking-info {{
            background-color: #333333;
            padding: 20px;
            border-radius: 6px;
            margin-bottom: 20px;
        }}
        .booking-info h2 {{
            font-size: 18px;
            margin-top: 0;
        }}
        .booking-info ul {{
            list-style: none;
            padding: 0;
            margin: 0;
        }}
        .booking-info li {{
            margin-bottom: 10px;
        }}
        .booking-info li strong {{
            font-weight: bold;
        }}
        .footer {{
            text-align: center;
            padding-top: 20px;
            font-size: 12px;
            color: #bbbbbb;
        }}
    </style>
</head>
<body>
<div class=""container"">
    <div class=""header"">
        <h1>Бронирование коворкинга зафиксировано!</h1>
    </div>
    <div class=""content"">
        <p>Спасибо за бронирование коворкинга в <b>cobro</b>! Мы рады видеть вас у нас.</p>
        <div class=""booking-info"">
            <h2>Детали бронирования:</h2>
            <ul>
                <li><strong>Дата:</strong> {{date}}</li>
                <li><strong>Время:</strong> {{time}} — {{time2}}</li>
                <li><strong>Коворкинг:</strong> {{coworkingName}}</li>
                <li><strong>Наш адрес:</strong> Москва, бульвар Маршала Рокоссовского, 25, подъезд 1</li>
                <a href=""https://yandex.ru/maps/?rtext=~55.814598,37.718186&rtt=auto"">Маршрут на Я.Картах</a>
            </ul>
        </div>
        <p>Мы ждем вас в назначенное время. Если у вас есть вопросы, не стесняйтесь обращаться к нам.</p>
    </div>
    <div class=""footer"">
        <p>&copy; 2025 cobro. All rights reserved.</p>
    </div>
</div>
</body>
</html>
";

        var message = messageTemplate
            .Replace("{{date}}", bookRequest.From.AddHours(3).ToString("d"))
            .Replace("{{time}}", bookRequest.From.AddHours(3).ToString("t"))
            .Replace("{{time2}}", bookRequest.To.AddHours(3).ToString("t"))
            .Replace("{{coworkingName}}", zone.Name);
        await emailService.SendEmailAsync(userService.UserById(userId).Result.Email,
            "Бронирование коворкинго успешно!",
            message);
    }

    private async Task BookOfficeSeat(OfficeZone zone, Guid seatId, Guid userId, BookRequest req)
    {
        var seat = await context.Entry(zone)
            .Collection(x => x.Seats)
            .Query()
            .SingleAsync(x => x.Id == seatId);

        if (!await IsOfficeSeatAvailable(seat, req.From, req.To))
            throw new ForbiddenException("Time not available");

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
        if (!await IsTalkroomZoneAvailable(zone, req.From, req.To))
            throw new ForbiddenException("Time not available");

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
        if (!await IsOpenZoneAvailable(zone, req.From, req.To))
            throw new ForbiddenException("Time not available");

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
        var book = await BooksQuery.SingleAsync(b => b.Id == id);
        context.Books.Remove(book);
        await context.SaveChangesAsync();
        return BookResponse.From(book);
    }

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
        var (c, ttl) = qrCodeService.GetByValue(bookId);
        if (c != null)
            return new QrResponse
            {
                Code = c.Value.ToString(),
                Ttl = ttl!.Value
            };

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

    public Task<List<Book>> ActiveBooks(Guid id) =>
        BooksQuery.Where(b => b.UserId == id && b.Status == Status.Active).ToListAsync();

    public async Task<List<BookWithUserResponse>> ActiveBooks() =>
        (await BooksQuery.Where(b => b.Status == Status.Active || b.Status == Status.Pending).ToListAsync())
        .Select(BookWithUserResponse.From).ToList();

    public async Task<List<Book>> UserHistory(Guid id) =>
        await BooksQuery.Where(b => b.UserId == id && b.Status != Status.Active).ToListAsync();

    public async Task<Book?> LastBook(Guid id) =>
        await BooksQuery.Where(b => b.UserId == id && b.Status == Status.Ended)
            .OrderByDescending(b => b.End)
            .LastOrDefaultAsync();

    public async Task<ConfirmQrResponse> ConfirmQr(ConfirmQrRequest req)
    {
        var expectedCode = qrCodeService[long.Parse(req.Code)];
        if (expectedCode == null)
            throw new ForbiddenException("Qr code not found or expired");
        var book = await context.Books
            .Include(b => b.User)
            .ThenInclude(u => u.Passport)
            .SingleAsync(b => b.Id == expectedCode);
        if (book.Status != Status.Pending)
            throw new ForbiddenException("This book is not pending");

        if (book.User.PassportId == null && book.User.Role == Role.CLIENT)
            return new ConfirmQrResponse
            {
                BookId = book.Id,
                NeedsPassport = true,
                UserId = book.UserId
            };

        book.Status = Status.Active;
        await context.SaveChangesAsync();

        return new ConfirmQrResponse
        {
            BookId = book.Id,
            NeedsPassport = false,
            UserId = book.UserId
        };
    }

    public async Task<bool> Validate(Guid zoneId, DateTime from, DateTime to, Guid guid, Guid? seat)
    {
        var zone = await context.Zones.SingleAsync(z => z.Id == zoneId);
        return zone switch
        {
            OfficeZone officeZone => await ValidateOfficeZone(officeZone, from, to, guid, seat!.Value),
            TalkroomZone talkroomZone => await ValidateTalkroomZone(talkroomZone, from, to, guid),
            OpenZone openZone => await ValidateOpenZone(openZone, from, to, guid),
            _ => throw new ForbiddenException("Not a bookable zone")
        };
    }

    public async Task<List<BookWithUserResponse>> GetAll()
    {
        var entities = await BooksQuery
            .ToListAsync();

        foreach (var entity in entities)
        {
            entity.User.Books.Clear();
        }

        return entities.Select(BookWithUserResponse.From).ToList();
    }

    public async Task<List<BookResponse>> MassBook(Guid zoneId, DateOnly from, DateOnly to, TimeOnly fromTime,
        TimeOnly toTime, string description, Guid userId)
    {
        var bookResponses = new List<BookResponse>();

        for (var date = from; date <= to; date = date.AddDays(1))
        {
            var startDateTime = date.ToDateTime(fromTime);
            var endDateTime = date.ToDateTime(toTime);

            var bookRequest = new BookRequest
            {
                From = startDateTime,
                To = endDateTime,
                Description = description
            };

            Guid? seatId = null;
            var zone = await context.Zones.FindAsync(zoneId) ?? throw new NotFoundException("Zone not found");

            switch (zone)
            {
                case OfficeZone officeZone:
                    var seat = await GetAvailableSeat(officeZone, startDateTime, endDateTime);
                    if (seat == null)
                        throw new ForbiddenException("No seats available for the specified date range.");
                    seatId = seat.Id;
                    break;

                case TalkroomZone talkroomZone:
                    if (!await IsTalkroomZoneAvailable(talkroomZone, startDateTime, endDateTime))
                        throw new ForbiddenException("Talkroom is not available for the specified date range.");
                    break;

                case OpenZone openZone:
                    if (!await IsOpenZoneAvailable(openZone, startDateTime, endDateTime))
                        throw new ForbiddenException("Open zone is not available for the specified date range.");

                    break;

                default:
                    throw new BadHttpRequestException("Zone type not supported.");
            }

            // Бронируем место
            await Book(zoneId, seatId, userId, bookRequest);

            // Получаем созданную запись из таблицы Books
            var book = await GetCreatedBook(zone, userId, startDateTime, endDateTime, seatId);

            if (book != null)
                bookResponses.Add(BookResponse.From(book));
            else
                throw new Exception("Failed to retrieve the newly created book.");
        }

        return bookResponses;
    }

    private async Task<bool> ValidateOfficeZone(OfficeZone zone, DateTime from, DateTime to, Guid userId, Guid seatId)
    {
        if (seatId == Guid.Empty)
            throw new ArgumentException("Seat ID cannot be empty", nameof(seatId));
        var user = await userService.UserById(userId);
        if (!zone.IsPublic && user.Role == Role.CLIENT)
            return false;

        var seat = await context.OfficeSeats
            .Include(s => s.Books)
            .FirstAsync(s => s.Id == seatId && s.OfficeZoneId == zone.Id);

        if (seat == null)
            throw new ArgumentException("Seat not found in the specified zone", nameof(seatId));

        var isSeatAvailable = !seat.Books.Any(b =>
            (b.Status == Status.Active || b.Status == Status.Pending) &&
            b.Start < to && from < b.End);

        return isSeatAvailable;
    }

    private async Task<bool> ValidateTalkroomZone(TalkroomZone zone, DateTime from, DateTime to, Guid userId)
    {
        if (!zone.IsPublic && (await userService.UserById(userId)).Role == Role.CLIENT) return false;
        return await IsTalkroomZoneAvailable(zone, from, to);
    }

    private async Task<bool> ValidateOpenZone(OpenZone zone, DateTime from, DateTime to, Guid userId)
    {
        if (!zone.IsPublic && (await userService.UserById(userId)).Role == Role.CLIENT) return false;
        return await IsOpenZoneAvailable(zone, from, to);
    }

    private async Task<bool> IsOfficeSeatAvailable(OfficeSeat seat, DateTime from, DateTime to) =>
        !await context.Entry(seat)
            .Collection(s => s.Books)
            .Query()
            .Where(b => b.Status == Status.Active || b.Status == Status.Pending)
            .AnyAsync(b => b.Start < to.ToUniversalTime() && from.ToUniversalTime() < b.End);

    private async Task<bool> IsTalkroomZoneAvailable(TalkroomZone zone, DateTime from, DateTime to) =>
        !await context.Books
            .OfType<TalkroomBook>()
            .AnyAsync(b =>
                b.TalkroomZoneId == zone.Id &&
                (b.Status == Status.Active || b.Status == Status.Pending) &&
                b.Start < to.ToUniversalTime() && from.ToUniversalTime() < b.End);

    private async Task<bool> IsOpenZoneAvailable(OpenZone zone, DateTime from, DateTime to)
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
                if (overlapCount == capacity && time >= from.ToUniversalTime() && time <= to.ToUniversalTime())
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
        Guid? seatId) =>
        zone switch
        {
            OfficeZone => await context.Books.OfType<OfficeBook>()
                .FirstOrDefaultAsync(b =>
                    b.OfficeSeatId == seatId && b.UserId == userId && b.Start == startDateTime.ToUniversalTime() &&
                    b.End == endDateTime.ToUniversalTime()),
            TalkroomZone => await context.Books.OfType<TalkroomBook>()
                .FirstOrDefaultAsync(b =>
                    b.TalkroomZoneId == zone.Id && b.UserId == userId && b.Start == startDateTime.ToUniversalTime() &&
                    b.End == endDateTime.ToUniversalTime()),
            OpenZone => await context.Books.OfType<OpenBook>()
                .FirstOrDefaultAsync(b =>
                    b.OpenZoneId == zone.Id && b.UserId == userId && b.Start == startDateTime.ToUniversalTime() &&
                    b.End == endDateTime.ToUniversalTime()),
            _ => null
        };

    private Task<OfficeSeat?> GetAvailableSeat(OfficeZone officeZone, DateTime start, DateTime end) =>
        context.Entry(officeZone)
            .Collection(oz => oz.Seats)
            .Query()
            .Include(s => s.Books)
            .FirstOrDefaultAsync(seat => !seat.Books.Any(b =>
                (b.Status == Status.Active || b.Status == Status.Pending)
                && b.Start < end.ToUniversalTime() && start.ToUniversalTime() < b.End));

    public async Task<Book> GetBookByQr(long id)
    {
        var qr = qrCodeService[id];
        if (qr == null)
            throw new NotFoundException("QR code not found");

        var book = await GetBookById(qr.Value);
        if (book == null)
            throw new NotFoundException("Book not found");

        return book;
    }
}
