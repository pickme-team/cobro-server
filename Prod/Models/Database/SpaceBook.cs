namespace Prod.Models.Database;

public class SpaceBook : Book
{
    public Guid SpaceId { get; set; }
    public Space Space { get; set; } = null!;
}
