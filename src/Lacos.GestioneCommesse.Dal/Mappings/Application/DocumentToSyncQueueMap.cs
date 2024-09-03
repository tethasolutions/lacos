using Lacos.GestioneCommesse.Domain.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Application;

public class DocumentToSyncQueueMap : BaseEntityMapping<DocumentToSyncQueue>
{
    public override void Configure(EntityTypeBuilder<DocumentToSyncQueue> builder)
    {
        base.Configure(builder);

        builder.ToTable("DocumentsToSyncQueue", "Application");

        builder.Property(x => x.DeviceGuid)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.DocumentName)
            .IsRequired();
    }
}