using Lacos.GestioneCommesse.Application.Customers.DTOs;
using Lacos.GestioneCommesse.Domain.Docs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Application.Jobs.DTOs
{
    public class JobDetailDto
    {
        public long? Id { get; set; }
        public string? Description { get; set; }
        public long OperatorId { get; set; }
        public DateTimeOffset JobDate { get; set; }
        public long CustomerId { get; set; }
        public CustomerDto? Customer { get; set; }
        public long? CustomerAddressId { get; set; }
        public long ProductTypeId { get; set; }
        public JobStatus Status { get; set; }
    }
}
