namespace Prod.Models.Database;

public class User
{
    public Guid Id { get; set; }

    // TODO: MAKE IT TO BE VALIDATED
    public string Name { get; set; } = null!;

    // TODO: MAKE IT TO BE UNIQUE
    public string Email { get; set; } = null!;

    // TODO: MAKE IT TO BE VALIDATED
    public string Password { get; set; } = null!;

    // TODO: ADD TO DTO
    public string City { get; set; } = null!;
}
