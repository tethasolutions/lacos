using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class ActivityAttachmentMap : BaseEntityMapping<ActivityAttachment>
{
    public override void Configure(EntityTypeBuilder<ActivityAttachment> builder)
    {
        base.Configure(builder);

        builder.ToTable("ActivityAttachments", "Docs");

        builder.Property(e => e.DisplayName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(e => e.FileName)
            .HasMaxLength(64)
            .IsRequired();
    }
}