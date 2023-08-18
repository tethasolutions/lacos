using Lacos.GestioneCommesse.Application.Customers.DTOs;
using Lacos.GestioneCommesse.Domain.Docs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Application.Jobs.DTOs
{
    public class JobSearchReadModel
    {
        public long? Id { get; set; }
        public string? Description { get; set; }
        public string? Code { get; set; }
        public long OperatorId { get; set; }
        public DateTimeOffset JobDate { get; set; }
        public long CustomerId { get; set; }
        public CustomerReadModel? Customer { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerFullAddress { get; set; }
        public long? CustomerAddressId { get; set; }
        public AddressDto? CustomerAddress { get; set; }
        public JobStatus Status { get; set; }
    }
}
