using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

public class HelperDocumentMap : BaseEntityMapping<HelperDocument>
{
    public override void Configure(EntityTypeBuilder<HelperDocument> builder)
    {
        base.Configure(builder);

        builder.ToTable("HelperDocuments", "Registry");

        builder.Property(e => e.Description)
            .IsRequired();
    }
}