namespace Prod.Models.Database;

public class Place
{
    public Guid Id { get; set; }
    public string? Description { get; set; } = null;

    public Guid SpaceId { get; set; }
    public Space Space { get; set; } = null!;

    public List<PlaceBook> Books { get; } = [];
}
