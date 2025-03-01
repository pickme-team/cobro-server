using Prod.Models.Database;

namespace Prod.Services;

public interface IZoneService
{
    List<Zone> GetAll();
    Task<Zone?> Get(Guid id);
    Task<Zone> Create(Zone zone);
}