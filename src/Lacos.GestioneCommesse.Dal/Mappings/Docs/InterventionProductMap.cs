using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class InterventionProductMap : BaseEntityMapping<InterventionProduct>
{
    public override void Configure(EntityTypeBuilder<InterventionProduct> builder)
    {
        base.Configure(builder);

        builder.ToTable("InterventionProducts", "Docs");

        builder.OneToOne(e => e.CheckList, e => e.InterventionProduct, e => e.InterventionProductId);
        builder.OneToMany(e => e.Pictures, e => e.InterventionProduct, e => e.InterventionProductId);
    }
}