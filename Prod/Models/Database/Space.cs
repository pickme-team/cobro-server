namespace Prod.Models.Database;

public class Space
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int Capacity { get; set; }

    public List<Place> Places { get; } = [];

    public List<SpaceBook> Books { get; } = [];
}
