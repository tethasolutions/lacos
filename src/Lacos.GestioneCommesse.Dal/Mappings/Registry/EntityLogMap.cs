using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class EntityLogMap : BaseEntityMapping<EntityLog>
{
    public override void Configure(EntityTypeBuilder<EntityLog> builder)
    {
        base.Configure(builder);

        builder.ToTable("EntityLogs", "Registry");

        builder.HasKey(e => e.Id);

    }
}