using System.ComponentModel.DataAnnotations;

namespace Prod.Models.Requests;

public class DecorationCreateRequest : IValidatableObject
{
    [AllowedValues("Rectangle", "Icon")] public string Type { get; set; } = null!;

    public float X { get; set; }

    public float Y { get; set; }

    public string? Name { get; set; }

    public float? Width { get; set; }

    public float? Height { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Type == "Icon" && Name == null)
            yield return new ValidationResult("Name is required for icons", [nameof(Name)]);

        if (Type != "Rectangle") yield break;

        if (Width == null)
            yield return new ValidationResult("Width is required for rectangles", [nameof(Width)]);
        if (Height == null)
            yield return new ValidationResult("Height is required for rectangles", [nameof(Height)]);
    }
}
