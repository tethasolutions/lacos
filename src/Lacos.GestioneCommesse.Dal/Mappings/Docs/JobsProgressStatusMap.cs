using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class JobsProgressStatusMap : BaseEntityMapping<JobsProgressStatus>
{
    public override void Configure(EntityTypeBuilder<JobsProgressStatus> builder)
    {
        base.Configure(builder);

        builder.HasNoKey();

        builder.ToView("v_JobsProgressStatus", "Docs");
    }
}