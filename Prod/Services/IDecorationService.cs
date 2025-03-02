using Prod.Models.Database;

namespace Prod.Services;

public interface IDecorationService
{
    Task<List<Decoration>> GetAll();
}