using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Docs
{
    public class SyncActivityAttachmentsDto:SyncBaseDto
    {
        public string? DisplayName { get; set; }
        public string? FileName { get; set; }
        public long? ActivityId { get; set; }

    }
}
