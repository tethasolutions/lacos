using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class ActivityTypeMap : BaseEntityMapping<ActivityType>
{
    public override void Configure(EntityTypeBuilder<ActivityType> builder)
    {
        base.Configure(builder);

        builder.ToTable("ActivityTypes", "Registry");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.OneToMany(e => e.Activities, e => e.Type, e => e.TypeId);
        builder.OneToMany(e => e.CheckLists, e => e.ActivityType, e => e.ActivityTypeId);
    }
}