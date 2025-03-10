using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Application
{
    public class SyncSignDto
    {
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }

        public long InterventionId { get; set; }
        public string FinalNotes { get; set; }
        public string NameSurname { get; set; }
        public string Filename { get; set; }
        public byte[]? Content{ get; set; }
        public bool ToBeReschedule { get; set; }
        public SyncSignDto()
        { 
            Content = null;
        }
    }
}
