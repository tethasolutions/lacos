using AutoMapper;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Registry
{
    internal class AccountingTypeMappingProfile : Profile
    {
        public AccountingTypeMappingProfile()
        {
            CreateMap<AccountingType, AccountingTypeDto>();

            CreateMap<AccountingTypeDto, AccountingType>()
                .IgnoreCommonMembers()
                .Ignore(x => x.JobAccountings);
        }
    }
}
