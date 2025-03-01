namespace Prod.Models.Database;

public class OfficeZone : Zone
{
    public List<OfficeSeat> Seats { get; } = [];
}
