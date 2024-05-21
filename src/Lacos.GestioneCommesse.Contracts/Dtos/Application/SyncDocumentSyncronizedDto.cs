using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Application
{
    public class SyncDocumentSyncronizedDto
    {

        public string DeviceGuid { get; set; }
        public string DocumentName { get; set; }

        public SyncDocumentSyncronizedDto()
        { 
        }
    }
}
