using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class ProductMap : BaseEntityMapping<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);

        builder.ToTable("Products", "Registry");

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.PictureFileName)
            .HasMaxLength(50);

        builder.Property(e => e.QrCodePrefix)
            .HasMaxLength(10);

        builder.Property(e => e.Location)
            .HasMaxLength(200);

        builder.Property(e => e.SerialNumber)
            .HasMaxLength(50);

        builder.Property(e => e.ReiType)
            .HasMaxLength(50);

        builder.Property(e => e.ConstructorName)
            .HasMaxLength(200);

        builder.Property(e => e.VocType)
            .HasMaxLength(50);

        builder.OneToMany(e => e.PurchaseOrderItems, e => e.Product, e => e.ProductId);
        builder.OneToMany(e => e.ActivityProducts, e => e.Product, e => e.ProductId);
        builder.OneToMany(e => e.Documents, e => e.Product, e => e.ProductId);
    }
}