using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Prod.Models.Database;

public class Book
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Guid BookedRoomId { get; set; }
    public Room BookedRoom { get; set; }

    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public User User { get; set; }
    public string Description { get; set; } = null!;
}
