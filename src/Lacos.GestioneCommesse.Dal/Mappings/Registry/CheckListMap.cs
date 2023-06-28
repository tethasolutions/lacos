using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class CheckListMap : BaseEntityMapping<CheckList>
{
    public override void Configure(EntityTypeBuilder<CheckList> builder)
    {
        base.Configure(builder);

        builder.ToTable("CheckLists", "Registry");

        builder.Property(e => e.PictureFileName)
            .HasMaxLength(50);

        builder.Property(e => e.Description)
            .IsRequired();

        builder.OneToMany(e => e.Items, e => e.CheckList, e => e.CheckListId);
    }
}