using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class InterventionProductCheckListMap : BaseEntityMapping<InterventionProductCheckList>
{
    public override void Configure(EntityTypeBuilder<InterventionProductCheckList> builder)
    {
        base.Configure(builder);

        builder.ToTable("InterventionProductCheckLists", "Docs");

        builder.Property(e => e.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.OneToMany(e => e.Items, e => e.CheckList, e => e.CheckListId);
    }
}