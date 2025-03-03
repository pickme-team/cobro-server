using System.ComponentModel.DataAnnotations;

namespace Prod.Models.Requests;

public class PassportCreateRequest
{
    [Required] public string Serial { get; set; }

    [Required] public string Number { get; set; }

    // public string IssuedBy { get; set; }
    // public DateTime IssuedOn { get; set; }
    [Required] public string Firstname { get; set; }

    [Required] public string Lastname { get; set; }

    [Required] public string Middlename { get; set; }

    // public string CodeOfIssuer { get; set; }
    [Required] public string Birthday { get; set; }
}
