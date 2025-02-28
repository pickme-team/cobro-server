using System.ComponentModel.DataAnnotations;

namespace Prod.Models.Database;

public class Place
{
    public Guid Id { get; set; }
    [Required] public string Adress { get; set; }
    [Required] public string Name { get; set; }
    [Required] public string Description { get; set; }
}
