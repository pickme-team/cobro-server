using AspNetCore.Yandex.ObjectStorage;
using Microsoft.EntityFrameworkCore;
using Prod.Models.Database;
using Prod.Models.Responses;

namespace Prod.Services;

public class UserService(ProdContext context, IYandexStorageService objectStoreService) : IUserService
{
    private IQueryable<User> UserQuery() => context.Users
        .Include(u => u.Books)
        .ThenInclude(b => ((OpenBook)b).OpenZone)
        .Include(u => u.Books)
        .ThenInclude(b => ((OfficeBook)b).OfficeSeat)
        .ThenInclude(s => s.OfficeZone)
        .Include(u => u.Books)
        .ThenInclude(b => ((TalkroomBook)b).TalkroomZone);

    public async Task<UserResponse> UserById(Guid id) =>
        UserResponse.From(await UserQuery().SingleAsync(u => u.Id == id));

    public async Task<UserResponse> UserByEmail(string email) =>
        UserResponse.From(await UserQuery().SingleAsync(u => u.Email == email));

    public async Task Update(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        var user = await context.Users.SingleAsync(u => u.Id == id);
        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }

    public async Task UpdateCity(Guid id, string city)
    {
        var user = await context.Users.SingleAsync(u => u.Id == id);
        user.City = city;
        await context.SaveChangesAsync();
    }

    public async Task<List<UserResponse>> AllUsers() =>
        (await UserQuery().ToListAsync()).Select(UserResponse.From).ToList();

    public async Task<string> UploadMedia(IFormFile file, Guid id)
    {
        if (file.Length == 0 || file.FileName.Split(".")[^1].Length == 0)
            throw new ArgumentException("Invalid file");
        string fileName = Guid.NewGuid() + "." + file.FileName.Split('.')[^1];

        await objectStoreService.ObjectService.PutAsync(file.OpenReadStream(), fileName);
        var url = "https://storage.yandexcloud.net/cobro/" + fileName;

        var user = await context.Users.SingleAsync(u => u.Id == id);
        user.AvatarUrl = new Uri(url);
        await context.SaveChangesAsync();

        return url;
    }
}
