using System.ComponentModel.DataAnnotations;
using Prod.Models.Database;

namespace Prod.Models.Requests;

public class BookRequest : IValidatableObject
{
    public DateTime From { get; init; }

    public DateTime To { get; init; }

    public string? Description { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (From < DateTime.UtcNow)
        {
            yield return new ValidationResult("From date must be in the future", new[] { nameof(From) });
        }

        if (From >= To)
        {
            yield return new ValidationResult("From date must be before To date", new[] { nameof(From), nameof(To) });
        }
    }
}
