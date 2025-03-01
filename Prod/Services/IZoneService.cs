using Prod.Models.Database;

namespace Prod.Services;

public interface IZoneService
{
    Task<List<Zone>> GetAll();
    Task<Zone> Get(Guid id);
    Task<Zone> Create(string type, Zone zone);
}
