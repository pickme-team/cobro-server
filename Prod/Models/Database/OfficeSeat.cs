namespace Prod.Models.Database;

public class OfficeSeat
{
    public Guid Id { get; set; }

    public Guid OfficeZoneId { get; set; }
    public OfficeZone OfficeZone { get; set; } = null!;

    public List<OfficeBook> Books { get; } = [];
}
