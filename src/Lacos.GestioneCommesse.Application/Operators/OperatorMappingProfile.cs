using AutoMapper;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Operators
{
    public class OperatorMappingProfile : Profile
    {
        public OperatorMappingProfile()
        {
            CreateMap<Operator, OperatorDto>()

                .MapMember(x => x.hasUser, y => y.UserId != null)
                .Ignore(x=>x.UserName)
                .Ignore(x=>x.Password);


            CreateMap<OperatorDto, Operator>()
                .IgnoreCommonMembers()
                .Ignore(x => x.Interventions)
                .Ignore(x => x.InterventionNotes)
                .Ignore(x => x.InterventionProductCheckListItems)
                .Ignore(x => x.InterventionProductPictures)
                .Ignore(x => x.DefaultVehicle)
                .Ignore(x => x.UserId)
                .Ignore(x => x.User);

            CreateMap<Operator, OperatorReadModel>()
                .MapMember(x => x.hasUser, y => y.UserId != null)
                .MapMember(x => x.Username, y => y.User == null? null : y.User.UserName);
                

            CreateMap<OperatorDocument, OperatorDocumentReadModel>();
            CreateMap<OperatorDocumentReadModel, OperatorDocument>()
                .Ignore(x=>x.Operator)
                .Ignore(x => x.OperatorId)
                .IgnoreCommonMembers();


            CreateMap<OperatorDocument, OperatorDocumentDto>();
            CreateMap<OperatorDocumentDto, OperatorDocument>()
                .Ignore(x=>x.Operator)
                .IgnoreCommonMembers();

        }
    }
}
