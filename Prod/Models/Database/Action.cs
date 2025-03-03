using System.Drawing;

namespace Prod.Models.Database;

public class Action
{
    public long Id { get; set; }

    public string Icon { get; set; } = null!;
    public string Text { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool AdditionalInfo { get; set; }
    public Color Color { get; set; }

    public List<Request> Requests { get; set; } = [];
}
