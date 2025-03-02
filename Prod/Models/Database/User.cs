using System.ComponentModel.DataAnnotations;

namespace Prod.Models.Database;

public enum Role
{
    ADMIN,
    CLIENT,
    INTERNAL,
}

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public Uri? AvatarUrl { get; set; }

    public List<Book> Books { get; } = [];

    public Role Role { get; set; }

    public Guid? PassportId { get; set; }
    public Passport? Passport { get; set; }

    public Uri? VerificationPhoto { get; set; }
}
