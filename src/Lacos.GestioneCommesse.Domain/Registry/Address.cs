using Lacos.GestioneCommesse.Domain.Docs;
using System.Text.Json.Serialization;

namespace Lacos.GestioneCommesse.Domain.Registry;

public class Address : FullAuditedEntity, ILogEntity
{
    public string? Description { get; set; }
    public string? City { get; set; }
    public string? StreetAddress { get; set; }
    public string? Province { get; set; }
    public string? ZipCode { get; set; }
    public string? Telephone { get; set; }
    public string? Email { get; set; }
    public bool IsMainAddress { get; set; }
    public string? Notes { get; set; }

    public string? JobReference { get; set; }
    public string? ContactName { get; set; }
    public string? ContactReference { get; set; }

    public decimal? DistanceKm { get; set; }
    public bool? IsInsideAreaC { get; set; }

    public long? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public long? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public ICollection<Job> Jobs { get; set; }
    public ICollection<Activity> Activities { get; set; }
    public ICollection<Ticket> Tickets { get; set; }
}

public class DistanceResult
{
    public decimal DistanceKm { get; set; }
    public decimal DurationMinutes { get; set; }
    public bool IsInsideAreaC { get; set; }

    public double OriginLatitude { get; set; }
    public double OriginLongitude { get; set; }
    public double DestinationLatitude { get; set; }
    public double DestinationLongitude { get; set; }
}

public class NominatimResult
{
    public string Lat { get; set; }
    public string Lon { get; set; }
    public string Display_Name { get; set; }
}
public class OsrmRouteResponse
{
    public string Code { get; set; }
    public List<OsrmRoute> Routes { get; set; }
}

public class OsrmRoute
{
    /// <summary>
    /// Distanza in metri
    /// </summary>
    public double Distance { get; set; }

    /// <summary>
    /// Durata in secondi
    /// </summary>
    public double Duration { get; set; }
}