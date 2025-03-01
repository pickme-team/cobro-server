namespace Prod.Models.Database;

public class OfficeSeat : Zone
{
    public Guid OfficeZoneId { get; set; }
    public OfficeZone OfficeZone { get; set; } = null!;

    public List<OfficeBook> Books { get; } = [];

    public float X { set; get; }
    public float Y { set; get; }
}
