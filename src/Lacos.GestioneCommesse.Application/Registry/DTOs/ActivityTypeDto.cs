using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Application.Registry.DTOs
{
    public class ActivityTypeDto
    {
        public long? Id { get; set; }
        public string? Name { get; set; }
        public bool PictureRequired { get; set; }
    }
}
