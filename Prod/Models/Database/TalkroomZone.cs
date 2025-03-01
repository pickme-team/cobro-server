namespace Prod.Models.Database;

public class TalkroomZone : Zone
{
    public List<TalkroomBook> Books { get; } = [];
}
