using Microsoft.AspNetCore.Identity;
using Prod.Models.Requests;
using Prod.Models.Responses;

namespace Prod.Services;

public class AuthService(ProdContext context) : IAuthService
{
    private readonly PasswordHasher<string> _passwordHasher = new();

    public Task<AuthResponse> SignUp(SignUpRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponse> SignIn(SignInRequest request)
    {
        throw new NotImplementedException();
    }
}
