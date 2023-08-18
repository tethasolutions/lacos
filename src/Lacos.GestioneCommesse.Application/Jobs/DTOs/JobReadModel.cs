using Lacos.GestioneCommesse.Application.Customers.DTOs;
using Lacos.GestioneCommesse.Domain.Docs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Application.Jobs.DTOs
{
    public class JobReadModel
    {
        public long Id { get; set; }
        public DateTimeOffset JobDate { get; set; }
        public int Number { get; set; }
        public int Year { get; set; }
        public string? Description { get; set; }
        public JobStatus Status { get; set; }
        public CustomerReadModel? Customer { get; set; }
       
    }
}
