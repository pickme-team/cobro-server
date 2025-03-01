namespace Prod.Models.Database;

public class OpenBook : Book
{
    public Guid OpenZoneId { get; set; }
    public OpenZone OpenZone { get; set; } = null!;
}
