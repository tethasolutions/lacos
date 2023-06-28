namespace Lacos.GestioneCommesse.WebApi.Models.Security;

public class ChangeUserPasswordModel
{
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
}