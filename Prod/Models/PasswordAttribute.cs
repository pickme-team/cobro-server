using System.ComponentModel.DataAnnotations;

namespace Prod.Models;

public class PasswordAttribute() : RegularExpressionAttribute(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
