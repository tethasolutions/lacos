using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Contracts.Dtos.Application
{
    public class SyncImageDto
    {
        public string Filename { get; set; }
        public byte[] Content{ get; set; }

        public SyncImageDto()
        { 
            Content = Array.Empty<byte>();
        }
    }
}
