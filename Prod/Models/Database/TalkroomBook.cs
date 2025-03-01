namespace Prod.Models.Database;

public class TalkroomBook : Book
{
    public Guid TalkroomZoneId { get; set; }
    public TalkroomZone TalkroomZone { get; set; } = null!;
}
