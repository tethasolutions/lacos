using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class CheckListItemMap : BaseEntityMapping<CheckListItem>
{
    public override void Configure(EntityTypeBuilder<CheckListItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("CheckListItems", "Registry");

        builder.Property(e => e.Description)
            .HasMaxLength(500)
            .IsRequired();
    }
}