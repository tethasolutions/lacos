namespace Lacos.GestioneCommesse.Contracts.Dtos.Security
{
    public class SyncUserDto:SyncBaseDto
    {
        public string? UserName { get; set; }
        public string? PasswordHash { get; set; }
        public string? Salt { get; set; }
        public bool Enabled { get; set; }
        public bool? IsDeleted { get; set; }
        public string? AccessToken { get; set; }
    }
}
