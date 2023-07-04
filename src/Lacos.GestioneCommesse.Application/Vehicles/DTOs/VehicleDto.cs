using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Application.Vehicles.DTOs
{
    public class VehicleDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Plate { get; set; }
        public string? Notes { get; set; }
    }
}
