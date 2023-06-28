using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Security;

namespace Lacos.GestioneCommesse.Application.Security.DTOs;

public class UserDto : BaseEntityDto
{
    public string? UserName { get; set; }
    public bool Enabled { get; set; }
    public Role Role { get; set; }
    public string? EmailAddress { get; set; }
    public string? AccessToken { get; set; }
    public string? ColorHex { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }

    public UserDto()
    {
            
    }

    public UserDto(string? userName, bool enabled, Role role, string? emailAddress, string? accessToken, string? colorHex, string? name, string? surname)
    {
        UserName = userName;
        Enabled = enabled;
        Role = role;
        EmailAddress = emailAddress;
        AccessToken = accessToken;
        ColorHex = colorHex;
        Name = name;
        Surname = surname;
    }
}