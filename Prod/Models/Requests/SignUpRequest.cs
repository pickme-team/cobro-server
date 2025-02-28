using System.ComponentModel.DataAnnotations;

namespace Prod.Models.Requests;

public class SignUpRequest
{
    [Required] public string Name { get; init; }

    [Required] [Email] public string Email { get; init; }

    [Required] [Password] public string Password { get; init; }
}
