using AutoMapper;
using Lacos.GestioneCommesse.Application.Customers.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Application.Registry
{
    internal class AddressMappingProfile : Profile
    {
        public AddressMappingProfile()
        {
            CreateMap<CustomerAddress, AddressDto>();

            CreateMap<AddressDto, CustomerAddress>()
                .IgnoreCommonMembers()
                .Ignore(x => x.Description)
                .Ignore(x => x.Notes)
                .Ignore(x => x.CustomerId)
                .Ignore(x => x.Customer)
                .Ignore(x => x.Products)
                .Ignore(x => x.Tickets)
                .Ignore(x => x.Activities);
        }
    }
}
