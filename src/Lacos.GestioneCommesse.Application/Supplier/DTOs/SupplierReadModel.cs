﻿using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Application.Suppliers.DTOs
{
    public class SupplierReadModel
    {
        public long? Id { get; set; }
        public string? Name { get; set; }
        public string? Notes { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public ICollection<AddressDto>? Addresses { get; set; }
    }
}
