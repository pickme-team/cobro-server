using Microsoft.AspNetCore.Mvc;
using Prod.Models.Requests;
using Prod.Models.Responses;
using Prod.Services;

namespace Prod.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("sign-up")]
    public Task<AuthResponse> SignUp([FromBody] SignUpRequest request) => authService.SignUp(request);

    [HttpPost("sign-in")]
    public Task<AuthResponse> SignIn([FromBody] SignInRequest request) => authService.SignIn(request);
}
