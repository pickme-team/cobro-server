using Prod.Models.Database;

namespace Prod.Models.Responses;

public class BookResponse
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public string? Description { get; set; }

    public Guid ZoneId { get; set; }
    public string ZoneName { get; set; } = null!;

    public Guid? OfficeSeatId { get; set; }
    public string? OfficeSeatNumber { get; set; }

    public Status Status { get; set; }

    public static BookResponse From(Book book) => new()
    {
        Id = book.Id,
        CreatedAt = book.CreatedAt,
        Start = book.Start,
        End = book.End,
        Description = book.Description,
        Status = book.Status,
        ZoneId = book switch
        {
            OfficeBook officeBook => officeBook.OfficeSeat.OfficeZoneId,
            OpenBook openBook => openBook.OpenZoneId,
            TalkroomBook talkroomBook => talkroomBook.TalkroomZoneId,
            _ => throw new ArgumentOutOfRangeException(nameof(book), book, null)
        },
        ZoneName = book switch
        {
            OfficeBook officeBook => officeBook.OfficeSeat.OfficeZone.Name,
            OpenBook openBook => openBook.OpenZone.Name,
            TalkroomBook talkroomBook => talkroomBook.TalkroomZone.Name,
            _ => throw new ArgumentOutOfRangeException(nameof(book), book, null)
        },
        OfficeSeatId = (book as OfficeBook)?.OfficeSeatId,
        OfficeSeatNumber = (book as OfficeBook)?.OfficeSeat.InnerNumber
    };
}
