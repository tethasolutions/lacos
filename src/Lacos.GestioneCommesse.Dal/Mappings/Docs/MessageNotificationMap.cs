using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class MessageNotificationMap : BaseEntityMapping<MessageNotification>
{
    public override void Configure(EntityTypeBuilder<MessageNotification> builder)
    {
        base.Configure(builder);

        builder.ToTable("MessageNotfications", "Docs");

    }
}