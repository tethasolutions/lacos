using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class SupplierMap : BaseEntityMapping<Supplier>
{
    public override void Configure(EntityTypeBuilder<Supplier> builder)
    {
        base.Configure(builder);

        builder.ToTable("Suppliers", "Registry");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.OneToMany(e => e.Addresses, e => e.Supplier, e => e.SupplierId);
        builder.OneToMany(e => e.PurchaseOrders, e => e.Supplier, e => e.SupplierId);
    }
}