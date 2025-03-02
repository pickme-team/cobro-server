using Prod.Models.Requests;
using Prod.Models.Responses;

namespace Prod.Services;

public interface IAuthService
{
    Task<AuthResponse> SignUp(SignUpRequest request);

    Task<AuthResponse> SignIn(SignInRequest request);
}
