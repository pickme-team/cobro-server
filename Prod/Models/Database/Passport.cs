namespace Prod.Models.Database;

public class Passport
{
    public Guid Id { get; set; }
    public string Serial { get; set; } = null!;

    public string Number { get; set; } = null!;

    // public string IssuedBy { get; set; }
    // public DateTime IssuedOn { get; set; }
    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;

    public string Middlename { get; set; } = null!;

    // public string CodeOfIssuer { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public string Birthday { get; set; } = null!;
}
