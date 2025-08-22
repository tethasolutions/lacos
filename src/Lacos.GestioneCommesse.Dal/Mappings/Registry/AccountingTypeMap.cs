using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class AccountingTypeMap : BaseEntityMapping<AccountingType>
{
    public override void Configure(EntityTypeBuilder<AccountingType> builder)
    {
        base.Configure(builder);

        builder.ToTable("AccountingTypes", "Registry");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.OneToMany(e => e.JobAccountings, e => e.AccountingType, e => e.AccountingTypeId);
    }
}