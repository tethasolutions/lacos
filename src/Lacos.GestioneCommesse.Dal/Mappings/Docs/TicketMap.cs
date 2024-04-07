using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class TicketMap : BaseEntityMapping<Ticket>
{
    public override void Configure(EntityTypeBuilder<Ticket> builder)
    {
        base.Configure(builder);

        builder.ToTable("Tickets", "Docs");

        builder.Property(e => e.Description)
            .IsRequired();

        builder.OneToMany(e => e.Pictures, e => e.Ticket, e => e.TicketId);
        builder.OneToMany(e => e.Messages, e => e.Ticket, e => e.TicketId);
    }
}