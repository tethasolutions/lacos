

using AutoMapper;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using Lacos.GestioneCommesse.Application.Security;
using Lacos.GestioneCommesse.Application.Security.DTOs;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Domain.Security;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Operators.Services
{
    public interface IOperatorService
    {
        IQueryable<OperatorReadModel> GetOperators();
        Task<OperatorDto> GetOperator(long id);
        Task<OperatorDto> GetOperatorByUserId(long userId);
        Task UpdateOperator(OperatorDto operatorDto);
        Task DeleteOperator(long id);
        Task<OperatorDto> CreateOperator(OperatorDto operatorDto);
        Task<OperatorDocumentReadModel> DownloadOperatorDocument(string filename);
        Task<IEnumerable<OperatorDocumentReadModel>> GetAllOperatorDocuments(long operatorId);

    }
    public class OperatorService : IOperatorService
    {
        private readonly ILacosDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IRepository<Operator> operatorRepository;
        private readonly IRepository<OperatorDocument> operatorDocumentRepository;
        private readonly IRepository<User> userRepository;
        private readonly ISecurityService securityService;
        private readonly IRepository<ActivityType> activityTypeRepository;

        public OperatorService(
            IMapper mapper, 
            IRepository<Operator> operatorRepository,
            ILacosDbContext dbContext, 
            IRepository<OperatorDocument> operatorDocumentRepository,
            IRepository<User> userRepository, 
            ISecurityService securityService, 
            IRepository<ActivityType> activityTypeRepository
        )
        {
            this.mapper = mapper;
            this.operatorRepository = operatorRepository;
            this.dbContext = dbContext;
            this.operatorDocumentRepository = operatorDocumentRepository;
            this.userRepository = userRepository;
            this.securityService = securityService;
            this.activityTypeRepository = activityTypeRepository;
        }

        public IQueryable<OperatorReadModel> GetOperators()
        {
            return operatorRepository.Query()
                .AsNoTracking()
                .Project<OperatorReadModel>(mapper);
        }

        public async Task<OperatorDto> GetOperator(long id)
        {
            var @operator = await operatorRepository.Query()
                .AsNoTracking()
                .Include(e => e.User)
                .Include(x => x.Documents)
                .Include(x => x.ActivityTypes)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (@operator == null)
            {
                throw new LacosException($"Impossibile trovare l'operatore con id {id}");
            }

            return @operator.MapTo<OperatorDto>(mapper);
        }

        public async Task<OperatorDto> GetOperatorByUserId(long userId)
        {
            var @operator = await operatorRepository.Query()
                .AsNoTracking()
                .Include(e => e.User)
                .Where(x => x.UserId == userId)
                .SingleOrDefaultAsync();

            if (@operator == null)
            {
                throw new LacosException($"Impossibile trovare l'operatore con id utente {userId}");
            }

            return @operator.MapTo<OperatorDto>(mapper);
        }

        public async Task UpdateOperator(OperatorDto operatorDto)
        {
            var singleOperator = await operatorRepository
                .Query()
                .Include(x => x.Documents)
                .Include(x => x.ActivityTypes)
                .Where(e => e.Id == operatorDto.Id)
                .SingleOrDefaultAsync();

            if (singleOperator == null)
            {
                throw new LacosException($"Impossibile trovare operatore con id {operatorDto.Id}");
            }

            await using (var transaction = await dbContext.BeginTransaction())
            {
                if (singleOperator.UserId != null && operatorDto.HasUser != true)
                {
                    await DeleteUser(singleOperator.UserId.Value);
                    singleOperator.UserId = null;
                    singleOperator.User = null;
                }
                else if (singleOperator.UserId == null && operatorDto.HasUser == true)
                {
                    singleOperator.UserId = (await CreateUser(operatorDto.UserName, operatorDto.Password)).Id;
                }
                else if (singleOperator.UserId != null && operatorDto.HasUser == true)
                {
                    if (await securityService.CheckUserNameExists(singleOperator.UserId.Value, operatorDto.UserName))
                    {
                        throw new LacosException($"Lo username {operatorDto.UserName} è già associato ad un'altro utente");
                    }

                    await UpdateUser(singleOperator.UserId.Value, operatorDto.UserName, operatorDto.Password);
                }

                var activityTypes = await activityTypeRepository.Query()
                    .Where(e => operatorDto.ActivityTypes.Contains(e.Id))
                    .ToListAsync();

                singleOperator.ActivityTypes.Clear();
                singleOperator.ActivityTypes.AddRange(activityTypes);

                operatorDto.MapTo(singleOperator, mapper);
                operatorRepository.Update(singleOperator);

                await dbContext.SaveChanges();

                await transaction.CommitAsync();
            }
        }

        public async Task<OperatorDto> CreateOperator(OperatorDto operatorDto)
        {
            await using (var transaction = await dbContext.BeginTransaction())
            {
                var singleOperator = operatorDto.MapTo<Operator>(mapper);

                if (operatorDto.HasUser == true)
                {
                    singleOperator.UserId = (await CreateUser(operatorDto.UserName, operatorDto.Password)).Id;
                }

                var activityTypes = await activityTypeRepository.Query()
                    .Where(e => operatorDto.ActivityTypes.Contains(e.Id))
                    .ToListAsync();

                singleOperator.ActivityTypes.Clear();
                singleOperator.ActivityTypes.AddRange(activityTypes);

                await operatorRepository.Insert(singleOperator);

                await dbContext.SaveChanges();

                await transaction.CommitAsync();

                return singleOperator.MapTo<OperatorDto>(mapper);
            }
        }

        public async Task DeleteOperator(long id)
        {
            if (id == 0)
                throw new LacosException("Impossible eliminare un operatore con id 0");

            var singleOperator = await operatorRepository
                .Query()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (singleOperator == null)
                throw new LacosException($"Impossibile trovare il contatto con id {id}");

            operatorRepository.Delete(singleOperator);

            if (singleOperator.UserId != null)
            {
                await DeleteUser(singleOperator.UserId.Value);
            }
           
            await dbContext.SaveChanges();
        }

        public async Task<OperatorDocumentReadModel> DownloadOperatorDocument(string filename)
        {
            var operatorDocument = await operatorDocumentRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.FileName == filename)
                .SingleOrDefaultAsync();

            return operatorDocument.MapTo<OperatorDocumentReadModel>(mapper);
        }

        public async Task<IEnumerable<OperatorDocumentReadModel>> GetAllOperatorDocuments(long operatorId)
        {
            var operatorDocuments = await operatorDocumentRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.OperatorId == operatorId)
                .SingleOrDefaultAsync();

            return operatorDocuments.MapTo<IEnumerable<OperatorDocumentReadModel>>(mapper);
        }

        public async Task<OperatorDocumentDto> GetOperatorDocmument(long docId)
        {
            if (docId == 0)
                throw new LacosException("Impossibile recuperare un documento operatore con id 0");

            var documentOperator = await operatorDocumentRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == docId)
                .SingleOrDefaultAsync();

            if (documentOperator == null)
                throw new LacosException($"Impossibile trovare il docmumento operatore con id {docId}");

            return documentOperator.MapTo<OperatorDocumentDto>(mapper);

        }

        private async Task<UserDto> CreateUser (string username, string password)
        {
            var user = new UserDto() {UserName = username, Enabled = true, Id = 0, Role = Role.Operator};

            return await securityService.Register(user, password);
        }

        private async Task UpdateUser (long id, string username, string password)
        {
            await securityService.ExecuteOnUser(id,
                async context =>
                {
                    await context.ChangeUserName(username);

                    if (!string.IsNullOrEmpty(password) && !context.VerifyPassword(password))
                    {
                        await context.ChangePassword(password);
                    }
                });
        }

        private async Task DeleteUser (long id)
        {
            await securityService.DeleteUser(id);
        }
    }
}
