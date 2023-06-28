using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class InterventionDisputeMap : BaseEntityMapping<InterventionDispute>
{
    public override void Configure(EntityTypeBuilder<InterventionDispute> builder)
    {
        base.Configure(builder);

        builder.ToTable("InterventionDisputes", "Docs");

        builder.Property(e => e.Description)
            .IsRequired();
    }
}