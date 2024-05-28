using Lacos.GestioneCommesse.Contracts.Dtos.Enums;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Registry
{
    public class SyncCustomerDto:SyncBaseDto
    {
        public string? Name { get; set; }
        public string? Notes { get; set; }
        public CustomerFiscalType FiscalType { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public bool CanGenerateTickets { get; set; }

        public string? Contact { get; set; }
        public string? ContactTelephone { get; set; }
        public string? ContactEmail { get; set; }

        public string? SDICode { get; set; }

        public long? UserId { get; set; }
    }
   

}
