using System.ComponentModel.DataAnnotations;

namespace Prod.Models.Requests;

public class OfficeSeatCreateRequest
{
    [Required] public float X { get; set; }

    [Required] public float Y { get; set; }
    
    public string? InnerNumber { get; set; }
}
