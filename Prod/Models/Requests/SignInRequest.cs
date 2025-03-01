using System.ComponentModel.DataAnnotations;

namespace Prod.Models.Requests;

public class SignInRequest
{
    /// <summary>
    /// Почта.
    /// </summary>
    /// <example>john@doe.ru</example>
    [Required] [Email] public string Email { get; init; }

    /// <summary>
    /// Пароль.
    /// </summary>
    /// <example>JohnDoe12!</example>
    [Required] [Password] public string Password { get; init; }
}
