using System.ComponentModel.DataAnnotations;
using Prod.Models.Database;

namespace Prod.Models.Requests;

public class ZoneCreateRequest
{
    [Required] public string Name { get; set; } = null!;

    [Required] public string Description { get; set; } = null!;

    [Required] public int Capacity { get; set; }

    public string? Class { get; set; }
    
    [Required] public bool IsPublic { get; set; }

    [Required] public float XCoordinate { get; set; }

    [Required] public float YCoordinate { get; set; }

    [Required] public float Width { get; set; }

    [Required] public float Height { get; set; }

    public List<Tag> Tags { get; set; } = [];
}
