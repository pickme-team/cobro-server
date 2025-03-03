using System.Drawing;

namespace Prod.Models.Database;

public class Action
{
    public long Id { get; set; }
    public string Icon { get; set; } 
    public string Text { get; set; }
    public string Description { get; set; }
    public bool AdditionalInfo { get; set; }
    public Color Color { get; set; }
}
