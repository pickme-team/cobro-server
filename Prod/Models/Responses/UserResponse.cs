using Prod.Models.Database;

namespace Prod.Models.Responses;

public class UserResponse
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;

    public string Email { get; init; } = null!;

    public Uri? AvatarUrl { get; init; }

    public string? City { get; init; }

    public List<BookResponse> Books { get; init; } = [];

    public Role Role { get; set; }

    public static UserResponse From(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        AvatarUrl = user.AvatarUrl,
        City = user.City,
        Books = user.Books.Select(BookResponse.From).ToList(),
        Role = Role.ADMIN
    };
}
