namespace Lacos.GestioneCommesse.Application.CheckLists.DTOs
{
    public class CheckListItemDto
    {
        public long? Id { get; set; }
        public string? Description { get; set; }
        public long CheckListId { get; set; }
    }
}
