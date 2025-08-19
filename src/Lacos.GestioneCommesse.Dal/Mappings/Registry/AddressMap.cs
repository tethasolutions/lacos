using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class AddressMap : BaseEntityMapping<Address>
{
    public override void Configure(EntityTypeBuilder<Address> builder)
    {
        base.Configure(builder);

        builder.ToTable("Addresses", "Registry");

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
            .HasMaxLength(5);

        builder.Property(e => e.Telephone)
            .HasMaxLength(20);

        builder.Property(e => e.Email)
            .HasMaxLength(200);

        builder.OneToMany(e => e.Jobs, e => e.Address, e => e.AddressId);
        builder.OneToMany(e => e.Activities, e => e.Address, e => e.AddressId);
        builder.OneToMany(e => e.Tickets, e => e.Address, e => e.AddressId);
        
    }
}