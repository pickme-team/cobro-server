namespace Prod.Models.Database;

public class OfficeBook : Book
{
    public Guid OfficeSeatId { get; set; }
    public OfficeSeat OfficeSeat { get; set; } = null!;
}
