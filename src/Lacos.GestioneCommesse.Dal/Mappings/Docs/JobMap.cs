﻿using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class JobMap : BaseEntityMapping<Job>
{
    public override void Configure(EntityTypeBuilder<Job> builder)
    {
        base.Configure(builder);

        builder.ToTable("Jobs", "Docs");

        builder.OneToMany(e => e.Activities, e => e.Job, e => e.JobId);
        builder.OneToMany(e => e.Tickets, e => e.Job, e => e.JobId);
        builder.OneToMany(e => e.Messages, e => e.Job, e => e.JobId);

    }
}