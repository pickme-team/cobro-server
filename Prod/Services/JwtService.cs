using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Prod.Models.Database;

namespace Prod.Services;

public class JwtService : IJwtService
{
    // private readonly SymmetricSecurityKey _signingKey = new(Encoding.ASCII.GetBytes(cfg.RandomSecret));
    private readonly SymmetricSecurityKey _signingKey;
    private readonly SigningCredentials _signingCredentials;

    public JwtService()
    {
        _signingKey = new SymmetricSecurityKey(new byte[32]);
        _signingCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
    }

    private const string Issuer = "PROD";
    private const string Audience = "PROD";

    public TokenValidationParameters ValidationParameters =>
        new()
        {
            ValidateIssuer = true,
            ValidIssuer = Issuer,

            ValidateAudience = true,
            ValidAudience = Audience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _signingKey
        };

    public string GenerateToken(User user)
    {
        var jwt = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims:
            [
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                new Claim("admin", (user.Role == Role.ADMIN).ToString())
            ],
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: _signingCredentials
        );
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public User GetUserFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = _signingKey;

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = Issuer,
            ValidateAudience = true,
            ValidAudience = Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

        var userIdClaim = principal.Claims.First(c => c.Type == ClaimsIdentity.DefaultNameClaimType);
        var adminClaim = principal.Claims.First(c => c.Type == "admin");

        return new User
        {
            Id = Guid.Parse(userIdClaim.Value),
            Role = bool.Parse(adminClaim.Value) ? Role.ADMIN : Role.CLIENT
        };
    }
}
