using Microsoft.EntityFrameworkCore;
using Action = Prod.Models.Database.Action;

namespace Prod.Services;

public class ActionService(ProdContext prodContext)
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
    
    public async Task<List<Action>> AllActions() =>
        await prodContext.Actions.ToListAsync();
    
    public async Task<Action?> Get(Guid id) =>
        await prodContext.Actions.FindAsync(id);
}
