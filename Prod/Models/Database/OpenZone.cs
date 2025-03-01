namespace Prod.Models.Database;

public class OpenZone : Zone
{
    public List<OpenBook> Books { get; } = [];
}
