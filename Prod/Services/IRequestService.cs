using Prod.Models.Database;
using Prod.Models.Requests;

namespace Prod.Services;

public interface IRequestService
{
    Task<Request> Add(RequestRequest request);
    Task<List<Request>> Today();
    Task Mark(Guid id, RequestStatus status);
}
