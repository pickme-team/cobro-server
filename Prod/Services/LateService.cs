using Microsoft.EntityFrameworkCore;
using Prod.Models.Database;

namespace Prod.Services;

public class LateService(ProdContext context) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var now = DateTime.UtcNow;
        var nextIter = (int)Math.Ceiling((float)now.Minute / 15) * 15 + 10;
        var waitUntil = new DateTime(now.Year, now.Month, now.Day, now.Hour, nextIter, 0).ToUniversalTime();

        await Task.Delay(waitUntil - now, stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            await CancelBooks();

            await Task.Delay(1000 * 15 * 60, stoppingToken);
        }
    }

    private Task<int> CancelBooks() => context.Books
        .Where(b => b.Start < DateTime.UtcNow && b.Status == Status.Pending)
        .ExecuteUpdateAsync(s => s.SetProperty(b => b.Status, Status.Cancelled));
}
