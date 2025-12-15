using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class PurchaseOrderExpenseMap : BaseEntityMapping<PurchaseOrderExpense>
{
    public override void Configure(EntityTypeBuilder<PurchaseOrderExpense> builder)
    {
        base.Configure(builder);

        builder.ToTable("PurchaseOrderExpenses", "Docs");
    }
}