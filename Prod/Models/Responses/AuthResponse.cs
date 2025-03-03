namespace Prod.Models.Responses;

public class AuthResponse
{
    public string Token { get; init; } = null!;

    public bool Admin { get; init; }
}
