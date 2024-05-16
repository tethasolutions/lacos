using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Application
{
    public class SyncDocumentListDto
    {
        public string DeviceGuid { get; set; }
        public List<string> DocumentNames { get; set; }

        public SyncDocumentListDto()
        { 
            DocumentNames = new List<string>();
        }
    }
}
