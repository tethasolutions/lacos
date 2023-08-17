using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Operators.Services
{
    public interface IOperatorService
    {
        IQueryable<OperatorDto> GetOperators();
        Task<OperatorReadModel> GetOperator(long id);
        Task UpdateOperator(long id, OperatorDto operatorDto);
        Task DeleteOperator(long id);
        Task<OperatorDto> CreateOperator(OperatorDto operatorDto);

        Task<OperatorDocumentDto> GetOperatorDocmument(long docId);

    }
    public class OperatorService : IOperatorService
    {
        private readonly ILacosDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IRepository<Operator> operatorRepository;
        private readonly IRepository<OperatorDocument> operatorDocumentRepository;

        public OperatorService(IMapper mapper, IRepository<Operator> operatorRepository, ILacosDbContext dbContext, IRepository<OperatorDocument> operatorDocumentRepository)
        {
            this.mapper = mapper;
            this.operatorRepository = operatorRepository;
            this.dbContext = dbContext;
            this.operatorDocumentRepository = operatorDocumentRepository;
        }

        public IQueryable<OperatorDto> GetOperators()
        {
            var operators = operatorRepository
                .Query()
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Project<OperatorDto>(mapper);
            return operators;
        }

        public async Task<OperatorReadModel> GetOperator(long id)
        {
            if (id == 0)
                throw new ApplicationException("Impossibile recuperare un operatore con id 0");

            var singleOperator = await operatorRepository
                .Query()
                .AsNoTracking()
                .Include(x => x.DefaultVehicle)
                .Include(x => x.User)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (singleOperator == null)
                throw new ApplicationException($"Impossibile trovare l'operatore con id {id}");

            return singleOperator.MapTo<OperatorReadModel>(mapper);

        }

        public async Task UpdateOperator(long id, OperatorDto operatorDto)
        {
            if (id == 0)
                throw new ApplicationException("Impossibile aggiornare un operatore con id 0");

            var singleOperator = await operatorRepository
                .Query()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (singleOperator == null)
                throw new ApplicationException($"Impossibile trovare attività con id {id}");

            operatorDto.MapTo(singleOperator, mapper);
            operatorRepository.Update(singleOperator);

            await dbContext.SaveChanges();
        }

        public async Task<OperatorDto> CreateOperator(OperatorDto operatorDto)
        {
            var singleOperator = operatorDto.MapTo<Operator>(mapper);

            await operatorRepository.Insert(singleOperator);

            await dbContext.SaveChanges();

            return singleOperator.MapTo<OperatorDto>(mapper);
        }

        public async Task DeleteOperator(long id)
        {
            if (id == 0)
                throw new ApplicationException("Impossible eliminare un operatore con id 0");

            var singleOperator = await operatorRepository
                .Query()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (singleOperator == null)
                throw new ApplicationException($"Impossibile trovare il contatto con id {id}");

            operatorRepository.Delete(singleOperator);
            await dbContext.SaveChanges();
        }

        public async Task<OperatorDocumentDto> GetOperatorDocmument(long docId)
        {
            if (docId == 0)
                throw new ApplicationException("Impossibile recuperare un docmumento operatore con id 0");

            var documentOperator = await operatorDocumentRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == docId)
                .SingleOrDefaultAsync();

            if (documentOperator == null)
                throw new ApplicationException($"Impossibile trovare il docmumento operatore con id {docId}");

            return documentOperator.MapTo<OperatorDocumentDto>(mapper);

        }

    }
}
