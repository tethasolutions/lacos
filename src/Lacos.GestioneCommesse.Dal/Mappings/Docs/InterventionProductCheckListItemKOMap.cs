using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Docs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Docs;

public class InterventionProductCheckListItemKOMap : BaseEntityMapping<InterventionProductCheckListItemKO>
{
    public override void Configure(EntityTypeBuilder<InterventionProductCheckListItemKO> builder)
    {
        base.Configure(builder);

        builder.HasNoKey();

        builder.ToView("v_CheckListItems_KoOutcome", "Docs");

    }
}