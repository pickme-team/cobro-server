using Prod.Models.Database;
using Prod.Models.Requests;

namespace Prod.Services;

public interface IZoneService
{
    Task<List<Zone>> GetAll();
    Task<Zone> Get(Guid id);
    Task<Zone> Create(string type, ZoneCreateRequest req);
    Task<int> GetCurrentUsersCount(Guid id);
}
