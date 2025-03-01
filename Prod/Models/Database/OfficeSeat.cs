using System.ComponentModel.DataAnnotations;

namespace Prod.Models.Database;

public class OfficeSeat : Zone
{
    public Guid OfficeZoneId { get; set; }
    public OfficeZone OfficeZone { get; set; } = null!;

    public List<OfficeBook> Books { get; } = [];

    [Required] public float X { set; get; }
    [Required] public float Y { set; get; }
}
