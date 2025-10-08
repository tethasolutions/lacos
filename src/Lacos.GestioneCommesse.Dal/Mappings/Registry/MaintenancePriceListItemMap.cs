using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class MaintenancePriceListItemMap : BaseEntityMapping<MaintenancePriceListItem>
{
    public override void Configure(EntityTypeBuilder<MaintenancePriceListItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("MaintenancePriceListItems", "Registry");

    }
}