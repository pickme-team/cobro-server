using System.ComponentModel.DataAnnotations;

namespace Prod.Models.Requests;

public class SignInRequest
{
    /// <example>john@doe.ru</example>
    [Required]
    [Email]
    public string Email { get; set; }

    /// <example>JohnDoe12!</example>
    [Required]
    [Password]
    public string Password { get; set; }
}
