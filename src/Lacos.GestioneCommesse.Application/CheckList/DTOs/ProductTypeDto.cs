using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Application.CheckList.DTOs
{
    public class ProductTypeDto
    {
        public long? Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsReiDoor { get; set; }
        public bool IsSparePart { get; set; }
    }
}
