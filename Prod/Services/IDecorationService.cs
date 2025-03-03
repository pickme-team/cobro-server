using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Models.Responses;

namespace Prod.Services;

public interface IDecorationService
{
    Task<List<DecorationResponse>> GetAll();
    Task Add(DecorationCreateRequest req);
}
