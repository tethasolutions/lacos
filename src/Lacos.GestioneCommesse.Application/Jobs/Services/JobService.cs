
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Lacos.GestioneCommesse.Application.Customers.DTOs;
using Lacos.GestioneCommesse.Application.Jobs.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Domain.Security;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Jobs.Services
{
    public interface IJobService
    {
        Task<IEnumerable<JobReadModel>> GetAllJobs();
        Task<JobDetailDto> UpdateJob(long id, JobDetailDto jobDto);
        Task<JobDetailDto> CreateJob(JobDetailDto jobDto);
        Task DeleteJob(long id);
        Task<JobDetailReadModel> GetJobDetail(long id);
        Task<IEnumerable<CustomerReadModel>> GetJobCustomers();   
        Task<IEnumerable<JobOperatorDto>> GetOperators();
        Task<IEnumerable<JobSearchReadModel>> GetJobsSearch();
        Task<Job>  GetJob(long id);
    }

    public class JobService : IJobService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Job> jobRepository;
        private readonly IRepository<Customer> customerRepository;
        private readonly IRepository<Activity> activityRepository;
        private readonly IRepository<User> userRepository;
        private readonly ILacosDbContext dbContext;

        public JobService(
            IMapper mapper,
            IRepository<Job> jobRepository,
            IRepository<ProductType> productTypeRepository,
            ILacosDbContext dbContext, IRepository<User> userRepository,
            IRepository<Activity> activityRepository, IRepository<Customer> customerRepository, IRepository<Note> noteRepository)
        {
            this.mapper = mapper;
            this.jobRepository = jobRepository;
            this.dbContext = dbContext;
            this.userRepository = userRepository;
            this.activityRepository = activityRepository;
            this.customerRepository = customerRepository;
        }

        public async Task<IEnumerable<JobReadModel>> GetAllJobs()
        {
            var jobs = await jobRepository
                .Query()
                .AsNoTracking()
                .Include(x=>x.Customer)
                .ToArrayAsync();

            return jobs.MapTo<IEnumerable<JobReadModel>>(mapper);
        }

        public async Task<JobDetailDto> UpdateJob(long id, JobDetailDto jobDto)
        {

            if (id == 0)
                throw new ApplicationException("Impossibile aggiornare un job con id 0");

            var job= await jobRepository
                .Query()
                .AsNoTracking()
                //.Include(x=>x.Customer)
                //.Include(x=>x.CustomerAddress)
                //.Include(x=>x.Notes)
                //.Include(x=>x.Orders)
                //.Include(x=>x.Quotations)
                //.Include(x=>x.Source)
                //.Include(x=>x.ProductType)
                //.Include(x=>x.Activities)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (job == null)
                throw new ApplicationException($"Impossibile trovare job con id {id}");

            //se sto cambiando stato e il nuovo stato è "in fatturazione" allora sposto la data di scadenza avanti di 30 gg
            if (job.Status != jobDto.Status)
            {
                jobDto.ExpirationDate = DateTime.Now.AddDays(30);
            }

            jobDto.MapTo(job, mapper);
            jobRepository.Update(job);
            await dbContext.SaveChanges();

            return job.MapTo<JobDetailDto>(mapper);

        }

        public async Task<JobDetailDto> CreateJob(JobDetailDto jobDto)
        {
            if (jobDto.Id > 0)
                throw new ApplicationException("Impossibile creare un nuovo job con un id già esistente");

            var job = jobDto.MapTo<Job>(mapper);
            
            // TODO MB Introdurre un campo "Data commessa" in Job, non usare il campo CreatedOn
            var year = job.JobDate.Year;
            var currentNumber = await jobRepository.Query()
                .Where(e => e.Year == year)
                .MaxAsync(e => (int?) e.Number);

            job.Year = year;
            job.Number = (currentNumber ?? 0) + 1;
            
            await jobRepository.Insert(job);

            try
            {
                await dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

            return job.MapTo<JobDetailDto>(mapper);
        }

        public async Task<Job> GetJob(long id)
        {
            if (id == 0)
                throw new ApplicationException("Impossibile recuperare un job con id 0");

            var job = await jobRepository
                .Query()
                .AsNoTracking()
                .Include(x=>x.Customer)
                .ThenInclude(x=>x.Addresses)
                //.Include(x=>x.CustomerAddress)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (job == null)
                throw new ApplicationException($"Impossibile trovare il job con id {id}");

            return job;
        }

       

        public async Task<JobDetailReadModel> GetJobDetail(long id)
        {
            if (id == 0)
                throw new ApplicationException("Impossibile recuperare un job con id 0");

            var job = await jobRepository
                .Query()
                .AsNoTracking()
                .Include(x=>x.Customer)
                .ThenInclude(x=>x.Addresses)
                //.Include(x=>x.CustomerAddress)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (job == null)
                throw new ApplicationException($"Impossibile trovare il job con id {id}");

            return job.MapTo<JobDetailReadModel>(mapper);
        }

        public async Task<IEnumerable<CustomerReadModel>> GetJobCustomers()
        {

            var customers = await customerRepository
                .Query()
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.Addresses)
                .OrderBy(x => x.Name)
                .ToArrayAsync();

            return customers.MapTo<IEnumerable<CustomerReadModel>>(mapper);
        }

        public async Task<IEnumerable<JobOperatorDto>> GetOperators()
        {
            var customers = await userRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.UserName != "Administrator")
                .OrderBy(x => x.UserName)
                .ToArrayAsync();

            return customers.MapTo<IEnumerable<JobOperatorDto>>(mapper);
        }

        public async Task<IEnumerable<JobSearchReadModel>> GetJobsSearch()
        {
            var searchedJobs = await jobRepository
                .Query()
                .AsNoTracking()
                .Where(x => (x.Number != 0))
                .OrderByDescending(x => x.JobDate)
                .ThenBy(x => x.Customer.Name)
                .Project<JobSearchReadModel>(mapper)
                .ToArrayAsync();
            return searchedJobs;
        }

        public async Task DeleteJob(long id)
        {
            if (id == 0)
                throw new ApplicationException("Impossible eliminare la richiesta con id 0");

            var job = await jobRepository
                .Query()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (job == null)
                throw new ApplicationException($"Impossibile trovare la richiesta con id {id}");

            jobRepository.Delete(job);
            await dbContext.SaveChanges();
        }

    }
}
