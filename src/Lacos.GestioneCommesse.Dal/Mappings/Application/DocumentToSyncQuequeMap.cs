using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class DocumentToSyncQuequeMap : BaseEntityMapping<DocumentToSyncQueue>
{
    public override void Configure(EntityTypeBuilder<DocumentToSyncQueue> builder)
    {
        base.Configure(builder);

        builder.ToTable("DocumentsToSyncQueue", "Application");

        builder.Property(x => x.DeviceGuid)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.DocumentName)
            .HasMaxLength(100)
            .IsRequired();

       
    }
}