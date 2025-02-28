using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Prod.Models.Database;

public class Book
{
    public Guid Id { get; set; }
    [JsonIgnore] public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Required] public Room BookedRoom { get; set; }
    [Required] public DateTime start { get; set; }
    [Required] public DateTime end { get; set; }
    [Required] public User User { get; set; }
    [Required] public string Description { get; set; } = null!;
}
