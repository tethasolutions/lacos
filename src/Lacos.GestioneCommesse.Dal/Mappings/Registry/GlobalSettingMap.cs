using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class GlobalSettingMap : BaseEntityMapping<GlobalSetting>
{
    public override void Configure(EntityTypeBuilder<GlobalSetting> builder)
    {
        base.Configure(builder);

        builder.ToTable("GlobalSettings", "Registry");

    }
}