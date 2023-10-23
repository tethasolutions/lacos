namespace Lacos.GestioneCommesse.Contracts.Dtos.Enums
{
    public enum InterventionStatus
    {
        Scheduled,
        CompletedSuccesfully,
        CompletedUnsuccesfully
    }

    public enum InterventionProductCheckListItemOutcome
    {
        Positive,
        Negative,
        Unknown
    }

    public enum InterventionProductPictureType
    {
        Generic,
        Final
    }

    public enum PurchaseOrderStatus
    {
        Pending,
        Ordered,
        Completed,
        Canceled
    }

    public enum TicketStatus
    {
        Opened,
        InProgress,
        Resolved,
        Canceled
    }
    public enum CustomerFiscalType
    {
        PrivatePerson,
        Company
    }

    public enum ActivityStatus
    {
        Pending,
        InProgress,
        ReadyForCompletion,
        Completed
    }
}
