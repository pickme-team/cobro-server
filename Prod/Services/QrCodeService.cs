using System.Collections.Concurrent;

namespace Prod.Services;

public class QrCodeService : BackgroundService
{
    private readonly ConcurrentDictionary<Guid, (long, DateTime)> _codes = new();

    public const int Ttl = 3 * 60;

    public long? this[Guid id]
    {
        get => Get(id);
        set => Set(id, value);
    }

    private void Set(Guid id, long? code)
    {
        if (code == null) return;

        _codes[id] = (code.Value, DateTime.UtcNow.AddSeconds(Ttl));
    }

    private long? Get(Guid id)
    {
        if (!_codes.TryGetValue(id, out var result)) return null;

        var (code, ttl) = result;
        if (ttl >= DateTime.UtcNow) return code;

        _codes.Remove(id, out _);
        return null;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000 * 60, stoppingToken);

            foreach (var keyValuePair in _codes)
            {
                var (_, ttl) = keyValuePair.Value;
                if (ttl < DateTime.UtcNow)
                    _codes.Remove(keyValuePair.Key, out _);
            }
        }
    }
}
