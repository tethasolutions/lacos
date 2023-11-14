using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class InterventionMap : BaseEntityMapping<Intervention>
{
    public override void Configure(EntityTypeBuilder<Intervention> builder)
    {
        base.Configure(builder);

        builder.ToTable("Interventions", "Docs");

        builder.Property(e => e.Description)
            .IsRequired();

        builder.Property(e => e.ReportFileName)
            .HasMaxLength(50);

        builder.Property(e => e.CustomerSignatureFileName)
            .HasMaxLength(50);

        builder.ManyToMany(e => e.Operators, e => e.Interventions, "InterventionOperators", "Docs");
        builder.OneToMany(e => e.Notes, e => e.Intervention, e => e.InterventionId);
        builder.OneToMany(e => e.Products, e => e.Intervention, e => e.InterventionId);
        builder.OneToMany(e => e.Disputes, e => e.Intervention, e => e.InterventionId);
      
    }
}