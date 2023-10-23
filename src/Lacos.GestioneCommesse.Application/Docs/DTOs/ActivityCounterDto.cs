using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Application.Docs.DTOs;

    public class ActivityCounterDto
    {
        public long ActivityId { get; set; }
        public string? ActivityName { get; set; }
        public string? ActivityColor { get; set; }
        public int Active { get; set; }
        public int Expired { get; set; }
    }

