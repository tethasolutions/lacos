using AutoMapper;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Domain.Security;
using Lacos.GestioneCommesse.Framework.Extensions;
using Lacos.GestioneCommesse.Framework.Session;
using Microsoft.EntityFrameworkCore;
using Lacos.GestioneCommesse.Framework.Exceptions;

namespace Lacos.GestioneCommesse.Application.Registry.Services
{
    public interface IAccountingTypeService
    {
        Task<IEnumerable<AccountingTypeDto>> GetAccountingTypes(bool filterPO = false);

        Task<AccountingTypeDto> CreateAccountingType(AccountingTypeDto accountingTypeDto);

        Task UpdateAccountingType(long id, AccountingTypeDto accountingTypeDto);

        Task<AccountingTypeDto> GetAccountingType(long id);

    }

    public class AccountingTypeService : IAccountingTypeService
    {
        private readonly IMapper mapper;
        private readonly IRepository<AccountingType> accountingTypeRepository;
        private readonly ILacosDbContext dbContext;
        private readonly ILacosSession session;

        public AccountingTypeService(
            IMapper mapper,
            IRepository<AccountingType> accountingTypeRepository,
            ILacosDbContext dbContext, ILacosSession session)
        {
            this.mapper = mapper;
            this.accountingTypeRepository = accountingTypeRepository;
            this.dbContext = dbContext;
            this.session = session;
        }

        public async Task<AccountingTypeDto> CreateAccountingType(AccountingTypeDto accountingTypeDto)
        {
            if (accountingTypeDto.Id > 0)
                throw new LacosException("Impossibile creare un nuovo tipo con un id già esistente");

            var accountingType = accountingTypeDto.MapTo<AccountingType>(mapper);

            await accountingTypeRepository.Insert(accountingType);

            await dbContext.SaveChanges();

            return accountingType.MapTo<AccountingTypeDto>(mapper);
        }

        public async Task<AccountingTypeDto> GetAccountingType(long id)
        {
            var accountingType = await accountingTypeRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return accountingType.MapTo<AccountingTypeDto>(mapper);
        }

        public async Task<IEnumerable<AccountingTypeDto>> GetAccountingTypes(bool filterPO = false)
        {
            var query = accountingTypeRepository.Query()
                .AsNoTracking();

            return await query
                .Project<AccountingTypeDto>(mapper)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task UpdateAccountingType(long id, AccountingTypeDto accountingTypeDto)
        {
            if (id == 0)
                throw new LacosException("Impossibile aggiornare un tipo con id 0");

            var accountingType = await accountingTypeRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (accountingType == null)
                throw new LacosException($"Impossibile trovare un tipo con id {id}");

            accountingTypeDto.MapTo(accountingType, mapper);
            accountingTypeRepository.Update(accountingType);
            await dbContext.SaveChanges();
        }
    }
}
