using AspNetCore.Yandex.ObjectStorage;
using AspNetCore.Yandex.ObjectStorage.Object.Responses;
using Microsoft.EntityFrameworkCore;
using Prod.Models.Database;

namespace Prod.Services;

public class UserService(ProdContext context, IYandexStorageService _objectStoreService) : IUserService
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
        context.Users.Add(user);
        await context.SaveChangesAsync();
    }

    public Task<List<User>> AllUsers() => context.Users.ToListAsync();

    public async Task<string> UploadMedia(IFormFile file)
    {
        if (file.FileName == null || file.Length == 0 || file.FileName.Split(".")[^1] == null) throw new ArgumentException("Invalid file");
        string fileName = Guid.NewGuid() + "." + file.FileName.Split('.')[^1];;
        await _objectStoreService.ObjectService.PutAsync(file.OpenReadStream(), fileName);
        return "https://storage.yandexcloud.net/cobro/" + fileName;
    } 
}
