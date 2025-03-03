using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Models.Responses;

namespace Prod.Services;

public interface IUserService
{
    Task<UserResponse> UserById(Guid id);

    Task<UserResponse> UserByEmail(string email);

    Task Update(User user);

    Task Delete(Guid id);

    // Task UpdateCity(Guid id, string city);

    Task<List<UserResponse>> AllUsers();

    Task<string> UploadMedia(IFormFile file, Guid id);

    Task SetVerificationPhoto(Stream file, string name, Guid id);
    Task<Uri> GetVerificationPhoto(Guid id);

    Task SetPassport(Guid userId, PassportCreateRequest req);

    Task<User> Get(Guid id);
}
