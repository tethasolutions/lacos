using Lacos.GestioneCommesse.Application.Vehicles.DTOs;

namespace Lacos.GestioneCommesse.Application.Operators.DTOs
{
    public class OperatorReadModel
    {
        public long? Id { get; set; }
        public string? Email { get; set; }
        public string? ColorHex { get; set; }
        public string? Name { get; set; }
        public long? DefaultVehicleId { get; set; }
        public VehicleDto? DefaultVehicle { get; set; }
        public bool HasUser { get; set; }
        public string? Username { get; set; }

        public ICollection<OperatorDocumentDto> Documents { get; set; }
    }
}
