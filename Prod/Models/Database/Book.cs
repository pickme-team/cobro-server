using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Prod.Models.Database;

public enum Status
{
    Pending,
    Active,
    Cancelled,
    Ended
}

public class Book
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string? Description { get; set; }

    public Status Status { get; set; }
}
