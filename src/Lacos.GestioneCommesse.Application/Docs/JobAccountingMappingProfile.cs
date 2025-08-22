using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Docs;

public class JobAccountingMappingProfile : Profile
{
    public JobAccountingMappingProfile()
    {
        CreateMap<JobAccountingDto, JobAccounting>()
           .IgnoreCommonMembers()
           .Ignore(x => x.Job)
           .Ignore(x => x.AccountingType);

        CreateMap<JobAccounting, JobAccountingDto>();

        CreateMap<JobAccountingReadModel, JobAccounting>()
           .IgnoreCommonMembers()
           .Ignore(x => x.Job)
           .Ignore(x => x.AccountingType);

        CreateMap<JobAccounting, JobAccountingReadModel>()
           .MapMember(x => x.JobCode, y => CustomDbFunctions.FormatCode(y.Job!.Number, y.Job.Year, 3))
           .MapMember(x => x.JobReference, y => y.Job!.Reference)
           .MapMember(x => x.AccountingTypeName, y => y.AccountingType.Name)
           .MapMember(x => x.GenerateAlert, y => y.AccountingType.GenerateAlert);

    }
}