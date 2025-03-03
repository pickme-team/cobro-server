using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Prod.Exceptions;
using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Models.Responses;

namespace Prod.Services;

public class AuthService(
    ProdContext context,
    IJwtService jwtService,
    IConfiguration configuration,
    IEmailService emailService) : IAuthService
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
            Role = request.Email.EndsWith(configuration["CORPDOMAIN"] ?? "isntrui.ru") ? Role.INTERNAL : Role.CLIENT,
            AvatarUrl = new Uri("https://storage.yandexcloud.net/cobro/e18a6de4fb8f19391b6a9b606bf0d7a6.jpg")
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        string role = user.Role == Role.CLIENT ? "При первом посещении вам потребуется иметь при себе паспорт." : "";

        emailService.SendEmailAsync(request.Email, "Добро пожаловать!", $@"
<!DOCTYPE html>
<html lang=""ru"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Добро пожаловать в cobro!</title>
    <style>
        body {{
            font-family: Helvetica, Arial, sans-serif;
            background-color: #000000;
            margin: 0;
            padding: 0;
            color: #ffffff;
        }}
        .container {{
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            border: 1px solid #444444;
            border-radius: 8px;
        }}
        .header {{
            text-align: center;
            padding: 20px 0;
            border-bottom: 1px solid #444444;
        }}
        .header h1 {{
            font-size: 24px;
            margin: 0;
        }}
        .content {{
            padding: 20px 0;
        }}
        .content p {{
            font-size: 16px;
            line-height: 1.5;
            margin-bottom: 20px;
        }}
        .footer {{
            text-align: center;
            padding-top: 20px;
            font-size: 12px;
            color: #bbbbbb;
        }}
    </style>
</head>
<body>
<div class=""container"">
    <div class=""header"">
        <h1>Добро пожаловать в cobro!</h1>
    </div>
    <div class=""content"">
        <p>Спасибо за регистрацию в <b>cobro</b>! Мы надеемся, вам у нас понравится!</p>
        <p>Ваш аккаунт активен. Можете начинать пользоваться сервисом :).</p>
        <p>{role}</p>
    </div>
    <div class=""footer"">
        <p>&copy; 2025 cobro. All rights reserved.</p>
    </div>
</div>
</body>
</html>
");

        return new AuthResponse { Token = jwtService.GenerateToken(user), Admin = false };
    }

    public async Task<AuthResponse> SignIn(SignInRequest request)
    {
        var user = await context.Users.SingleAsync(u => u.Email == request.Email);

        var result = _passwordHasher.VerifyHashedPassword(user.Email, user.Password, request.Password);
        if (result == PasswordVerificationResult.Failed) throw new UnauthorizedException();

        return new AuthResponse { Token = jwtService.GenerateToken(user), Admin = user.Role == Role.ADMIN };
    }
}
