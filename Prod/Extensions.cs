using System.Security.Claims;
using Prod.Models.Database;

namespace Prod;

public static class Extensions
{
    public static Guid Id(this ClaimsPrincipal user) => new(user.Identity!.Name!);
}
