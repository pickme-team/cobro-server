using System.ComponentModel.DataAnnotations;

namespace Prod.Models.Database;

public class Zone
{
    public Guid Id { get; set; }

    [Required] public string Name { get; set; } = null!;

    [Required] public string Description { get; set; } = null!;

    [Required] public int Capacity { get; set; }

    public string? Class { get; set; }

    [Required] public float XCoordinate { get; set; }

    [Required] public float YCoordinate { get; set; }

    [Required] public float Width { get; set; }

    [Required] public float Height { get; set; }

    [Required] public bool IsPublic { get; set; }
}
