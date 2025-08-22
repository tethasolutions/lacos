using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class JobAccountingMap : BaseEntityMapping<JobAccounting>
{
    public override void Configure(EntityTypeBuilder<JobAccounting> builder)
    {
        base.Configure(builder);

        builder.ToTable("JobAccountings", "Docs");


    }
}