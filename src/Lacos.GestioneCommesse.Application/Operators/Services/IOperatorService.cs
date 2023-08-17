using AutoMapper;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Docs;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;
using Lacos.GestioneCommesse.Framework.Session;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Application.Operators.Services
{
    public interface IOperatorService
    {
        Task<IEnumerable<OperatorDto>> GetOperators();

        Task<OperatorDto> CreateOperator(OperatorDto operatorDto);

        Task UpdateOperator(long id, OperatorDto operatorDto);

        Task<OperatorDto> GetOperator(long id);

    }

    public class OperatorService : IOperatorService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Operator> operatorRepository;
        private readonly ILacosDbContext dbContext;
        private readonly ILacosSession session;

        public OperatorService(
            IMapper mapper,
            IRepository<Operator> operatorRepository,
            ILacosDbContext dbContext, ILacosSession session)
        {
            this.mapper = mapper;
            this.operatorRepository = operatorRepository;
            this.dbContext = dbContext;
            this.session = session;
        }

        public async Task<OperatorDto> CreateOperator(OperatorDto operatorDto)
        {
            if (operatorDto.Id > 0)
                throw new ApplicationException("Impossibile creare un nuovo tipo con un id già esistente");

            var operatorResult = operatorDto.MapTo<Operator>(mapper);

            await operatorRepository.Insert(operatorResult);

            await dbContext.SaveChanges();

            return operatorResult.MapTo<OperatorDto>(mapper);
        }

        public async Task<OperatorDto> GetOperator(long id)
        {
            var operatorResult = await operatorRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return operatorResult.MapTo<OperatorDto>(mapper);
        }

        public async Task<IEnumerable<OperatorDto>> GetOperators()
        {
            var operators = await operatorRepository
                .Query()
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToArrayAsync();

            return operators.MapTo<IEnumerable<OperatorDto>>(mapper);
        }

        public async Task UpdateOperator(long id, OperatorDto operatorDto)
        {
            if (id == 0)
                throw new ApplicationException("Impossibile aggiornare un tipo con id 0");

            var operatorResult = await operatorRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (operatorResult == null)
                throw new ApplicationException($"Impossibile trovare un tipo con id {id}");

            operatorDto.MapTo(operatorResult, mapper);
            operatorRepository.Update(operatorResult);
            await dbContext.SaveChanges();
        }
    }
}
