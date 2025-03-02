namespace Prod.Models.Database;

public class Passport
{
    public Guid Id { get; set; }
    public string Serial { get; set; }
    public string Number { get; set; }
    public string IssuedBy { get; set; }
    public DateTime IssuedOn { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Middlename { get; set; }
    public string CodeOfIssuer { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime PassportBirthday { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }
}
