using Lacos.GestioneCommesse.Domain.Security;

namespace Lacos.GestioneCommesse.WebApi.Models.Security;

public class UpdateUserRequest
{
    public string? Password { get; set; }
    public string? UserName { get; set; }
    public bool Enabled { get; set; }
    public Role Role { get; set; }
    public string? EmailAddress { get; set; }
    public string? ColorHex { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
}