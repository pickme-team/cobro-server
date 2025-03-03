using Microsoft.EntityFrameworkCore;
using Prod.Models.Database;
using Prod.Models.Requests;

namespace Prod.Services;

public class DecorationService(ProdContext context) : IDecorationService
{
    public Task<List<Decoration>> GetAll() => context.Decorations.ToListAsync();

    public Task Add(DecorationCreateRequest req)
    {
        if (req.Type == "Icon")
            context.Decorations.Add(new IconDecoration
            {
                X = req.X,
                Y = req.Y,
                Name = req.Name!
            });
        if (req.Type == "Rectangle")
            context.Decorations.Add(new RectangleDecoration
            {
                X = req.X,
                Y = req.Y,
                Width = req.Width!.Value,
                Height = req.Height!.Value,
                Color = req.Color!
            });
        return context.SaveChangesAsync();
    }
}
