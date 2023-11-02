using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class SupplierAddressMap : BaseEntityMapping<SupplierAddress>
{
    public override void Configure(EntityTypeBuilder<SupplierAddress> builder)
    {
        base.Configure(builder);

        builder.ToTable("SupplierAddresses", "Registry");

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.City)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.StreetAddress)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Province)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.ZipCode)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(e => e.Telephone)
            .HasMaxLength(20);

        builder.Property(e => e.Email)
            .HasMaxLength(200);

    }
}