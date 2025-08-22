using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class JobAccountingService : IJobAccountingService
{
    private readonly IMapper mapper;
    private readonly IRepository<JobAccounting> repository;
    private readonly ILacosDbContext dbContext;

    public JobAccountingService(
        IMapper mapper,
        IRepository<JobAccounting> repository,
        ILacosDbContext dbContext
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.dbContext = dbContext;
    }

    public IQueryable<JobAccountingReadModel> Query()
    {
        return repository.Query()
            .Include(e => e.Job)
            .Include(e => e.AccountingType)
            .Project<JobAccountingReadModel>(mapper);
    }

    public async Task Create(JobAccountingDto jobAccountingDto)
    {
        var jobAccounting = jobAccountingDto.MapTo<JobAccounting>(mapper);

        await repository.Insert(jobAccounting);

        await dbContext.SaveChanges();
    }

    public async Task Delete(long id)
    {
        var jobAccounting = await repository.Query()
            .AsSplitQuery()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (jobAccounting == null)
        {
            return;
        }

        repository.Delete(jobAccounting);

        await dbContext.SaveChanges();
    }
    public async Task<JobAccountingDto> Get(long id)
    {
        var jobAccountingDto = await repository.Query()
            .Where(e => e.Id == id)
            .Project<JobAccountingDto>(mapper)
            .FirstOrDefaultAsync();

        if (jobAccountingDto == null)
        {
            throw new NotFoundException($"Voce commessa con Id {id} non trovata.");
        }

        return jobAccountingDto;
    }

    public async Task<JobAccountingDto> Update(JobAccountingDto jobAccountingDto)
    {
        var jobAccounting = await repository.Get(jobAccountingDto.Id);

        if (jobAccounting == null)
        {
            throw new NotFoundException($"Voce commessa con Id {jobAccountingDto.Id} non trovata.");
        }

        jobAccounting = jobAccountingDto.MapTo(jobAccounting, mapper);

        repository.Update(jobAccounting);

        await dbContext.SaveChanges();

        return await Get(jobAccounting.Id);
    }
}