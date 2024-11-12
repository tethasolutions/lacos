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
    public interface IHelperDocumentService
    {
        Task<IEnumerable<HelperDocumentReadModel>> GetHelperDocuments();
        Task<HelperDocumentDto> CreateHelperDocument(HelperDocumentDto helperDocumentDto);
        Task UpdateHelperDocument(long id, HelperDocumentDto helperDocumentDto);
        Task DeleteHelperDocument(long id);
        Task<HelperDocumentDto> GetHelperDocument(long id);

    }

    public class HelperDocumentService : IHelperDocumentService
    {
        private readonly IMapper mapper;
        private readonly IRepository<HelperDocument> helperDocumentRepository;
        private readonly ILacosDbContext dbContext;
        private readonly ILacosSession session;

        public HelperDocumentService(
            IMapper mapper,
            IRepository<HelperDocument> helperDocumentRepository,
            ILacosDbContext dbContext, ILacosSession session)
        {
            this.mapper = mapper;
            this.helperDocumentRepository = helperDocumentRepository;
            this.dbContext = dbContext;
            this.session = session;
        }

        public async Task<HelperDocumentDto> CreateHelperDocument(HelperDocumentDto helperDocumentDto)
        {
            if (helperDocumentDto.Id > 0)
                throw new LacosException("Impossibile creare un nuovo helper con un id già esistente");

            var helperDocument = helperDocumentDto.MapTo<HelperDocument>(mapper);

            await helperDocumentRepository.Insert(helperDocument);

            await dbContext.SaveChanges();

            return helperDocument.MapTo<HelperDocumentDto>(mapper);
        }

        public async Task DeleteHelperDocument(long id)
        {
            var helperDocument = await helperDocumentRepository.Get(id);

            if (helperDocument == null)
            {
                throw new NotFoundException(typeof(HelperDocument), id);
            }

            helperDocumentRepository.Delete(helperDocument);

            await dbContext.SaveChanges();
        }

        public async Task<HelperDocumentDto> GetHelperDocument(long id)
        {
            var helperDocument = await helperDocumentRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return helperDocument.MapTo<HelperDocumentDto>(mapper);
        }

        public async Task<IEnumerable<HelperDocumentReadModel>> GetHelperDocuments()
        {
            var helperDocuments = await helperDocumentRepository
                .Query()
                .AsNoTracking()
                .Include(x => x.HelperType)
                .OrderBy(x => x.Description)
                .ToArrayAsync();

            return helperDocuments.MapTo<IEnumerable<HelperDocumentReadModel>>(mapper);
        }

        public async Task UpdateHelperDocument(long id, HelperDocumentDto helperDocumentDto)
        {
            if (id == 0)
                throw new LacosException("Impossibile aggiornare un helper con id 0");

            var helperDocument = await helperDocumentRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (helperDocument == null)
                throw new LacosException($"Impossibile trovare un helper con id {id}");

            helperDocumentDto.MapTo(helperDocument, mapper);
            helperDocumentRepository.Update(helperDocument);
            await dbContext.SaveChanges();
        }
    }
}
