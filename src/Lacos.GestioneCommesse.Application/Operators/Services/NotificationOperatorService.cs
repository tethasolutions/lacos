using AutoMapper;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;
using Lacos.GestioneCommesse.Framework.Exceptions;

namespace Lacos.GestioneCommesse.Application.NotificationOperators.Service
{
    public interface INotificationOperatorService
    {
        IQueryable<NotificationOperatorReadModel> GetNotificationOperators();
        Task<NotificationOperatorDto> GetNotificationOperatorDetail(long notificationOperatorId);
        Task UpdateNotificationOperator(long id, NotificationOperatorDto notificationOperatorDto);
        Task<NotificationOperatorDto> CreateNotificationOperator(NotificationOperatorDto notificationOperatorDto);
        Task DeleteNotificationOperator(long notificationOperatorId);
    }
    public class NotificationOperatorService : INotificationOperatorService
    {
        private readonly ILacosDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IRepository<NotificationOperator> notificationOperatorRepository;

        public NotificationOperatorService(IRepository<NotificationOperator> notificationOperatorRepository, IMapper mapper, ILacosDbContext dbContext)
        {
            this.notificationOperatorRepository = notificationOperatorRepository;
            this.mapper = mapper;
            this.dbContext = dbContext;
        }

        public IQueryable<NotificationOperatorReadModel> GetNotificationOperators()
        {
            var notificationOperators = notificationOperatorRepository
                .Query()
                .AsNoTracking()
                .Project<NotificationOperatorReadModel>(mapper);
            return notificationOperators;
        }

        public async Task<NotificationOperatorDto> GetNotificationOperatorDetail(long notificationOperatorId)
        {
            if (notificationOperatorId == 0)
                throw new LacosException("Impossibile recuperare un operatore con id 0");

            var notificationOperator = await notificationOperatorRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == notificationOperatorId)
                .SingleOrDefaultAsync();

            if (notificationOperator == null)
                throw new LacosException($"Impossibile trovare l'operatore con id {notificationOperatorId}");

            return notificationOperator.MapTo<NotificationOperatorDto>(mapper);

        }

        public async Task UpdateNotificationOperator(long id, NotificationOperatorDto notificationOperatorDto)
        {
            if (id == 0)
                throw new LacosException("Impossibile aggiornare un operatore con id 0");

            var notificationOperator = await notificationOperatorRepository
                .Query()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (notificationOperator == null)
                throw new LacosException($"Impossibile trovare operatore con id {id}");

            notificationOperatorDto.MapTo(notificationOperator, mapper);

            await dbContext.SaveChanges();
        }

        public async Task<NotificationOperatorDto> CreateNotificationOperator(NotificationOperatorDto notificationOperatorDto)
        {
            var notificationOperator = notificationOperatorDto.MapTo<NotificationOperator>(mapper);

            await notificationOperatorRepository.Insert(notificationOperator);

            await dbContext.SaveChanges();

            return notificationOperator.MapTo<NotificationOperatorDto>(mapper);
        }

        public async Task DeleteNotificationOperator(long notificationOperatorId)
        {
            if (notificationOperatorId == 0)
                throw new LacosException("Impossible eliminare un prodotto con id 0");

            var notificationOperator = await notificationOperatorRepository
                .Query()
                .Where(x => x.Id == notificationOperatorId)
                .SingleOrDefaultAsync();

            if (notificationOperator == null)
                throw new LacosException($"Impossibile trovare il prodotto con id {notificationOperatorId}");

            notificationOperatorRepository.Delete(notificationOperator);
            await dbContext.SaveChanges();
        }

    }
}
