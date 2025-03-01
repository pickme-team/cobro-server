using System.ComponentModel.DataAnnotations;

namespace Prod.Models.Database;

public class Room
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Capacity { get; set; }

    public List<RoomBook> Books { get; } = [];
}
