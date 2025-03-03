using Prod.Models.Database;
using Prod.Models.Requests;

namespace Prod.Services;

public interface IDecorationService
{
    Task<List<Decoration>> GetAll();
    Task Add(DecorationCreateRequest req);
}
