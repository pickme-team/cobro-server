using Prod.Models.Database;

namespace Prod.Services;

public interface IRequestService
{
    Task Add(Request request);
    Task<List<Request>> Today();
    Task Mark(Guid id, RequestStatus status);
}
