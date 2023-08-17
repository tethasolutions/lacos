using AutoMapper;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Application.Registry
{
    internal class OperatorMappingProfile : Profile
    {
        public OperatorMappingProfile()
        {
            CreateMap<Operator, OperatorDto>();

            CreateMap<OperatorDto, Operator>()
                .IgnoreCommonMembers()
                .Ignore(x=> x.Documents)
                .Ignore(x => x.Interventions)
                .Ignore(x => x.InterventionNotes)
                .Ignore(x => x.InterventionProductCheckListItems)
                .Ignore(x => x.InterventionProductPictures);
        }
    }
}
