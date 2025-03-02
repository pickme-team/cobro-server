using System.ComponentModel.DataAnnotations;

namespace Prod.Models.Requests;

/// <summary>
/// Запрос на регистрацию пользователя.
/// </summary>
public class SignUpRequest
{
    /// <summary>
    /// Имя пользователя.
    /// </summary>
    /// <example>John Doe</example>
    [Required]
    public string Name { get; set; }

    /// <summary>
    /// Email пользователя.
    /// </summary>
    /// <example>john.doe@example.com</example>
    [Required, Email]
    public string Email { get; set; }

    /// <summary>
    /// Пароль пользователя.
    /// </summary>
    /// <example>StrongPassword123!</example>
    [Required]
    public string Password { get; set; }
}
