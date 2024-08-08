using Lacos.GestioneCommesse.Domain.Registry;
using System.Diagnostics.Contracts;

namespace Lacos.GestioneCommesse.Domain.Docs;

public class JobsProgressStatus
{
    public long JobId { get; set; }
    public string? JobCode { get; set; }
    public int? JobYear { get; set; }
    public DateTimeOffset JobDate { get; set; }
    public string? JobReference { get; set; }
    public JobStatus? JobStatus { get; set; }
    public string? CustomerName { get; set; }
    public int? Activities_list { get; set; }
    public int? Activities_completed { get; set; }
    public decimal? Activities_progress { get; set; }
    public int? Interventions_list { get; set; }
    public int? Interventions_completed { get; set; }
    public decimal? Interventions_progress { get; set; }
    public int? PurchaseOrders_list { get; set; }
    public int? PurchaseOrders_completed { get; set; }
    public decimal? PurchaseOrders_progress { get; set; }

}