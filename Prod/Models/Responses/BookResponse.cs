using Prod.Models.Database;

namespace Prod.Models.Responses;

public class BookResponse
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public string? Description { get; set; }
    public Zone Zone { get; set; } = null!;

    public OfficeSeat? OfficeSeat { get; set; }

    public Status Status { get; set; }

    public static BookResponse From(Book book) => new()
    {
        Id = book.Id,
        CreatedAt = book.CreatedAt,
        Start = book.Start,
        End = book.End,
        Description = book.Description,
        Status = Status.Pending,
        Zone = book switch
        {
            OfficeBook officeBook => officeBook.OfficeSeat.OfficeZone,
            OpenBook openBook => openBook.OpenZone,
            TalkroomBook talkroomBook => talkroomBook.TalkroomZone,
            _ => throw new ArgumentOutOfRangeException(nameof(book), book, null)
        },
        OfficeSeat = book is OfficeBook ob ? ob.OfficeSeat : null
    };
}
