using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class CustomerMap : BaseEntityMapping<Customer>
{
    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);

        builder.ToTable("Customers", "Registry");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.OneToMany(e => e.Addresses, e => e.Customer, e => e.CustomerId);
        builder.OneToMany(e => e.Jobs, e => e.Customer, e => e.CustomerId);
        builder.OneToMany(e => e.Products, e => e.Customer, e => e.CustomerId);
    }
}