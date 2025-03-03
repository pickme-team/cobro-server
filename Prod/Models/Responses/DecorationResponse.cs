using System.Drawing;
using Prod.Models.Database;

namespace Prod.Models.Responses;

public class DecorationResponse
{
    public string Type { get; set; } = null!;

    public float X { get; set; }

    public float Y { get; set; }

    public string Name { get; set; } = null!;

    public float? Width { get; set; }

    public float? Height { get; set; }

    public static DecorationResponse From(Decoration decoration) => new()
    {
        Type = decoration.Type,
        X = decoration.X,
        Y = decoration.Y,
        Name = decoration.Name,
        Width = (decoration as RectangleDecoration)?.Width,
        Height = (decoration as RectangleDecoration)?.Height
    };
}
