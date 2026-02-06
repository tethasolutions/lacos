using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class WarehouseMovementMap : BaseEntityMapping<WarehouseMovement>
{
    public override void Configure(EntityTypeBuilder<WarehouseMovement> builder)
    {
        base.Configure(builder);

        builder.ToTable("WarehouseMovements", "Docs");
    }
}