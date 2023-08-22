using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class ProductDocumentMap : BaseEntityMapping<ProductDocument>
{
    public override void Configure(EntityTypeBuilder<ProductDocument> builder)
    {
        base.Configure(builder);

        builder.ToTable("ProductDocuments", "Registry");

        builder.Property(e => e.OriginalFilename)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(200);
    }
}