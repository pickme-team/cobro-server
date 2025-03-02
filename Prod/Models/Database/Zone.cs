namespace Prod.Models.Database;

public class Zone
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Capacity { get; set; }

    public string? Class { get; set; }

    public float XCoordinate { get; set; }

    public float YCoordinate { get; set; }

    public float Width { get; set; }

    public float Height { get; set; }

    public string Type { get; set; } = null!;
    
    public List<ZoneTag> ZoneTags { get; set; } = [];
    
    public bool IsPublic { get; set; }
}
