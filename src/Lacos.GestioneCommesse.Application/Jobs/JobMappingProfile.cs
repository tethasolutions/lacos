using AutoMapper;
using Lacos.GestioneCommesse.Application.Jobs.DTOs;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Extensions;

namespace Lacos.GestioneCommesse.Application.Jobs;

public class JobMappingProfile : Profile
{
    public JobMappingProfile()
    {

        CreateMap<Job, JobReadModel>()
            .MapMember(x => x.Code, y => y.Number + "/" + y.Year)
            .MapMember(x => x.Date, y => y.JobDate)
            .MapMember(x => x.Customer, y => y.Customer!.Name)
            .MapMember(x => x.CanBeRemoved, y => y.Status == JobStatus.Pending);

        CreateMap<JobDto, Job>()
            .IgnoreCommonMembers()
            .Ignore(x => x.Activities)
            .Ignore(x => x.Customer)
            .Ignore(x => x.Number)
            .Ignore(x => x.Year)
            .Ignore(x => x.Status)
            .MapMember(x => x.JobDate, (x, y) => y.IsTransient() ? x.Date : y.JobDate)
            .MapMember(x => x.CustomerId, (x, y) => y.IsTransient() ? x.CustomerId : y.CustomerId);
        
        CreateMap<Job, JobDto>()
            .MapMember(x => x.Date, y => y.JobDate);
    }
}