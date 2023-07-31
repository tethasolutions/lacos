using Lacos.GestioneCommesse.Application.CheckList.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Application.Products.DTOs
{
    public class ProductDto
    {
        public long Id { get; set; }
        public long ProductTypeId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? PictureFileName { get; set; }
        public string? QrCode { get; set; }
        public long? CustomerId { get; set; }
        public long? CustomerAddressId { get; set; }
    }
}
