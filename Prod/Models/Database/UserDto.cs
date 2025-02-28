namespace Prod.Models.Database;

public class UserDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;
}
