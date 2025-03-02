using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Prod.Exceptions;
using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Models.Responses;

namespace Prod.Services;

public class AuthService(ProdContext context, IJwtService jwtService, IConfiguration configuration, IJSRuntime jsRuntime) : IAuthService
{
    private readonly PasswordHasher<string> _passwordHasher = new();

    public async Task<AuthResponse> SignUp(SignUpRequest request)
    {
        var hash = _passwordHasher.HashPassword(request.Email, request.Password);
        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = hash,
            Role = request.Email.EndsWith(configuration["CORPDOMAIN"] ?? "isntrui.ru") ? Role.INTERNAL : Role.CLIENT
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new AuthResponse { Token = jwtService.GenerateToken(user), Admin = false };
    }

    public async Task<AuthResponse> SignIn(SignInRequest request)
    {
        var user = await context.Users.SingleAsync(u => u.Email == request.Email);

        var result = _passwordHasher.VerifyHashedPassword(user.Email, user.Password, request.Password);
        if (result == PasswordVerificationResult.Failed) throw new UnauthorizedException();

        return new AuthResponse { Token = jwtService.GenerateToken(user), Admin = user.Role == Role.ADMIN };
    }
    
    public async Task SetTokenAsync(string token) => 
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);

    public async Task<string> GetTokenAsync() =>
        await jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

    public async Task RemoveTokenAsync() =>
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
}
