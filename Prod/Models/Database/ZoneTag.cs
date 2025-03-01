using System.Text.Json.Serialization;

namespace Prod.Models.Database;

public class ZoneTag
{
    [JsonIgnore] Guid Id { get; set; }

    [JsonIgnore] public Guid ZoneId { get; set; }
    [JsonIgnore] public Zone Zone { get; set; } = null!;

    public Tag Tag { get; set; }
}
