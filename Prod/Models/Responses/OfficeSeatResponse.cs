using Prod.Models.Database;

namespace Prod.Models.Responses;

public class OfficeSeatResponse
{
    public Guid Id { get; set; }

    public float X { get; set; }

    public float Y { get; set; }
    
    public string? InnerNumber { get; set; }

    public static OfficeSeatResponse From(OfficeSeat seat) => new()
    {
        Id = seat.Id,
        X = seat.X,
        Y = seat.Y,
        InnerNumber = seat.InnerNumber,
    };
}
