using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class OperatorDocumentMap : BaseEntityMapping<OperatorDocument>
{
    public override void Configure(EntityTypeBuilder<OperatorDocument> builder)
    {
        base.Configure(builder);

        builder.ToTable("OperatorDocuments", "Registry");

        builder.Property(e => e.OriginalFilename)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(50);
    }
}