using Microsoft.EntityFrameworkCore;

namespace Prod.Models.Database;

public class RoomBook : Book
{
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = null!;
}
