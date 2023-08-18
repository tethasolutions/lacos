using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Lacos.GestioneCommesse.Application.Customers.DTOs;
using Lacos.GestioneCommesse.Application.Jobs.DTOs;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Domain.Security;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Jobs
{

    public class JobMappingProfile : Profile
    {
        public JobMappingProfile()
        {

            CreateMap<Job, JobReadModel>();
            CreateMap<Customer, CustomerReadModel>();

            CreateMap<JobDetailDto, Job>()
                .Ignore(x => x.Number)
                .Ignore(x => x.Year)
                .Ignore(x => x.Activities)
                .Ignore(x => x.Customer)
                .IgnoreCommonMembers();
           
            CreateMap<Job, JobDetailDto>()
                .Ignore(x=>x.OperatorId)
                .Ignore(x => x.CustomerAddressId)
                .Ignore(x => x.ProductTypeId);

            CreateMap<Job, JobDetailReadModel>()
                .MapMember(x=>x.Code,y=>y.Number+"/"+ y.Year)
                .Ignore(x => x.CustomerFullAddress)
                .Ignore(x => x.CustomerAddressId)
                .Ignore(x => x.CustomerAddress)
                //.MapMember(x=>x.CustomerFullAddress,y => y.CustomerAddress.StreetAddress + " - " + y.CustomerAddress.City + " - " + y.CustomerAddress.Province)
                .Ignore(x => x.OperatorId);

            CreateMap<User, JobOperatorDto>()
                .Ignore(x => x.Name);

            CreateMap<Job, JobSearchReadModel>()
                .MapMember(x => x.Code, y => y.Number + "/" + y.Year)
                .Ignore(x => x.CustomerFullAddress)
                .Ignore(x => x.CustomerAddressId)
                .Ignore(x => x.CustomerAddress)
                //.MapMember(x => x.CustomerFullAddress, y => y.CustomerAddress.StreetAddress + " - " + y.CustomerAddress.City + " - " + y.CustomerAddress.Province)
                .Ignore(x => x.OperatorId);
        }
    }
}
