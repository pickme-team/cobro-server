using Prod.Models.Database;

namespace Prod.Services;

public class UserService(ProdContext context)
{
    public User? UserById(Guid id)
    {
        return context.Users.Find(id);
    }

    public User? UserByEmail(string email)
    {
        return context.Users.FirstOrDefault(u => u.Email == email);
    }

    public void Update(User user)
    {
        context.Users.Update(user);
        context.SaveChanges();
    }

    public void Delete(User user)
    {
        context.Users.Remove(user);
        context.SaveChanges();
    }

    public void Create(User user)
    {
        context.Users.Add(user);
        context.SaveChanges();
    }

    public IEnumerable<User> AllUsers()
    {
        return context.Users.ToList();
    }
}
