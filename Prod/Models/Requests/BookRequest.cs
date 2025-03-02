using Prod.Models.Database;

namespace Prod.Models.Requests;

public class BookRequest
{
    public DateTime From { get; set; }

    public DateTime To { get; set; }

    public string? Description { get; set; }
}
