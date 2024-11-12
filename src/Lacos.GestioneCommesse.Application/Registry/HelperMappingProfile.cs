using AutoMapper;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Registry
{
    internal class HelperMappingProfile : Profile
    {
        public HelperMappingProfile()
        {
            CreateMap<HelperType, HelperTypeDto>();

            CreateMap<HelperTypeDto, HelperType>()
                .IgnoreCommonMembers()
                .Ignore(x => x.HelperDocuments);

            CreateMap<HelperDocument, HelperDocumentDto>();

            CreateMap<HelperDocumentDto, HelperDocument>()
                .IgnoreCommonMembers()
                .Ignore(x => x.HelperType);

            CreateMap<HelperDocument, HelperDocumentReadModel>()
                .MapMember(x => x.HelperTypeName, y => y.HelperType.Type);

            CreateMap<HelperDocumentReadModel, HelperDocument>()
                .IgnoreCommonMembers()
                .Ignore(x => x.HelperTypeId)
                .Ignore(x => x.HelperType);
        }
    }
}
