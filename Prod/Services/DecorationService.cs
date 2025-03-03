using Microsoft.EntityFrameworkCore;
using Prod.Models.Database;
using Prod.Models.Requests;
using Prod.Models.Responses;

namespace Prod.Services;

public class DecorationService(ProdContext context) : IDecorationService
{
    public async Task<List<DecorationResponse>> GetAll() =>
        (await context.Decorations.ToListAsync()).Select(DecorationResponse.From).ToList();

    public Task Add(DecorationCreateRequest req)
    {
        if (req.Type == "Icon")
            context.Decorations.Add(new IconDecoration
            {
                X = req.X,
                Y = req.Y,
                Name = req.Name
            });
        if (req.Type == "Rectangle")
            context.Decorations.Add(new RectangleDecoration
            {
                X = req.X,
                Y = req.Y,
                Name = req.Name,
                Width = req.Width!.Value,
                Height = req.Height!.Value
            });
        return context.SaveChangesAsync();
    }
}
