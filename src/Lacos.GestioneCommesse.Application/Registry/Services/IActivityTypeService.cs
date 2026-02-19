using AutoMapper;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Domain.Security;
using Lacos.GestioneCommesse.Framework.Extensions;
using Lacos.GestioneCommesse.Framework.Session;
using Microsoft.EntityFrameworkCore;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Application.Operators.DTOs;

namespace Lacos.GestioneCommesse.Application.Registry.Services
{
    public interface IActivityTypeService
    {
        Task<IEnumerable<ActivityTypeDto>> GetActivityTypes(bool filterPO = false);

        Task<ActivityTypeDto> CreateActivityType(ActivityTypeDto activityTypeDto);

        Task UpdateActivityType(long id, ActivityTypeDto activityTypeDto);

        Task<ActivityTypeDto> GetActivityType(long id);
        Task<OperatorDto> GetDefaultOperator(long activityTypeId);

    }

    public class ActivityTypeService : IActivityTypeService
    {
        private readonly IMapper mapper;
        private readonly IRepository<ActivityType> activityTypeRepository;
        private readonly IRepository<Operator> operatorRepository;
        private readonly ILacosDbContext dbContext;
        private readonly ILacosSession session;

        public ActivityTypeService(
            IMapper mapper,
            IRepository<ActivityType> activityTypeRepository,
            IRepository<Operator> operatorRepository,
            ILacosDbContext dbContext, ILacosSession session)
        {
            this.mapper = mapper;
            this.activityTypeRepository = activityTypeRepository;
            this.operatorRepository = operatorRepository;
            this.dbContext = dbContext;
            this.session = session;
        }

        public async Task<ActivityTypeDto> CreateActivityType(ActivityTypeDto activityTypeDto)
        {
            if (activityTypeDto.Id > 0)
                throw new LacosException("Impossibile creare un nuovo tipo con un id già esistente");

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

        public async Task<IEnumerable<ActivityTypeDto>> GetActivityTypes(bool filterPO = false)
        {
            var query = activityTypeRepository.Query()
                .AsNoTracking();

            if(session.IsAuthenticated() && session.IsAuthorized(Role.Operator))
            {
                var user = session.CurrentUser!;
                query = query
                    .Where(t =>
                        t.Operators.Any(o => o.Id == user.OperatorId)
                    );
            }

            if (filterPO)
                query = query
                    .Where(t => t.ViewInPurchaseOrder == true);

            return await query
                .Project<ActivityTypeDto>(mapper)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<OperatorDto> GetDefaultOperator(long activityTypeId)
        {
            var result = await operatorRepository
                .Query()
                .AsNoTracking()
                .Include(o => o.ActivityTypes)
                .Where(o => o.ActivityTypes.Any(a => a.Id == activityTypeId))
                .FirstOrDefaultAsync();

            if (result == null)
            {
                throw new LacosException("Nessun operatore associato al tipo attività");
            }

            return result.MapTo<OperatorDto>(mapper);
        }

        public async Task UpdateActivityType(long id, ActivityTypeDto activityTypeDto)
        {
            if (id == 0)
                throw new LacosException("Impossibile aggiornare un tipo con id 0");

            var activityType = await activityTypeRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (activityType == null)
                throw new LacosException($"Impossibile trovare un tipo con id {id}");

            activityTypeDto.MapTo(activityType, mapper);
            activityTypeRepository.Update(activityType);
            await dbContext.SaveChanges();
        }
    }
}
