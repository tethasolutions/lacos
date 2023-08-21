using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Application.CheckLists.DTOs
{
    public class CheckListItemDto
    {
        public long? Id { get; set; }
        public string? Description { get; set; }
        public long CheckListId { get; set; }
    }
}
