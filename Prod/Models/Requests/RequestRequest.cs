using System.ComponentModel.DataAnnotations;
using Prod.Models.Database;

namespace Prod.Models.Requests;

public class RequestRequest
{
    [Required] public string Text { get; set; } = null!;

    [Required] public long ActionId { get; set; }

    [Required] public RequestStatus Status { get; set; }

    public string? AdditionalInfo { get; set; }

    [Required] public Guid BookId { get; set; }
}
