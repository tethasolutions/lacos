using AutoMapper;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
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

namespace Lacos.GestioneCommesse.Application.Registry.Services
{
    public interface IActivityTypeService
    {
        Task<IEnumerable<ActivityTypeDto>> GetActivityTypes();

        Task<ActivityTypeDto> CreateActivityType(ActivityTypeDto activityTypeDto);

        Task UpdateActivityType(long id, ActivityTypeDto activityTypeDto);

        Task<ActivityTypeDto> GetActivityType(long id);

    }

    public class ActivityTypeService : IActivityTypeService
    {
        private readonly IMapper mapper;
        private readonly IRepository<ActivityType> activityTypeRepository;
        private readonly ILacosDbContext dbContext;
        private readonly ILacosSession session;

        public ActivityTypeService(
            IMapper mapper,
            IRepository<ActivityType> activityTypeRepository,
            ILacosDbContext dbContext, ILacosSession session)
        {
            this.mapper = mapper;
            this.activityTypeRepository = activityTypeRepository;
            this.dbContext = dbContext;
            this.session = session;
        }

        public async Task<ActivityTypeDto> CreateActivityType(ActivityTypeDto activityTypeDto)
        {
            if (activityTypeDto.Id > 0)
                throw new ApplicationException("Impossibile creare un nuovo tipo con un id già esistente");

            var activityType = activityTypeDto.MapTo<ActivityType>(mapper);

            await activityTypeRepository.Insert(activityType);

            await dbContext.SaveChanges();

            return activityType.MapTo<ActivityTypeDto>(mapper);
        }

        public async Task<ActivityTypeDto> GetActivityType(long id)
        {
            var activityType = await activityTypeRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return activityType.MapTo<ActivityTypeDto>(mapper);
        }

        public async Task<IEnumerable<ActivityTypeDto>> GetActivityTypes()
        {
            var activityTypes = await activityTypeRepository
                .Query()
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToArrayAsync();

            return activityTypes.MapTo<IEnumerable<ActivityTypeDto>>(mapper);
        }

        public async Task UpdateActivityType(long id, ActivityTypeDto activityTypeDto)
        {
            if (id == 0)
                throw new ApplicationException("Impossibile aggiornare un tipo con id 0");

            var activityType = await activityTypeRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (activityType == null)
                throw new ApplicationException($"Impossibile trovare un tipo con id {id}");

            activityTypeDto.MapTo(activityType, mapper);
            activityTypeRepository.Update(activityType);
            await dbContext.SaveChanges();
        }
    }
}
