using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Prod.Services;

public class JwtBearerOptionsConfiguration(IJwtService jwtService) : IConfigureNamedOptions<JwtBearerOptions>
{
    public void Configure(JwtBearerOptions options) => Configure(Options.DefaultName, options);

    public void Configure(string? name, JwtBearerOptions options)
    {
        options.TokenValidationParameters = jwtService.ValidationParameters;
    }
}
