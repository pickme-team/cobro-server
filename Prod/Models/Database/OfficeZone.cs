namespace Prod.Models.Database;

public class OfficeZone : Zone
{
    public List<OfficeSeat> Seats { get; } = [];

    public OfficeZone(Zone zone)
    {
        Id = zone.Id;
        Name = zone.Name;
        Description = zone.Description;
        Capacity = zone.Capacity;
        Class = zone.Class;
        XCoordinate = zone.XCoordinate;
        YCoordinate = zone.YCoordinate;
        Width = zone.Width;
        Height = zone.Height;
        ZoneTags = zone.ZoneTags;
    }
}
