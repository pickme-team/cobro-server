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

    public long? this[Guid id]
    {
        get => Get(id);
        set => Set(id, value);
    }

    private void Set(Guid id, long? code)
    {
        if (code == null) return;

        _db.StringSet(id.ToString(), code.Value.ToString(), TimeSpan.FromSeconds(Ttl));
    }

    private long? Get(Guid id)
    {
        var value = _db.StringGet(id.ToString());
        if (!value.HasValue) return null;

        return long.Parse(value);
    }
}
