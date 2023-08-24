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
    public interface IProductTypeService
    {
        Task<IEnumerable<ProductTypeDto>> GetProductTypes();

        Task<ProductTypeDto> CreateProductType(ProductTypeDto productTypeDto);

        Task UpdateProductType(long id, ProductTypeDto productTypeDto);

        Task<ProductTypeDto> GetProductType(long id);

    }

    public class ProductTypeService : IProductTypeService
    {
        private readonly IMapper mapper;
        private readonly IRepository<ProductType> productTypeRepository;
        private readonly ILacosDbContext dbContext;
        private readonly ILacosSession session;

        public ProductTypeService(
            IMapper mapper,
            IRepository<ProductType> productTypeRepository,
            ILacosDbContext dbContext, ILacosSession session)
        {
            this.mapper = mapper;
            this.productTypeRepository = productTypeRepository;
            this.dbContext = dbContext;
            this.session = session;
        }

        public async Task<ProductTypeDto> CreateProductType(ProductTypeDto productTypeDto)
        {
            if (productTypeDto.Id > 0)
                throw new LacosException("Impossibile creare un nuovo tipo con un id già esistente");

            var productType = productTypeDto.MapTo<ProductType>(mapper);

            await productTypeRepository.Insert(productType);

            await dbContext.SaveChanges();

            return productType.MapTo<ProductTypeDto>(mapper);
        }

        public async Task<ProductTypeDto> GetProductType(long id)
        {
            var productType = await productTypeRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return productType.MapTo<ProductTypeDto>(mapper);
        }

        public async Task<IEnumerable<ProductTypeDto>> GetProductTypes()
        {
            var productTypes = await productTypeRepository
                .Query()
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToArrayAsync();

            return productTypes.MapTo<IEnumerable<ProductTypeDto>>(mapper);
        }

        public async Task UpdateProductType(long id, ProductTypeDto productTypeDto)
        {
            if (id == 0)
                throw new LacosException("Impossibile aggiornare un tipo con id 0");

            var productType = await productTypeRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (productType == null)
                throw new LacosException($"Impossibile trovare un tipo con id {id}");

            productTypeDto.MapTo(productType, mapper);
            productTypeRepository.Update(productType);
            await dbContext.SaveChanges();
        }
    }
}
