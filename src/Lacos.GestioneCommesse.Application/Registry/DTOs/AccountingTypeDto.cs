namespace Lacos.GestioneCommesse.Application.Registry.DTOs
{
    public class AccountingTypeDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool GenerateAlert { get; set; }
        public bool IsNegative { get; set; }
        public int Order { get; set; }
    }
}
