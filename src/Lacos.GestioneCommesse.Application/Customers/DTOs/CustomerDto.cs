﻿using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;

namespace Lacos.GestioneCommesse.Application.Customers.DTOs
{
    public class CustomerDto
    {
        public long? Id { get; set; }
        public string? Name { get; set; }
        public string? Notes { get; set; }
        public CustomerFiscalType FiscalType { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public string? Contact { get; set; }
        public string? ContactTelephone { get; set; }
        public string? ContactEmail { get; set; }
        public bool CanGenerateTickets { get; set; }
        public string? SDICode { get; set; }
        public string? VatNumber { get; set; }
        public string? FiscalCode { get; set; }
        public ICollection<AddressDto>? Addresses { get; set; }
        public string? MainFullAddress { get; set; }
    }
}
