﻿using Lacos.GestioneCommesse.Application.Vehicles.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ICollection<OperatorDocumentDto> Documents { get; set; }
    }
}