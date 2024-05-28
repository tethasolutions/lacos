using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Application
{
    public class SyncCountersSyncDto
    {
        public string DeviceGuid { get; set; }

        public int TotalDocuments { get; set; }
        public int SyncrnizedDocuments { get; set; }
        public int ErrorDocuments { get; set; }

        public SyncCountersSyncDto()
        {
            TotalDocuments = 0;
            SyncrnizedDocuments = 0;
            ErrorDocuments = 0;

        }
    }
}
