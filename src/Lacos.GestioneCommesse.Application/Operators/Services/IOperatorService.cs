

using AutoMapper;
using Lacos.GestioneCommesse.Application.Operators.DTOs;
using Lacos.GestioneCommesse.Application.Security;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Domain.Security;
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
        private readonly ISecurityContextFactory securityContextFactory;

        public OperatorService(IMapper mapper, IRepository<Operator> operatorRepository, ILacosDbContext dbContext, IRepository<OperatorDocument> operatorDocumentRepository, IRepository<User> userRepository, ISecurityContextFactory securityContextFactory)
        {
            this.mapper = mapper;
            this.operatorRepository = operatorRepository;
            this.dbContext = dbContext;
            this.operatorDocumentRepository = operatorDocumentRepository;
            this.userRepository = userRepository;
            this.securityContextFactory = securityContextFactory;
        }

        public IQueryable<OperatorDto> GetOperators()
        {
            var operators = operatorRepository
                .Query()
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Include(x=>x.DefaultVehicle)
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
                .Include(x=>x.Documents)
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
                .Include(x=>x.Documents)
                .SingleOrDefaultAsync();
            
            if (singleOperator == null)
                throw new ApplicationException($"Impossibile trovare operatore con id {id}");
            
            foreach (var operatorDocument in singleOperator.Documents.Reverse<OperatorDocument>())
            {
                if (operatorDto.Documents.All(x => x.FileName != operatorDocument.FileName))
                {
                    singleOperator.Documents.Remove(operatorDocument);
                }
            }
            operatorRepository.Update(singleOperator);

            foreach (var operatorDtoDocument in operatorDto.Documents.Reverse<OperatorDocumentDto>())
            {
                if (singleOperator.Documents.All(x => x.FileName != operatorDtoDocument.FileName))
                {
                    operatorDto.Documents.ToList().Remove(operatorDtoDocument);
                }
            }
            operatorDto.MapTo(singleOperator, mapper);
            operatorRepository.Update(singleOperator);

            await dbContext.SaveChanges();
        }

        public async Task<OperatorDto> CreateOperator(OperatorDto operatorDto)
        {
            var singleOperator = operatorDto.MapTo<Operator>(mapper);

            await operatorRepository.Insert(singleOperator);
            
            try
            {
                await dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

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
                throw new ApplicationException("Impossibile recuperare un documento operatore con id 0");

            var documentOperator = await operatorDocumentRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == docId)
                .SingleOrDefaultAsync();

            if (documentOperator == null)
                throw new ApplicationException($"Impossibile trovare il docmumento operatore con id {docId}");

            return documentOperator.MapTo<OperatorDocumentDto>(mapper);

        }

        private async Task CreateUser (string username, string password)
        {

            var user = new User() {Enabled = true, Id = 0, Role = Role.Operator};

            await ExecuteOnUser(user, async context =>
            {
                await context.EnsureUserNameIsUnique(user.UserName);
                await context.HashPasswordWithUniqueSalt(password);
                await context.GenerateUniqueAccessToken();
            });
        }

        private async Task UpdateUser (long id, string username, string password)
        {
            
            //await ExecuteOnUser(id,
            //    async context =>
            //    {
            //        await context.ChangeUserName(username);

            //        if (!string.IsNullOrEmpty(password) && !context.VerifyPassword(password))
            //        {
            //            await context.ChangePassword(password);
            //        }

            //        await context.EnableUser(true);
            //    });

        }

        private async Task ExecuteOnUser(User user, Func<ISecurityContext, Task> action)
        {
            using (var context = securityContextFactory.CreateSecurityContext(user))
            {
                await action(context);
            }

            await userRepository.InsertOrUpdate(user);
            await dbContext.SaveChanges();
        }

    }
}
