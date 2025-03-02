namespace Prod.Services
{
}

namespace Prod.Services
{
    public interface IQrCodeService
    {
        Guid? this[long id] { get; set; }
        Tuple<long?, int?> GetByValue(Guid value);
    }
}
