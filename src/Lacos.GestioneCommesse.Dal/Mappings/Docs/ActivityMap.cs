using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class ActivityMap : BaseEntityMapping<Activity>
{
    public override void Configure(EntityTypeBuilder<Activity> builder)
    {
        base.Configure(builder);

        builder.ToTable("Activities", "Docs");

        builder.Property(e => e.Description)
            .IsRequired();

        builder.OneToMany(e => e.Interventions, e => e.Activity, e => e.ActivityId);
    }
}