﻿using Lacos.GestioneCommesse.Dal.Extensions;
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

        builder.Property(e => e.QrCode)
            .HasMaxLength(50);

        builder.OneToMany(e => e.PurchaseOrderItems, e => e.Product, e => e.ProductId);
        builder.OneToMany(e => e.InterventionProducts, e => e.Product, e => e.ProductId);
        builder.OneToMany(e => e.Documents, e => e.Product, e => e.ProductId);
    }
}