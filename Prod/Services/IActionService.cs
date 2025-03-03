using Action = Prod.Models.Database.Action;

namespace Prod.Services;

public interface IActionService
{
    Task<Action> Create(Action action);
    Task<Action> Update(Action action);
    Task<Action> Delete(Action action);
    Task<List<Action>> AllActions();
    Task<Action> Get(long id);
}
