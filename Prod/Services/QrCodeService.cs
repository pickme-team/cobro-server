using System.Drawing;
using StackExchange.Redis;

namespace Prod.Services;

public class QrCodeService : IQrCodeService
{
    private readonly IConnectionMultiplexer _redis;
    private IDatabase _db;

    public const int Ttl = 3 * 60;

    public QrCodeService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = _redis.GetDatabase();
    }

    public Guid? this[long id]
    {
        get => Get(id);
        set => Set(value!.Value, id);
    }

    public Tuple<long?, int?> GetByValue(Guid value)
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: "*").ToArray();

        foreach (var key in keys)
        {
            var storedValue = _db.StringGet(key);
            if (storedValue.HasValue && Guid.TryParse(storedValue, out Guid storedGuid) && storedGuid == value &&
                long.TryParse(key, out var longKey))
            {
                var ttl = _db.KeyTimeToLive(key);
                return new Tuple<long?, int?>(longKey, (int)ttl?.TotalSeconds);
            }
        }

        return new Tuple<long?, int?>(null, null);
    }


    private void Set(Guid id, long code)
    {
        if (_db.StringGet(code.ToString()).HasValue)
        {
            return;
        }

        _db.StringSet(code.ToString(), id.ToString(), TimeSpan.FromSeconds(Ttl));
    }

    public Guid? Get(long id)
    {
        var value = _db.StringGet(id.ToString());
        if (!value.HasValue) return null;

        return new Guid(value.ToString());
    }

    public async Task<string?> ScanQrCode(byte[] imageBytes)
    {
        try
        {
            return "hola";
        }
        catch
        {
            return null;
        }
    }
}
