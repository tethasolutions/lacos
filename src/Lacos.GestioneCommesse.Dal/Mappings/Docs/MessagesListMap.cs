using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class MessagesListMap : BaseEntityMapping<MessagesList>
{
    public override void Configure(EntityTypeBuilder<MessagesList> builder)
    {
        base.Configure(builder);

        builder.HasNoKey();

        builder.ToView("v_MessagesList", "Docs");
    }
}