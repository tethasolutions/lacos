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

        builder.Property(e => e.QrCode)
            .HasMaxLength(50);

        builder.OneToMany(e => e.PurchaseOrderItems, e => e.Product, e => e.ProductId);
        builder.OneToMany(e => e.InterventionProducts, e => e.Product, e => e.ProductId);
        builder.OneToMany(e => e.Documents, e => e.Product, e => e.ProductId);
    }
}

public class ProductTypeMap : BaseEntityMapping<ProductType>
{
    public override void Configure(EntityTypeBuilder<ProductType> builder)
    {
        base.Configure(builder);

        builder.ToTable("ProductTypes", "Registry");

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.OneToMany(e => e.Products, e => e.ProductType, e => e.ProductTypeId);
        builder.OneToMany(e => e.CheckLists, e => e.ProductType, e => e.ProductTypeId);
    }
}

public class ProductDocumentMap : BaseEntityMapping<ProductDocument>
{
    public override void Configure(EntityTypeBuilder<ProductDocument> builder)
    {
        base.Configure(builder);

        builder.ToTable("ProductDocuments", "Registry");

        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(200);
    }
}

public class CheckListMap : BaseEntityMapping<CheckList>
{
    public override void Configure(EntityTypeBuilder<CheckList> builder)
    {
        base.Configure(builder);

        builder.ToTable("CheckLists", "Registry");

        builder.Property(e => e.PictureFileName)
            .HasMaxLength(50);

        builder.Property(e => e.Description)
            .IsRequired();

        builder.OneToMany(e => e.Items, e => e.CheckList, e => e.CheckListId);
    }
}

public class CheckListItemMap : BaseEntityMapping<CheckListItem>
{
    public override void Configure(EntityTypeBuilder<CheckListItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("CheckListItems", "Registry");

        builder.Property(e => e.Description)
            .HasMaxLength(500)
            .IsRequired();
    }
}

public class ActivityTypeMap : BaseEntityMapping<ActivityType>
{
    public override void Configure(EntityTypeBuilder<ActivityType> builder)
    {
        base.Configure(builder);

        builder.ToTable("ActivityTypes", "Registry");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.OneToMany(e => e.Activities, e => e.Type, e => e.TypeId);
        builder.OneToMany(e => e.CheckLists, e => e.ActivityType, e => e.ActivityTypeId);
    }
}

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
    }
}

public class OperatorMap : BaseEntityMapping<Operator>
{
    public override void Configure(EntityTypeBuilder<Operator> builder)
    {
        base.Configure(builder);

        builder.ToTable("Operators", "Registry");

        builder.Property(e => e.Email)
            .HasMaxLength(200);

        builder.Property(e => e.ColorHex)
            .HasMaxLength(7);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.OneToMany(e => e.Documents, e => e.Operator, e => e.OperatorId);
        builder.OneToMany(e => e.InterventionProductCheckListItems, e => e.Operator, e => e.OperatorId);
    }
}

public class OperatorDocumentMap : BaseEntityMapping<OperatorDocument>
{
    public override void Configure(EntityTypeBuilder<OperatorDocument> builder)
    {
        base.Configure(builder);

        builder.ToTable("OperatorDocuments", "Registry");

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(50);
    }
}

public class VehicleMap : BaseEntityMapping<Vehicle>
{
    public override void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        base.Configure(builder);

        builder.ToTable("Vehicles", "Registry");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Plate)
            .HasMaxLength(20);

        builder.OneToMany(e => e.Operators, e => e.DefaultVehicle, e => e.DefaultVechicleId);
    }
}