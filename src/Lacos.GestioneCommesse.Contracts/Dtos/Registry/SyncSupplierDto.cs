using Lacos.GestioneCommesse.Contracts.Dtos.Enums;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Registry
{
    public class SyncSupplierDto:SyncBaseDto
    {
        public string? Name { get; set; }
        public string? Notes { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        
    }
   

}
