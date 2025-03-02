using Microsoft.IdentityModel.Tokens;
using Prod.Models.Database;

namespace Prod.Services;

public interface IJwtService
{
    TokenValidationParameters ValidationParameters { get; }
    string GenerateToken(User user);
    public User GetUserFromToken(string token);
}
