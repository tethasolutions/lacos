using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Domain.Security;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class Intervention : FullAuditedEntity
{
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }

    public string? Notes { get; set; }
    public string? FinalNotes { get; set; }

    public string? ReportFileName { get; set; }
    public string? CustomerSignature { get; set; }

    public long VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    public ICollection<User> Operators { get; set; }
    public ICollection<InterventionPicture> Pictures { get; set; }
    public ICollection<InterventionProduct> Products { get; set; }
    public ICollection<InterventionDispute> Disputes { get; set; }
    public ICollection<Ticket> Tickets { get; set; }
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; }

    public Intervention()
    {
        Pictures = new List<InterventionPicture>();
        Operators = new List<User>();
        Products = new List<InterventionProduct>();
        Disputes = new List<InterventionDispute>();
        Tickets = new List<Ticket>();
        PurchaseOrders = new List<PurchaseOrder>();
    }
}