using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class ActivityProductMap : BaseEntityMapping<ActivityProduct>
{
    public override void Configure(EntityTypeBuilder<ActivityProduct> builder)
    {
        base.Configure(builder);

        builder.ToTable("ActivityProducts", "Docs");

        builder.OneToMany(e => e.InterventionProducts, e => e.ActivityProduct, e => e.ActivityProductId);
    }
}