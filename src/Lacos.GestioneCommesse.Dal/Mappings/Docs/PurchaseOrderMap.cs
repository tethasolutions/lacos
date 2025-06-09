using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class PurchaseOrderMap : BaseEntityMapping<PurchaseOrder>
{
    public override void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        base.Configure(builder);

        builder.ToTable("PurchaseOrders", "Docs");

        builder.Property(e => e.Description)
            .IsRequired();

        builder.OneToMany(e => e.Items, e => e.PurchaseOrder, e => e.PurchaseOrderId);
        builder.OneToMany(e => e.Messages, e => e.PurchaseOrder, e => e.PurchaseOrderId);
        builder.ManyToMany(e => e.Jobs, e => e.PurchaseOrders, "PurchaseOrdersJobs", "Docs");
    }
}