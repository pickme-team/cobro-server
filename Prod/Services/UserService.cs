using AspNetCore.Yandex.ObjectStorage;
using Microsoft.EntityFrameworkCore;
using Prod.Exceptions;
using Prod.Models.Database;
using Prod.Models.Requests;
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
        .ThenInclude(b => ((TalkroomBook)b).TalkroomZone)
        .Include(u => u.Passport);

    public async Task<UserResponse> UserById(Guid id) =>
        UserResponse.From(await UserQuery().SingleAsync(u => u.Id == id));

    public async Task<User> Get(Guid id) =>
        await UserQuery().SingleAsync(u => u.Id == id);

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

    // public async Task UpdateCity(Guid id, string city)
    // {
    //     var user = await context.Users.SingleAsync(u => u.Id == id);
    //     await context.SaveChangesAsync();
    // }

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

    public async Task SetPassport(Guid userId, PassportCreateRequest req)
    {
        var user = await context.Users
            .Include(u => u.Passport)
            .SingleAsync(u => u.Id == userId);
        if (user.Passport != null)
            throw new ArgumentException("Passport already exists");
        user.Passport = new Passport
        {
            Serial = req.Serial,
            Number = req.Number,
            Firstname = req.Firstname,
            Lastname = req.Lastname,
            Middlename = req.Middlename,
            Birthday = req.Birthday
        };
        await context.SaveChangesAsync();
    }

    public async Task SetVerificationPhoto(Stream file, string name, Guid id)
    {
        if (file.Length == 0 || name.Split(".")[^1].Length == 0)
            throw new ArgumentException("Invalid file");
        string fileName = Guid.NewGuid() + "." + name.Split('.')[^1];

        await objectStoreService.ObjectService.PutAsync(file, fileName);
        var url = "https://storage.yandexcloud.net/cobro/" + fileName;

        var user = await context.Users.SingleAsync(u => u.Id == id);
        user.VerificationPhoto = new Uri(url);
        await context.SaveChangesAsync();
    }

    public async Task<Uri> GetVerificationPhoto(Guid id)
    {
        var user = await context.Users.SingleAsync(u => u.Id == id);
        if (user.VerificationPhoto == null)
            throw new NotFoundException("Verification photo not found");
        return user.VerificationPhoto;
    }
}
