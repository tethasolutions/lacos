using AutoMapper;
using Lacos.GestioneCommesse.Application.Customers.DTOs;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;
using Telerik.Barcode;

namespace Lacos.GestioneCommesse.Application.Customers
{
    internal class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            CreateMap<Customer, CustomerDto>()
                .MapMember(x => x.MainFullAddress, y => y.Addresses.Where(a => a.IsMainAddress).FirstOrDefault() != null ?
                y.Addresses.Where(a => a.IsMainAddress).FirstOrDefault().Description + " - " 
                + y.Addresses.Where(a => a.IsMainAddress).FirstOrDefault().StreetAddress + ", " 
                + y.Addresses.Where(a => a.IsMainAddress).FirstOrDefault().City 
                + " (" + y.Addresses.Where(a => a.IsMainAddress).FirstOrDefault().Province + ")" : "");

            CreateMap<CustomerDto, Customer>()
                .IgnoreCommonMembers()
                .Ignore(x => x.User)
                .Ignore(x => x.UserId)
                .Ignore(x => x.Jobs)
                .Ignore(x => x.Products)
                .Ignore(x => x.Tickets)
                .Ignore(x => x.Addresses)
                .AfterMap(AfterMap);

        }
        private static void AfterMap(CustomerDto customerDto, Customer customer, ResolutionContext context)
        {
            if (customerDto.Addresses != null) customerDto.Addresses.Merge(customer.Addresses, (addressDto, address) => addressDto.Id == address.Id, (_, item) => item.CustomerId = customer.Id, context);
        }
    }
}
