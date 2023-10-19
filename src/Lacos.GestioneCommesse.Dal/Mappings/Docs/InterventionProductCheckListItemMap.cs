using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class InterventionProductCheckListItemMap : BaseEntityMapping<InterventionProductCheckListItem>
{
    public override void Configure(EntityTypeBuilder<InterventionProductCheckListItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("InterventionProductCheckListItems", "Docs");

        builder.Property(e => e.Description)
            .HasMaxLength(500)
            .IsRequired();

    }
}