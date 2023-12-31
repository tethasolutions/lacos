﻿using AutoMapper;
using Lacos.GestioneCommesse.Application.Docs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Docs.Services;

public class JobsService : IJobsService
{
    private readonly IMapper mapper;
    private readonly IRepository<Job> repository;
    private readonly ILacosDbContext dbContext;

    public JobsService(
        IMapper mapper,
        IRepository<Job> repository,
        ILacosDbContext dbContext
    )
    {
        this.mapper = mapper;
        this.repository = repository;
        this.dbContext = dbContext;
    }

    public IQueryable<JobReadModel> Query()
    {
        return repository.Query()
            //.Where(e => !e.IsInternalJob)
            .Project<JobReadModel>(mapper);
    }

    public async Task<JobDto> GetTicketJob(long CustomerId)
    {
        var jobDto = await repository.Query()
            .Where(e => e.IsInternalJob && e.Year == DateTime.Now.Year && e.CustomerId == CustomerId)
            .Project<JobDto>(mapper)
            .FirstOrDefaultAsync();

        if (jobDto == null)
        {
            jobDto = new JobDto();
            jobDto.CustomerId = CustomerId;
            jobDto.Year = DateTime.Now.Year;
            jobDto.Reference = "Ticket " + DateTime.Now.Year.ToString();
            jobDto.Date = DateTime.Now;
            jobDto.Status = JobStatusDto.Pending;
            jobDto.Description = " ";

            var job = jobDto.MapTo<Job>(mapper);
            job.IsInternalJob= true;
            var number = await GetNextNumber(job.JobDate.Year);
            job.SetCode(job.JobDate.Year, number);

            await repository.Insert(job);

            await dbContext.SaveChanges();
            return await Get(job.Id);
        }
        else
            return jobDto;
    }

    public async Task<JobDto> Get(long id)
    {
        var jobDto = await repository.Query()
            .Where(e => e.Id == id)
            .Project<JobDto>(mapper)
            .FirstOrDefaultAsync();

        if (jobDto == null)
        {
            throw new NotFoundException($"Commessa con Id {id} non trovata.");
        }

        return jobDto;
    }


    public async Task<JobDto> Create(JobDto jobDto)
    {
        var job = jobDto.MapTo<Job>(mapper);
        var number = await GetNextNumber(job.JobDate.Year);

        job.SetCode(job.JobDate.Year, number);

        await repository.Insert(job);

        await dbContext.SaveChanges();

        return await Get(job.Id);
    }

    public async Task<JobDto> Update(JobDto jobDto)
    {
        var job = await repository.Get(jobDto.Id);

        if (job == null)
        {
            throw new NotFoundException($"Commessa con Id {jobDto.Id} non trovata.");
        }

        job = jobDto.MapTo(job, mapper);

        repository.Update(job);

        await dbContext.SaveChanges();

        return await Get(job.Id);
    }

    public async Task Delete(long id)
    {
        var job = await repository.Query()
            .AsSplitQuery()
            .Include(e => e.Activities)
            .ThenInclude(e => e.Interventions)
            .Include(e => e.Activities)
            .ThenInclude(e => e.ActivityProducts)
            .ThenInclude(e => e.InterventionProducts)
            .ThenInclude(e => e.CheckList)
            .ThenInclude(e => e!.Items)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (job == null)
        {
            return;
        }

        if (job.HasCompletedInterventions())
        {
            throw new LacosException("Non puoi eliminare una commessa con interventi completati.");
        }

        repository.Delete(job);
        
        await dbContext.SaveChanges();
    }

    private async Task<int> GetNextNumber(int year)
    {
        var maxNumber = await repository.Query()
            .Where(e => e.Year == year)
            .Select(e => (int?)e.Number)
            .MaxAsync();

        return (maxNumber ?? 0) + 1;
    }

    public async Task<long> CopyJob(JobCopyDto jobCopyDto)
    {
        var newJob = await repository.Query()
            .AsNoTracking()
            .Where(e => e.Id == jobCopyDto.OriginalId)
            .Include(e => e.Activities)
            .FirstOrDefaultAsync();

        DateTime originalDate = newJob.JobDate.Date;
        newJob.Id = 0;
        newJob.JobDate = jobCopyDto.Date;
        newJob.Year = jobCopyDto.Date.Year;
        newJob.CustomerId = jobCopyDto.CustomerId;
        newJob.AddressId = jobCopyDto.AddressId;
        newJob.Reference = jobCopyDto?.Reference;
        newJob.Description= jobCopyDto?.Description;
        var number = await GetNextNumber(newJob.JobDate.Year);
        newJob.SetCode(newJob.JobDate.Year, number);

        foreach (var activity in newJob.Activities)
        {
            activity.Id = 0;
            if (activity.ExpirationDate!= null)
            {
                var dateDiff = activity.ExpirationDate - originalDate;
                activity.ExpirationDate = newJob.JobDate + dateDiff;
            }
        }

        await repository.Insert(newJob);

        await dbContext.SaveChanges();

        return newJob.Id;
    }
}