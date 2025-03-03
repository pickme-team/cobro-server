using System.ComponentModel.DataAnnotations;

namespace Prod.Models.Requests;

public class DecorationCreateRequest : IValidatableObject
{
    [AllowedValues("Rectangle", "Icon")]
    [Required]
    public string Type { get; set; } = null!;

    [Required] public float X { get; set; }

    [Required] public float Y { get; set; }

    [Required] public string Name { get; set; } = null!;

    public float? Width { get; set; }

    public float? Height { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Type != "Rectangle") yield break;

        if (Width == null)
            yield return new ValidationResult("Width is required for rectangles", [nameof(Width)]);
        if (Height == null)
            yield return new ValidationResult("Height is required for rectangles", [nameof(Height)]);
    }
}
