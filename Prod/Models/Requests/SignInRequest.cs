using System.ComponentModel.DataAnnotations;

namespace Prod.Models.Requests;

public class SignInRequest
{
    [Required] [Email] public string Email { get; init; }

    [Required] [Password] public string Password { get; init; }
}
