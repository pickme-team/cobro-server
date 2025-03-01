using Microsoft.EntityFrameworkCore;
using Prod.Models.Database;

namespace Prod.Services;

public class UserService(ProdContext context, IConfiguration configuration) : IUserService
{
    public async Task<User?> UserById(Guid id) => await context.Users.FindAsync(id);

    public async Task<User?> UserByEmail(string email) =>
        await context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task Update(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task Delete(User user)
    {
        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }

    public async Task Create(User user)
    {
        if (user.Email.EndsWith(configuration["CORPDOMAIN"] is null ? "isntrui.ru" : configuration["CORPDOMAIN"]!)) user.Role = Role.INTERNAL;
        else user.Role = Role.CLIENT;
        context.Users.Add(user);
        await context.SaveChangesAsync();
    }

    public Task<List<User>> AllUsers() => context.Users.ToListAsync();
}
