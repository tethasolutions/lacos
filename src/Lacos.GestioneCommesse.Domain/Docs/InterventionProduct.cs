﻿namespace Lacos.GestioneCommesse.Domain.Docs;

public class InterventionProduct : FullAuditedEntity, ILogEntity
{
    public long ActivityProductId { get; set; }
    public ActivityProduct? ActivityProduct { get; set; }

    public long InterventionId { get; set; }
    public Intervention? Intervention { get; set; }

    public InterventionProductCheckList? CheckList { get; set; }

    public ICollection<InterventionProductPicture> Pictures { get; set; }

    public InterventionProduct()
    {
        Pictures = new List<InterventionProductPicture>();
    }
}