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

    private void Set(Guid id, long code)
    {
        _db.StringSet(code.ToString(), id.ToString(), TimeSpan.FromSeconds(Ttl));
    }

    private Guid? Get(long id)
    {
        var value = _db.StringGet(id.ToString());
        if (!value.HasValue) return null;

        return new Guid(value.ToString());
    }
}
