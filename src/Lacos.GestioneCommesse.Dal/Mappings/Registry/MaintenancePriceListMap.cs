using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class MaintenancePriceListMap : BaseEntityMapping<MaintenancePriceList>
{
    public override void Configure(EntityTypeBuilder<MaintenancePriceList> builder)
    {
        base.Configure(builder);

        builder.ToTable("MaintenancePriceLists", "Registry");

        builder.OneToMany(e => e.Items, e => e.MaintenancePriceList, e => e.MaintenancePriceListId);
    }
}