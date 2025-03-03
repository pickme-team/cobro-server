using System.ComponentModel.DataAnnotations;

namespace Prod.Models.Requests;

public class RescheduleRequest : IValidatableObject
{
    public DateTime From { get; set; }

    public DateTime To { get; set; }

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
