using Microsoft.EntityFrameworkCore;
using Prod.Models.Database;

namespace Prod.Services;

public class LateService(IServiceProvider services) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var curHour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).ToUniversalTime();
            var nextIter = (int)Math.Ceiling((float)(now.Minute + 1) / 15) * 15 + 11;
            var waitUntil = curHour.AddMinutes(nextIter);
            await Task.Delay(waitUntil - now, stoppingToken);

            using var scope = services.CreateScope();
            await CancelBooks(scope.ServiceProvider.GetRequiredService<ProdContext>());
        }
    }

    private Task<int> CancelBooks(ProdContext context) => context.Books
        .Where(b => b.Start < DateTime.UtcNow && b.Status == Status.Pending)
        .ExecuteUpdateAsync(s => s.SetProperty(b => b.Status, Status.Cancelled));
}
