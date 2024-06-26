﻿namespace Lacos.GestioneCommesse.Application.Operators.DTOs
{
    public class OperatorDto
    {
        public long? Id { get; set; }
        public string? Email { get; set; }
        public string? ColorHex { get; set; }
        public string? Name { get; set; }
        public long? DefaultVehicleId { get; set; }
        public bool? HasUser { get; set; }
        public ICollection<OperatorDocumentDto> Documents { get; set; }
        public IEnumerable<long> ActivityTypes { get; set; }
        public string? SignatureFileName { get; set; }

        public string? UserName { get; set; }
        public string? Password { get; set; }

        public OperatorDto()
        {
            Documents = new List<OperatorDocumentDto>();
            ActivityTypes = new List<long>();
        }
    }
}
