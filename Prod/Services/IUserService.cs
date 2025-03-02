using Prod.Models.Database;

namespace Prod.Services;

public interface IUserService
{
    Task<User?> UserById(Guid id);
    Task<User?> UserByEmail(string email);
    Task Update(User user);
    Task Delete(User user);
    Task Create(User user);
    Task<List<User>> AllUsers();
    Task<string> UploadMedia(IFormFile file);

}
