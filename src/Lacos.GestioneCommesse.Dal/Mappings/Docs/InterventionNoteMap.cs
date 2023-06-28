using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class InterventionNoteMap : BaseEntityMapping<InterventionNote>
{
    public override void Configure(EntityTypeBuilder<InterventionNote> builder)
    {
        base.Configure(builder);

        builder.ToTable("InterventionNotes", "Docs");

        builder.Property(e => e.PictureFileName)
            .HasMaxLength(50);

        builder.Property(e => e.Notes)
            .IsRequired();
    }
}