using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class InterventionProductPictureMap : BaseEntityMapping<InterventionProductPicture>
{
    public override void Configure(EntityTypeBuilder<InterventionProductPicture> builder)
    {
        base.Configure(builder);

        builder.ToTable("InterventionProductPictures", "Docs");

        builder.Property(e => e.FileName)
            .HasMaxLength(50);
    }
}