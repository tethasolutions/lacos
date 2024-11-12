using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class HelperTypeMap : BaseEntityMapping<HelperType>
{
    public override void Configure(EntityTypeBuilder<HelperType> builder)
    {
        base.Configure(builder);

        builder.ToTable("HelperTypes", "Registry");

        builder.Property(e => e.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.OneToMany(e => e.HelperDocuments, e => e.HelperType, e => e.HelperTypeId);
    }
}