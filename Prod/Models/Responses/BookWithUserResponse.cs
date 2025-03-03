using Prod.Models.Database;

namespace Prod.Models.Responses;

public class BookWithUserResponse : BookResponse
{
    public UserResponse User { get; set; } = null!;

    public new static BookWithUserResponse From(Book book) => new()
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
        OfficeSeatNumber = (book as OfficeBook)?.OfficeSeat.InnerNumber,
        User = UserResponse.From(book.User)
    };
}
