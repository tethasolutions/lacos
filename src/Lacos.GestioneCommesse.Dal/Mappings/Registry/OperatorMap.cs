using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Registry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Registry;

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

        builder.ManyToMany(e => e.ActivityTypes, e => e.Operators, "OperatorsActivityTypes", "Registry");
        builder.OneToMany(e => e.Jobs, e => e.Referent, e => e.ReferentId);
        builder.OneToMany(e => e.Documents, e => e.Operator, e => e.OperatorId);
        builder.OneToMany(e => e.InterventionProductPictures, e => e.Operator, e => e.OperatorId);
        builder.OneToMany(e => e.InterventionProductCheckListItems, e => e.Operator, e => e.OperatorId);
        builder.OneToMany(e => e.InterventionNotes, e => e.Operator, e => e.OperatorId);
        builder.OneToMany(e => e.NotificationOperators, e => e.Operator, e => e.OperatorId);
    }
}