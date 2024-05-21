using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Application
{
    public class SyncDocumentDto
    {
        public string DeviceGuid { get; set; }
        public string DocumentName { get; set; }
        public byte[]? Content{ get; set; }

        public SyncDocumentDto()
        { 
            Content = null;
        }
    }
}
