    namespace Prod.Models.Database;

public enum RequestStatus
{
    Unread,
    Read,
    Done
}

public class Request
{
    public Guid Id { get; set; }
    public string Text { get; set; } = null!;

    public long ActionId { get; set; }
    public Action Action { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public RequestStatus Status { get; set; }

    public string? AdditionalInfo { get; set; }

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;
}
