using Lacos.GestioneCommesse.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings;

public class AppLogMap : BaseEntityMapping<AppLog>
{
    public override void Configure(EntityTypeBuilder<AppLog> builder)
    {
        base.Configure(builder);
        builder.ToTable("AppLogs", "Logs");
        builder.Property(e => e.Endpoint).HasMaxLength(200);
        builder.Property(e => e.Data).HasColumnType("nvarchar(max)");
    }
}
