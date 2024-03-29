﻿using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class JobAttachmentMap : BaseEntityMapping<JobAttachment>
{
    public override void Configure(EntityTypeBuilder<JobAttachment> builder)
    {
        base.Configure(builder);

        builder.ToTable("JobAttachments", "Docs");

        builder.Property(e => e.DisplayName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(e => e.FileName)
            .HasMaxLength(64)
            .IsRequired();
    }
}