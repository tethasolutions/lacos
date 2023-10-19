using AutoMapper;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Operators;

public class OperatorMappingProfile : Profile
{
    public OperatorMappingProfile()
    {
        CreateMap<Operator, OperatorDto>()
            .MapMember(x => x.HasUser, y => y.UserId.HasValue)
            .MapMember(x => x.UserName, y => y.User == null ? null : y.User.UserName)
            .Ignore(x => x.Password)
            .MapMember(x => x.ActivityTypes, y => y.ActivityTypes.Select(e => e.Id));

        CreateMap<OperatorDto, Operator>()
            .IgnoreCommonMembers()
            .Ignore(x => x.Interventions)
            .Ignore(x => x.InterventionNotes)
            .Ignore(x => x.InterventionProductCheckListItems)
            .Ignore(x => x.InterventionProductPictures)
            .Ignore(x => x.DefaultVehicle)
            .Ignore(x => x.UserId)
            .Ignore(x => x.User)
            .Ignore(x => x.ActivityTypes)
            .Ignore(x => x.Documents)
            .AfterMap(AfterMap);

        CreateMap<Operator, OperatorReadModel>()
            .MapMember(x => x.HasUser, y => y.UserId.HasValue)
            .MapMember(x => x.Username, y => y.User == null ? null : y.User.UserName);


        CreateMap<OperatorDocument, OperatorDocumentReadModel>();
        CreateMap<OperatorDocumentReadModel, OperatorDocument>()
            .Ignore(x => x.Operator)
            .Ignore(x => x.OperatorId)
            .IgnoreCommonMembers();


        CreateMap<OperatorDocument, OperatorDocumentDto>();
        CreateMap<OperatorDocumentDto, OperatorDocument>()
            .Ignore(x => x.Operator)
            .IgnoreCommonMembers();

    }

    private static void AfterMap(OperatorDto arg1, Operator arg2, ResolutionContext arg3)
    {
        arg1.Documents.Merge(arg2.Documents, (dto, document) => dto.Id == document.Id, (_, _) => { }, arg3);
    }
}