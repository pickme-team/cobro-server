using System.ComponentModel.DataAnnotations;

namespace Prod.Models.Database;

public class Room
{
    public Guid Id { get; set; }

    [Required] public string Name { get; set; } = null!;

    [Required] public string Description { get; set; } = null!;

    [Required] public int Capacity { get; set; }
}
