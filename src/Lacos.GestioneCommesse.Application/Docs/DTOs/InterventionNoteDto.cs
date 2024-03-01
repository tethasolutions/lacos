namespace Lacos.GestioneCommesse.Application.Docs.DTOs
{
    public class InterventionNoteDto
    {
        public long? Id { get; set; }
        public string? PictureFileName { get; set; }
        public string? Notes { get; set; }
        public string? OperatorName { get; set; }
        public long InterventionId { get; set; }
    }
}
