using System.ComponentModel.DataAnnotations;

namespace Prod.Models.Requests;

public class PassportCreateRequest
{
    [Required] public string Serial { get; set; } = null!;

    [Required] public string Number { get; set; } = null!;

    // public string IssuedBy { get; set; }
    // public DateTime IssuedOn { get; set; }
    [Required] public string Firstname { get; set; } = null!;

    [Required] public string Lastname { get; set; } = null!;

    [Required] public string Middlename { get; set; } = null!;

    // public string CodeOfIssuer { get; set; }
    [Required] public string Birthday { get; set; } = null!;
}
