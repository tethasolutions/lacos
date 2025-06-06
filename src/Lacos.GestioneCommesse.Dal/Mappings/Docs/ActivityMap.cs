﻿using Lacos.GestioneCommesse.Dal.Extensions;
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

        builder.OneToMany(e => e.Interventions, e => e.Activity, e => e.ActivityId);
        builder.OneToMany(e => e.ActivityProducts, e => e.Activity, e => e.ActivityId);
        builder.OneToMany(e => e.Attachments, e => e.Activity, e => e.ActivityId);
        builder.OneToMany(e => e.Messages, e => e.Activity, e => e.ActivityId);
        builder.ManyToMany(e => e.ActivityDependencies, e => e.ParentActivities, "ActivityDependencies", "Docs");
        builder.ManyToMany(e => e.PurchaseOrderDependencies, e => e.ParentActivities, "PurchaseOrderDependencies", "Docs");

    }
}