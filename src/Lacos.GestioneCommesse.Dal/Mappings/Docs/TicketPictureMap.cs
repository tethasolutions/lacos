using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class TicketPictureMap : BaseEntityMapping<TicketPicture>
{
    public override void Configure(EntityTypeBuilder<TicketPicture> builder)
    {
        base.Configure(builder);

        builder.ToTable("TicketPictures", "Docs");

        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(50);
    }
}