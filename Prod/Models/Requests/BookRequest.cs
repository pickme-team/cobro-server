namespace Prod.Models.Requests;

public class BookRequest
{
    public DateTime From { get; init; }

    public DateTime To { get; init; }

    public string? Description { get; init; }
    
    public string? ZoneName { get; set; }
}
