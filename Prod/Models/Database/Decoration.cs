namespace Prod.Models.Database;

public class Decoration
{
    public Guid Id { get; set; }

    public string Type { get; set; } = null!;

    public float X { get; set; }

    public float Y { get; set; }
}
