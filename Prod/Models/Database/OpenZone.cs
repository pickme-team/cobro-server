namespace Prod.Models.Database;

public class OpenZone : Zone
{
    public List<OpenBook> Books { get; } = [];

    public OpenZone(Zone zone)
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
