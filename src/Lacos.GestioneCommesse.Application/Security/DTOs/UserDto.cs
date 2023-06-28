using Lacos.GestioneCommesse.Application.Shared;
using Lacos.GestioneCommesse.Domain.Security;

namespace Lacos.GestioneCommesse.Application.Security.DTOs;

public class UserDto : BaseEntityDto
{
    public string? UserName { get; set; }
    public bool Enabled { get; set; }
    public Role Role { get; set; }
    public string? AccessToken { get; set; }

    public UserDto()
    {
            
    }

    public UserDto(string? userName, bool enabled, Role role, string? accessToken)
    {
        UserName = userName;
        Enabled = enabled;
        Role = role;
        AccessToken = accessToken;
    }
}