using AutoMapper;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Framework.Extensions;
using Lacos.GestioneCommesse.Framework.Session;
using Microsoft.EntityFrameworkCore;
using Lacos.GestioneCommesse.Framework.Exceptions;

namespace Lacos.GestioneCommesse.Application.Registry.Services
{
    public interface IHelperTypeService
    {
        Task<IEnumerable<HelperTypeDto>> GetHelperTypes();
        Task<HelperTypeDto> CreateHelperType(HelperTypeDto helperTypeDto);
        Task UpdateHelperType(long id, HelperTypeDto helperTypeDto);
        Task DeleteHelperType(long id);
        Task<HelperTypeDto> GetHelperType(long id);

    }

    public class HelperTypeService : IHelperTypeService
    {
        private readonly IMapper mapper;
        private readonly IRepository<HelperType> helperTypeRepository;
        private readonly ILacosDbContext dbContext;
        private readonly ILacosSession session;

        public HelperTypeService(
            IMapper mapper,
            IRepository<HelperType> helperTypeRepository,
            ILacosDbContext dbContext, ILacosSession session)
        {
            this.mapper = mapper;
            this.helperTypeRepository = helperTypeRepository;
            this.dbContext = dbContext;
            this.session = session;
        }

        public async Task<HelperTypeDto> CreateHelperType(HelperTypeDto helperTypeDto)
        {
            if (helperTypeDto.Id > 0)
                throw new LacosException("Impossibile creare un nuovo tipo con un id già esistente");

            var helperType = helperTypeDto.MapTo<HelperType>(mapper);

            await helperTypeRepository.Insert(helperType);

            await dbContext.SaveChanges();

            return helperType.MapTo<HelperTypeDto>(mapper);
        }

        public async Task DeleteHelperType(long id)
        {
            var helperType = await helperTypeRepository.Get(id);

            if (helperType == null)
            {
                throw new NotFoundException(typeof(HelperType), id);
            }

            helperTypeRepository.Delete(helperType);

            await dbContext.SaveChanges();
        }

        public async Task<HelperTypeDto> GetHelperType(long id)
        {
            var helperType = await helperTypeRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return helperType.MapTo<HelperTypeDto>(mapper);
        }

        public async Task<IEnumerable<HelperTypeDto>> GetHelperTypes()
        {
            var helperTypes = await helperTypeRepository
                .Query()
                .AsNoTracking()
                .OrderBy(x => x.Type)
                .ToArrayAsync();

            return helperTypes.MapTo<IEnumerable<HelperTypeDto>>(mapper);
        }

        public async Task UpdateHelperType(long id, HelperTypeDto helperTypeDto)
        {
            if (id == 0)
                throw new LacosException("Impossibile aggiornare un tipo con id 0");

            var helperType = await helperTypeRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (helperType == null)
                throw new LacosException($"Impossibile trovare un tipo con id {id}");

            helperTypeDto.MapTo(helperType, mapper);
            helperTypeRepository.Update(helperType);
            await dbContext.SaveChanges();
        }
    }
}
