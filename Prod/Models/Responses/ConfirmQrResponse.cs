namespace Prod.Models.Responses;

public class ConfirmQrResponse
{
    public Guid BookId { get; set; }
    public bool NeedsPassport { get; set; }

    public Guid UserId { get; set; }
}
