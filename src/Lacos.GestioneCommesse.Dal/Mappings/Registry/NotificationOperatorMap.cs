using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class NotificationOperatorMap : BaseEntityMapping<NotificationOperator>
{
    public override void Configure(EntityTypeBuilder<NotificationOperator> builder)
    {
        base.Configure(builder);

        builder.ToTable("NotificationOperators", "Registry");

    }
}