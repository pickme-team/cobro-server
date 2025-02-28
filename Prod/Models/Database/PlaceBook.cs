namespace Prod.Models.Database;

public class PlaceBook : Book
{
    public Guid PlaceId { get; set; }
    public Place Place { get; set; } = null!;
}
