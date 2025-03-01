using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Prod.Models.Database;

public class User
{
    public Guid Id { get; set; }

    [Required] public string Name { get; set; } = null!;

    [Required] [Email] public string Email { get; set; } = null!;

    [Required] [Password] public string Password { get; set; } = null!;
    
    public string? City { get; set; }
}
