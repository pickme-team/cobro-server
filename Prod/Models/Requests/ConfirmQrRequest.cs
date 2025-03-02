using System.ComponentModel.DataAnnotations;

namespace Prod.Models.Requests;

public class ConfirmQrRequest : IValidatableObject
{
    [Required] [Length(7, 10)] public string Code { get; init; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!long.TryParse(Code, out _))
            yield return new ValidationResult("code must be a numeric value.", [nameof(Code)]);
    }
}
