using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class CustomerAddressMap : BaseEntityMapping<CustomerAddress>
{
    public override void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        base.Configure(builder);

        builder.ToTable("CustomerAddresses", "Registry");

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

        builder.OneToMany(e => e.Activities, e => e.CustomerAddress, e => e.CustomerAddressId);
        builder.OneToMany(e => e.Products, e => e.CustomerAddress, e => e.CustomerAddressId);
        builder.OneToMany(e => e.Tickets, e => e.CustomerAddress, e => e.CustomerAddressId);
    }
}