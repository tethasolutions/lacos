

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
        private readonly ISecurityService securityService;

        public OperatorService(IMapper mapper, IRepository<Operator> operatorRepository, ILacosDbContext dbContext, IRepository<OperatorDocument> operatorDocumentRepository, IRepository<User> userRepository, ISecurityService securityService)
        {
            this.mapper = mapper;
            this.operatorRepository = operatorRepository;
            this.dbContext = dbContext;
            this.operatorDocumentRepository = operatorDocumentRepository;
            this.userRepository = userRepository;
            this.securityService = securityService;
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
                throw new LacosException("Impossibile recuperare un operatore con id 0");

            var singleOperator = await operatorRepository
                .Query()
                .AsNoTracking()
                .Include(x=>x.Documents)
                .Include(x => x.DefaultVehicle)
                .Include(x => x.User)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (singleOperator == null)
                throw new LacosException($"Impossibile trovare l'operatore con id {id}");

            return singleOperator.MapTo<OperatorReadModel>(mapper);

        }

        public async Task UpdateOperator(long id, OperatorDto operatorDto)
        {
            if (id == 0)
                throw new LacosException("Impossibile aggiornare un operatore con id 0");

            var singleOperator = await operatorRepository
                .Query()
                .Where(x => x.Id == id)
                .Include(x=>x.Documents)
                .SingleOrDefaultAsync();
            
            if (singleOperator == null)
                throw new LacosException($"Impossibile trovare operatore con id {id}");

            bool userOp = false;
            if (singleOperator.UserId != null && (operatorDto.hasUser is null || !operatorDto.hasUser.Value) && !userOp)
            {
                 await DeleteUser(singleOperator.UserId.Value);
                singleOperator.UserId = null;
                singleOperator.User = null;
                userOp = true;
            }
            
            if (singleOperator.UserId == null && operatorDto.hasUser.Value && !userOp)
            {
                singleOperator.UserId =  (await CreateUser(operatorDto.UserName, operatorDto.Password)).Id;
                userOp = true;
            }

            if (singleOperator.UserId != null && operatorDto.hasUser.Value && !userOp)
            {

                if (await securityService.CheckUserNameExists(singleOperator.UserId.Value, operatorDto.UserName))
                {
                    throw new LacosException($"Lo username {operatorDto.UserName} è già associato ad un'altro utente");
              
                }

                await UpdateUser(singleOperator.UserId.Value, operatorDto.UserName, operatorDto.Password);
                userOp = true;
            }



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


            try
            {
                await dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public async Task<OperatorDto> CreateOperator(OperatorDto operatorDto)
        {
            var singleOperator = operatorDto.MapTo<Operator>(mapper);
            await operatorRepository.Insert(singleOperator);

            if (operatorDto.hasUser.Value)
            {
              singleOperator.UserId =  (await CreateUser(operatorDto.UserName, operatorDto.Password)).Id;
            }

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
