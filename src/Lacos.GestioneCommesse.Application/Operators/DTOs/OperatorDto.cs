using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Application.Operators.DTOs
{
    public class OperatorDto
    {
        public long? Id { get; set; }
        public string? Email { get; set; }
        public string? ColorHex { get; set; }
        public string? Name { get; set; }
        public long? DefaultVehicleId { get; set; }
        public bool? hasUser { get; set; }
        public ICollection<OperatorDocumentDto>? Documents { get; set; }
        public Vehicle? DefaultVehicle { get; set; }

        public string? UserName { get; set; }
        public string? Password { get; set; }

        public OperatorDto()
        {
            Documents = new List<OperatorDocumentDto>();
        }
    }
}
