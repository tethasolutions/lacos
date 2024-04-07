using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class MessageMap : BaseEntityMapping<Message>
{
    public override void Configure(EntityTypeBuilder<Message> builder)
    {
        base.Configure(builder);

        builder.ToTable("Messages", "Docs");

        builder.Property(e => e.Note)
            .IsRequired();

        builder.OneToMany(e => e.MessageNotifications, e => e.Message, e => e.MessageId);
    }
}