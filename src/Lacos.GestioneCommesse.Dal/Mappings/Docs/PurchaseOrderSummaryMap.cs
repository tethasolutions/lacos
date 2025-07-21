using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class PurchaseOrderSummaryMap : BaseEntityMapping<PurchaseOrderSummary>
{
    public override void Configure(EntityTypeBuilder<PurchaseOrderSummary> builder)
    {
        base.Configure(builder);

        builder.HasNoKey();

        builder.ToView("v_PurchaseOrdersSummary", "Docs");
    }
}