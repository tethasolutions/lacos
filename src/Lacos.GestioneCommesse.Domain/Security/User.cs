using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Domain.Security;

public class User : BaseEntity
{
    public string? UserName { get; set; }
    public string? PasswordHash { get; set; }
    public string? Salt { get; set; }
    public string? AccessToken { get; set; }
    public bool Enabled { get; set; }
    public Role Role { get; set; }

    public Operator? Operator { get; set; }
    public Customer? Customer { get; set; }
}