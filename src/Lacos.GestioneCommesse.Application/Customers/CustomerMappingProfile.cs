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
    internal class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            CreateMap<Customer, CustomerDto>();

            CreateMap<CustomerDto, Customer>()
                .IgnoreCommonMembers()
                .Ignore(x => x.User)
                .Ignore(x => x.UserId)
                .Ignore(x => x.Jobs)
                .Ignore(x => x.Products)
                .Ignore(x => x.Tickets)
                .Ignore(x => x.PurchaseOrders);
        }
    }
}
