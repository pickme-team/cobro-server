using Microsoft.EntityFrameworkCore;
using Action = Prod.Models.Database.Action;

namespace Prod.Services;

public class ActionService(ProdContext prodContext) : IActionService
{
    public async Task<Action> Create(Action action)
    {
        await prodContext.Actions.AddAsync(action);
        await prodContext.SaveChangesAsync();
        return action;
    }

    public async Task<Action> Update(Action action)
    {
        prodContext.Actions.Update(action);
        await prodContext.SaveChangesAsync();
        return action;
    }

    public async Task<Action> Delete(Action action)
    {
        prodContext.Actions.Remove(action);
        await prodContext.SaveChangesAsync();
        return action;
    }

    public Task<List<Action>> AllActions() =>
        prodContext.Actions.ToListAsync();

    public Task<Action> Get(long id) =>
        prodContext.Actions.SingleAsync(a => a.Id == id);
}
