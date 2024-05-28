using AutoMapper;
using Lacos.GestioneCommesse.Dal;
using Lacos.GestioneCommesse.Domain.Registry;
using Lacos.GestioneCommesse.Application.Products.DTOs;
using Lacos.GestioneCommesse.Framework.Extensions;
using Microsoft.EntityFrameworkCore;
using Lacos.GestioneCommesse.Application.Registry.DTOs;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Domain.Docs;

namespace Lacos.GestioneCommesse.Application.Products.Service
{
     public interface IProductService
     {
         IQueryable<ProductReadModel> GetProducts();
        IQueryable<ProductReadModel> GetSpareParts();
        Task<ProductDto> GetProductDetail(long productId);
         Task UpdateProduct(long id, ProductDto productDto);
         Task<ProductDto> CreateProduct(ProductDto productDto);
         Task DeleteProduct(long productId);
         Task<IEnumerable<ProductTypeDto>> GetProductTypes();
        Task<ProductDocumentReadModel> DownloadProductDocument(string filename);
        Task<IEnumerable<ProductDocumentReadModel>> GetAllProductDocuments(long productId);
    }
    public class ProductService : IProductService
    {
        private readonly ILacosDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IRepository<Product> productRepository;
        private readonly IRepository<ProductType> productTypeRepository;
        private readonly IRepository<ProductDocument> productDocumentRepository;
        private readonly IRepository<InterventionProduct> interventionProductRepository;

        public ProductService(IRepository<Product> productRepository, IMapper mapper, ILacosDbContext dbContext, IRepository<ProductType> productTypeRepository, IRepository<ProductDocument> productDocumentRepository, IRepository<InterventionProduct> interventionProductRepository)
        {
            this.productRepository = productRepository;
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.productTypeRepository = productTypeRepository;
            this.productDocumentRepository = productDocumentRepository;
            this.interventionProductRepository = interventionProductRepository;
        }

        public IQueryable<ProductReadModel> GetProducts()
        {
            var products = productRepository
                .Query()
                .AsNoTracking()
                .Project<ProductReadModel>(mapper);
            return products;
        }

        public IQueryable<ProductReadModel> GetSpareParts()
        {
            var products = productRepository
                .Query()
                .AsNoTracking()
                .Where(e => e.ProductType.IsSparePart)
                .Project<ProductReadModel>(mapper);
            return products;
        }

        public async Task<ProductDto> GetProductDetail(long productId)
        {
            if (productId == 0)
                throw new LacosException("Impossibile recuperare un prodotto con id 0");

            var product = await productRepository
                .Query()
                .AsNoTracking()
                .Include(x=>x.ProductType)
                .Include(x=>x.Documents)
                .Where(x => x.Id == productId)
                .SingleOrDefaultAsync();

            if (product == null)
                throw new LacosException($"Impossibile trovare l'articolo con id {productId}");

            return product.MapTo<ProductDto>(mapper);

        }

        public async Task UpdateProduct(long id, ProductDto productDto)
        {
            if (id == 0)
                throw new LacosException("Impossibile aggiornare un operatore con id 0");

            var product = await productRepository
                .Query()
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();
            
            if (product == null)
                throw new LacosException($"Impossibile trovare operatore con id {id}");

            productDto.MapTo(product, mapper);

            await dbContext.SaveChanges();
        }

        public async Task<ProductDto> CreateProduct(ProductDto productDto)
        {
            var product = productDto.MapTo<Product>(mapper);

            await productRepository.Insert(product);
                     
            await dbContext.SaveChanges();

            return product.MapTo<ProductDto>(mapper);
        }

        public async Task DeleteProduct(long productId)
        {
            if (productId == 0)
                throw new LacosException("Impossible eliminare un prodotto con id 0");

            var product = await productRepository
                .Query()
                .Where(x => x.Id == productId)
                .SingleOrDefaultAsync();

            if (product == null)
                throw new LacosException($"Impossibile trovare il prodotto con id {productId}");

            productRepository.Delete(product);
            await dbContext.SaveChanges();
        }

        public async Task<IEnumerable<ProductTypeDto>> GetProductTypes()
        {
            var productTypes = await productTypeRepository
                .Query()
                .AsNoTracking()
                .ToListAsync();

            return productTypes.MapTo<IEnumerable<ProductTypeDto>>(mapper);
        }

        public async Task<ProductDocumentReadModel> DownloadProductDocument(string filename)
        {
            var productDocument = await productDocumentRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.FileName == filename)
                .SingleOrDefaultAsync();

            return productDocument.MapTo<ProductDocumentReadModel>(mapper);
        }

        public async Task<IEnumerable<ProductDocumentReadModel>> GetAllProductDocuments(long productId)
        {
            var productDocuments = await productDocumentRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.ProductId == productId)
                .SingleOrDefaultAsync();

            return productDocuments.MapTo<IEnumerable<ProductDocumentReadModel>>(mapper);
        }

        public async Task<ProductDocumentReadModel> GetProductDocument(long docId)
        {
            if (docId == 0)
                throw new LacosException("Impossibile recuperare un documento operatore con id 0");

            var productDocument = await productDocumentRepository
                .Query()
                .AsNoTracking()
                .Where(x => x.Id == docId)
                .SingleOrDefaultAsync();

            if (productDocument == null)
                throw new LacosException($"Impossibile trovare il docmumento operatore con id {docId}");

            return productDocument.MapTo<ProductDocumentReadModel>(mapper);

        }
    }
}
