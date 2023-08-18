using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Application.Customers.DTOs
{
    public class AddressDto
    {
        public long? Id { get; set; }
        public long? CustomerId { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public string Province { get; set; }
        public string ZipCode { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public bool IsMainAddress { get; set; }
    }
}
